using Backend_Cooking_Kid_BusinessLogic.DTOs.Requests;
using Backend_Cooking_Kid_BusinessLogic.Helps;
using Backend_Cooking_Kid_DataAccess.Repositories;
using Backend_Cooking_Kid_DataAccess.ValidateConverts;
using FastReport;
using FastReport.Data;
using FastReport.Export.Pdf;
using FastReport.Table;
using FastReport.Utils;
using System.Data;
using System.Dynamic;
using System.Xml.Linq;

namespace Backend_Cooking_Kid_BusinessLogic.Services
{
	public interface IFileService
	{
		Task<ServiceResponse<object>> ImportFileAsync(FileRequest fileRequest);
		Task<ServiceResponse<MemoryStream>> ExportFileAsync(FileRequest request);
	}
	public class FileService : IFileService
	{
		private readonly IBaseRepository<ExpandoObject> _repository;
		public FileService(IBaseRepository<ExpandoObject> baseRepository)
		{
			_repository = baseRepository;
		}

		public async Task<ServiceResponse<object>> ImportFileAsync(FileRequest fileRequest)
		{
			var result = new ServiceResponse<object>();
			try
			{
				var extension = Path.GetExtension(fileRequest.File!.FileName).ToLower();
				var sqlDefinition = await _repository.GetTemplateModelAsync(fileRequest.Controller!);
				//read file
				DataTable table = extension switch
				{
					".csv" => FileHelpers.ReadCsvToDataTable(fileRequest.File),
					".xlsx" or ".xsl" => FileHelpers.ReadXlsxToDataTable(fileRequest.File),
					_ => throw new ExceptionFormat("Unsupported file format.")
				};
				//check col table
				var newTableMap = _repository.CheckExcelColumnMapping(table , sqlDefinition.ExcelIntegration);
				//call repo
				var upsertResult = await _repository.UpsertAsync(newTableMap , sqlDefinition);
				result.Success = true;
				result.Message = "File import successfully";
			}
			catch ( Exception ex )
			{
				return ExceptionHandler.Handle<object>(ex , "Import");
			}
			return result;
		}

		public Task<ServiceResponse<MemoryStream>> ExportFileAsync(FileRequest request)
		{
			var response = new ServiceResponse<MemoryStream>();
			var isExport = request.IsPdfOrExcel;
			string fileName = "";
			try
			{
				using var report = new Report();
				string filePath = request.FilePath!;
				report.Load(filePath);

				// Tạo DataSet
				var dataSet = new DataSet("ReportData");
				//tạo dataSource dựa vào dataSet

				foreach ( var tableEntry in request.Tables! )
				{
					var tableName = tableEntry.Key;
					var data = tableEntry.Value;

					var dataTable = FileHelpers.ConvertToDataTable(data);
					dataTable.TableName = tableName;
					dataSet.Tables.Add(dataTable);
				}
				//tạo tên file dựa theo các bảng có
				fileName = dataSet.Tables.Count > 0 ? string.Join("_" , dataSet.Tables.Cast<DataTable>().Select(t => t.TableName)) : "Export";

				// Đăng ký toàn bộ DataSet vào report
				report.RegisterData(dataSet , "ReportData" , true);

				//tạo các datasource dựa vào dataset
				CreateDataSourceByDataSet(report , dataSet);

				//Nếu là dạng report thì phải đủ datasource trong frx
				if ( request.IsReport == true )
				{
					//đọc frx và lấy ra thông tin data source
					var dataSource = GetTablesFromDataSource(filePath);
					//nếu là dạng report có nghĩa là query đến nhiều bảng
					//kiểm tra xem dữ liệu đưa về có đủ không 
					foreach ( var kvp in dataSource )
					{
						var tableName = kvp.Key.ToLower();
						var expectedCols = kvp.Value.Select(col => col.ToLower()).ToList();
						var matchingKey = request.Tables.Keys.FirstOrDefault(k => k.Equals(tableName , StringComparison.OrdinalIgnoreCase));
						if ( matchingKey == null )
							throw new ExceptionFormat($"Missing table '{tableName}' in request");

						var actualRows = request.Tables[tableName];
						if ( actualRows.Count == 0 )
							continue;

						var actualCols = actualRows[0].Keys.Select(k => k.ToLower()).ToList();
						var missingCols = expectedCols.Except(actualCols).ToList();
						if ( missingCols.Any() )
							throw new ExceptionFormat($"Table '{tableName}' missing column: {string.Join(", " , missingCols)}");
					}
				}
				else
				{
					//trường hợp là dạng (bảng) chỉ in ra list
					AddDynamicBandsAndTables(report , dataSet);
				}

				// Bật dữ liệu trong data set
				foreach ( DataTable table in dataSet.Tables )
				{
					var source = report.GetDataSource(table.TableName);
					if ( source != null )
						source.Enabled = true;
				}

				// Prepare
				report.Prepare();
				using var ms = new MemoryStream();

				string exportType = isExport?.Trim().ToLowerInvariant() ?? "";
				//chọn phương thức export
				if ( exportType == "pdf" )
				{
					var pdfExport = new PDFExport();
					report.Export(pdfExport , ms);
					response.Message = $"{fileName}.pdf";
				}
				else if ( exportType == "excel" )
				{
					var excelExport = new FastReport.Export.OoXML.Excel2007Export
					{
						ShowProgress = false ,
					};
					report.Export(excelExport , ms);
					response.Message = $"{fileName}.xlsx";
				}
				else
				{
					response.Success = false;
					response.Message = "Invalid export type. Only 'pdf' or 'excel' allowed.";
					return Task.FromResult(response);
				}
				ms.Position = 0;
				response.Data = ms;
				response.Success = true;
			}
			catch ( Exception ex )
			{
				return Task.FromResult(ExceptionHandler.Handle<MemoryStream>(ex , "Export"));
			}
			return Task.FromResult(response);
		}

		/// <summary>
		/// Hàm tạo dataSource dựa vào dataSet 
		/// </summary>
		/// <param name="report"></param>
		/// <param name="dataSet"></param>
		private void CreateDataSourceByDataSet(Report report , DataSet dataSet)
		{
			foreach ( DataTable table in dataSet.Tables )
			{
				var ds = report.GetDataSource(table.TableName);
				if ( ds == null )
				{
					var dict = report.Dictionary;
					var newTable = new TableDataSource
					{
						Name = table.TableName ,
						ReferenceName = $"ReportData.{table.TableName}" ,
						Enabled = true
					};
					foreach ( DataColumn col in table.Columns )
					{
						newTable.Columns.Add(new Column
						{
							Name = col.ColumnName ,
							DataType = col.DataType ,
						});
					}
					dict.DataSources.Add(newTable);
				}
			}
		}

		/// <summary>
		/// Hàm tạo form export nếu là dạng list 
		/// </summary>
		/// <param name="report"></param>
		/// <param name="dataSet"></param>
		private void AddDynamicBandsAndTables(Report report , DataSet dataSet)
		{
			// Xoá các trang cũ
			var clonedHeader = new PageHeaderBand();
			if ( report.Pages.Count > 0 )
			{
				var firstPage = report.Pages[0] as ReportPage;
				var originPageHeader = firstPage?.PageHeader;
				if ( originPageHeader != null )
				{
					clonedHeader = ClonePageHeader(originPageHeader);
				}
				for ( int i = report.Pages.Count - 1; i >= 0; i-- )
				{
					report.Pages.RemoveAt(i);
				}
			}

			foreach ( DataTable table in dataSet.Tables )
			{
				var page = new ReportPage
				{
					Name = $"Page_{table.TableName}"
				};
				report.Pages.Add(page);

				string tableName = table.TableName;
				string dataBandName = $"Data_{tableName}";
				if ( report.FindObject(dataBandName) != null ) continue;

				// Tách hàm tính chiều rộng, trả về layout
				var layout = CalculateTableLayout(page , table);

				float top = 0;
				float rowHeight = Units.Centimeters * 0.9f;

				// --- GroupHeader ---
				if ( clonedHeader != null )
				{
					var headerClone = ClonePageHeader(clonedHeader);
					var textObj = headerClone.Objects.OfType<TextObject>().FirstOrDefault();
					if ( textObj != null )
						textObj.Text = table.TableName.ToUpper();
					page.PageHeader = headerClone;
					top += headerClone.Height;
				}

				// --- Header Band ---
				var headerBand = new DataBand
				{
					Name = $"Header_{tableName}" ,
					Top = top ,
					Height = rowHeight ,
					CanGrow = true
				};

				var headerTable = new TableObject
				{
					Name = $"HeaderTable_{tableName}" ,
					Width = layout.TotalWidth ,
					Height = rowHeight ,
					Left = layout.LeftOffset
				};

				var headerRow = new TableRow
				{
					Height = rowHeight ,
					AutoSize = true
				};

				headerRow.AddChild(new TableCell
				{
					Text = "STT" ,
					Border = { Lines = BorderLines.All } ,
					HorzAlign = HorzAlign.Center ,
					VertAlign = VertAlign.Center
				});

				foreach ( DataColumn col in table.Columns )
				{
					headerRow.AddChild(new TableCell
					{
						Text = col.ColumnName ,
						Border = { Lines = BorderLines.All } ,
						HorzAlign = HorzAlign.Center ,
						VertAlign = VertAlign.Center
					});
				}

				headerTable.Columns.AddRange(layout.ColumnWidths.Select(w => new TableColumn { Width = w }).ToArray());
				headerTable.Rows.Add(headerRow);
				headerBand.Objects.Add(headerTable);
				page.Bands.Add(headerBand);
				top += rowHeight;

				// --- Data Band ---
				var dataBand = new DataBand
				{
					Name = dataBandName ,
					DataSource = report.GetDataSource(tableName) ,
					Top = top ,
					Height = rowHeight ,
					CanGrow = true
				};

				var dataTable = new TableObject
				{
					Name = $"Table_{tableName}" ,
					Width = layout.TotalWidth ,
					Height = rowHeight ,
					Left = layout.LeftOffset
				};

				var dataRow = new TableRow
				{
					Height = rowHeight ,
					AutoSize = true ,
				};

				dataRow.AddChild(new TableCell
				{
					Text = "[Row#]" ,
					Border = { Lines = BorderLines.All } ,
					HorzAlign = HorzAlign.Center ,
					VertAlign = VertAlign.Center
				});

				foreach ( DataColumn col in table.Columns )
				{
					dataRow.AddChild(new TableCell
					{
						Text = $"[{tableName}.{col.ColumnName}]" ,
						Border = { Lines = BorderLines.All } ,
						HorzAlign = HorzAlign.Center ,
						VertAlign = VertAlign.Center ,
					});
				}

				dataTable.Columns.AddRange(layout.ColumnWidths.Select(w => new TableColumn { Width = w }).ToArray());
				dataTable.Rows.Add(dataRow);
				dataBand.Objects.Add(dataTable);
				page.Bands.Add(dataBand);
			}
		}

		/// <summary>
		/// Hàm khai báo các dữ liệu cần thiết của 1 layour trong page 
		/// </summary>
		private class TableLayoutInfo
		{
			public List<float> ColumnWidths { get; set; } = new();
			public float TotalWidth { get; set; }
			public float LeftOffset { get; set; }
		}

		/// <summary>
		/// Hàm tính layour của form 
		/// </summary>
		/// <param name="page"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		private TableLayoutInfo CalculateTableLayout(ReportPage page , DataTable table)
		{
			float sttWidth = Units.Centimeters * 2.0f;
			float minCol = Units.Centimeters * 2.0f;
			float maxCol = Units.Centimeters * 4.0f;
			float minMargin = Units.Centimeters * 1.0f;

			// Khổ giấy
			float pageWidth = Units.Millimeters * page.PaperWidth;
			float pageHeight = Units.Millimeters * page.PaperHeight;

			// Landscape nếu nhiều cột
			if ( table.Columns.Count > 6 )
			{
				page.Landscape = true;
				(pageWidth, pageHeight) = (pageHeight, pageWidth);
			}

			// Lề
			float leftMargin = Units.Millimeters * page.LeftMargin;
			float rightMargin = Units.Millimeters * page.RightMargin;
			float printableWidth = pageWidth - leftMargin - rightMargin;

			// Tính độ rộng các cột
			int maxLen = table.Columns.Cast<DataColumn>().Max(c => c.ColumnName.Length);
			List<float> columnWidths = new() { sttWidth };

			foreach ( var col in table.Columns.Cast<DataColumn>() )
			{
				float ratio = (float)col.ColumnName.Length / Math.Max(maxLen , 1);
				float colWidth = minCol + (maxCol - minCol) * ratio;
				columnWidths.Add(colWidth);
			}

			float rawWidth = columnWidths.Sum();

			// Nếu bảng quá rộng, scale lại
			if ( rawWidth > printableWidth - minMargin * 2 )
			{
				float scale = (printableWidth - minMargin * 2) / rawWidth;
				for ( int i = 0; i < columnWidths.Count; i++ )
					columnWidths[i] *= scale;
				rawWidth = columnWidths.Sum();
			}

			// Căn bảng sao cho:
			// Khoảng cách từ lề trái đến cột STT == khoảng cách từ cột cuối đến mép phải
			float leftOffset = leftMargin + (printableWidth - rawWidth - sttWidth);

			return new TableLayoutInfo
			{
				ColumnWidths = columnWidths ,
				TotalWidth = rawWidth ,
				LeftOffset = leftOffset
			};
		}

		/// <summary>
		/// Hàm lấy lại page header
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		PageHeaderBand ClonePageHeader(PageHeaderBand source)
		{
			var count = 0;
			var clone = new PageHeaderBand
			{
				Name = $"pageHeader_{source.Name}" , // tránh trùng tên
				Height = source.Height ,
				Top = source.Top ,
				Width = source.Width
			};

			foreach ( var obj in source.Objects )
			{
				if ( obj is TextObject txt )
				{
					var newText = new TextObject
					{
						Name = $"textObject_{source.Name}" + count++ ,
						Bounds = txt.Bounds ,
						Text = txt.Text ,
						Font = txt.Font ,
						HorzAlign = txt.HorzAlign ,
						VertAlign = txt.VertAlign ,
						TextColor = txt.TextColor
					};
					clone.Objects.Add(newText);
				}
			}
			return clone;
		}

		/// <summary>
		/// Hàm lấy thông tin table cần thiết của data source trong frx 
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		private Dictionary<string , List<string>> GetTablesFromDataSource(string filePath)
		{
			var result = new Dictionary<string , List<string>>();
			var xDoc = XDocument.Load(filePath);
			var tables = xDoc.Descendants("TableDataSource");
			foreach ( var table in tables )
			{
				var tableName = table.Attribute("TableName")?.Value;
				if ( string.IsNullOrEmpty(tableName) ) continue;
				var columns = table.Elements("Column")
						   .Select(col => col.Attribute("Name")?.Value)
						   .Where(name => !string.IsNullOrEmpty(name))
						   .ToList();

				result[tableName] = columns!;
			}
			return result;
		}


	}
}
