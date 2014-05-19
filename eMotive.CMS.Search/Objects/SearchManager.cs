using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using eMotive.CMS.Extensions;
using eMotive.CMS.Models.Objects.Search;
using eMotive.CMS.Search.Interfaces;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace eMotive.CMS.Search.Objects
{
    public class SearchManager : ISearchManager, IDisposable
    {
        private readonly FSDirectory _directory;
        private static IndexWriter _writer;
        private readonly Analyzer _analyzer;
        private IndexSearcher _searcher;
        private readonly Version _luceneVersion;


        public SearchManager(string indexLocation)
        {
            if (string.IsNullOrEmpty(indexLocation))
                throw new FileNotFoundException("The lucene index could not be found.");

            _luceneVersion = Version.LUCENE_30;

            var resolvedServerLocation = HttpContext.Current.Server.MapPath(string.Format("~{0}", indexLocation));
            _directory = FSDirectory.Open(new DirectoryInfo(resolvedServerLocation));

            var createIndex = !IndexReader.IndexExists(_directory);

            _writer = new IndexWriter(_directory, new StandardAnalyzer(_luceneVersion), createIndex, IndexWriter.MaxFieldLength.UNLIMITED);

            _analyzer = new PerFieldAnalyzerWrapper(new StandardAnalyzer(_luceneVersion));
        }

        public SearchResult DoSearch(Search search)
        {
            //todo: is this needed? i.e. dispose first

            _searcher = new IndexSearcher(_writer.GetReader());

            //TODO: need to tidy this up, perhaps only initialise parser if _search.Query
            var items = new Collection<ResultItem>();
            try//todo: do i need to make title DocumentTITLE AND UNALAYZED AGAIN - thenadd an analyzed title in? YESSSSSSSSSSS
            {
                var bq = new BooleanQuery();
                var parser = new QueryParser(_luceneVersion, string.Empty, _analyzer);
                if (!string.IsNullOrEmpty(search.Query) && !search.CustomQuery.HasContent())
                {
                    var query = parser.Parse(search.Query);
                    bq = new BooleanQuery
                        {
                            {
                                parser.Parse(string.Format("Title:{0}", query)), Occur.MUST
                            },
                            {
                                parser.Parse(string.Format("Description:{0}", query)), Occur.MUST
                            }
                        };
                }
                else
                {
                    if (!search.CustomQuery.HasContent())
                        throw new ArgumentException("Neither Query or CustomQuery have been defined.");

                    bq = new BooleanQuery();
                    //TODO: need a way of passing in occur.must and occur.should
                    foreach (var query in search.CustomQuery.Where(n => !string.IsNullOrEmpty(n.Value.Field)))
                    {
                        bq.Add(new BooleanClause(parser.Parse(string.Format("{0}:{1}", query.Key, query.Value.Field)), query.Value.Term));
                    }
                }

                Sort sort = null;//new Sort(new SortField("Forename", SortField.STRING, true));

                if (!string.IsNullOrEmpty(search.SortBy))
                {
                    sort = new Sort(new SortField(search.SortBy, SortField.STRING, search.OrderBy != SortDirection.ASC));
                }


                //    var tfc = TopFieldCollector.Create(sort, 10000, true, true, true, false);

                TopDocs docs;
                if (search.Type.HasContent())
                {
                    var filterBq = new BooleanQuery();
                    foreach (var type in search.Type)
                    {
                        filterBq.Add(new BooleanClause(parser.Parse(string.Format("Type:{0}", type)), Occur.MUST));
                    }
                    var test = new QueryWrapperFilter(filterBq);
                    docs = sort != null ? _searcher.Search(bq, test, 10000, sort) : _searcher.Search(bq, test, 10000);


                }
                else
                {
                    docs = sort != null ? _searcher.Search(bq, null, 10000, sort) : _searcher.Search(bq, null, 10000);
                }



                if (docs.ScoreDocs.Length > 0)
                {
                    search.NumberOfResults = docs.ScoreDocs.Length;// -1;

                    var page = search.CurrentPage - 1;

                    var first = page * search.PageSize;
                    int last;

                    if (search.NumberOfResults > first + search.PageSize)
                    {
                        last = first + search.PageSize;
                    }
                    else
                    {
                        last = search.NumberOfResults;
                    }

                    for (var i = first; i < last; i++)
                    {
                        var scoreDoc = docs.ScoreDocs[i];

                        var score = scoreDoc.Score;

                        var docId = scoreDoc.Doc;

                        var doc = _searcher.Doc(docId);

                        items.Add(new ResultItem
                        {
                            ID = Convert.ToInt32(doc.Get("DatabaseID")),
                            Title = doc.Get("Title"),
                            Type = doc.Get("Type"),
                            Description = doc.Get("Description"),
                            Score = score
                        });
                    }
                }
            }
            catch (ParseException)
            {

                search.Error = "The search query was malformed. For help with searching, please click the help link.";
            }
            catch
            {
                search.Error = "An error occured. Please try again.";
            }

            _searcher.Dispose();
            _searcher = null;
            //  reader.Dispose();
            return new SearchResult
            {
                CurrentPage = search.CurrentPage,
                Error = search.Error,
                NumberOfResults = search.NumberOfResults,
                PageSize = search.PageSize,
                Query = search.Query,
                Items = items
            };
        }


        public bool Add(ISearchDocument document)
        {
            var success = true;
            try
            {
                var doc = document.BuildRecord();
                _writer.AddDocument(doc);
                _writer.Commit();
            }
            catch (AlreadyClosedException)
            {
                success = false;
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }

        public bool Update(ISearchDocument document)
        {
            var success = true;
            try
            {
                _writer.UpdateDocument(new Term("UniqueID", document.UniqueID), document.BuildRecord());
                _writer.Commit();
            }
            catch (AlreadyClosedException)
            {
                success = false;
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }

        public bool Delete(ISearchDocument document)
        {
            var success = true;
            try
            {
                _writer.DeleteDocuments(new Term("UniqueID", document.UniqueID));
                _writer.Commit();
            }
            catch (AlreadyClosedException)
            {
                success = false;
            }
            catch (Exception)
            {
                success = false;
            }

            return success;

        }

        public void DeleteAll()
        {
            _writer.DeleteAll();
            _writer.Commit();
        }

        public int NumberOfDocuments()
        {
            var reader = _writer.GetReader();

            var numDocs = reader.NumDocs();

            reader.Dispose();

            return numDocs;
        }

        public void Dispose()
        {
            _writer.Commit();

            _writer.Dispose();
            _directory.Dispose();
            _searcher.Dispose();
            _analyzer.Dispose();
        }
    }
}
