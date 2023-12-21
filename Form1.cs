using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tool_Facebook.Controller;
using Tool_Facebook.Helper;
using Tool_Facebook.Model;
using Tool_Facebook.Properties;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace Tool_Facebook
{
    public partial class Form1 : Form
    {
        public static ComboBox cbbFolderManageAcc1, cbbFolderManageGroup1;
        public static DataGridView tblManageAcc, tblManageGroup;
        public static SqlController sqlController;
        public static ToolStripLabel _lblSumRowGroup;
        public static int CurrentWidth, CurrentHeight;
        public static string _cbbFolderManageAcc, _cbbFolderManageGroup;
        public int _numberThreadAcc, _threadRunningAcc,
                   _success, _fail,
                   _numberThreadGroup, _threadRunningGroup,
                   _groupPerAcc;
        public double ScaleChrome;
        public static HashSet<string> _listNotDupilicate;
        public static List<string> _listFolderManageAcc, _listFolderManageGroup;
        public List<string> _proxyList, _contentList,
                            _linkVideoList, _listKeyWord;
        public static bool stop;
        public bool finish,
                            _checkLiveUID, _solveCheckPoint,
                            _postText, _postLink,
                            _editTextToLink, _getCookie,
                            _scanPage;
        public static object lockChrome, lockProxy,
            lockTopic, lockfolder,
            lockRowAccChecked, lockRowGroupChecked,
            lockContent, locklinkVideo,
            lockKeyword, lockListNotDupilicate;
        List<DataGridViewRow> rowsAccChecked, rowsGroupChecked;
        Stopwatch stopwatch;
        public Form1()
        {
            InitializeComponent();
            tblManageAcc = dataGirdViewAcc;
            tblManageGroup = dataGridViewGroup;
            cbbFolderManageAcc1 = cbbFolderManageAcc;
            cbbFolderManageGroup1 = cbbFolderManageGroup;
            _lblSumRowGroup = lblSumRowGroup;
        }
        #region datagrid =""
        private void dataGridViewGroup_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null)
            {
                e.Value = "";
            }
        }
        private void dataGirdViewAcc_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null)
            {
                e.Value = "";
            }
        }
        private void dataGirdViewAcc_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (tblManageAcc.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
            {
                tblManageAcc.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = ""; // Đặt giá trị của ô thành chuỗi rỗng nếu nó là null
            }
        }
        private void dataGridViewGroup_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (tblManageGroup.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
            {
                tblManageGroup.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = ""; // Đặt giá trị của ô thành chuỗi rỗng nếu nó là null
            }
        }
        private void dataGirdViewAcc_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
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
        private void dataGridViewGroup_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
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
        private void btnOpenProxy_Click(object sender, EventArgs e)
        {
            Process.Start(Path.GetFullPath("input/Proxy.txt"));
        }

        private void btnOpenContent_Click(object sender, EventArgs e)
        {
            Process.Start(Path.GetFullPath("input/ListContent.txt"));
        }

        private void btnOpenLinkVideo_Click(object sender, EventArgs e)
        {
            Process.Start(Path.GetFullPath("input/ListLinkVideo.txt"));
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            txtGroupPerAcc.Text = Settings.Default.txtGroupPerAcc;
            txtNumberThreadAcc.Text = Settings.Default.txtNumberThreadAcc;
            txtNumberThreadGroup.Text = Settings.Default.txtNumberThreadGroup;
            ckbCheckLiveUID.Checked = Settings.Default.ckbCheckLiveUID;
            ckbSolveCheckPoint.Checked = Settings.Default.ckbSolveCheckPoint;
            ckbPostText.Checked = Settings.Default.ckbPostText;
            ckbPostLink.Checked = Settings.Default.ckbPostLink;
            ckbEditTextToLink.Checked = Settings.Default.ckbEditTextToLink;
            #region Mượt mà datagridview
            var dvgType = tblManageAcc.GetType();
            var pi = dvgType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(tblManageAcc, true, null);
            pi.SetValue(tblManageGroup, true, null);
            tblManageAcc.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            tblManageGroup.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            #endregion
            if (!Directory.Exists("input"))
            {
                Directory.CreateDirectory("input");
            }
            if (!Directory.Exists("output"))
            {
                Directory.CreateDirectory("output");
            }
            if (!File.Exists("input/_listNotDupilicate.txt"))
            {
                File.Create("input/_listNotDupilicate.txt").Close();
            }
            if (!File.Exists("input/Proxy.txt"))
            {
                File.Create("input/Proxy.txt").Close();
            }
            if (!File.Exists("input/ListLinkVideo.txt"))
            {
                File.Create("input/ListLinkVideo.txt").Close();
            }
            if (!File.Exists("input/ListContent.txt"))
            {
                File.Create("input/ListContent.txt").Close();
            }
            if (!File.Exists("input/ApiKeyTMProxy.txt"))
            {
                File.Create("input/ApiKeyTMProxy.txt").Close();
            }

            sqlController = new SqlController();
            sqlController.createTable("CREATE TABLE IF NOT EXISTS tbl_folders (C_ID INTEGER PRIMARY KEY AUTOINCREMENT,C_Folder TEXT,C_Type TEXT)");
            //sqlController.excuteSQL("INSERT INTO tbl_folders(C_Folder,C_Type) VALUES('All Acc','Account'),('All Group','Group')");
            sqlController.createTable("CREATE TABLE IF NOT EXISTS tbl_accounts (C_UID TEXT PRIMARY KEY,C_Password,C_Email TEXT, C_PassEmail TEXT, C_2FA TEXT, C_Cookie TEXT, C_Token TEXT,C_Status TEXT, C_Proxy TEXT,C_Folder Text,C_UserAgent TEXT)");
            sqlController.createTable("CREATE TABLE IF NOT EXISTS tbl_groups (C_IDGroup TEXT PRIMARY KEY,C_UIDGroup TEXT,C_NameGroup TEXT,C_Censorship TEXT, C_UIDVia TEXT, C_PostID TEXT, C_StatusGroup TEXT, C_CreatedPost TEXT,C_TimeEditPost TEXT, C_MemberGroup TEXT,C_TypeGroup Text,C_FolderGroup TEXT)");
            sqlController.LoadDataIntoComboBoxManageAcc();
            sqlController.LoadDataIntoComboBoxManageGroup();
            lockChrome = new object();
            lockProxy = new object();
            lockContent = new object();
            locklinkVideo = new object();
            lockRowAccChecked = new object();
            lockRowGroupChecked = new object();
            lockKeyword = new object();
            lockListNotDupilicate = new object();
            _proxyList = new List<string>(File.ReadAllLines("input/Proxy.txt"));
            lblCountProxy.Text = _proxyList.Count.ToString();
            _contentList = new List<string>(File.ReadAllLines("input/ListContent.txt"));
            lblCountContent.Text = _contentList.Count.ToString();
            _linkVideoList = new List<string>(File.ReadAllLines("input/ListLinkVideo.txt"));
            lblCountLinkVideo.Text = _linkVideoList.Count.ToString();

        }
        #region ManageFolderAcc
        private void btnAddFolderAcc_Click(object sender, EventArgs e)
        {
            FolderManageAccForm folderManageAccForm = new FolderManageAccForm("Account");
            folderManageAccForm.ShowDialog();
            //sqlController.createTable($"INSERT INTO tbl_topic(C_Topic) VALUES ()");

            MoveDataManageAcc.DropDownItems.Clear();
            foreach (var line in _listFolderManageAcc.Where(line => line != cbbFolderManageAcc.Text))
            {
                MoveDataManageAcc.DropDownItems.Add(line, null, ManageFolderHelper.OnClickTabAcc);
            }
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
            foreach (var line in _listFolderManageAcc.Where(line => line != cbbFolderManageAcc.Text))
            {
                MoveDataManageAcc.DropDownItems.Add(line, null, ManageFolderHelper.OnClickTabAcc);
            }
        }
        #endregion
        #region MenuManageAcc
        //Thêm
        //UID|Pass|2FA
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
        //UID|Pass
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
        //UID|Pass|Cookie
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
        //Chọn
        private void chọnTấtCảToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var rows = tblManageAcc.Rows;
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].Cells["C_Check"].Value = true;
                rows[i].DefaultCellStyle.ForeColor = Settings.Default.colorOrange;
            }
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

        private void rtbListKeyWord_TextChanged(object sender, EventArgs e)
        {
            lblSumKeyword.Text = rtbListKeyWord.Lines.Count().ToString();
        }

        private void cbbFolderManageAcc_SelectedValueChanged(object sender, EventArgs e)
        {
            _cbbFolderManageAcc = cbbFolderManageAcc.Text;
            string selectedItem = cbbFolderManageAcc.SelectedItem.ToString();
            //MessageBox.Show("Bạn đã chọn: " + selectedItem);
            if (cbbFolderManageAcc.Text == "All Acc")
                sqlController.ReloadDataManageAcc();
            else
            {
                sqlController.ReloadDataFolderManageAcc(cbbFolderManageAcc.Text);
            }

            MoveDataManageAcc.DropDownItems.Clear();
            foreach (var line in _listFolderManageAcc.Where(line => line != cbbFolderManageAcc.Text))
            {
                MoveDataManageAcc.DropDownItems.Add(line, null, ManageFolderHelper.OnClickTabAcc);
            }
            lblSumRowCountAcc.Text = tblManageAcc.RowCount.ToString();
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
            sqlController.BulkUpdate(accountModel);
            MessageBox.Show("Đã Lưu!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private void tảiLạiToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (cbbFolderManageAcc.Text == "All Acc")
                sqlController.ReloadDataManageAcc();
            else
            {
                sqlController.ReloadDataFolderManageAcc(cbbFolderManageAcc.Text);
            }
        }
        private void mởChromeToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
            _proxyList = new List<string>(File.ReadAllLines("input/Proxy.txt"));
            int countPerform = 0;
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
                            if (_proxyList.Count == 0)
                            {
                                FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
                                return;
                            }
                            account.C_Proxy = _proxyList[0];
                            _proxyList.RemoveAt(0);
                            File.WriteAllLines("input/Proxy.txt", _proxyList);
                            FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
                        }
                    }

                    account.driver = browserController.OpenChrome(account.C_UserAgent, ScaleChrome, position: position);
                    FunctionHelper.EditValueColumn(account, "C_Status", account.C_Status);

                }
                catch
                {
                    lock (lockProxy)
                    {
                        if (_proxyList.Count == 0)
                        {
                            FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
                            return;
                        }
                        account.C_Proxy = _proxyList[0];
                        _proxyList.RemoveAt(0);
                        File.WriteAllLines("input/Proxy.txt", _proxyList);
                        FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
                    }
                    countPerform++;
                    if (countPerform <= 2)
                    {
                        goto reStart;

                    }
                    else
                    {
                        browserController.CloseChrome();
                        FunctionHelper.EditValueColumn(account, "C_Status", "Mở chrome lỗi, đã đổi proxy 2 lần!", true);
                        return;
                    }

                }
            }
        }
        private void loginCookieToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
        private void loginPass2FAToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void btnStopAcc_Click(object sender, EventArgs e)
        {
            stop = true;
            finish = true;
            btnStopAcc.Enabled = false;
        }
        private void btnStartAcc_Click(object sender, EventArgs e)
        {
            btnSave_Click(sender, e);
            _getCookie = ckbGetCookie.Checked;
            stop = false;
            _success = 0;
            _fail = 0;
            CurrentWidth = 0;
            CurrentHeight = 0;

            if (txtNumberThreadAcc.Text == "")
            {
                MessageBox.Show("Vui lòng điền số luồng!");
                return;
            }
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

            _numberThreadAcc = int.Parse(txtNumberThreadAcc.Text);
            if (_numberThreadAcc == 0)
            {
                MessageBox.Show("Vui lòng chọn số luồng!");
                return;
            }
            _threadRunningAcc = _numberThreadAcc;
            lblThreadRunningAcc.Text = _threadRunningAcc.ToString();
            lblSuccessAcc.Text = _success.ToString();
            lblFailAcc.Text = _fail.ToString();

            for (int i = 0; i < _numberThreadAcc; i++)
            {
                Thread thread = new Thread(OneThreadTabAcc) { IsBackground = true };
                thread.Start();
            }
            stopwatch = new Stopwatch();
            stopwatch.Start();
            timer1.Start();
            btnStartAcc.Enabled = false;
            btnStopAcc.Enabled = true;
        }
        private void OneThreadTabAcc()
        {
            AccountModel account = null;
            FacebookAPIController apiFacebook = null;
            DataGridViewRow row;
            bool success = false;
            while (!stop)
            {
                lock (lockRowAccChecked)
                {
                    if (rowsAccChecked.Count == 0)
                        break;
                    row = rowsAccChecked[0];
                    rowsAccChecked.RemoveAt(0);
                }
                account = FunctionHelper.ConvertRowToAccountModel(row);
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
                        if (_proxyList.Count == 0)
                        {
                            FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
                            break;
                        }
                        account.C_Proxy = _proxyList[0];
                        _proxyList.RemoveAt(0);
                        File.WriteAllLines("input/Proxy.txt", _proxyList);
                        FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
                    }
                }
                apiFacebook = new FacebookAPIController();
                if (_getCookie)
                {
                    FunctionHelper.EditValueColumn(account, "C_Status", "Đang login lấy cookie");
                    if (!apiFacebook.LoginWWW(account))
                    {
                        success = false;
                        goto finish;
                    }
                    else
                    {
                        success = true;
                        FunctionHelper.EditValueColumn(account, "C_Status", "Get cookie ok");
                    }
                }
                if (_checkLiveUID)
                {
                    FunctionHelper.EditValueColumn(account, "C_Status", $"Đang check live uid");
                    var status = apiFacebook.CheckLiveUid(account);
                    if (status == ResultModel.Success)
                    {
                        success = true;
                        FunctionHelper.EditValueColumn(account, "C_Status", $"Uid live", true);
                    }
                    else if (status == ResultModel.Fail)
                    {
                        success = false;
                        FunctionHelper.EditValueColumn(account, "C_Status", $"Uid die", true);
                        //goto finish;
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
            }
            try
            {
                account.C_Row.Cells["C_Check"].Value = false;
            }
            catch
            {

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
                    _proxyList = new List<string>(File.ReadAllLines("input/Proxy.txt"));
                    lblCountProxy.Text = _proxyList.Count.ToString();
                    btnStopAcc.Enabled = false;
                    btnStartAcc.Enabled = true;
                    timer1.Stop();
                }));

                MessageBox.Show("Tool đã dừng");
            }
        }
        #endregion

        #region MenuManageGroup
        private void cbbFolderManageGroup_SelectedValueChanged(object sender, EventArgs e)
        {
            _cbbFolderManageGroup = cbbFolderManageGroup.Text;
            string selectedItemGroup = cbbFolderManageGroup.SelectedItem.ToString();
            //MessageBox.Show("Bạn đã chọn: " + selectedItem);
            if (cbbFolderManageGroup.Text == "All Group")
                sqlController.ReloadDataManageGroup();
            else
            {
                sqlController.ReloadDataFolderManageGroup(cbbFolderManageGroup.Text);
            }

            MoveDataManageGroup.DropDownItems.Clear();
            foreach (var line in _listFolderManageGroup.Where(line => line != cbbFolderManageGroup.Text))
            {
                MoveDataManageGroup.DropDownItems.Add(line, null, ManageFolderHelper.OnClickTabGroup);
            }
            copyDữLiệuToolStripMenuItem.DropDownItems.Clear();
            foreach (var line in _listFolderManageGroup.Where(line => line != cbbFolderManageGroup.Text))
            {
                copyDữLiệuToolStripMenuItem.DropDownItems.Add(line, null, ManageFolderHelper.OnClickCopyTabGroup);
            }
            lblSumRowGroup.Text = tblManageGroup.RowCount.ToString();
        }
        //Thêm ID
        private void iDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<GroupModel> listGroupModel = new List<GroupModel>();
            var items = Clipboard.GetText().Replace("\r", "").Split('\n').ToList();
            for (int i = 0; i < items.Count; i++)
            {
                try
                {
                    var item = items[i].Split('|');
                    if (item.Length == 1)
                    {
                        listGroupModel.Add(new GroupModel
                        {
                            C_IDGroup = Guid.NewGuid().ToString(),
                            C_UIDGroup = item[0].Trim(),
                            C_FolderGroup = cbbFolderManageGroup.Text
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
            if (listGroupModel.Count > 0)
            {
                sqlController.BulkInsert(listGroupModel);

                sqlController.ReloadDataFolderManageGroup(cbbFolderManageGroup.Text);
            }
        }
        //Thêm ID | Name
        private void iDNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<GroupModel> listGroupModel = new List<GroupModel>();
            var items = Clipboard.GetText().Replace("\r", "").Split('\n').ToList();
            for (int i = 0; i < items.Count; i++)
            {
                try
                {
                    var item = items[i].Split('|');
                    if (item.Length == 2)
                    {
                        listGroupModel.Add(new GroupModel
                        {
                            C_IDGroup = Guid.NewGuid().ToString(),
                            C_UIDGroup = item[0].Trim(),
                            C_NameGroup = item[1].Trim(),
                            C_FolderGroup = cbbFolderManageGroup.Text
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
            if (listGroupModel.Count > 0)
            {
                sqlController.BulkInsert(listGroupModel);

                sqlController.ReloadDataFolderManageGroup(cbbFolderManageGroup.Text);
            }
        }
        //Chọn
        private void chọnTấtCảToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var rows = tblManageGroup.Rows;
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].Cells["C_CheckGroup"].Value = true;
                rows[i].DefaultCellStyle.ForeColor = Settings.Default.colorOrange;
            }
        }
        private void bỏChọnTấtCảToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var rows = tblManageAcc.Rows;
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].Cells["C_CheckGroup"].Value = false;
                rows[i].DefaultCellStyle.ForeColor = Color.Black;
            }
        }
        private void chọnHoặcBỏChọnCácDòngBôiĐenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var row = FunctionHelper.GetRowSelected(tblManageGroup);
            for (int i = 0; i < tblManageGroup.Rows.Count; i++)
            {
                if ((bool)tblManageGroup.Rows[i].Cells["C_CheckGroup"].Value == false)
                    tblManageGroup.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
            }

            for (int i = 0; i < row.Count; i++)
            {
                if ((bool)row[i].Cells["C_CheckGroup"].Value == true)
                {
                    row[i].Cells["C_CheckGroup"].Value = false;
                    row[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    row[i].Cells["C_CheckGroup"].Value = true;
                    row[i].DefaultCellStyle.ForeColor = Settings.Default.colorOrange;
                }
            }
        }
        private void xóaDòngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<GroupModel> listGroupModel = new List<GroupModel>();
            var item = FunctionHelper.GetRowSelected(tblManageGroup);
            for (int i = 0; i < item.Count; i++)
            {
                listGroupModel.Add(new GroupModel
                {
                    C_IDGroup = (item[i].Cells["C_IDGroup"].Value.ToString())
                });
            }
            var status = MessageBox.Show("Bạn có chắc muốn xóa các dòng này?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (status == DialogResult.Yes)
            {
                sqlController.BulkDelete(listGroupModel);
                MessageBox.Show("Đã xóa!");
                for (int i = 0; i < item.Count; i++)
                {
                    item[i].Cells["C_StatusGroup"].Value = "Đã Xóa";
                    item[i].DefaultCellStyle.ForeColor = Settings.Default.colorRed;
                }
            }
        }
        private void lưuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<GroupModel> listGroupModel = new List<GroupModel>();
            for (int i = 0; i < tblManageAcc.Rows.Count; i++)
            {
                try
                {
                    var group = FunctionHelper.ConvertRowToGroupModel(tblManageGroup.Rows[i]);
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
        private void tảiLạiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cbbFolderManageGroup.Text == "All Group")
                sqlController.ReloadDataManageGroup();
            else
            {
                sqlController.ReloadDataFolderManageGroup(cbbFolderManageGroup.Text);
            }
        }
        private void btnCloseAllChromeGroup_Click(object sender, EventArgs e)
        {
            btnCloseAllChrome_Click(sender, e);
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
        private void btnStop_Click(object sender, EventArgs e)
        {
            stop = true;
            finish = true;
            btnStopGroup.Enabled = false;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            Settings.Default.ckbCheckLiveUID = ckbCheckLiveUID.Checked;
            Settings.Default.txtGroupPerAcc = txtGroupPerAcc.Text;
            Settings.Default.txtNumberThreadAcc = txtNumberThreadAcc.Text;
            Settings.Default.txtNumberThreadGroup = txtNumberThreadGroup.Text;
            Settings.Default.ckbSolveCheckPoint = ckbSolveCheckPoint.Checked;
            Settings.Default.ckbPostText = ckbPostText.Checked;
            Settings.Default.ckbPostLink = ckbPostLink.Checked;
            Settings.Default.ckbEditTextToLink = ckbEditTextToLink.Checked;
            Settings.Default.Save();
            _checkLiveUID = ckbCheckLiveUID.Checked;
            _solveCheckPoint = ckbSolveCheckPoint.Checked;
            _postText = ckbPostText.Checked;
            _postLink = ckbPostLink.Checked;
            _editTextToLink = ckbEditTextToLink.Checked;
            if (txtGroupPerAcc.Text == "")
            {
                MessageBox.Show("Vui lòng điền số group/acc!");
                return;
            }
            _groupPerAcc = int.Parse(txtGroupPerAcc.Text);
            _proxyList = new List<string>(File.ReadAllLines("input/Proxy.txt"));
            lblCountProxy.Text = _proxyList.Count.ToString();
            _contentList = new List<string>(File.ReadAllLines("input/ListContent.txt"));
            lblCountContent.Text = _contentList.Count.ToString();
            _linkVideoList = new List<string>(File.ReadAllLines("input/ListLinkVideo.txt"));
            lblCountLinkVideo.Text = _linkVideoList.Count.ToString();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            btnSave_Click(sender, e);
            if (cbbFolderManageGroup.Text == "")
            {
                MessageBox.Show("Vui lòng chọn thư mục!");
                return;
            }

            stop = false;
            _success = 0;
            _fail = 0;
            CurrentWidth = 0;
            CurrentHeight = 0;
            _scanPage = ckbScanPage.Checked;
            _listKeyWord = new List<string>(rtbListKeyWord.Text.Split('\n'));
            if (txtNumberThreadGroup.Text == "")
            {
                MessageBox.Show("Vui lòng điền số luồng!");
                return;
            }
            _numberThreadGroup = int.Parse(txtNumberThreadGroup.Text);
            if (_numberThreadGroup == 0)
            {
                MessageBox.Show("Vui lòng tăng số luồng!");
            }
            _threadRunningGroup = _numberThreadGroup;
            lblthreadRunningGroup.Text = _threadRunningGroup.ToString();
            lblSuccessGroup.Text = _success.ToString();
            lblFailGroup.Text = _fail.ToString();
            rowsAccChecked = FunctionHelper.GetRowChecked(tblManageAcc);
            if (!_editTextToLink)
            {
                if (rowsAccChecked.Count == 0)
                {
                    MessageBox.Show("Chưa chọn acc nào!");
                    return;
                }
                for (int i = 0; i < rowsAccChecked.Count; i++)
                {
                    rowsAccChecked[i].Cells["C_Status"].Value = "";
                }
            }

            if (_scanPage)
            {
                _listNotDupilicate = new HashSet<string>();
                using (StreamReader reader = new StreamReader("input/_listNotDupilicate.txt"))
                {
                    while (!reader.EndOfStream)
                    {
                        _listNotDupilicate.Add(reader.ReadLine());
                    }
                }

                for (int i = 0; i < _numberThreadGroup; i++)
                {
                    Thread thread = new Thread(OneThreadPostToGroup) { IsBackground = true };
                    thread.Start();
                }

                goto startAction;
            }
            rowsGroupChecked = FunctionHelper.GetRowCheckedGroup(tblManageGroup);
            for (int i = 0; i < rowsGroupChecked.Count; i++)
            {
                rowsGroupChecked[i].Cells["C_StatusGroup"].Value = "";
            }
            if (rowsGroupChecked.Count == 0)
            {
                MessageBox.Show("Chưa chọn group nào!");
                return;
            }

            if (!_editTextToLink)
            {
                for (int i = 0; i < _numberThreadGroup; i++)
                {
                    Thread thread = new Thread(OneThreadPostToGroup) { IsBackground = true };
                    thread.Start();
                }
            }
            else if (_editTextToLink)
            {
                sqlController.ReloadDataManageAcc();
                for (int i = 0; i < _numberThreadGroup; i++)
                {
                    Thread thread = new Thread(EditTextToLink) { IsBackground = true };
                    thread.Start();
                }
            }
            startAction:
            stopwatch = new Stopwatch();
            stopwatch.Start();
            timer1.Start();
            btnStartGroup.Enabled = false;
            btnStopGroup.Enabled = true;
        }
        #endregion
        #region FolderManageGroup
        private void btnAddFolderGroup_Click(object sender, EventArgs e)
        {
            FolderManageAccForm folderManageAccForm = new FolderManageAccForm("Group");
            folderManageAccForm.ShowDialog();

            MoveDataManageGroup.DropDownItems.Clear();
            foreach (var line in _listFolderManageGroup.Where(line => line != cbbFolderManageGroup.Text))
            {
                MoveDataManageGroup.DropDownItems.Add(line, null, ManageFolderHelper.OnClickTabGroup);
            }
            copyDữLiệuToolStripMenuItem.DropDownItems.Clear();
            foreach (var line in _listFolderManageGroup.Where(line => line != cbbFolderManageGroup.Text))
            {
                copyDữLiệuToolStripMenuItem.DropDownItems.Add(line, null, ManageFolderHelper.OnClickCopyTabGroup);
            }
        }
        private void btnDeleteFolderGroup_Click(object sender, EventArgs e)
        {
            var status = MessageBox.Show($"Bạn có chắc muốn xóa chủ đề và dữ liệu trong chủ đề {cbbFolderManageGroup.Text} này? ", "Cảnh báo!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (status == DialogResult.Yes)
            {
                sqlController.excuteSQL($"DELETE FROM tbl_folders WHERE C_Folder='{cbbFolderManageGroup.Text}'");
                sqlController.excuteSQL($"DELETE FROM tbl_groups WHERE C_FolderGroup='{cbbFolderManageGroup.Text}'");
                sqlController.LoadDataIntoComboBoxManageGroup();
                tblManageGroup.Rows.Clear();
                cbbFolderManageGroup.Text = "";
            }

            MoveDataManageGroup.DropDownItems.Clear();
            foreach (var line in _listFolderManageGroup.Where(line => line != cbbFolderManageGroup.Text))
            {
                MoveDataManageGroup.DropDownItems.Add(line, null, ManageFolderHelper.OnClickTabGroup);
            }
            copyDữLiệuToolStripMenuItem.DropDownItems.Clear();
            foreach (var line in _listFolderManageGroup.Where(line => line != cbbFolderManageGroup.Text))
            {
                copyDữLiệuToolStripMenuItem.DropDownItems.Add(line, null, ManageFolderHelper.OnClickCopyTabGroup);
            }
        }
        #endregion
        private void OneThreadPostToGroup()
        {
            AccountModel account = null;
            GroupModel group = null;
            FacebookAPIController apiFacebook = null;
            DataGridViewRow rowAcc;
            DataGridViewRow rowGroup;
            //int randomRowIndex = 0;
            bool success = false;
            string content = "", linkVideo = "", keyWord = "";
            while (!stop)
            {
                lock (lockRowAccChecked)
                {
                    if (rowsAccChecked.Count == 0)
                        break;
                    rowAcc = rowsAccChecked[0];
                    rowsAccChecked.RemoveAt(0);
                    if (!_scanPage)
                        rowsAccChecked.Add(rowAcc);
                }
                account = FunctionHelper.ConvertRowToAccountModel(rowAcc);
                if (account == null)
                {
                    success = false;
                    FunctionHelper.EditValueColumn(account, "C_Status", "Vui lòng thử lại!", true);
                    goto continueToNextAcc;
                }

                apiFacebook = new FacebookAPIController();
                if (_scanPage)
                {
                    reStartScanPage:
                    lock (lockKeyword)
                    {
                        if (_listKeyWord.Count == 0)
                        {
                            break;
                        }
                        keyWord = _listKeyWord[0];
                        _listKeyWord.RemoveAt(0);
                    }
                    FunctionHelper.EditValueColumn(account, "C_Status", $"Đang scan page");
                    var status = apiFacebook.ScanGroups(account, keyWord);
                    if (status == ResultModel.Success)
                    {
                        success = true;
                        //FunctionHelper.EditValueColumn(account, "C_Status", $"Scan xong", true);
                        if (!stop)
                        {
                            goto reStartScanPage;
                        }
                        account.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorGreen;
                        break;
                    }
                    else if (status == ResultModel.Fail)
                    {
                        success = false;
                        //FunctionHelper.EditValueColumn(account, "C_Status", $"Scan lỗi", true);
                        account.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorRed;
                        goto continueToNextAcc;
                    }
                }
                if (_postText)
                {
                    for (int i = 1; i <= _groupPerAcc; i++)
                    {
                        lock (lockContent)
                        {
                            if (_contentList.Count == 0)
                            {
                                goto end;
                            }
                            content = _contentList[0];
                            _contentList.RemoveAt(0);
                            _contentList.Add(content);
                        }
                        lock (lockRowGroupChecked)
                        {
                            if (rowsGroupChecked.Count == 0)
                                goto end;
                            rowGroup = rowsGroupChecked[0];
                            rowsGroupChecked.RemoveAt(0);
                        }

                        group = FunctionHelper.ConvertRowToGroupModel(rowGroup);
                        if (group == null)
                        {
                            FunctionHelper.EditValueColumn(group, "C_StatusGroup", "Vui lòng thử lại!", true);
                            group.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorRed;
                            Interlocked.Increment(ref _fail);
                            Invoke((MethodInvoker)delegate ()
                            {
                                lblFailGroup.Text = _fail.ToString();
                            });
                            group.C_Row.Cells["C_CheckGroup"].Value = false;
                            i--;
                            continue;
                        }
                        FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Đang post text!");
                        var status = apiFacebook.PostTextToGroup(account, group, content);
                        if (status == ResultModel.Success)
                        {
                            success = true;
                            FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Post text ok!", true);
                            FunctionHelper.EditValueColumn(group, "C_UIDVia", group.C_UIDVia);
                            FunctionHelper.EditValueColumn(group, "C_PostID", group.C_PostID);
                            FunctionHelper.EditValueColumn(group, "C_CreatedPost", group.C_CreatedPost);
                        }
                        else if (status == ResultModel.Fail)
                        {
                            i--;
                            success = false;
                            FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Post text fail", true);

                        }
                        else if (status == ResultModel.PostDeleted)
                        {
                            i--;
                            success = false;
                            FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Post bị xóa!", true);

                        }
                        else if (status == ResultModel.CheckPoint)
                        {
                            success = false;
                            FunctionHelper.EditValueColumn(account, "C_Status", $"CheckPoint", true);
                            lock (lockRowGroupChecked)
                            {
                                rowsGroupChecked.Insert(0, rowGroup);
                            }
                            goto continueToNextAcc;
                        }

                        if (success)
                        {
                            group.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorGreen;
                            Interlocked.Increment(ref _success);
                            Invoke((MethodInvoker)delegate ()
                            {
                                lblSuccessGroup.Text = _success.ToString();
                            });
                        }
                        else
                        {
                            group.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorRed;
                            Interlocked.Increment(ref _fail);
                            Invoke((MethodInvoker)delegate ()
                            {
                                lblFailGroup.Text = _fail.ToString();
                            });
                        }

                        group.C_Row.Cells["C_CheckGroup"].Value = false;
                    }
                    account.C_Row.Cells["C_Check"].Value = false;
                }
                if (_postLink)
                {
                    for (int i = 1; i <= _groupPerAcc; i++)
                    {
                        lock (lockContent)
                        {
                            if (_contentList.Count == 0)
                                goto end;
                            content = _contentList[0];
                            _contentList.RemoveAt(0);
                            _contentList.Add(content);
                        }
                        lock (locklinkVideo)
                        {
                            if (_linkVideoList.Count == 0)
                                goto end;
                            linkVideo = _linkVideoList[0];
                            _linkVideoList.RemoveAt(0);
                            _linkVideoList.Add(linkVideo);
                        }
                        lock (lockRowGroupChecked)
                        {
                            if (rowsGroupChecked.Count == 0)
                                goto end;
                            rowGroup = rowsGroupChecked[0];
                            rowsGroupChecked.RemoveAt(0);
                        }
                        group = FunctionHelper.ConvertRowToGroupModel(rowGroup);
                        if (group == null)
                        {
                            FunctionHelper.EditValueColumn(group, "C_StatusGroup", "Vui lòng thử lại!", true);
                            group.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorRed;
                            Interlocked.Increment(ref _fail);
                            Invoke((MethodInvoker)delegate ()
                            {
                                lblFailGroup.Text = _fail.ToString();
                            });
                            group.C_Row.Cells["C_CheckGroup"].Value = false;
                            i--;
                            continue;
                        }

                        FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Đang post link!");
                        var status = apiFacebook.ShareLinkPost(account, group, linkVideo, content);
                        if (status == ResultModel.Success)
                        {
                            success = true;
                            FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Post link ok!", true);
                            FunctionHelper.EditValueColumn(group, "C_UIDVia", group.C_UIDVia);
                            FunctionHelper.EditValueColumn(group, "C_PostID", group.C_PostID);
                            FunctionHelper.EditValueColumn(group, "C_CreatedPost", group.C_CreatedPost);
                        }
                        else if (status == ResultModel.Fail)
                        {
                            success = false;
                            FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Post link fail", true);
                            i--;
                        }
                        else if (status == ResultModel.CheckPoint956)
                        {
                            success = false;
                            FunctionHelper.EditValueColumn(account, "C_Status", $"CheckPoint 956", true);
                            lock (lockRowGroupChecked)
                            {
                                rowsGroupChecked.Insert(0, rowGroup);
                            }
                            i--;
                            goto continueToNextAcc;
                        }
                        else if (status == ResultModel.PostDeleted)
                        {
                            i--;
                            success = false;
                            FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Post bị xóa!", true);

                        }
                        if (success)
                        {
                            group.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorGreen;
                            Interlocked.Increment(ref _success);
                            Invoke((MethodInvoker)delegate ()
                            {
                                lblSuccessGroup.Text = _success.ToString();
                            });
                        }
                        else
                        {
                            group.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorRed;
                            Interlocked.Increment(ref _fail);
                            Invoke((MethodInvoker)delegate ()
                            {
                                lblFailGroup.Text = _fail.ToString();
                            });
                        }

                        group.C_Row.Cells["C_CheckGroup"].Value = false;
                    }
                    account.C_Row.Cells["C_Check"].Value = false;
                }
                continueToNextAcc:
                account.C_Row.Cells["C_Check"].Value = false;
            }
            end:
            try
            {
                if (account != null)
                    account.C_Row.Cells["C_Check"].Value = false;
            }
            catch
            {

            }
            Interlocked.Decrement(ref _threadRunningGroup);
            Invoke((MethodInvoker)delegate ()
            {
                lblthreadRunningGroup.Text = _threadRunningGroup.ToString();
            });
            if (_threadRunningGroup == 0)
            {
                Invoke(new MethodInvoker(() =>
                {
                    _proxyList = new List<string>(File.ReadAllLines("input/Proxy.txt"));
                    lblCountProxy.Text = _proxyList.Count.ToString();
                    btnStopGroup.Enabled = false;
                    btnStartGroup.Enabled = true;
                    timer1.Stop();
                }));

                MessageBox.Show("Tool đã dừng");
            }
        }
        private void EditTextToLink()
        {
            AccountModel account = null;
            GroupModel group = null;
            FacebookAPIController apiFacebook = null;
            //DataGridViewRow rowAcc;
            DataGridViewRow rowGroup;
            //int randomRowIndex = 0;
            bool success = false;
            string content = "", linkVideo = "";
            while (!stop)
            {
                apiFacebook = new FacebookAPIController();
                lock (lockRowGroupChecked)
                {
                    if (rowsGroupChecked.Count == 0)
                        break;
                    rowGroup = rowsGroupChecked[0];
                    rowsGroupChecked.RemoveAt(0);
                }
                group = FunctionHelper.ConvertRowToGroupModel(rowGroup);
                if (group == null)
                {
                    FunctionHelper.EditValueColumn(group, "C_StatusGroup", "Vui lòng thử lại!", true);
                    group.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorRed;
                    Interlocked.Increment(ref _fail);
                    Invoke((MethodInvoker)delegate ()
                    {
                        lblFailGroup.Text = _fail.ToString();
                    });
                    group.C_Row.Cells["C_CheckGroup"].Value = false;
                    group.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorRed;
                    Interlocked.Increment(ref _fail);
                    Invoke((MethodInvoker)delegate ()
                    {
                        lblFailGroup.Text = _fail.ToString();
                    });
                    break;
                }
                for (int i = 0; i < tblManageAcc.Rows.Count; i++)
                {
                    if (tblManageAcc.Rows[i].Cells["C_UID"].Value.ToString() == group.C_UIDVia)
                    {
                        account = FunctionHelper.ConvertRowToAccountModel(tblManageAcc.Rows[i]);
                        account.C_Row.Cells["C_Check"].Value = true;
                    }
                }
                if (account == null)
                {
                    success = false;
                    FunctionHelper.EditValueColumn(account, "C_Status", "Vui lòng thử lại!", true);
                    break;
                }
                lock (lockContent)
                {
                    if (_contentList.Count == 0)
                        break;
                    content = _contentList[0];
                    _contentList.RemoveAt(0);
                    _contentList.Add(content);
                }
                lock (locklinkVideo)
                {
                    if (_linkVideoList.Count == 0)
                        break;
                    linkVideo = _linkVideoList[0];
                    _linkVideoList.RemoveAt(0);
                    _linkVideoList.Add(linkVideo);
                }
                FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Đang edit text thành link!");

                var status = apiFacebook.EditPost(account, group, linkVideo, "");

                if (status == ResultModel.Success)
                {
                    success = true;
                    FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Edit post ok!", true);
                    FunctionHelper.EditValueColumn(group, "C_TimeEditPost", group.C_TimeEditPost);
                }
                else if (status == ResultModel.Fail)
                {
                    success = false;
                    FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Edit post fail", true);

                }
                else if (status == ResultModel.PostDeleted)
                {
                    success = false;
                    FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Post bị xóa!", true);

                }
                else if (status == ResultModel.CheckPoint956)
                {
                    success = false;
                    FunctionHelper.EditValueColumn(account, "C_Status", $"CheckPoint 956", true);
                    lock (lockRowGroupChecked)
                    {
                        rowsGroupChecked.Insert(0, rowGroup);
                    }
                }
                if (success)
                {
                    group.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorGreen;
                    Interlocked.Increment(ref _success);
                    Invoke((MethodInvoker)delegate ()
                    {
                        lblSuccessGroup.Text = _success.ToString();
                    });
                }
                else
                {
                    group.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorRed;
                    Interlocked.Increment(ref _fail);
                    Invoke((MethodInvoker)delegate ()
                    {
                        lblFailGroup.Text = _fail.ToString();
                    });
                }
                try
                {
                    group.C_Row.Cells["C_CheckGroup"].Value = false;
                    account.C_Row.Cells["C_Check"].Value = false;
                }
                catch
                {

                }
            }
            end:
            Interlocked.Decrement(ref _threadRunningGroup);
            Invoke((MethodInvoker)delegate ()
            {
                lblthreadRunningGroup.Text = _threadRunningGroup.ToString();
            });
            if (_threadRunningGroup == 0)
            {
                Invoke(new MethodInvoker(() =>
                {
                    _proxyList = new List<string>(File.ReadAllLines("input/Proxy.txt"));
                    lblCountProxy.Text = _proxyList.Count.ToString();
                    btnStopGroup.Enabled = false;
                    btnStartGroup.Enabled = true;
                    timer1.Stop();
                }));

                MessageBox.Show("Tool đã dừng");
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTimeRunning.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
            lblTimeRunningGroup.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
        }

    }
}
