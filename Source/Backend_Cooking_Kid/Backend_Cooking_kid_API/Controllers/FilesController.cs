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
			if ( string.IsNullOrEmpty(fileRequest.TableName) )
			{
				return BadRequest("Controller name is requied.");
			}
			var result = await _fileService.ImportFileAsync(fileRequest);

			return StatusCode(result.Success == true ? 200 : 400 , new
			{
				message = result.Message ,
				data = result.Data
			});
		}
	}
}
