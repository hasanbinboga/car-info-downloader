namespace Cars2Json.Models
{
    public class CarFeatures
    {
        public CarFeatures()
        {
            Convenience = new List<string>();
            Entertainment = new List<string>();
            Exterior = new List<string>();
            Safety = new List<string>();
            Seating = new List<string>();
            AdditionalPopularFeatures = new List<string>();
        }
        public List<string> Convenience { get; set; }
        public List<string> Entertainment { get; set; }
        public List<string> Exterior { get; set; }
        public List<string> Safety { get; set; }
        public List<string> Seating { get; set; }
        public List<string> AdditionalPopularFeatures { get; set; }

    }
}
