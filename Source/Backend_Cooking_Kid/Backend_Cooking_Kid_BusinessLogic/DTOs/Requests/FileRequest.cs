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
		public string Controller { get; set; } = string.Empty;
		public string? FilePath { get; set; } = string.Empty;
		public string? IsPdfOrExcel { get; set; } = string.Empty;
		public Dictionary<string , List<Dictionary<string , object>>>? Tables { get; set; } = new();
		public bool? IsReport { get; set; } = false;

	}
}
