﻿namespace Backend_Cooking_Kid_BusinessLogic.Helps
{
	/// <summary>
	/// Class defination response
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ServiceResponse<T>
	{
		public T? Data { get; set; }
		public bool Success { get; set; } = true;
		public string Message { get; set; } = string.Empty;
		public int StatusCode { get; set; } = 200;

		public static ServiceResponse<T> CreateSuccess(T data , string message = "Thành công")
		{
			return new ServiceResponse<T>
			{
				Data = data ,
				Success = true ,
				Message = message ,
				StatusCode = 200
			};
		}

		public static ServiceResponse<T> CreateError(string message , int statusCode = 400)
		{
			return new ServiceResponse<T>
			{
				Success = false ,
				Message = message ,
				StatusCode = statusCode
			};
		}
	}
}
