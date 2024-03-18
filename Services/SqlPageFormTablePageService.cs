using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tool_Facebook.Controller;
using Tool_Facebook.Model;

namespace Tool_Facebook.Services
{
	public class SqlPageFormTablePageService : SqlController
	{
		public void BulkInsert(List<PageModel> pageModels)
		{
			try
			{
				using (var transaction = _con.BeginTransaction())
				{

					_con.Execute(@"
                INSERT INTO tblPage(C_UIDVia,C_IDPage,C_NamePage, C_Follower, C_StatusPage,C_CookieVia,C_FolderPage,C_ProxyPage) 
                VALUES(@C_UIDVia,@C_IDPage,@C_NamePage,@C_Follower, @C_StatusPage,@C_CookieVia,@C_FolderPage,@C_ProxyPage)",
					pageModels,
					transaction);

					transaction.Commit();
				}
			}
			catch
			{
				// Xử lý ngoại lệ
				//MessageBox.Show("Có lỗi xảy ra!");
			}
		}
		public void Update(PageModel pageModel)
		{
			try
			{
				using (SQLiteTransaction transaction = _con.BeginTransaction())
				{

					_con.Execute(@"UPDATE tblPage 
                               SET C_UIDVia = @C_UIDVia, 
                                   C_IDPage = @C_IDPage,
                                   C_NamePage = @C_NamePage,
                                   C_Follower = @C_Follower,
                                   C_StatusPage = @C_StatusPage,
                                   C_CookieVia = @C_CookieVia,
									C_FolderPage = @C_FolderPage,
								C_ProxyPage = @C_ProxyPage
                               WHERE C_IDPage = @C_IDPage",
								   pageModel);


					transaction.Commit();
				}
			}
			catch
			{
				// Xử lý ngoại lệ
			}
		}
		public void BulkUpdate(List<PageModel> pageModels)
		{
			try
			{
				using (SQLiteTransaction transaction = _con.BeginTransaction())
				{

					_con.Execute(@"UPDATE tblPage 
                               SET C_UIDVia = @C_UIDVia, 
                                   C_IDPage = @C_IDPage,
                                   C_NamePage = @C_NamePage,
                                   C_Follower = @C_Follower,
                                   C_StatusPage = @C_StatusPage,
                                   C_CookieVia = @C_CookieVia,
									C_FolderPage = @C_FolderPage,
								C_ProxyPage = @C_ProxyPage
                               WHERE C_IDPage = @C_IDPage",
								   pageModels);


					transaction.Commit();
				}
			}
			catch
			{
				// Xử lý ngoại lệ
			}
		}
		public void BulkDelete(List<PageModel> pageModels)
		{
			try
			{
				using (SQLiteTransaction transaction = _con.BeginTransaction())
				{
					for (var i = 0; i < pageModels.Count; i++)
					{
						var page = pageModels[i];
						using (SQLiteCommand command = new SQLiteCommand(_con))
						{
							command.CommandText = $"DELETE FROM tblPage WHERE C_IDPage=@C_IDPage";
							command.CommandType = CommandType.Text;
							command.Parameters.Add(new SQLiteParameter("@C_IDPage", page.C_IDPage));

							try
							{
								command.ExecuteNonQuery();
							}
							catch
							{

							}
						}
					}

					transaction.Commit();
				}
			}
			catch
			{
				//
			}
		}
		public void ReloadDataManageAcc()
		{
			try
			{
				var table = Select("SELECT * FROM tbl_accounts").Tables[0];

				List<DataGridViewRow> rows = new List<DataGridViewRow>();

				for (var i = 0; i < table.Rows.Count; i++)
				{
					int stt = 0;
					var row = table.Rows[i];
					DataGridViewRow row1 = new DataGridViewRow();
					row1.CreateCells(ManagePageForm.tblManageAcc);
					row1.Cells[stt++].Value = false;
					row1.Cells[stt++].Value = row["C_UID"].ToString();
					row1.Cells[stt++].Value = row["C_Password"].ToString();
					row1.Cells[stt++].Value = row["C_Email"].ToString();
					row1.Cells[stt++].Value = row["C_PassEmail"].ToString();
					row1.Cells[stt++].Value = row["C_2FA"].ToString();
					row1.Cells[stt++].Value = row["C_Status"].ToString();
					row1.Cells[stt++].Value = row["C_Folder"].ToString();
					row1.Cells[stt++].Value = row["C_Cookie"].ToString();
					row1.Cells[stt++].Value = row["C_Token"].ToString();
					row1.Cells[stt++].Value = row["C_Proxy"].ToString();
					rows.Add(row1);
				}

				ManagePageForm.tblManageAcc.Invoke(new MethodInvoker(delegate
				{
					ManagePageForm.tblManageAcc.Rows.Clear();

					ManagePageForm.tblManageAcc.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
					//ManagePageForm.tblManageAcc.ColumnHeadersVisible = false;
					ManagePageForm.tblManageAcc.RowHeadersVisible = false;
					ManagePageForm.tblManageAcc.Rows.AddRange(rows.ToArray());
					ManagePageForm.tblManageAcc.ColumnHeadersVisible = true;
					ManagePageForm.tblManageAcc.RowHeadersVisible = true;
				}));
			}
			catch
			{
				//
			}
		}
		public void ReloadDataFolderManageAcc(string folder)
		{
			try
			{
				var table = Select($"SELECT * FROM tbl_accounts WHERE C_Folder = '{folder}'").Tables[0];

				List<DataGridViewRow> rows = new List<DataGridViewRow>();

				for (var i = 0; i < table.Rows.Count; i++)
				{
					int stt = 0;
					var row = table.Rows[i];
					DataGridViewRow row1 = new DataGridViewRow();
					row1.CreateCells(Form1.tblManageAcc);
					row1.Cells[stt++].Value = false;
					row1.Cells[stt++].Value = row["C_UID"].ToString();
					row1.Cells[stt++].Value = row["C_Password"].ToString();
					row1.Cells[stt++].Value = row["C_Email"].ToString();
					row1.Cells[stt++].Value = row["C_PassEmail"].ToString();
					row1.Cells[stt++].Value = row["C_2FA"].ToString();
					row1.Cells[stt++].Value = row["C_Status"].ToString();
					row1.Cells[stt++].Value = row["C_Folder"].ToString();
					row1.Cells[stt++].Value = row["C_Cookie"].ToString();
					row1.Cells[stt++].Value = row["C_Token"].ToString();
					row1.Cells[stt++].Value = row["C_Proxy"].ToString();
					rows.Add(row1);
				}

				ManagePageForm.tblManageAcc.Invoke(new MethodInvoker(delegate
				{
					ManagePageForm.tblManageAcc.Rows.Clear();

					ManagePageForm.tblManageAcc.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
					//ManagePageForm.tblManageAcc.ColumnHeadersVisible = false;
					ManagePageForm.tblManageAcc.RowHeadersVisible = false;
					ManagePageForm.tblManageAcc.Rows.AddRange(rows.ToArray());
					ManagePageForm.tblManageAcc.ColumnHeadersVisible = true;
					ManagePageForm.tblManageAcc.RowHeadersVisible = true;
				}));
			}
			catch
			{
				//
			}
		}
		public void LoadDataIntoComboBoxManageAcc()
		{
			try
			{
				//ManagePageForm._listFolderManageAcc = new List<string>();
				var table = Select("SELECT * FROM tbl_folders ").Tables[0];
				for (int i = ManagePageForm._cbbManageFolderAcc.Items.Count - 1; i >= 0; i--)
				{
					object currentItem = ManagePageForm._cbbManageFolderAcc.Items[i];

					if (currentItem.ToString() != "All Acc")
					{
						ManagePageForm._cbbManageFolderAcc.Items.RemoveAt(i);
					}
				}
				//Form1._cbbTopic.DisplayMember = table.Columns["C_Topic"].ToString();
				for (int i = 0; i < table.Rows.Count; i++)
				{
					if (table.Rows[i]["C_Type"].ToString() == "Account")
					{
						ManagePageForm._cbbManageFolderAcc.Items.Add(table.Rows[i]["C_Folder"].ToString());
						ManagePageForm._listFolderManageAcc.Add(table.Rows[i]["C_Folder"].ToString());
					}

				}

			}
			catch { }
		}
		public void ReloadDataManagePage()
		{
			try
			{
				var table = Select("SELECT * FROM tblPage").Tables[0];
				List<DataGridViewRow> rows = new List<DataGridViewRow>();

				for (var i = 0; i < table.Rows.Count; i++)
				{
					int stt = 0;
					var row = table.Rows[i];
					DataGridViewRow row1 = new DataGridViewRow();
					row1.CreateCells(ManagePageForm.tblPages);
					row1.Cells[stt++].Value = false;
					row1.Cells[stt++].Value = row["C_UIDVia"].ToString();
					row1.Cells[stt++].Value = row["C_IDPage"].ToString();
					row1.Cells[stt++].Value = row["C_NamePage"].ToString();
					row1.Cells[stt++].Value = row["C_Follower"].ToString();
					row1.Cells[stt++].Value = row["C_StatusPage"].ToString();
					row1.Cells[stt++].Value = row["C_CookieVia"].ToString();
					row1.Cells[stt++].Value = row["C_FolderPage"].ToString();
					row1.Cells[stt++].Value = row["C_ProxyPage"].ToString();
					rows.Add(row1);
				}

				ManagePageForm.tblPages.Invoke(new MethodInvoker(delegate
				{
					ManagePageForm.tblPages.Rows.Clear();

					ManagePageForm.tblPages.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
					ManagePageForm.tblPages.ColumnHeadersVisible = false;
					ManagePageForm.tblPages.RowHeadersVisible = false;
					ManagePageForm.tblPages.Rows.AddRange(rows.ToArray());
					ManagePageForm.tblPages.ColumnHeadersVisible = true;
					ManagePageForm.tblPages.RowHeadersVisible = true;
				}));
			}
			catch
			{
				//
			}
		}
		public void ReloadDataFolderManagePage(string folder)
		{
			try
			{
				var table = Select($"SELECT * FROM tblPage WHERE C_FolderPage = '{folder}'").Tables[0];

				List<DataGridViewRow> rows = new List<DataGridViewRow>();

				for (var i = 0; i < table.Rows.Count; i++)
				{
					int stt = 0;
					var row = table.Rows[i];
					DataGridViewRow row1 = new DataGridViewRow();
					row1.CreateCells(ManagePageForm.tblPages);
					row1.Cells[stt++].Value = false;
					row1.Cells[stt++].Value = row["C_UIDVia"].ToString();
					row1.Cells[stt++].Value = row["C_IDPage"].ToString();
					row1.Cells[stt++].Value = row["C_NamePage"].ToString();
					row1.Cells[stt++].Value = row["C_Follower"].ToString();
					row1.Cells[stt++].Value = row["C_StatusPage"].ToString();
					row1.Cells[stt++].Value = row["C_CookieVia"].ToString();
					row1.Cells[stt++].Value = row["C_FolderPage"].ToString();
					row1.Cells[stt++].Value = row["C_ProxyPage"].ToString();
					rows.Add(row1);
				}

				ManagePageForm.tblPages.Invoke(new MethodInvoker(delegate
				{
					ManagePageForm.tblPages.Rows.Clear();

					ManagePageForm.tblPages.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
					ManagePageForm.tblPages.ColumnHeadersVisible = false;
					ManagePageForm.tblPages.RowHeadersVisible = false;
					ManagePageForm.tblPages.Rows.AddRange(rows.ToArray());
					ManagePageForm.tblPages.ColumnHeadersVisible = true;
					ManagePageForm.tblPages.RowHeadersVisible = true;
				}));
			}
			catch
			{
				//
			}
		}

		//Load Page khi double click tblAcc
		public void ReloadDataPageWhenDoubleClickAcc(string C_UID)
		{
			try
			{
				var table = Select($"SELECT * FROM tblPage WHERE C_UIDVia  = '{C_UID}'").Tables[0];

				List<DataGridViewRow> rows = new List<DataGridViewRow>();

				for (var i = 0; i < table.Rows.Count; i++)
				{
					int stt = 0;
					var row = table.Rows[i];
					DataGridViewRow row1 = new DataGridViewRow();
					row1.CreateCells(ManagePageForm.tblPages);
					row1.Cells[stt++].Value = false;
					row1.Cells[stt++].Value = row["C_UIDVia"].ToString();
					row1.Cells[stt++].Value = row["C_IDPage"].ToString();
					row1.Cells[stt++].Value = row["C_NamePage"].ToString();
					row1.Cells[stt++].Value = row["C_Follower"].ToString();
					row1.Cells[stt++].Value = row["C_StatusPage"].ToString();
					row1.Cells[stt++].Value = row["C_CookieVia"].ToString();
					row1.Cells[stt++].Value = row["C_FolderPage"].ToString();
					row1.Cells[stt++].Value = row["C_ProxyPage"].ToString();
					rows.Add(row1);
				}

				ManagePageForm.tblPages.Invoke(new MethodInvoker(delegate
				{
					ManagePageForm.tblPages.Rows.Clear();

					ManagePageForm.tblPages.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
					ManagePageForm.tblPages.ColumnHeadersVisible = false;
					ManagePageForm.tblPages.RowHeadersVisible = false;
					ManagePageForm.tblPages.Rows.AddRange(rows.ToArray());
					ManagePageForm.tblPages.ColumnHeadersVisible = true;
					ManagePageForm.tblPages.RowHeadersVisible = true;
				}));
			}
			catch
			{
				//
			}
		}

	}
}
