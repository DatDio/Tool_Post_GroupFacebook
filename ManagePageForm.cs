using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tool_Facebook.Controller;
using Tool_Facebook.Helper;
using Tool_Facebook.Model;
using Tool_Facebook.Properties;
using Tool_Facebook.Services;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Tool_Facebook
{
	public partial class ManagePageForm : Form
	{
		public static System.Windows.Forms.ComboBox _cbbManageFolderAcc;
		public Random random;
		public static DataGridView tblPages, tblManageAcc;
		public static SqlPageFormTablePageService sqlController;
		public static List<string> _listFolderManageAcc, _listFolderManagePage;
		public List<string> _listProxy, _listNamePage, _listAvatarRegPage;
		public string _selectedProxyType, _cbbFolderManageAcc, ApiGPM;
		public int _numberThreadAcc, _threadRunningAcc,
				   _success, _fail,
				   _numberThreadGroup,
				   _pagePerAcc;
		public static int CurrentWidth, CurrentHeight;
		public static bool stop;
		public bool saved = false;
		public bool finish,
					_checkLiveUID, _solveCheckPoint, _logingetCookie, _interact, _createPage, _loginChrome,
					_upReel;
		public double ScaleChrome;
		List<DataGridViewRow> rowsAccChecked, rowsPageChecked;
		public object lockProxy, lockRowAccChecked, lockRowPageChecked, lockChrome,
			lockNamePage, lockAvatarRegPage;

		Stopwatch stopwatch;
		public ManagePageForm()
		{
			InitializeComponent();
			tblPages = DataGridViewPage;
			tblManageAcc = dataGridViewAcc;
			_cbbManageFolderAcc = cbbFolderManageAcc;
		}

		#region datagrid=""
		private void dataGridViewAcc_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.Value == null)
			{
				e.Value = "";
			}
		}
		private void DataGridViewPage_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.Value == null)
			{
				e.Value = "";
			}

		}
		private void dataGridViewAcc_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
				return;
			if (tblManageAcc.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
			{
				tblManageAcc.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = ""; // Đặt giá trị của ô thành chuỗi rỗng nếu nó là null
			}
		}

		private void DataGridViewPage_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
				return;
			if (tblPages.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
			{
				tblPages.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = ""; // Đặt giá trị của ô thành chuỗi rỗng nếu nó là null
			}
		}
		private void dataGridViewAcc_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
		{
			var grid = sender as DataGridView;
			var rowIdx = (e.RowIndex + 1).ToString();
			var centerFormat = new StringFormat()
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};
			var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
			e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
		}
		private void DataGridViewPage_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
		{
			var grid = sender as DataGridView;
			var rowIdx = (e.RowIndex + 1).ToString();
			var centerFormat = new StringFormat()
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};
			var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
			e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
		}
		#endregion

		#region doubleClick
		//tblAcc
		private void dataGridViewAcc_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
			{
				string C_UID = dataGridViewAcc.Rows[e.RowIndex].Cells["C_UID"].Value.ToString();
				if (!string.IsNullOrEmpty(C_UID))
				{
					sqlController.ReloadDataPageWhenDoubleClickAcc(C_UID);
				}
				else
				{
					MessageBox.Show("Có lỗi xảy ra, không load được page!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			lblRowCountPage.Text = tblPages.RowCount.ToString();
		}
		#endregion
		private void ManagePageForm_Load(object sender, EventArgs e)
		{
			_listFolderManageAcc = new List<string>();
			random = new Random();
			ckbCheckLiveUID.Checked = Settings.Default.ckbCheckLiveUID;
			txtNumberPagePerAcc.Text = Settings.Default.txtNumberPagePerAcc;
			txtNumberThread.Text = Settings.Default.txtNumberThread;
			txtAPIGPM.Text = Settings.Default.txtAPIGPM;
			ckbSolveCheckPoint.Checked = Settings.Default.ckbSolveCheckPoint;
			if (!string.IsNullOrEmpty(Properties.Settings.Default.cbbProxyType))
			{
				cbbProxyType.SelectedItem = Properties.Settings.Default.cbbProxyType;
			}
			sqlController = new SqlPageFormTablePageService();
			//sqlController.ReloadDataManagePage();
			#region Mượt mà datagridview
			var dvgType = tblManageAcc.GetType();
			var pi = dvgType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
			pi.SetValue(tblManageAcc, true, null);
			pi.SetValue(tblPages, true, null);
			tblManageAcc.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
			tblPages.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
			#endregion
			if (!File.Exists("input/ProxyVN.txt"))
			{
				File.Create("input/ProxyVN.txt").Close();
			}
			if (!File.Exists("input/ProxyUS.txt"))
			{
				File.Create("input/ProxyUS.txt").Close();
			}
			if (!Directory.Exists("AvatarPage"))
			{
				Directory.CreateDirectory("AvatarPage");
			}
			if (!Directory.Exists("VideoReel"))
			{
				Directory.CreateDirectory("VideoReel");
			}
			if (!File.Exists("input/NamePage.txt"))
			{
				File.Create("input/NamePage.txt").Close();
			}
			#region lockObject
			lockProxy = new object();
			lockRowAccChecked = new object();
			lockRowPageChecked = new object();
			lockChrome = new object();
			lockNamePage = new object();
			lockAvatarRegPage = new object();
			#endregion

			sqlController.LoadDataIntoComboBoxManageAcc();
		}
		private void mởChromeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ApiGPM = txtAPIGPM.Text;
			ScaleChrome = int.Parse(txtScale.Text);
			for (int i = 0; i < tblManageAcc.Rows.Count; i++)
			{
				tblManageAcc.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
			}
			var rows = FunctionHelper.GetRowSelected(tblManageAcc);
			foreach (var row in rows)
			{
				row.DefaultCellStyle.ForeColor = Settings.Default.colorOrange;
				Thread thread = new Thread(new ParameterizedThreadStart(OneThreadOpenChrome)) { IsBackground = true };
				thread.Start(row);
			}
		}

		private void OneThreadOpenChrome(object data)
		{
			DataGridViewRow row = (DataGridViewRow)data;
			string position = BrowserController.GetNewPosition(800, 800, ScaleChrome);
			_listProxy = new List<string>(File.ReadAllLines("input/Proxy.txt"));
			int countPerform = 0;
			string tokenTM = "";
			var account = FunctionHelper.ConvertRowToAccountModel(row);
			if (account != null)
			{
			reStart:
				FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
				BrowserController browserController = new BrowserController(account);
				try
				{
					if (account.C_Proxy == "")
					{
						lock (lockProxy)
						{
							if (_listProxy.Count == 0)
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
								return;
							}
							account.C_Proxy = _listProxy[0];
							_listProxy.RemoveAt(0);
							File.WriteAllLines("input/Proxy.txt", _listProxy);
							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
						}
					}

					account.driver = browserController.OpenChromeGpm(ApiGPM, account.C_GPMID, account.C_Email, "", ScaleChrome, account.C_Proxy, position: position);
					FunctionHelper.EditValueColumn(account, "C_Status", account.C_Status);

				}
				catch
				{
					lock (lockProxy)
					{
						if (_listProxy.Count == 0)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
							return;
						}
						account.C_Proxy = _listProxy[0];
						_listProxy.RemoveAt(0);
						File.WriteAllLines("input/Proxy.txt", _listProxy);
						FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
					}
					countPerform++;
					if (countPerform == 2)
					{
						browserController.CloseChrome();
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy!", true);
						return;
					}
					else
					{
						browserController.CloseChrome();
						goto reStart;
					}

				}
				if (account.driver == null)
				{
					lock (lockProxy)
					{
						if (_listProxy.Count == 0)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
							return;
						}
						account.C_Proxy = _listProxy[0];
						_listProxy.RemoveAt(0);
						File.WriteAllLines("input/Proxy.txt", _listProxy);
						FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
					}
					countPerform++;
					if (countPerform == 2)
					{
						browserController.CloseChrome();
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy!", true);
						return;
					}
					else
					{
						browserController.CloseChrome();
						goto reStart;
					}
				}
			}
		}
		private void uIDPassToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			var items = Clipboard.GetText().Replace("\r", "").Split('\n').ToList();
			for (int i = 0; i < items.Count; i++)
			{
				try
				{
					var item = items[i].Split('|');
					if (item.Length == 2)
					{
						accountModel.Add(new AccountModel
						{
							C_UID = item[0].Trim(),
							C_Password = item[1].Trim(),
							C_Folder = cbbFolderManageAcc.Text
						});
					}
					else
					{
						MessageBox.Show("Không đúng định dạng!");
						break;
					}
				}
				catch
				{
				}
			}
			if (accountModel.Count > 0)
			{
				sqlController.BulkInsert(accountModel);

				sqlController.ReloadDataFolderManageAcc(cbbFolderManageAcc.Text);
			}
		}

		private void uIDPass2FaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			var items = Clipboard.GetText().Replace("\r", "").Split('\n').ToList();
			for (int i = 0; i < items.Count; i++)
			{
				try
				{
					var item = items[i].Split('|');
					if (item.Length == 3)
					{
						accountModel.Add(new AccountModel
						{
							C_UID = item[0].Trim(),
							C_Password = item[1].Trim(),
							C_2FA = item[2].Trim(),
							C_Folder = cbbFolderManageAcc.Text
						});
					}
					else
					{
						MessageBox.Show("Không đúng định dạng!");
						break;
					}
				}
				catch
				{
				}
			}
			if (accountModel.Count > 0)
			{
				sqlController.BulkInsert(accountModel);

				sqlController.ReloadDataFolderManageAcc(cbbFolderManageAcc.Text);
			}
		}

		private void uIDPassCookieToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			var items = Clipboard.GetText().Replace("\r", "").Split('\n').ToList();
			for (int i = 0; i < items.Count; i++)
			{
				try
				{
					var item = items[i].Split('|');
					if (item.Length == 3)
					{
						accountModel.Add(new AccountModel
						{
							C_UID = item[0].Trim(),
							C_Password = item[1].Trim(),
							C_Cookie = item[2].Trim(),
							C_Folder = cbbFolderManageAcc.Text
						});
					}
					else
					{
						MessageBox.Show("Không đúng định dạng!");
						break;
					}
				}
				catch
				{
				}
			}
			if (accountModel.Count > 0)
			{
				sqlController.BulkInsert(accountModel);

				sqlController.ReloadDataFolderManageAcc(cbbFolderManageAcc.Text);
			}
		}

		private void chọnTấtCảToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var rows = tblManageAcc.Rows;
			for (int i = 0; i < rows.Count; i++)
			{
				rows[i].Cells["C_Check"].Value = true;
				rows[i].DefaultCellStyle.ForeColor = Settings.Default.colorOrange;
			}
		}

		private void btnNamePage_Click(object sender, EventArgs e)
		{
			Process.Start(Path.GetFullPath("input/NamePage.txt"));
		}

		private void btnAddImagesRegPage_Click(object sender, EventArgs e)
		{
			Process.Start(Path.GetFullPath("AvatarPage"));
		}

		private void xóaDòngToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			var item = FunctionHelper.GetRowSelected(tblManageAcc);
			for (int i = 0; i < item.Count; i++)
			{
				accountModel.Add(new AccountModel
				{
					C_UID = item[i].Cells["C_UID"].Value.ToString()
				});
			}
			var status = MessageBox.Show("Bạn có chắc muốn xóa các dòng này?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (status == DialogResult.Yes)
			{
				sqlController.BulkDelete(accountModel);
				MessageBox.Show("Đã xóa!");
				for (int i = 0; i < item.Count; i++)
				{
					item[i].Cells["C_Status"].Value = "Đã Xóa";
					item[i].DefaultCellStyle.ForeColor = Settings.Default.colorRed;
				}
			}
		}

		private void cbbFolderManageAcc_SelectedValueChanged(object sender, EventArgs e)
		{
			_cbbFolderManageAcc = cbbFolderManageAcc.Text;
			string selectedItem = cbbFolderManageAcc.SelectedItem.ToString();
			//MessageBox.Show("Bạn đã chọn: " + selectedItem);
			if (cbbFolderManageAcc.Text == "All Acc")
			{
				sqlController.ReloadDataManageAcc();
				sqlController.ReloadDataManagePage();
			}
			else
			{
				sqlController.ReloadDataFolderManageAcc(cbbFolderManageAcc.Text);
				sqlController.ReloadDataFolderManagePage(cbbFolderManageAcc.Text);
			}

			MoveDataManageAcc.DropDownItems.Clear();
			foreach (var line in ManagePageForm._listFolderManageAcc.Where(line => line != cbbFolderManageAcc.Text))
			{
				MoveDataManageAcc.DropDownItems.Add(line, null, ManageFolderHelper.OnClickTabAcc);
			}
			lblRowCountAcc.Text = tblManageAcc.RowCount.ToString();
			lblRowCountPage.Text = tblPages.RowCount.ToString();
		}

		private void bỏChọnTấtCảToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var rows = tblManageAcc.Rows;
			for (int i = 0; i < rows.Count; i++)
			{
				rows[i].Cells["C_Check"].Value = false;
				rows[i].DefaultCellStyle.ForeColor = Color.Black;
			}
		}

		private void chọnHoặcBỏChọnCácDòngBôiĐenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var row = FunctionHelper.GetRowSelected(tblManageAcc);
			for (int i = 0; i < tblManageAcc.Rows.Count; i++)
			{
				if ((bool)tblManageAcc.Rows[i].Cells["C_Check"].Value == false)
					tblManageAcc.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
			}

			for (int i = 0; i < row.Count; i++)
			{
				if ((bool)row[i].Cells["C_Check"].Value == true)
				{
					row[i].Cells["C_Check"].Value = false;
					row[i].DefaultCellStyle.ForeColor = Color.Black;
				}
				else
				{
					row[i].Cells["C_Check"].Value = true;
					row[i].DefaultCellStyle.ForeColor = Settings.Default.colorOrange;
				}
			}
		}

		private void xóaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			var item = FunctionHelper.GetRowSelected(tblManageAcc);
			for (int i = 0; i < item.Count; i++)
			{
				accountModel.Add(new AccountModel
				{
					C_UID = item[i].Cells["C_UID"].Value.ToString()
				});
			}
			var status = MessageBox.Show("Bạn có chắc muốn xóa các dòng này?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (status == DialogResult.Yes)
			{
				sqlController.BulkDelete(accountModel);
				MessageBox.Show("Đã xóa!");
				for (int i = 0; i < item.Count; i++)
				{
					item[i].Cells["C_Status"].Value = "Đã Xóa";
					item[i].DefaultCellStyle.ForeColor = Settings.Default.colorRed;
				}
			}
		}

		private void btnCloseAllChrome_Click(object sender, EventArgs e)
		{
			var Processs = new List<Process>();
			Processs.AddRange(Process.GetProcessesByName("chromedriver"));
			Processs.AddRange(Process.GetProcessesByName("chrome"));
			foreach (var pr in Processs)
			{
				try
				{
					pr.Kill();
				}
				catch
				{

				}
			}
		}

		private void lưuTấtCảToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			for (int i = 0; i < tblManageAcc.Rows.Count; i++)
			{
				//DataGridViewRow row = tblMain.Rows[i];
				//var proxy = tblMain.Rows[i].Cells["C_Proxy"].Value.ToString();
				try
				{
					var account = FunctionHelper.ConvertRowToAccountModel(tblManageAcc.Rows[i]);
					if (account != null)
					{
						accountModel.Add(account);
					}
				}
				catch
				{

				}
			}
			if (accountModel.Count == 0)
			{
				MessageBox.Show("Có lỗi xảy ra!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				sqlController.BulkUpdate(accountModel);
				MessageBox.Show("Đã Lưu!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void MoveDataManageAcc_Click(object sender, EventArgs e)
		{

		}

		private void btnDeleteFolderAcc_Click(object sender, EventArgs e)
		{
			var status = MessageBox.Show($"Bạn có chắc muốn xóa chủ đề và dữ liệu trong chủ đề {cbbFolderManageAcc.Text} này? ", "Cảnh báo!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (status == DialogResult.Yes)
			{
				sqlController.excuteSQL($"DELETE FROM tbl_folders WHERE C_Folder='{cbbFolderManageAcc.Text}'");
				sqlController.excuteSQL($"DELETE FROM tbl_accounts WHERE C_Folder='{cbbFolderManageAcc.Text}'");
				sqlController.LoadDataIntoComboBoxManageAcc();
				tblManageAcc.Rows.Clear();
				cbbFolderManageAcc.Text = "";
			}

			MoveDataManageAcc.DropDownItems.Clear();
			foreach (var line in ManagePageForm._listFolderManageAcc.Where(line => line != cbbFolderManageAcc.Text))
			{
				MoveDataManageAcc.DropDownItems.Add(line, null, ManageFolderHelper.OnClickTabAcc);
			}
		}

		private void lưuTấtCảToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			List<GroupModel> listGroupModel = new List<GroupModel>();
			for (int i = 0; i < tblManageAcc.Rows.Count; i++)
			{
				try
				{
					var group = FunctionHelper.ConvertRowToGroupModel(tblPages.Rows[i]);
					if (group != null)
					{
						listGroupModel.Add(group);
					}
				}
				catch
				{

				}
			}
			sqlController.BulkUpdate(listGroupModel);
			MessageBox.Show("Đã Lưu!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void chọnTấtCảToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			var rows = tblPages.Rows;
			for (int i = 0; i < rows.Count; i++)
			{
				rows[i].Cells["C_CheckPage"].Value = true;
				rows[i].DefaultCellStyle.ForeColor = Settings.Default.colorOrange;
			}
		}

		private void bỏChọnTấtCảToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			var rows = tblPages.Rows;
			for (int i = 0; i < rows.Count; i++)
			{
				rows[i].Cells["C_CheckPage"].Value = false;
				rows[i].DefaultCellStyle.ForeColor = Color.Black;
			}
		}

		private void chọnHoặcBỏChọnCácDòngBôiĐenToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			var row = FunctionHelper.GetRowSelected(tblPages);
			for (int i = 0; i < tblPages.Rows.Count; i++)
			{
				if ((bool)tblPages.Rows[i].Cells["C_CheckPage"].Value == false)
					tblPages.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
			}

			for (int i = 0; i < row.Count; i++)
			{
				if ((bool)row[i].Cells["C_CheckPage"].Value == true)
				{
					row[i].Cells["C_CheckPage"].Value = false;
					row[i].DefaultCellStyle.ForeColor = Color.Black;
				}
				else
				{
					row[i].Cells["C_CheckPage"].Value = true;
					row[i].DefaultCellStyle.ForeColor = Settings.Default.colorOrange;
				}
			}
		}

		private void btnAddFolderAcc_Click(object sender, EventArgs e)
		{
			FolderManageAccForm folderManageAccForm = new FolderManageAccForm("Account");
			folderManageAccForm.ShowDialog();
			//sqlController.createTable($"INSERT INTO tbl_topic(C_Topic) VALUES ()");

			MoveDataManageAcc.DropDownItems.Clear();
			foreach (var line in ManagePageForm._listFolderManageAcc.Where(line => line != cbbFolderManageAcc.Text))
			{
				MoveDataManageAcc.DropDownItems.Add(line, null, ManageFolderHelper.OnClickTabAcc);
			}
		}

		private void tảiLạiToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (cbbFolderManageAcc.Text == "All Acc")
				sqlController.ReloadDataManageAcc();
			else
			{
				sqlController.ReloadDataFolderManageAcc(cbbFolderManageAcc.Text);
			}
		}

		private void btnStopAcc_Click(object sender, EventArgs e)
		{
			stop = true;
			finish = true;
			btnStopAcc.Enabled = false;
		}

		private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
		{
			_selectedProxyType = cbbProxyType.SelectedItem.ToString();
		}

		private void btnOpenProxy_Click(object sender, EventArgs e)
		{
			if (_selectedProxyType == "Proxy VN")
			{
				Process.Start(Path.GetFullPath("input/ProxyVN.txt"));
			}
			else if (_selectedProxyType == "Proxy US")
			{
				Process.Start(Path.GetFullPath("input/ProxyUS.txt"));
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			Settings.Default.ckbCheckLiveUID = ckbCheckLiveUID.Checked;
			Settings.Default.txtNumberPagePerAcc = txtNumberPagePerAcc.Text;
			Settings.Default.txtNumberThread = txtNumberThread.Text;
			Settings.Default.txtAPIGPM = txtAPIGPM.Text;
			Settings.Default.ckbSolveCheckPoint = ckbSolveCheckPoint.Checked;
			try
			{
				Settings.Default.cbbProxyType = cbbProxyType.SelectedItem.ToString();
			}
			catch
			{
			}

			Settings.Default.Save();

			// List
			_listNamePage = new List<string>(File.ReadAllLines("input/NamePage.txt"));
			lblCountImagePage.Text = _listNamePage.Count().ToString();
			lblCountImagePage.Text = Directory.GetFiles(Path.GetFullPath("AvatarPage")).ToList().ToString();

			if (txtNumberPagePerAcc.Text == "")
			{
				MessageBox.Show("Vui lòng điền số Page/acc để tạo!");
				return;
			}
			_pagePerAcc = int.Parse(txtNumberPagePerAcc.Text);
			ApiGPM = txtAPIGPM.Text;
			ScaleChrome = int.Parse(txtScale.Text);
			//ProxyType
			if (_selectedProxyType == "Proxy VN")
			{
				_listProxy = new List<string>(File.ReadAllLines("input/ProxyVN.txt"));
			}
			else if (_selectedProxyType == "Proxy US")
			{
				_listProxy = new List<string>(File.ReadAllLines("input/ProxyUS.txt"));
			}
			lblSumProxy.Text = _listProxy.Count.ToString();

			//Chức Năng Acc
			_checkLiveUID = ckbCheckLiveUID.Checked;
			_solveCheckPoint = ckbSolveCheckPoint.Checked;
			_logingetCookie = ckbGetCookie.Checked;
			_loginChrome = ckbLoginGetCookieChrome.Checked;
			_interact = ckbInteract.Checked;
			//Chức Năng Page
			_upReel = ckbUpReel.Checked;
			if (!saved)
			{
				MessageBox.Show("Đã Lưu");
				saved = true;
			}
		}

		private void btnStartAcc_Click(object sender, EventArgs e)
		{
			if (!saved)
			{
				saved = true;
			}
			btnSave_Click(sender, e);
			stop = false;
			_success = 0;
			_fail = 0;
			CurrentWidth = 0;
			CurrentHeight = 0;

			if (txtNumberThread.Text == "")
			{
				MessageBox.Show("Vui lòng điền số luồng!");
				return;
			}

			_numberThreadAcc = int.Parse(txtNumberThread.Text);
			if (_numberThreadAcc == 0)
			{
				MessageBox.Show("Vui lòng chọn số luồng!");
				return;
			}
			_threadRunningAcc = _numberThreadAcc;
			lblThreadRunningAcc.Text = _threadRunningAcc.ToString();
			lblSuccessAcc.Text = _success.ToString();
			lblFailAcc.Text = _fail.ToString();
			lblSuccessPage.Text = _success.ToString();
			lblFailPage.Text = _fail.ToString();
			//Thread of Acc Api
			if (_checkLiveUID || _logingetCookie || _createPage)
			{
				rowsAccChecked = FunctionHelper.GetRowChecked(tblManageAcc);
				for (int i = 0; i < rowsAccChecked.Count; i++)
				{
					rowsAccChecked[i].Cells["C_Status"].Value = "";
				}
				if (rowsAccChecked.Count == 0)
				{
					MessageBox.Show("Vui lòng chọn acc!");
					return;
				}
				for (int i = 0; i < _numberThreadAcc; i++)
				{
					Thread thread = new Thread(OneThread) { IsBackground = true };
					thread.Start();
				}
				stopwatch = new Stopwatch();
				stopwatch.Start();
				timer1.Start();
			}
			//Thread of Acc Chrome
			else if (_interact || _loginChrome)
			{
				rowsAccChecked = FunctionHelper.GetRowChecked(tblManageAcc);
				for (int i = 0; i < rowsAccChecked.Count; i++)
				{
					rowsAccChecked[i].Cells["C_Status"].Value = "";
				}
				if (rowsAccChecked.Count == 0)
				{
					MessageBox.Show("Vui lòng chọn acc!");
					return;
				}
				for (int i = 0; i < _numberThreadAcc; i++)
				{
					Thread thread = new Thread(OneThreadChrome) { IsBackground = true };
					thread.Start();
				}
				stopwatch = new Stopwatch();
				stopwatch.Start();
				timer1.Start();
			}
			//Thread of Page
			else
			{
				rowsPageChecked = FunctionHelper.GetRowCheckedPage(tblPages);
				for (int i = 0; i < rowsPageChecked.Count; i++)
				{
					rowsPageChecked[i].Cells["C_StatusPage"].Value = "";
				}
				if (rowsPageChecked.Count == 0)
				{
					MessageBox.Show("Vui lòng chọn page!");
					return;
				}
				for (int i = 0; i < _numberThreadAcc; i++)
				{
					Thread thread = new Thread(OneThreadPage) { IsBackground = true };
					thread.Start();
				}
				stopwatch = new Stopwatch();
				stopwatch.Start();
				timerPage.Start();
			}
			btnStartAcc.Enabled = false;
			btnStopAcc.Enabled = true;
		}

		private void OneThread()
		{
			AccountModel account = null;
			FacebookAPIController apiFacebook = null;
			DataGridViewRow rowAcc;
			//DataGridViewRow rowPage;
			ResultModel status;
			//int randomRowIndex = 0;
			bool success = false;
			string namePage = "", avatarRegPage = "";
			while (!stop)
			{
				lock (lockRowAccChecked)
				{
					if (rowsAccChecked.Count == 0)
						break;
					rowAcc = rowsAccChecked[0];
					rowsAccChecked.RemoveAt(0);
				}
				account = FunctionHelper.ConvertRowToAccountModel(rowAcc);
				if (account == null)
				{
					success = false;
					FunctionHelper.EditValueColumn(account, "C_Status", "Vui lòng thử lại!", true);
					goto finish;
				}

				apiFacebook = new FacebookAPIController();
				if (_createPage)
				{
					int _countRegPagePerform = 0;
					for (int i = 0; i < _pagePerAcc; i++)
					{

					reRegPage:
						lock (lockNamePage)
						{
							namePage = _listNamePage[random.Next(0, _listNamePage.Count)];
						}

						lock (lockAvatarRegPage)
						{
							_listAvatarRegPage = Directory.GetFiles(Path.GetFullPath("AvatarPage")).ToList();
							avatarRegPage = _listAvatarRegPage[random.Next(0, _listAvatarRegPage.Count)];
						}

						FunctionHelper.EditValueColumn(account, "C_Status", $"Đang reg page [{i + 1}]...");
						status = apiFacebook.RegPage(account, namePage, avatarRegPage);
						if (status == ResultModel.Success)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", $"Reg Page [{i + 1}] thành công!");
						}
						else if (status == ResultModel.NameError)
						{
							goto reRegPage;

						}
						else if (status == ResultModel.Fail)
						{
							_countRegPagePerform++;
							FunctionHelper.EditValueColumn(account, "C_Status", $"Reg page [{i + 1}] lần thứ {_countRegPagePerform}...", true);
							if (_countRegPagePerform == 3)
							{
								success = false;
								FunctionHelper.EditValueColumn(account, "C_Status", $"Reg page [{i + 1}] thất bại!");
								break;
							}
							goto reRegPage;
						}
					}
					FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy token ...");
					var rs = apiFacebook.GetToken(account);
					if (rs)
					{
						FunctionHelper.EditValueColumn(account, "C_Token", account.C_Token, true);
					}
					else
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Lấy token thất bại", true);
						goto finish;
					}

					FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy id page ...");
					var rsPage = apiFacebook.GetAllPage(account);
					if (rsPage == ResultModel.Fail)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Không lấy được id page", true);
						goto finish;
					}
					success = true;
					FunctionHelper.EditValueColumn(account, "C_Status", "Lấy id page xong...");
				}
			finish:
				if (success)
				{
					account.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorGreen;
					Interlocked.Increment(ref _success);
					Invoke((MethodInvoker)delegate ()
					{
						lblSuccessAcc.Text = _success.ToString();
					});
				}
				else
				{
					account.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorRed;
					Interlocked.Increment(ref _fail);
					Invoke((MethodInvoker)delegate ()
					{
						lblFailAcc.Text = _fail.ToString();
					});
				}
				account.C_Row.Cells["C_Check"].Value = false;
			}

			Interlocked.Decrement(ref _threadRunningAcc);
			Invoke((MethodInvoker)delegate ()
			{
				lblThreadRunningAcc.Text = _threadRunningAcc.ToString();
			});
			if (_threadRunningAcc == 0)
			{
				Invoke(new MethodInvoker(() =>
				{
					lblSumProxy.Text = _listProxy.Count.ToString();
					btnStopAcc.Enabled = false;
					btnStartAcc.Enabled = true;
					timer1.Stop();
				}));

				MessageBox.Show("Tool đã dừng");
			}
		}

		private void OneThreadPage()
		{
			AccountModel account = null;
			FacebookAPIController apiController = null;
			DataGridViewRow rowPage;
			PageModel page = null;
			bool success = false;
			while (!stop)
			{
				lock (lockRowPageChecked)
				{
					if (rowsPageChecked.Count == 0)
						break;
					rowPage = rowsPageChecked[0];
					rowsPageChecked.RemoveAt(0);
				}
				page = FunctionHelper.ConvertRowToPageModel(rowPage);
				if (page == null)
				{
					success = false;
					FunctionHelper.EditValueColumn(page, "C_StatusPage", "Vui lòng thử lại!", true);
					goto finish;
				}

				apiController = new FacebookAPIController();
				if (_upReel)
				{

				}
			finish:
				if (success)
				{
					page.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorGreen;
					Interlocked.Increment(ref _success);
					Invoke((MethodInvoker)delegate ()
					{
						lblSuccessPage.Text = _success.ToString();
					});
				}
				else
				{
					page.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorRed;
					Interlocked.Increment(ref _fail);
					Invoke((MethodInvoker)delegate ()
					{
						lblFailPage.Text = _fail.ToString();
					});
				}
				page.C_Row.Cells["C_CheckPage"].Value = false;

			}
			Interlocked.Decrement(ref _threadRunningAcc);
			Invoke((MethodInvoker)delegate ()
			{
				lblThreadRunningAcc.Text = _threadRunningAcc.ToString();
			});
			if (_threadRunningAcc == 0)
			{
				Invoke(new MethodInvoker(() =>
				{
					btnStopAcc.Enabled = false;
					btnStartAcc.Enabled = true;
					timerPage.Stop();
				}));

				MessageBox.Show("Tool đã dừng");
			}
		}
		private void OneThreadChrome()
		{
			string position = BrowserController.GetNewPosition(800, 800, ScaleChrome);
			AccountModel account = null;
			BrowserController browserController = null;
			FacebookAPIController apiController = null;
			DataGridViewRow rowAcc;
			ResultModel status;
			bool success = false;
			while (!stop)
			{
				int countPerform = 0;
				lock (lockRowAccChecked)
				{
					if (rowsAccChecked.Count == 0)
						break;
					rowAcc = rowsAccChecked[0];
					rowsAccChecked.RemoveAt(0);
				}
				account = FunctionHelper.ConvertRowToAccountModel(rowAcc);
				if (account == null)
				{
					success = false;
					FunctionHelper.EditValueColumn(account, "C_Status", "Vui lòng thử lại!", true);
					goto finish;
				}
				if (account.C_Proxy == "")
				{
					lock (lockProxy)
					{
						if (_listProxy.Count == 0)
						{
							account.C_Row.Cells["C_Check"].Value = false;
							break;
						}

						account.C_Proxy = _listProxy[0];
						_listProxy.RemoveAt(0);
						_listProxy.Add(account.C_Proxy);
						//File.WriteAllLines("input/Proxy.txt", _listProxy);
						FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
					}
				}
				apiController = new FacebookAPIController();
				FacebookBrowserController fb = new FacebookBrowserController(account);
				if (_loginChrome)
				{
					FunctionHelper.EditValueColumn(account, "C_Status", "Đang login lấy cookie!", true);
					var statuslogin = apiController.LoginWWW(account);
					if (!statuslogin)
					{
						success = false;
						goto finish;
					}
					FunctionHelper.EditValueColumn(account, "C_Cookie", account.C_Cookie, true);
					FunctionHelper.EditValueColumn(account, "C_Status", "Login lấy cookie thành công!", true);
					success = true;
				}

			reStartInteract:
				if (_interact)
				{
					browserController = new BrowserController(account);
					try
					{
						//account.C_Proxy = "";
						account.driver = browserController.OpenChromeGpm(ApiGPM, account.C_GPMID, account.C_Email, account.C_UserAgent, ScaleChrome, account.C_Proxy, position: position);
					}
					catch
					{
						lock (lockProxy)
						{
							if (_listProxy.Count == 0)
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
								break;
							}
							account.C_Proxy = _listProxy[0];
							_listProxy.RemoveAt(0);
							File.WriteAllLines("input/Proxy.txt", _listProxy);
							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
						}
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartInteract;
						}
					}
					if (account.driver == null)
					{
						lock (lockProxy)
						{
							if (_listProxy.Count == 0)
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
								break;
							}
							account.C_Proxy = _listProxy[0];
							_listProxy.RemoveAt(0);
							File.WriteAllLines("input/Proxy.txt", _listProxy);
							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
						}
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartInteract;
						}
					}
					status = fb.InteractFacebookWWW();
					if (status == ResultModel.Success)
					{
						success = true;
						FunctionHelper.EditValueColumn(account, "C_Status", "Tương tác xong!", true);
						goto finish;
					}
					else if (status == ResultModel.Fail)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Tương tác thất bại!", true);
						goto finish;
					}
					else if (status == ResultModel.LoginFail)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Login thất bại!", true);
						goto finish;
					}
					else if (status == ResultModel.CheckPoint)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Check point", true);
						goto finish;
					}
					else if (status == ResultModel.CheckPoint956)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Check point 956", true);
						goto finish;
					}
					else if (status == ResultModel.CheckPoint282)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Check point 282", true);
						goto finish;
					}
				}

			finish:
				if (success)
				{
					account.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorGreen;
					Interlocked.Increment(ref _success);
					Invoke((MethodInvoker)delegate ()
					{
						lblSuccessAcc.Text = _success.ToString();
					});
				}
				else
				{
					account.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorRed;
					Interlocked.Increment(ref _fail);
					Invoke((MethodInvoker)delegate ()
					{
						lblFailAcc.Text = _fail.ToString();
					});
				}
				account.C_Row.Cells["C_Check"].Value = false;
				try
				{
					browserController.CloseChrome();
				}
				catch
				{

				}
			}
			Interlocked.Decrement(ref _threadRunningAcc);
			Invoke((MethodInvoker)delegate ()
			{
				lblThreadRunningAcc.Text = _threadRunningAcc.ToString();
			});
			if (_threadRunningAcc == 0)
			{
				Invoke(new MethodInvoker(() =>
				{
					btnStopAcc.Enabled = false;
					btnStartAcc.Enabled = true;
					timer1.Stop();
				}));

				MessageBox.Show("Tool đã dừng");
			}
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			lblTimeRunningAcc.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");

		}
		private void timerPage_Tick(object sender, EventArgs e)
		{
			lblTimeRunningPage.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
		}
	}
}
