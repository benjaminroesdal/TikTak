using TikTakServer.ApplicationServices;

namespace TikTakServer.Models.Business
{
    public class LocationResult
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }

    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public List<string> types { get; set; }
    }

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }
}
