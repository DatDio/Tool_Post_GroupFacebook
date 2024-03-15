using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tool_Facebook.Controller;
using Tool_Facebook.Model;

namespace Tool_Facebook.Helper
{
    public class ManageFolderHelper
    {
        public static void OnClickTabAcc(object sender, EventArgs e)
        {
            var item = (ToolStripItem)sender;
            var rows = FunctionHelper.GetRowSelected(Form1.tblManageAcc);
            var list = new List<AccountModel>();
            for (int i = 0; i < rows.Count; i++)
            {
                var account = FunctionHelper.ConvertRowToAccountModel(rows[i]);
                if (account != null)
                {
                    account.C_Folder = item.Text;
                    list.Add(account);
                    FunctionHelper.EditValueColumn(account, "C_Status", $"Đã chuyển data đến thư mục {item.Text}");
                }
            }
            Form1.sqlController.BulkUpdate(list);
        }
        public static void OnClickTabGroup(object sender, EventArgs e)
        {
            var item = (ToolStripItem)sender;
            var rows = FunctionHelper.GetRowSelected(Form1.tblManageGroup);
            var list = new List<GroupModel>();
            for (int i = 0; i < rows.Count; i++)
            {
                var group = FunctionHelper.ConvertRowToGroupModel(rows[i]);
                if (group != null)
                {
                    group.C_FolderGroup = item.Text;
                    list.Add(group);
                    FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Đã chuyển data đến thư mục {item.Text}");
                }
            }
            Form1.sqlController.BulkUpdate(list);
        }
        public static void OnClickCopyTabGroup(object sender, EventArgs e)
        {
            var item = (ToolStripItem)sender;
            var rows = FunctionHelper.GetRowSelected(Form1.tblManageGroup);
            var list = new List<GroupModel>();
            for (int i = 0; i < rows.Count; i++)
            {
                var group = FunctionHelper.ConvertRowToGroupModel(rows[i]);
                if (group != null)
                {
                    group.C_IDGroup = Guid.NewGuid().ToString();
                    group.C_PostID = "";
                    group.C_UIDVia = "";
                    group.C_CreatedPost = "";
                    group.C_TimeEditPost = "";
                    group.C_FolderGroup = item.Text;
                    list.Add(group);
                    FunctionHelper.EditValueColumn(group, "C_StatusGroup", $"Đã copy data sang thư mục {item.Text}");
                }
            }
            Form1.sqlController.BulkInsert(list);
        }
    }
}
