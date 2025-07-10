using Backend_Cooking_Kid_BusinessLogic.Helps;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Backend_Cooking_Kid_DataAccess.ValidateConverts
{
	//public class ExceptionHandler
	//{
	//	private readonly RequestDelegate _next;
	//	private readonly ILogger<ExceptionHandler> _logger;

	//	public ExceptionHandler(RequestDelegate next , ILogger<ExceptionHandler> logger)
	//	{
	//		_next = next;
	//		_logger = logger;
	//	}

	//	public async Task Invoke(HttpContext context)
	//	{
	//		try
	//		{
	//			await _next(context);
	//		}
	//		catch ( ExceptionFormat ex )
	//		{
	//			context.Response.StatusCode = 400;
	//			context.Response.ContentType = "application/json";
	//			var result = new ServiceResponse<object>
	//			{
	//				Success = false ,
	//				Message = string.Join("; " , ex.Errors) ,
	//				Data = null
	//			};
	//			await context.Response.WriteAsync(JsonSerializer.Serialize(result));
	//		}
	//		catch ( Exception ex )
	//		{
	//			_logger.LogError(ex , "Unhandled exception");
	//			context.Response.StatusCode = 500;
	//			context.Response.ContentType = "application/json";
	//			var result = new ServiceResponse<object>
	//			{
	//				Success = false ,
	//				Message = "Lỗi hệ thống" ,
	//				Data = null
	//			};
	//			await context.Response.WriteAsync(JsonSerializer.Serialize(result));
	//		}
	//	}
	//}
	public static class ExceptionHandler
	{
		public static ServiceResponse<T> Handle<T>(Exception ex, string serviceName)
		{
			if ( ex is ExceptionFormat ef )
			{
				var message = ef.Errors.Count > 1 ? string.Join("; " , ef.Errors) : ef.Message;
				return ServiceResponse<T>.CreateError($"{serviceName} error " + message);
			}
			return ServiceResponse<T>.CreateError($"{serviceName} error " + ex.Message);
		}
	}

}
