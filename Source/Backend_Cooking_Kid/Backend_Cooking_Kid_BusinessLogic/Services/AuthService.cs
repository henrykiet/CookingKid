using Backend_Cooking_Kid_BusinessLogic.DTOs.Requests;
using Backend_Cooking_Kid_BusinessLogic.DTOs.Response;
using Backend_Cooking_Kid_BusinessLogic.Helps;
using Backend_Cooking_Kid_DataAccess.Repositories;
using System.Dynamic;

namespace Backend_Cooking_Kid_BusinessLogic.Services
{
	public interface IAuthService
	{
		Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request);
		Task<ServiceResponse<object>> GetAllAsync();
    }
    public class AuthService : IAuthService
    {
        private readonly IBaseRepository<ExpandoObject> _repository;
        public AuthService(IBaseRepository<ExpandoObject> repository)
        {
            _repository = repository;
        }
        public Task<ServiceResponse<object>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
