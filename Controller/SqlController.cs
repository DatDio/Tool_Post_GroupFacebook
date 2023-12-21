using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tool_Facebook.Model;

namespace Tool_Facebook.Controller
{
    public class SqlController
    {
        private SQLiteConnection _con;
        private string tableName;
        public SqlController(string pathTableName = "input/Account.sqllite")
        {
            _con = new SQLiteConnection();
            tableName = Path.GetFullPath(pathTableName);
            createConnection();

        }
        public void createConnection()
        {
            _con.ConnectionString = $"Data Source={tableName};Version=3;";
            _con.Open();
        }
        public void closeConnection()
        {
            _con.Close();
        }
        public void createTable(string sql)
        {
            if (!File.Exists(tableName))
            {

                SQLiteConnection.CreateFile(tableName);
            }
            try
            {
                var command = new SQLiteCommand(sql, _con);
                command.ExecuteNonQuery();
            }
            catch { }
        }
        public void excuteSQL(string sql)
        {
            try
            {
                var command = new SQLiteCommand(sql, _con);
                command.ExecuteNonQuery();
            }
            catch { }
        }

        #region SqlManageAcc
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

                Form1.tblManageAcc.Invoke(new MethodInvoker(delegate
                {
                    Form1.tblManageAcc.Rows.Clear();

                    Form1.tblManageAcc.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    Form1.tblManageAcc.ColumnHeadersVisible = false;
                    Form1.tblManageAcc.RowHeadersVisible = false;
                    Form1.tblManageAcc.Rows.AddRange(rows.ToArray());
                    Form1.tblManageAcc.ColumnHeadersVisible = true;
                    Form1.tblManageAcc.RowHeadersVisible = true;
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

                Form1.tblManageAcc.Invoke(new MethodInvoker(delegate
                {
                    Form1.tblManageAcc.Rows.Clear();

                    Form1.tblManageAcc.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    Form1.tblManageAcc.ColumnHeadersVisible = false;
                    Form1.tblManageAcc.RowHeadersVisible = false;
                    Form1.tblManageAcc.Rows.AddRange(rows.ToArray());
                    Form1.tblManageAcc.ColumnHeadersVisible = true;
                    Form1.tblManageAcc.RowHeadersVisible = true;
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
                Form1._listFolderManageAcc = new List<string>();
                var table = Select("SELECT * FROM tbl_folders ").Tables[0];
                for (int i = Form1.cbbFolderManageAcc1.Items.Count - 1; i >= 0; i--)
                {
                    object currentItem = Form1.cbbFolderManageAcc1.Items[i];

                    if (currentItem.ToString() != "All Acc")
                    {
                        Form1.cbbFolderManageAcc1.Items.RemoveAt(i);
                    }
                }
                //Form1._cbbTopic.DisplayMember = table.Columns["C_Topic"].ToString();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    if (table.Rows[i]["C_Type"].ToString() == "Account")
                    {
                        Form1.cbbFolderManageAcc1.Items.Add(table.Rows[i]["C_Folder"].ToString());
                        Form1._listFolderManageAcc.Add(table.Rows[i]["C_Folder"].ToString());
                    }

                }

            }
            catch { }
        }
        public void BulkDelete(List<AccountModel> accountDtos)
        {
            try
            {
                using (SQLiteTransaction transaction = _con.BeginTransaction())
                {
                    for (var i = 0; i < accountDtos.Count; i++)
                    {
                        var account = accountDtos[i];
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText = $"DELETE FROM tbl_accounts WHERE C_UID=@C_UID";
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SQLiteParameter("@C_UID", account.C_UID));

                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch
                            {
                                MessageBox.Show("Có lỗi xảy ra");
                                break;
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
        public void BulkInsert(List<AccountModel> accountDtos)
        {
            try
            {
                using (SQLiteTransaction transaction = _con.BeginTransaction())
                {
                    for (var i = 0; i < accountDtos.Count; i++)
                    {
                        var account = accountDtos[i];
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText =
                                $"INSERT INTO tbl_accounts(C_UID,C_Password,C_Email, C_PassEmail, C_2FA,C_Status,C_Folder,C_Cookie,C_Token,C_Proxy,C_UserAgent) " +
                                $"VALUES(@C_UID,@C_Password,@C_Email,@C_PassEmail, @C_2FA,@C_Status,@C_Folder, @C_Cookie, @C_Token,  @C_Proxy,@C_UserAgent)";
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SQLiteParameter("@C_UID", account.C_UID));
                            command.Parameters.Add(new SQLiteParameter("@C_Password", account.C_Password));
                            command.Parameters.Add(new SQLiteParameter("@C_Email", account.C_Email));
                            command.Parameters.Add(new SQLiteParameter("@C_PassEmail", account.C_PassEmail));
                            command.Parameters.Add(new SQLiteParameter("@C_2FA", account.C_2FA));
                            command.Parameters.Add(new SQLiteParameter("@C_Status", account.C_Status));
                            command.Parameters.Add(new SQLiteParameter("@C_Folder", account.C_Folder));
                            command.Parameters.Add(new SQLiteParameter("@C_Cookie", account.C_Cookie));
                            command.Parameters.Add(new SQLiteParameter("@C_Token", account.C_Token));
                            command.Parameters.Add(new SQLiteParameter("@C_Proxy", account.C_Proxy));
                            command.Parameters.Add(new SQLiteParameter("@C_UserAgent", account.C_UserAgent));
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (SQLiteException ex)
                            {
                                if (ex.Message.Contains("UNIQUE constraint failed"))
                                {
                                    MessageBox.Show($"Tài khoản {account.C_UID} đã tồn tại!");
                                }
                                else
                                {
                                    MessageBox.Show("Có lỗi xảy ra!");
                                }
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
        public void BulkUpdate(List<AccountModel> accountDtos)
        {
            try
            {
                using (SQLiteTransaction transaction = _con.BeginTransaction())
                {
                    for (var i = 0; i < accountDtos.Count; i++)
                    {
                        var account = accountDtos[i];
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText = $"UPDATE tbl_accounts SET C_UID =@C_UID,C_Password = @C_Password,C_Email =@C_Email ,C_PassEmail = @C_PassEmail, C_2FA = @C_2FA,C_Folder=@C_Folder, C_Cookie = @C_Cookie,C_Token = @C_Token,C_Status = @C_Status,C_Proxy = @C_Proxy,C_UserAgent = @C_UserAgent WHERE C_UID = @C_UID";
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SQLiteParameter("@C_UID", account.C_UID));
                            command.Parameters.Add(new SQLiteParameter("@C_Password", account.C_Password));
                            command.Parameters.Add(new SQLiteParameter("@C_Email", account.C_Email));
                            command.Parameters.Add(new SQLiteParameter("@C_PassEmail", account.C_PassEmail));
                            command.Parameters.Add(new SQLiteParameter("@C_2FA", account.C_2FA));
                            command.Parameters.Add(new SQLiteParameter("@C_Status", account.C_Status));
                            command.Parameters.Add(new SQLiteParameter("@C_Folder", account.C_Folder));
                            command.Parameters.Add(new SQLiteParameter("@C_Cookie", account.C_Cookie));
                            command.Parameters.Add(new SQLiteParameter("@C_Token", account.C_Token));
                            command.Parameters.Add(new SQLiteParameter("@C_Proxy", account.C_Proxy));
                            command.Parameters.Add(new SQLiteParameter("@C_UserAgent", account.C_UserAgent));

                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch
                            {
                                //
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
        public void Update(AccountModel account)
        {
            try
            {
                using (SQLiteTransaction transaction = _con.BeginTransaction())
                {

                    using (SQLiteCommand command = new SQLiteCommand(_con))
                    {
                        command.CommandText = $"UPDATE tbl_accounts SET C_UID =@C_UID,C_Password = @C_Password,C_Email =@C_Email ,C_PassEmail = @C_PassEmail, C_2FA = @C_2FA,C_Folder=@C_Folder, C_Cookie = @C_Cookie,C_Token = @C_Token,C_Status = @C_Status,C_Proxy = @C_Proxy,C_UserAgent = @C_UserAgent WHERE C_UID = @C_UID";
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SQLiteParameter("@C_UID", account.C_UID));
                        command.Parameters.Add(new SQLiteParameter("@C_Password", account.C_Password));
                        command.Parameters.Add(new SQLiteParameter("@C_Email", account.C_Email));
                        command.Parameters.Add(new SQLiteParameter("@C_PassEmail", account.C_PassEmail));
                        command.Parameters.Add(new SQLiteParameter("@C_2FA", account.C_2FA));
                        command.Parameters.Add(new SQLiteParameter("@C_Status", account.C_Status));
                        command.Parameters.Add(new SQLiteParameter("@C_Folder", account.C_Folder));
                        command.Parameters.Add(new SQLiteParameter("@C_Cookie", account.C_Cookie));
                        command.Parameters.Add(new SQLiteParameter("@C_Token", account.C_Token));
                        command.Parameters.Add(new SQLiteParameter("@C_Proxy", account.C_Proxy));
                        command.Parameters.Add(new SQLiteParameter("@C_UserAgent", account.C_UserAgent));
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch
                        {
                            //
                        }


                        transaction.Commit();
                    }
                }
            }
            catch
            {
                //
            }
        }
        #endregion

        #region SqlManageGroup
        public void BulkDelete(List<GroupModel> groupDtos)
        {
            try
            {
                using (SQLiteTransaction transaction = _con.BeginTransaction())
                {
                    for (var i = 0; i < groupDtos.Count; i++)
                    {
                        var group = groupDtos[i];
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText = $"DELETE FROM tbl_groups WHERE C_IDGroup=@C_IDGroup";
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SQLiteParameter("@C_IDGroup", group.C_IDGroup));

                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch
                            {
                                MessageBox.Show("Có lỗi xảy ra");
                                break;
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
        public void BulkInsert(List<GroupModel> groupDtos)
        {
            try
            {
                using (SQLiteTransaction transaction = _con.BeginTransaction())
                {
                    for (var i = 0; i < groupDtos.Count; i++)
                    {
                        var group = groupDtos[i];
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText = $"INSERT INTO tbl_groups(C_IDGroup,C_UIDGroup,C_NameGroup,C_Censorship, C_UIDVia, " +
                                $"C_PostID, C_StatusGroup, C_CreatedPost,C_TimeEditPost," +
                                $" C_MemberGroup,C_TypeGroup,C_FolderGroup)" +
                                $"VALUES(@C_IDGroup,@C_UIDGroup,@C_NameGroup,@C_Censorship,@C_UIDVia," +
                                $" @C_PostID, @C_StatusGroup, @C_CreatedPost," +
                                $"@C_TimeEditPost, @C_MemberGroup,@C_TypeGroup,@C_FolderGroup)";
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SQLiteParameter("@C_IDGroup", group.C_IDGroup));
                            command.Parameters.Add(new SQLiteParameter("@C_UIDGroup", group.C_UIDGroup));
                            command.Parameters.Add(new SQLiteParameter("@C_NameGroup", group.C_NameGroup));
                            command.Parameters.Add(new SQLiteParameter("@C_Censorship", group.C_Censorship));
                            command.Parameters.Add(new SQLiteParameter("@C_UIDVia", group.C_UIDVia));
                            command.Parameters.Add(new SQLiteParameter("@C_PostID", group.C_PostID));
                            command.Parameters.Add(new SQLiteParameter("@C_StatusGroup", group.C_StatusGroup));
                            command.Parameters.Add(new SQLiteParameter("@C_CreatedPost", group.C_CreatedPost));
                            command.Parameters.Add(new SQLiteParameter("@C_TimeEditPost", group.C_TimeEditPost));
                            command.Parameters.Add(new SQLiteParameter("@C_MemberGroup", group.C_MemberGroup));
                            command.Parameters.Add(new SQLiteParameter("@C_TypeGroup", group.C_TypeGroup));
                            command.Parameters.Add(new SQLiteParameter("@C_FolderGroup", group.C_FolderGroup));
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (SQLiteException ex)
                            {
                                if (ex.Message.Contains("UNIQUE constraint failed"))
                                {
                                    MessageBox.Show("Tài khoản này đã tồn tại!");
                                }
                                else
                                {
                                    MessageBox.Show("Có lỗi xảy ra!");
                                }
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
        public void BulkUpdate(List<GroupModel> groupDtos)
        {
            try
            {
                using (SQLiteTransaction transaction = _con.BeginTransaction())
                {
                    for (var i = 0; i < groupDtos.Count; i++)
                    {
                        var group = groupDtos[i];
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText = $"UPDATE tbl_groups SET C_UIDGroup =@C_UIDGroup, C_NameGroup = @C_NameGroup,C_Censorship = @C_Censorship,C_UIDVia =@C_UIDVia ,C_PostID = @C_PostID, C_StatusGroup = @C_StatusGroup, C_CreatedPost = @C_CreatedPost,C_TimeEditPost = @C_TimeEditPost, C_MemberGroup = @C_MemberGroup,C_TypeGroup = @C_TypeGroup,C_FolderGroup = @C_FolderGroup WHERE C_IDGroup = @C_IDGroup";
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SQLiteParameter("@C_IDGroup", group.C_IDGroup));
                            command.Parameters.Add(new SQLiteParameter("@C_UIDGroup", group.C_UIDGroup));
                            command.Parameters.Add(new SQLiteParameter("@C_Censorship", group.C_Censorship));
                            command.Parameters.Add(new SQLiteParameter("@C_NameGroup", group.C_NameGroup));
                            command.Parameters.Add(new SQLiteParameter("@C_UIDVia", group.C_UIDVia));
                            command.Parameters.Add(new SQLiteParameter("@C_PostID", group.C_PostID));
                            command.Parameters.Add(new SQLiteParameter("@C_StatusGroup", group.C_StatusGroup));
                            command.Parameters.Add(new SQLiteParameter("@C_CreatedPost", group.C_CreatedPost));
                            command.Parameters.Add(new SQLiteParameter("@C_TimeEditPost", group.C_TimeEditPost));
                            command.Parameters.Add(new SQLiteParameter("@C_MemberGroup", group.C_MemberGroup));
                            command.Parameters.Add(new SQLiteParameter("@C_TypeGroup", group.C_TypeGroup));
                            command.Parameters.Add(new SQLiteParameter("@C_FolderGroup", group.C_FolderGroup));
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch
                            {
                                //
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
        public void Update(GroupModel group)
        {
            try
            {
                using (SQLiteTransaction transaction = _con.BeginTransaction())
                {

                    using (SQLiteCommand command = new SQLiteCommand(_con))
                    {
                        command.CommandText = $"UPDATE tbl_groups SET C_UIDGroup =@C_UIDGroup, C_NameGroup = @C_NameGroup,C_Censorship = @C_Censorship,C_UIDVia = @C_UIDVia, C_PostID = @C_PostID,C_StatusGroup = @C_StatusGroup, C_CreatedPost = @C_CreatedPost,C_TimeEditPost = @C_TimeEditPost,C_MemberGroup = @C_MemberGroup,C_TypeGroup = @C_TypeGroup WHERE C_IDGroup = @C_IDGroup";
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SQLiteParameter("@C_IDGroup", group.C_IDGroup));
                        command.Parameters.Add(new SQLiteParameter("@C_UIDGroup", group.C_UIDGroup));
                        command.Parameters.Add(new SQLiteParameter("@C_NameGroup", group.C_NameGroup));
                        command.Parameters.Add(new SQLiteParameter("@C_Censorship", group.C_Censorship));
                        command.Parameters.Add(new SQLiteParameter("@C_UIDVia", group.C_UIDVia));
                        command.Parameters.Add(new SQLiteParameter("@C_PostID", group.C_PostID));
                        command.Parameters.Add(new SQLiteParameter("@C_StatusGroup", group.C_StatusGroup));
                        command.Parameters.Add(new SQLiteParameter("@C_CreatedPost", group.C_CreatedPost));
                        command.Parameters.Add(new SQLiteParameter("@C_TimeEditPost", group.C_TimeEditPost));
                        command.Parameters.Add(new SQLiteParameter("@C_MemberGroup", group.C_MemberGroup));
                        command.Parameters.Add(new SQLiteParameter("@C_TypeGroup", group.C_TypeGroup));
                        command.Parameters.Add(new SQLiteParameter("@C_FolderGroup", group.C_FolderGroup));
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch
                        {
                            //
                        }


                        transaction.Commit();
                    }
                }
            }
            catch
            {
                //
            }
        }
        public void ReloadDataManageGroup()
        {
            try
            {
                var table = Select("SELECT * FROM tbl_groups").Tables[0];

                List<DataGridViewRow> rows = new List<DataGridViewRow>();

                for (var i = 0; i < table.Rows.Count; i++)
                {
                    int stt = 0;
                    var row = table.Rows[i];
                    DataGridViewRow row1 = new DataGridViewRow();
                    row1.CreateCells(Form1.tblManageGroup);
                    row1.Cells[stt++].Value = false;
                    row1.Cells[stt++].Value = row["C_IDGroup"].ToString();
                    row1.Cells[stt++].Value = row["C_UIDGroup"].ToString();
                    row1.Cells[stt++].Value = row["C_NameGroup"].ToString();
                    row1.Cells[stt++].Value = row["C_Censorship"].ToString();
                    row1.Cells[stt++].Value = row["C_TypeGroup"].ToString();
                    row1.Cells[stt++].Value = row["C_UIDVia"].ToString();
                    row1.Cells[stt++].Value = row["C_PostID"].ToString();
                    row1.Cells[stt++].Value = row["C_StatusGroup"].ToString();
                    row1.Cells[stt++].Value = row["C_CreatedPost"].ToString();
                    row1.Cells[stt++].Value = row["C_TimeEditPost"].ToString();

                    int mem = 0;
                    if (row["C_MemberGroup"].ToString() != "")
                    {
                        mem = int.Parse(row["C_MemberGroup"].ToString().Replace(",", ""));
                    }
                    row1.Cells[stt++].Value = mem;

                    row1.Cells[stt++].Value = row["C_FolderGroup"].ToString();
                    rows.Add(row1);
                }

                Form1.tblManageGroup.Invoke(new MethodInvoker(delegate
                {
                    Form1.tblManageGroup.Rows.Clear();

                    Form1.tblManageGroup.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    Form1.tblManageGroup.ColumnHeadersVisible = false;
                    Form1.tblManageGroup.RowHeadersVisible = false;
                    Form1.tblManageGroup.Rows.AddRange(rows.ToArray());
                    Form1.tblManageGroup.ColumnHeadersVisible = true;
                    Form1.tblManageGroup.RowHeadersVisible = true;
                }));
            }
            catch
            {
                //
            }
        }
        public void LoadDataIntoComboBoxManageGroup()
        {
            try
            {
                Form1._listFolderManageGroup = new List<string>();

                var table = Select("SELECT * FROM tbl_folders").Tables[0];
                for (int i = Form1.cbbFolderManageGroup1.Items.Count - 1; i >= 0; i--)
                {
                    object currentItem = Form1.cbbFolderManageGroup1.Items[i];

                    if (currentItem.ToString() != "All Group")
                    {
                        Form1.cbbFolderManageGroup1.Items.RemoveAt(i);
                    }
                }
                //Form1._cbbTopic.DisplayMember = table.Columns["C_Topic"].ToString();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    if (table.Rows[i]["C_Type"].ToString() == "Group")
                    {
                        Form1.cbbFolderManageGroup1.Items.Add(table.Rows[i]["C_Folder"].ToString());
                        Form1._listFolderManageGroup.Add(table.Rows[i]["C_Folder"].ToString());
                    }
                }

            }
            catch { }
        }
        public void ReloadDataFolderManageGroup(string folder)
        {
            try
            {
                var table = Select($"SELECT * FROM tbl_groups WHERE C_FolderGroup = '{folder}'").Tables[0];

                List<DataGridViewRow> rows = new List<DataGridViewRow>();

                for (var i = 0; i < table.Rows.Count; i++)
                {
                    int stt = 0;
                    var row = table.Rows[i];
                    DataGridViewRow row1 = new DataGridViewRow();
                    row1.CreateCells(Form1.tblManageGroup);
                    row1.Cells[stt++].Value = false;
                    row1.Cells[stt++].Value = row["C_IDGroup"].ToString();
                    row1.Cells[stt++].Value = row["C_UIDGroup"].ToString();
                    row1.Cells[stt++].Value = row["C_NameGroup"].ToString();
                    row1.Cells[stt++].Value = row["C_Censorship"].ToString();
                    row1.Cells[stt++].Value = row["C_TypeGroup"].ToString();
                    row1.Cells[stt++].Value = row["C_UIDVia"].ToString();
                    row1.Cells[stt++].Value = row["C_PostID"].ToString();
                    row1.Cells[stt++].Value = row["C_StatusGroup"].ToString();
                    row1.Cells[stt++].Value = row["C_CreatedPost"].ToString();
                    row1.Cells[stt++].Value = row["C_TimeEditPost"].ToString();
                    int mem = 0;
                    if (row["C_MemberGroup"].ToString() != "")
                    {
                        mem = int.Parse(row["C_MemberGroup"].ToString().Replace(",", ""));
                    }
                    row1.Cells[stt++].Value = mem;
                    row1.Cells[stt++].Value = row["C_FolderGroup"].ToString();
                    rows.Add(row1);
                }

                Form1.tblManageGroup.Invoke(new MethodInvoker(delegate
                {
                    Form1.tblManageGroup.Rows.Clear();

                    Form1.tblManageGroup.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    Form1.tblManageGroup.ColumnHeadersVisible = false;
                    Form1.tblManageGroup.RowHeadersVisible = false;
                    Form1.tblManageGroup.Rows.AddRange(rows.ToArray());
                    Form1.tblManageGroup.ColumnHeadersVisible = true;
                    Form1.tblManageGroup.RowHeadersVisible = true;
                }));
            }
            catch
            {
                //
            }
        }

        public void ReloadDataFolderManageGroup(List<GroupModel> listGroup)
        {
            try
            {
                List<DataGridViewRow> rows = new List<DataGridViewRow>();

                for (var i = 0; i < listGroup.Count; i++)
                {
                    int stt = 0;
                    DataGridViewRow row1 = new DataGridViewRow();
                    row1.CreateCells(Form1.tblManageGroup);
                    row1.Cells[stt++].Value = false;
                    row1.Cells[stt++].Value = listGroup[i].C_IDGroup;
                    row1.Cells[stt++].Value = listGroup[i].C_UIDGroup;
                    row1.Cells[stt++].Value = listGroup[i].C_NameGroup;
                    row1.Cells[stt++].Value = listGroup[i].C_Censorship;
                    row1.Cells[stt++].Value = listGroup[i].C_TypeGroup;
                    row1.Cells[stt++].Value = listGroup[i].C_UIDVia;
                    row1.Cells[stt++].Value = listGroup[i].C_PostID;
                    row1.Cells[stt++].Value = listGroup[i].C_StatusGroup;
                    row1.Cells[stt++].Value = listGroup[i].C_CreatedPost;
                    row1.Cells[stt++].Value = listGroup[i].C_TimeEditPost;

                    int mem = 0;
                    if (listGroup[i].C_MemberGroup != "")
                    {
                        mem = int.Parse(listGroup[i].C_MemberGroup.Replace(",", ""));
                    }
                    row1.Cells[stt++].Value = mem;

                    row1.Cells[stt++].Value = listGroup[i].C_FolderGroup;
                    rows.Add(row1);
                }

                Form1.tblManageGroup.Invoke(new MethodInvoker(delegate
                {
                    Form1.tblManageGroup.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    Form1.tblManageGroup.ColumnHeadersVisible = false;
                    Form1.tblManageGroup.RowHeadersVisible = false;
                    Form1.tblManageGroup.Rows.AddRange(rows.ToArray());
                    Form1.tblManageGroup.ColumnHeadersVisible = true;
                    Form1.tblManageGroup.RowHeadersVisible = true;
                }));
            }
            catch
            {
                //
            }
        }
        #endregion

        public DataSet Select(string sql)
        {
            //"SELECT stt, uid from tbl_accounts"
            var ds = new DataSet();
            try
            {
                //createConection();
            }
            catch
            {
                //
            }

            try
            {
                var da = new SQLiteDataAdapter(sql, _con);
                da.Fill(ds);
                //closeConnection();
            }
            catch
            {
                //
            }

            return ds;
        }

    }
}
