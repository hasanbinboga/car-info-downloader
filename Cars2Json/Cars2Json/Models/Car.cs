namespace Cars2Json.Models
{
    public class Car
    {
        public Car()
        {
            PriceHistories = new List<PriceHistory>();
            GalleryUrls = new List<string>();
        }
        public string Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public List<string> GalleryUrls { get; set; }
        public double Price { get; set; }
        public List<PriceHistory> PriceHistories { get; set; }
        public bool IsGreatDeal { get; set; }
        public bool IsHomeDelivery { get; set; }
        public bool IsVirtualAppointments { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Trim { get; set; }
        public int Year { get; set; }
        public string Category { get; set; }
        public string SellerType { get; set; }
        public string CustomerId { get; set; }
        public string BodyStyle { get; set; }
        public string ExteriorColor { get; set; }
        public string InteriorColor { get; set; }
        public string Drivetrain { get; set; }
        public string FuelType { get; set; }
        public string Transmission { get; set; }
        public string Engine { get; set; }
        public string Vin { get; set; }
        public string StockType { get; set; }
        public double Mileage { get; set; }
        public string SellersNotes { get; set; }

        public double? PriceGoodThreshold { get; set; }
        public double? GreatThreshold { get; set; }
        public double? PredictedPrice { get; set; }
        public int? ListedDays { get; set; }
    }
}
