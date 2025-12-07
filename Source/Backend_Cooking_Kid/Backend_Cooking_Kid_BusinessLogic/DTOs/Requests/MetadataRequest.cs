using Backend_Cooking_Kid_DataAccess;

namespace Backend_Cooking_Kid_BusinessLogic.DTOs.Requests
{
    public class MetadataRequest
    {
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public Dictionary<string, object>? PkValue { get; set; }
        public bool? IsPartition { get; set; }
        public string? Partition { get; set; }
    }

    public class UpdateMetadataRequest
    {
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public Dictionary<string, object>? PkValue { get; set; }
        public bool? IsPartition { get; set; }
        public string? Partition { get; set; }
        public object? InitialDatas { get; set; } = new Object();
    }
}
