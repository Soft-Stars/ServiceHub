namespace ServiceHub.Data
{
    public class Activity
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string? ConnectionId { get; set; }
        public string? Action { get; set; }
        public string? Comment { get; set; }
        public DateTime? Date { get; set; }
    }
}
