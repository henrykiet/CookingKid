using Backend_Cooking_Kid_BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Backend_Cooking_kid_API.Controllers
{
    [Route("api/data")]
    [ApiController]
    public class DatasController : ControllerBase
    {
        private readonly IDataService _dataService;
        public DatasController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("form")]
        public async Task<IActionResult> GetFormMetadata()
        {
            var result = await _dataService.GetFormMetadataAsync();
            return Ok(result);
        }
    }
}
