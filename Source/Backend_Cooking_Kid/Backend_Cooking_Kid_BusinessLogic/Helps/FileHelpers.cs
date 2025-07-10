using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Cooking_Kid_BusinessLogic.Helps
{
	public class FileHelpers
	{
		/// <summary>
		/// Hàm đọc csv sang data table 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static DataTable ReadCsvToDataTable(IFormFile file)
		{
			var table = new DataTable();

			using var reader = new StreamReader(file.OpenReadStream());
			using var parser = new TextFieldParser(reader)
			{
				TextFieldType = FieldType.Delimited ,
				HasFieldsEnclosedInQuotes = true
			};
			parser.SetDelimiters(",");

			bool isFirstRow = true;
			int columnCount = 0;

			while ( !parser.EndOfData )
			{
				string[] fields = parser.ReadFields() ?? Array.Empty<string>();

				if ( isFirstRow )//header
				{
					foreach ( var header in fields )
						table.Columns.Add(header.Trim().ToLower());
					columnCount = table.Columns.Count;
					isFirstRow = false;
				}
				else//rows
				{
					// Normalize số lượng cột
					var normalized = new string[columnCount];
					Array.Copy(fields , normalized , Math.Min(fields.Length , columnCount));
					//check row null
					if ( normalized.All(string.IsNullOrWhiteSpace) )
						continue;
					table.Rows.Add(normalized);
				}
			}
			return table;
		}

		/// <summary>
		/// Hàm đọc excel sang data table 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static DataTable ReadXlsxToDataTable(IFormFile file)
		{
			using var stream = file.OpenReadStream();
			using var reader = ExcelReaderFactory.CreateReader(stream);
			var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
			{
				ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
				{
					UseHeaderRow = true
				}
			});
			return dataSet.Tables[0];
		}


		/// <summary>
		/// Hàm convert từ dictionary sang data table 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static DataTable ConvertToDataTable(List<Dictionary<string , object>> data)
		{
			var dt = new DataTable();

			if ( data == null || data.Count == 0 )
				return dt;

			foreach ( var key in data[0].Keys )
				dt.Columns.Add(key);

			foreach ( var dict in data )
			{
				var row = dt.NewRow();
				foreach ( var key in dict.Keys )
				{
					row[key] = dict[key] ?? DBNull.Value;
				}
				dt.Rows.Add(row);
			}

			return dt;
		}
	}
}
