namespace Backend_Cooking_Kid_DataAccess
{
	/// <summary>
	/// Class định nghĩa model dựa vào file json trong entities 
	/// </summary>
	public class JsonDefination
	{
		public string Model { get; set; } = string.Empty;
		public SqlSchema Schema { get; set; } = new SqlSchema();
		public SqlDataProcessing DataProcessing { get; set; } = new SqlDataProcessing();
		public ExcelIntegrationMap ExcelIntegration { get; set; } = new ExcelIntegrationMap();
		public CheckingData Checking { get; set; } = new CheckingData();
		public class ExcelIntegrationMap
		{
			public string SheetName { get; set; } = string.Empty;
			public List<ExcelColumnMapping> ColumnMapping { get; set; } = new List<ExcelColumnMapping> { };
			public class ExcelColumnMapping
			{
				public string ExcelColumn { get; set; } = string.Empty ;
				public string FieldName { get; set; } = string.Empty;
				public bool Required { get; set; } 
				public string Default { get; set; } = string.Empty;
			}
		}
		public class SqlSchema
		{
			public List<Field> Fields { get; set; } = new List<Field> { };
			public class Field
			{
				public string Name { get; set; } = string.Empty;
				public string Property { get; set; } = string.Empty;
				public string Type { get; set; } = string.Empty;
				public string SqlType { get; set; } = string.Empty;
				public bool? PrimaryKey { get; set; } // optional
				public ForeignData? Foreign { get; set; }
				public class ForeignData
				{
					public string Table { get; set; } = string.Empty;
					public string Key { get; set; } = string.Empty;
				}
			}
		}
		public class SqlDataProcessing
		{
			public Statements SqlStatements { get; set; } = new Statements();
			public class Statements
			{
				public string Insert { get; set; } = string.Empty;
				public string Update { get; set; } = string.Empty;
				public string Delete { get; set; } = string.Empty;
				public string GetAll { get; set; } = string.Empty;
				public string GetById { get; set; } = string.Empty;
				public string MyTableType { get; set; } = string.Empty;
				public string DropTableType { get; set; } = string.Empty;
			}
		}
		public class CheckingData
		{
			public List<Rule> Rules { get; set; } = new List<Rule> { new Rule() };
			public class Rule
			{
				public string FieldName { get; set; } = string.Empty;
				public string Type { get; set; } = string.Empty;
				public string? Min { get; set; }
				public string? Max { get; set; }
				public string? MinLength { get; set; }
				public string? MaxLength { get; set; }
				public string? Pattern { get; set; }
				public string Message { get; set; } = string.Empty;

				// Các thuộc tính dành cho databaseCheck
				public string? CheckQuery { get; set; }
				public string? Threshold { get; set; }
			}

		}
	}
}
