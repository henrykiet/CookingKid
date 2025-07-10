namespace Backend_Cooking_Kid_DataAccess.ValidateConverts
{
	public class ExceptionFormat : Exception
	{
		public List<string> Errors { get; set; }
		
		/// <summary>
		/// constructor cho string error 
		/// </summary>
		/// <param name="message"></param>
		public ExceptionFormat(string message) : base(message)
		{
			Errors = new List<string>() { message };
		}

		/// <summary>
		/// constructor cho list error 
		/// </summary>
		/// <param name="title"></param>
		/// <param name="errors"></param>
		public ExceptionFormat(string title, List<string> errors) : base(title)
		{
			if ( errors == null || errors.Count == 0 )
			{
				Errors = new List<string> { title };
			}
			else
			{
				Errors = errors.Select(error => $"{title} {error}").ToList();
			}
		}
	}
}
