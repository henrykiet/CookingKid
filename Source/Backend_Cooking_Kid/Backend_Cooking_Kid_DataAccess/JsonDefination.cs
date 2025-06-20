namespace Backend_Cooking_Kid_DataAccess
{
	/// <summary>
	/// Class định nghĩa model dựa vào file json
	/// </summary>
	public class JsonDefination
	{
		public string Model { get; set; }
		public SqlSchema Schema { get; set; }
		public SqlDataProcessing DataProcessing { get; set; }
		public ExcelIntegrationMap ExcelIntegration { get; set; }
		public CheckingData Checking { get; set; }
		public class ExcelIntegrationMap
		{
			public string SheetName { get; set; }
			public List<ExcelColumnMapping> ColumnMapping { get; set; }
			public class ExcelColumnMapping
			{
				public string ExcelColumn { get; set; }
				public string FieldName { get; set; }
				public bool Required { get; set; }
				public string Default { get; set; }
			}
		}
		public class SqlSchema
		{
			public List<Field> Fields { get; set; }
			public class Field
			{
				public string Name { get; set; }
				public string Property { get; set; }
				public string Type { get; set; }
				public string SqlType { get; set; }
				public bool? PrimaryKey { get; set; } // optional
				public ForeignData? Foreign { get; set; }
				public class ForeignData
				{
					public string Table { get; set; }
					public string Key { get; set; }
				}
			}
		}
		public class SqlDataProcessing
		{
			public Statements SqlStatements { get; set; }
			public class Statements
			{
				public string Insert { get; set; }
				public string Update { get; set; }
				public string Delete { get; set; }
				public string GetAll { get; set; }
				public string GetById { get; set; }
				public string MyTableType { get; set; }
				public string DropTableType { get; set; }
			}
		}
		public class CheckingData
		{
			public List<Rule> Rules { get; set; }
			public class Rule
			{
				public string FieldName { get; set; }
				public string Type { get; set; }
				public string? Min { get; set; }
				public string? Max { get; set; }
				public string? MinLength { get; set; }
				public string? MaxLength { get; set; }
				public string? Pattern { get; set; }
				public string Message { get; set; }

				// Các thuộc tính dành cho databaseCheck
				public string? CheckQuery { get; set; }
				public string? Threshold { get; set; }
			}

		}
	}
}
