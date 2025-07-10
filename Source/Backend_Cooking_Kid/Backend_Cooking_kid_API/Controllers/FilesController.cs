using Azure.Core;
using Backend_Cooking_Kid_BusinessLogic.DTOs.Requests;
using Backend_Cooking_Kid_BusinessLogic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Cooking_kid_API.Controllers
{
	[Route("api/file")]
	[ApiController]
	public class FilesController : ControllerBase
	{
		private readonly IFileService _fileService;
		public FilesController(IFileService fileService)
		{
			_fileService = fileService;
		}
		[HttpPost("import")]
		public async Task<IActionResult> ImportFile([FromForm] FileRequest fileRequest)
		{
			if ( fileRequest == null || fileRequest.File == null )
			{
				return BadRequest("File is required.");
			}
			if ( string.IsNullOrEmpty(fileRequest.Controller) )
			{
				return BadRequest("Controller name is requied.");
			}
			var result = await _fileService.ImportFileAsync(fileRequest);

			return StatusCode(result.StatusCode , new
			{
				success = result.Success ,
				message = result.Message ,
				data = result.Data
			});
		}
		[HttpPost("export")]
		public async Task<IActionResult> ExportFile([FromBody] FileRequest fileRequest)
		{
			if ( string.IsNullOrEmpty(fileRequest.Controller) )
			{
				return BadRequest("Controller is required");
			}
			if ( fileRequest.Tables == null )
			{
				return BadRequest("Tables is null");
			}
			var fileName = $"{fileRequest.Controller.Trim().ToLower()}.frx";
			var frxFilePath = Path.Combine("FastReports" , fileName);
			// lấy đường dẫn vật lý chính xác:
			fileRequest.FilePath = Path.Combine(Directory.GetCurrentDirectory() , frxFilePath);
			var result = await _fileService.ExportFileAsync(fileRequest);
			if ( result.Success == true && result.Data != null )
			{
				var contentType = result.Message.Contains(".pdf") ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				// Gắn hậu tố dựa theo dạng báo cáo
				fileName = fileRequest.IsReport == true ? $"{result.Message}Report" : $"{result.Message}Table";
				return File(result.Data , contentType , fileName);
			}
			else
			{
				return BadRequest(result);
			}
		}
	}
}
