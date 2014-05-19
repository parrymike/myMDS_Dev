using System.Collections.Generic;
using System.Data;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Repositories.Objects.Pages;
using MySql.Data.MySqlClient;

namespace eMotive.CMS.Repositories.Objects.Repository
{
    public class PageRepository : IPageRepository
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public PageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal IDbConnection Connection
        {
            get
            {
                return _connection ?? new MySqlConnection(_connectionString);
            }
        }

        public Page New()
        {
            return new Page();
        }

        public bool Create(Page page, out int id)
        {
            throw new System.NotImplementedException();
        }

        public bool Put(Page page)
        {
            throw new System.NotImplementedException();
        }

        public bool Update(Page page)
        {
            throw new System.NotImplementedException();
        }

        public bool Delete(Page page)
        {
            throw new System.NotImplementedException();
        }

        public Page FetchPage(int id)
        {
            throw new System.NotImplementedException();
        }

        public Page FetchPage(string name)
        {
            throw new System.NotImplementedException();
        }

        public bool Create(Section section)
        {
            throw new System.NotImplementedException();
        }

        public bool Put(Section section)
        {
            throw new System.NotImplementedException();
        }

        public bool Update(Section section)
        {
            throw new System.NotImplementedException();
        }

        public bool Delete(Section section)
        {
            throw new System.NotImplementedException();
        }

        public Section FetchSection(int id)
        {
            throw new System.NotImplementedException();
        }

        public Section FetchSection(string name)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Section> Fetch()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Section> Fetch(IEnumerable<int> ids)
        {
            throw new System.NotImplementedException();
        }
    }
}
