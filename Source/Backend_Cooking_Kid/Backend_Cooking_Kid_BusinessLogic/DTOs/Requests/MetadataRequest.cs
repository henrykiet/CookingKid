namespace Backend_Cooking_Kid_BusinessLogic.DTOs.Requests
{
    public class MetadataRequest
    {
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public Dictionary<string, string>? PkValue { get; set; }
        public string? vCId { get; set; }
        public string? vCDate { get; set; }
    }
}
