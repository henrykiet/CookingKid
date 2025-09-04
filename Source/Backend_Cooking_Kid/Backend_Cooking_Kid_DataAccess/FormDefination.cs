
namespace Backend_Cooking_Kid_DataAccess
{
    public class FormDefination
    {
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string VCId { get; set; } = string.Empty;
        public string VCDate { get; set; } = string.Empty;
        public IForm? Form { get; set; }
        //public List<IForm>? ListForm { get; set; }
    }
    public class IForm
    {
        public string TitleForm { get; set; } = string.Empty;
        public string? TypeForm { get; set; }
        public string? ClassForm { get; set; }
        public string? Style { get; set; }
        public string? TableName { get; set; }
        public List<string>? PrimaryKey { get; set; }
        //public bool? IsReadOnly { get; set; }
        //public bool? IsHidden { get; set; }

        public List<ButtonControl>? ButtonControls { get; set; }
        public List<FieldControl>? FieldControls { get; set; }
        public List<IForm>? DetailForms { get; set; }
        public List<Dictionary<string,string>>? InitialDatas { get; set; }
    }
    public class FieldControl
    {
        public string? Name { get; set; }
        public string? Label { get; set; }
        public string? Value { get; set; }
        public string? Type { get; set; }
        public List<Option>? Options { get; set; }
        public class Option
        {
            public string? Value { get; set; }
            public string? Label { get; set; }
        }
        public List<string>? RadioOptions { get; set; }
        public string? PlaceHolder { get; set; }
        public string? Class { get; set; }
        public string? Style { get; set; }
        public bool? IsReadOnly { get; set; }
        public bool? IsDisabled { get; set; }
        public bool? IsHidden { get; set; }

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
