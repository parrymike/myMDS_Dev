using System;
using System.Collections.Generic;
using System.Text;
using eMotive.CMS.Extensions;

namespace eMotive.CMS.Models.Objects.Search
{
    public enum SortDirection { ASC, DESC }
    public class BasicSearch
    {

        private int? _page;

        public BasicSearch()
        {
            _page = 1;
            PageSize = 10;
        }

        public int NumberOfResults { get; set; }
        public int PageSize { get; set; }
        public int? Page
        {
            get
            {
                return _page;
            }
            set
            {
                if (!value.HasValue || !value.Value.IsNumeric())
                {
                    _page = 1;
                    return;
                }

                if (value <= 0)
                {
                    _page = 1;

                    return;
                }

                _page = value;
            }
        }

        public string[] Type { get; set; }

        public string Query { get; set; }

        public string Error { get; set; }

        public string ItemType { get; set; }

        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)NumberOfResults / PageSize); }
        }

        public string SortBy { get; set; }
        public SortDirection OrderBy { get; set; }

        public string BuildSearchQueryString(bool append, HashSet<string> omitParams = null)
        {
            var sb = new StringBuilder();
            var isFirst = true;

            sb.Append(append ? "&" : "?");

            if (omitParams == null || !omitParams.Contains("query"))
                if (!string.IsNullOrEmpty(Query))
                {
                   /* if (!isFirst)
                        sb.Append("&");*/

                    sb.Append("query=");
                    sb.Append(Query);
                    isFirst = false;
                }

            if (omitParams == null || !omitParams.Contains("page"))
                if (Page.HasValue)
                {
                    if (!isFirst)
                        sb.Append("&");

                    sb.Append("page=");
                    sb.Append(Page.Value);
                    isFirst = false;
                }

            if (omitParams == null || !omitParams.Contains("sortby"))
                if (!string.IsNullOrEmpty(SortBy))
                {
                    if (!isFirst)
                        sb.Append("&");

                    sb.Append("sortby=");
                    sb.Append(SortBy);
                    isFirst = false;
                }

            if (omitParams == null || !omitParams.Contains("pagesize"))
            {
                if (!isFirst)
                {
                    sb.Append("&");
                    isFirst = false;
                }
                sb.Append("pagesize=");
                sb.Append(PageSize);
            }

            if (omitParams == null || !omitParams.Contains("orderby"))
            {
                if (!isFirst)
                {
                    sb.Append("&");
                    isFirst = false;
                }
                sb.Append("orderby=");
                sb.Append(OrderBy);
            }
            return sb.ToString();
        }

        public string BuildSearchQueryString(string field, SortDirection direction, HashSet<string> omitParams = null)
        {
            var sb = new StringBuilder();
            var isFirst = true;

            sb.Append("?");
            if (omitParams == null || !omitParams.Contains("query"))
                if (!string.IsNullOrEmpty(Query))
                {
                  /*  if (!isFirst)
                        sb.Append("&");*/

                    sb.Append("query=");
                    sb.Append(Query);
                    isFirst = false;
                }

            if (omitParams == null || !omitParams.Contains("page"))
                if (Page.HasValue)
                {
                    if (!isFirst)
                        sb.Append("&");

                    sb.Append("page=");
                    sb.Append(Page.Value);
                    isFirst = false;
                }

            if (omitParams == null || !omitParams.Contains("sortby"))
                if (!string.IsNullOrEmpty(SortBy))
                {
                    if (!isFirst)
                        sb.Append("&");

                    sb.Append("sortby=");
                    sb.Append(field);
                    isFirst = false;
                }


            if (omitParams == null || !omitParams.Contains("pagesize"))
            {
                if (!isFirst)
                {
                    sb.Append("&");
                    isFirst = false;
                }
                sb.Append("pagesize=");
                sb.Append(PageSize);
            }

            if (omitParams == null || !omitParams.Contains("orderby"))
            {
                if (!isFirst)
                {
                    sb.Append("&");
                    isFirst = false;
                }
                sb.Append("orderby=");
                sb.Append(direction);
            }
            return sb.ToString();
        }

    }
}
