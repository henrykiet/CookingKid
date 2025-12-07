
using System.Text.Json.Serialization;

namespace Backend_Cooking_Kid_DataAccess
{
    public class FormDefination
    {
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Partition { get; set; } = string.Empty;
        public bool? IsPartition { get; set; } = false;
        public IForm? Form { get; set; }
    }
    public class IForm
    {
        public string TitleForm { get; set; } = string.Empty;
        public string? TypeForm { get; set; }
        public string? ClassForm { get; set; }
        public string? Style { get; set; }
        public string? TableName { get; set; }
        public List<string>? PrimaryKey { get; set; }
        public Dictionary<string, object>? PrimarykeyValue { get; set; }
        //public bool? IsReadOnly { get; set; }
        //public bool? IsHidden { get; set; }
        [JsonPropertyName("buttonControls")]
        public List<ButtonControl>? ButtonControls { get; set; }
        [JsonPropertyName("fieldControls")]
        public List<FieldControl>? FieldControls { get; set; }
        [JsonPropertyName("detailForms")]
        public List<IForm>? DetailForms { get; set; }
        [JsonPropertyName("initialDatas")]
        public object? InitialDatas { get; set; }
    }
    public class FieldControl
    {
        public string? Name { get; set; }
        public string? Label { get; set; }
        public string? Value { get; set; }
        public string? Type { get; set; }
        public List<string>? IsView { get; set; }
        public LookupDef? Lookup { get; set; }
        public class LookupDef
        {
            public string? Controller { get; set; }
            public string? pk { get; set; }
            public string? pkValue { get; set; }
        }
        [JsonPropertyName("options")]
        public List<Option>? Options { get; set; }
        public class Option
        {
            public string? Value { get; set; }
            public string? Id { get; set; }
        }
        [JsonPropertyName("radioOptions")]
        public List<string>? RadioOptions { get; set; }
        public string? PlaceHolder { get; set; }
        public string? Class { get; set; }
        public string? Style { get; set; }
        public bool? IsReadOnly { get; set; }
        public bool? IsDisabled { get; set; }
        public bool? IsHidden { get; set; }
        [JsonPropertyName("validators")]
        public List<Validator>? Validators { get; set; }


        public class Validator
        {
            public string? ValidatorName { get; set; }
            public bool? Required { get; set; }
            public string? Message { get; set; }
            public string? Minlength { get; set; }
            public string? Maxlengthh { get; set; }
            public string? Email { get; set; }
            public string? Pattern { get; set; }
        }
    }
    public class ButtonControl
    {
        public string? Name { get; set; }
        public string? Label { get; set; }
        public string? Type { get; set; }
        public string? Class { get; set; }
        public string? Style { get; set; }
        public string? Click { get; set; }
        public bool? IsReadOnly { get; set; }
        public bool? IsDisabled { get; set; }
        public bool? IsHidden { get; set; }
    }
}
