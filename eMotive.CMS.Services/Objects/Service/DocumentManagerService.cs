using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using eMotive.CMS.Extensions;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.DocumentManagerService;
using MySql.Data.MySqlClient;

namespace eMotive.CMS.Services.Objects.Service
{
    public class DocumentManagerService : IDocumentManagerService
    {
        private readonly IServiceRepository _serviceRepository;

        private readonly string _connectionString;

        public DocumentManagerService(IServiceRepository serviceRepository)
        {
            _connectionString = "";
            _serviceRepository = serviceRepository;
        }

        public IEnumerable<MimeType> Fetch()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                const string sql = "SELECT `extension`, `type` FROM `mimetypes`;";

                return connection.Query<MimeType>(sql);
            }
        }


        public MimeType FetchMimeTypeForExtension(string extension)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                const string sql = "SELECT `extension`, `type` FROM `mimetypes` WHERE `extension`=@extension;";

                var types = connection.Query<MimeType>(sql, new { extension = extension });

                if (!types.HasContent())
                {
                    var type = types.SingleOrDefault(n => String.Equals(n.Extension, extension, StringComparison.CurrentCultureIgnoreCase));

                    if (type != null)
                        return type;
                }

                return null;
            }
        }

        public IDictionary<string, MimeType> FetchMimeTypeDictionary()
        {
            throw new NotImplementedException();
        }

        public MimeType FetchDefaultMimeType()
        {
            throw new NotImplementedException();
        }

        public bool SaveDocumentInformation(Document document)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                const string sql = "INSERT INTO `FileUploads` (`name`,`location`,`extension`, `modifiedName`, `DateUploaded`, `UploadedByUsername`, `Reference`) VALUES (@Name, @Location, @Extension, @ModifiedName, @DateUploaded, @UploadedByUsername, @Reference);";

                return connection.Execute(sql, document) > 0;
            }
        }

        public IEnumerable<UploadLocation> FetchUploadLocationDictionary()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                const string sql = "SELECT `id`, `Reference`, `Directory` FROM `uploadLocations`;";

                var locations = connection.Query<UploadLocation>(sql);

                return locations;
            }
        }

        public UploadLocation FetchUploadLocation(string reference)
        {
            throw new NotImplementedException();
        }

        public Document FetchLastUploaded(string reference)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                const string sql = "SELECT `id`, `Name`, `Location`, `extension`, `modifiedName`, `DateUploaded`, `UploadedByUsername` FROM `fileUploads` WHERE `Reference`=@Reference ORDER BY `DateUploaded` DESC LIMIT 1;;";

                return connection.Query<Document>(sql, new { reference = reference }).SingleOrDefault();
            }
        }
    }
}
