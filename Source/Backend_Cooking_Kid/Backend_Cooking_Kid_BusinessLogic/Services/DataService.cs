using Backend_Cooking_Kid_BusinessLogic.DTOs;
using Backend_Cooking_Kid_BusinessLogic.DTOs.Requests;
using Backend_Cooking_Kid_BusinessLogic.Helps;
using Backend_Cooking_Kid_DataAccess.Repositories;
using Newtonsoft.Json;
using System.Dynamic;

namespace Backend_Cooking_Kid_BusinessLogic.Services
{
    public interface IDataService
    {
        Task<ServiceResponse<object>> GetFormMetadataAsync(MetadataRequest? request);
        Task<ServiceResponse<object>> UpdateFormMetadataAsync();
    }
    public class DataService : IDataService
    {
        private readonly IBaseRepository<ExpandoObject> _repository;
        public DataService(IBaseRepository<ExpandoObject> baseRepository)
        {
            _repository = baseRepository;
        }

        public async Task<ServiceResponse<object>> GetFormMetadataAsync(MetadataRequest? request)
        {
            var result = new ServiceResponse<object>();
            var controller = "";
            if (request != null && request.Controller != null)
            {
                controller = request.Controller;
                var formResult = await _repository.GetTemplateMetadataAsync(controller);
                if (formResult == null)
                {
                    result.Success = false;
                    result.Message = "Không tìm thấy dữ liệu";
                    result.StatusCode = 404;
                }
                else
                {
                    result.Data = formResult;
                    result.Message = "Lấy dữ liệu thành công";
                    result.StatusCode = 200;
                }
            }
            return result;
        }

        public async Task<ServiceResponse<object>> UpdateFormMetadataAsync()
        {
            var result = new ServiceResponse<object>();
            var controller = "update";
            var formResult = await _repository.GetTemplateMetadataAsync(controller);
            if(formResult == null)
            {
                result.Success = false;
                result.Message = "Không tìm thấy dữ liệu";
                result.StatusCode = 404;
            }
            else
            {
                result.Data = formResult;
                result.Message = "Lấy dữ liệu thành công";
                result.StatusCode = 200;
            }
            return result;
        }
    }
}
