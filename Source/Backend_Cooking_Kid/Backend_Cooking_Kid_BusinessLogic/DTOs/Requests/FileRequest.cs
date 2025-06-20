using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Cooking_Kid_BusinessLogic.DTOs.Requests
{
	public class FileRequest
	{
		public IFormFile? File { get; set; }
		public string? TableName { get; set; }
	}
}
