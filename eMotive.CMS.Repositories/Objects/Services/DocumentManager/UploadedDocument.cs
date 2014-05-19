using System;
using eMotive.CMS.Repositories.Objects.Users;

namespace eMotive.CMS.Repositories.Objects.Services.DocumentManager
{
    public class UploadedDocument
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Extension { get; set; }
        public string ModifiedName { get; set; }
        public DateTime DateUploaded { get; set; }
        public string UploadedByUsername { get; set; }
        public User UploadedBy { get; set; }
        public string Reference { get; set; }
    }
}
