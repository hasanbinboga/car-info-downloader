namespace Cars2Json.Models
{
    public class SearchCriterias
    {
        public SearchCriterias()
        {
            Cars = new List<Car>();
        }
        
        public string SearchMaxYear { get; set; }
        public string Transmission { get; set; }
        public string StockType { get; set; }
        public string Makes { get; set; }
        public string Models { get; set; }
        public double? MaxPrice { get; set; }
        public string Distance { get; set; }
        public string ZipCode { get; set; }
        public bool? HomeDelivery { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<Car> Cars { get; set; }
        public dynamic RawData { get; set; }
    }
}
