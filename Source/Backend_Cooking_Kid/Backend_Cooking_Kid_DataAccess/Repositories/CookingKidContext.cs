using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Cooking_Kid_DataAccess.Repositories
{
	public partial class CookingKidContext : DbContext
	{
		public CookingKidContext(DbContextOptions<CookingKidContext> context) : base (context)
		{
			
		}
	}
}
