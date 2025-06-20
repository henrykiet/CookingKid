namespace Backend_Cooking_Kid_DataAccess.ValidateConverts
{
	public class ValidateFortmat
	{
		public class ValidationException : Exception
		{
			public List<string> Errors { get; }

			public ValidationException(string message , List<string> errors)
				: base(message)
			{
				Errors = errors;
			}
		}
	}
}
