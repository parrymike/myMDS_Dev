namespace eMotive.CMS.Services.Objects.EmailService
{
    public class BinaryAttachment
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public byte[] Attachment { get; set; }
    }
}
