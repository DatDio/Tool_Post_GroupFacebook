using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Facebook.Data
{
	public class ApplicationDbContext: DbContext
	{
		public DbSet<tblPage> tblPages { get; set; }
		public DbSet<tblVia> tblVias { get; set; }
		string pathDatabase = Path.GetFullPath("input/Account.sqllite");
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			=> optionsBuilder.UseSqlite($"Data Source={pathDatabase}");
	}
}
