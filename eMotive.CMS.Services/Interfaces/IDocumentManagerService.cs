using System.Collections.Generic;
using eMotive.CMS.Services.Objects.DocumentManagerService;

namespace eMotive.CMS.Services.Interfaces
{
    /// <summary>
    /// A series of methods used for http document manipulation
    /// </summary>
    public interface IDocumentManagerService
    {
        /// <summary>
        /// Fetches a MimeType for a given extension
        /// </summary>
        /// <param name="extension">A file extension</param>
        /// <returns>A MimeType object</returns>
        MimeType FetchMimeTypeForExtension(string extension);
        /// <summary>
        /// Fetches a dictionary of all MimeTypes. The key is the extension mapped to the related MimeType object
        /// </summary>
        /// <returns>A dictionary object containing MimeTypes</returns>
        IDictionary<string, MimeType> FetchMimeTypeDictionary();
        /// <summary>
        /// Fetches a default MimeType hich can be used in cases where a MimeType mapping could not be found
        /// </summary>
        /// <returns>A MimeType object</returns>
        MimeType FetchDefaultMimeType();

        bool SaveDocumentInformation(Document document);

        //IDictionary<string, UploadLocation> FetchUploadLocationDictionary();
        UploadLocation FetchUploadLocation(string reference);

        Document FetchLastUploaded(string reference);
    }
}
