namespace eMotive.CMS.Search.Objects
{
    public class ResultItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public float Score { get; set; }
    }
}
