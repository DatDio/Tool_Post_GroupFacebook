using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tool_Facebook.Controller;

namespace Tool_Facebook
{
    public partial class FolderManageAccForm : Form
    {
        string type;
        SqlController sqlController;
        public FolderManageAccForm(string _type)
        {
            InitializeComponent();
            this.type = _type;
        }
        private void FolderManageAccForm_Load(object sender, EventArgs e)
        {
            sqlController = new SqlController();
        }
        private void btnAddFolderManageAcc_Click(object sender, EventArgs e)
        {
            if(type == "Account")
            {
                sqlController.excuteSQL($"INSERT INTO tbl_folders(C_Folder,C_Type) VALUES ('{txtFolderManageAcc.Text}','Account')");
                MessageBox.Show($"Đã thêm chủ đề {txtFolderManageAcc.Text}");
                Form1.cbbFolderManageAcc1.Items.Add($"{txtFolderManageAcc.Text}");
            }
            if (type == "Group")
            {
                sqlController.excuteSQL($"INSERT INTO tbl_folders(C_Folder,C_Type) VALUES ('{txtFolderManageAcc.Text}','Group')");
                MessageBox.Show($"Đã thêm chủ đề {txtFolderManageAcc.Text}");
                Form1.cbbFolderManageGroup1.Items.Add($"{txtFolderManageAcc.Text}");
            }
          
            this.Close();
        }

        private void btnCancelFolderManageAcc_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
