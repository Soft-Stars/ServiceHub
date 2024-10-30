namespace ServiceHub.Models
{
    public class LogActivityRequest
    {
        public string ClientId { get; set; }
        public string Action { get; set; }
        public string Comment { get; set; }
    }
}
