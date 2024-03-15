using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tool_Facebook.Data;

namespace Tool_Facebook.Respository
{
	public abstract class ICrudRepository<T> where T : class
	{
		private SQLiteConnection _con;
		public ICrudRepository()
		{
			createConnection();
		}
		public void createConnection()
		{
			_con = new SQLiteConnection();
			_con.ConnectionString = $"Data Source=input/Account.sqllite;Version=3;";
			_con.Open();
		}
		protected virtual void Add(T entity)
		{
			MessageBox.Show("OK!");
		}
		protected virtual async Task Update(T entity)
		{
			using (var context = new ApplicationDbContext())
			{
				await context.SaveChangesAsync();
			}
			MessageBox.Show("OK!");
		}
		protected virtual async Task BulkUpdate(IEnumerable<T> entity)
		{
			using (var context = new ApplicationDbContext())
			{
				await context.SaveChangesAsync();
			}
			MessageBox.Show("OK!");
		}
		protected virtual async Task Delete(T entity)
		{

		}
		protected virtual async Task BulkDelete(IEnumerable<T> entity)
		{

		}
		void excuteSQL(string sql)
		{
			try
			{
				var command = new SQLiteCommand(sql, _con);
				command.ExecuteNonQuery();
			}
			catch { }
		}
	}
}
