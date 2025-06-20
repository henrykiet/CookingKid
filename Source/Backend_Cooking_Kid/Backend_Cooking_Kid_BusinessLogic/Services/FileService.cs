using Backend_Cooking_Kid_BusinessLogic.DTOs.Requests;
using Backend_Cooking_Kid_BusinessLogic.Helps;
using Backend_Cooking_Kid_DataAccess.Repositories;
using Backend_Cooking_Kid_DataAccess.ValidateConverts;
using System.Data;
using System.Dynamic;

namespace Backend_Cooking_Kid_BusinessLogic.Services
{
	public interface IFileService
	{
		Task<ServiceResponse<object>> ImportFileAsync(FileRequest fileRequest);
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
				var sqlDefinition = await _repository.GetTemplateByTableNameAsync(fileRequest.TableName!);
				//read file
				DataTable table = extension switch
				{
					".csv" => FileHelpers.ReadCsvToDataTable(fileRequest.File),
					".xlsx" or ".xsl" => FileHelpers.ReadXlsxToDataTable(fileRequest.File),
					_ => throw new InvalidOperationException("Unsupported file format.")
				};
				//check col table
				var newTableMap = _repository.CheckExcelColumnMapping(table , sqlDefinition.ExcelIntegration);
				if ( newTableMap.Item2 != null && newTableMap.Item2.Count > 0 )
				{
					result.Success = true;
					result.Message = string.Join(", " , newTableMap.Item2.ToArray()) + " Please check and fix your excel!";
					return result;
				}
				//call repo
				var upsertResult = await _repository.UpsertAsync(newTableMap.Item1 , sqlDefinition);
				result.Success = true;
				result.Message = "File import successfully";
			}
			catch ( ValidateFortmat.ValidationException ex )
			{
				result.Success = false;
				result.Message = ex.Message;
				result.Data = ex.Errors; // Trả về list lỗi đầy đủ
			}
			catch ( Exception ex )
			{
				result.Success = false;
				result.Message = $"Import failed: {ex.Message}";
			}
			return result;
		}
	}
}
