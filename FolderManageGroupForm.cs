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
    public partial class FolderManageGroupForm : Form
    {
        SqlController sqlController;
        public FolderManageGroupForm()
        {
            InitializeComponent();
        }
        private void FolderManageGroupForm_Load(object sender, EventArgs e)
        {
            sqlController = new SqlController();
        }
        private void btnAddFolderManageGroup_Click(object sender, EventArgs e)
        {
            sqlController.excuteSQL($"INSERT INTO tbl_folders(C_Folder,C_Type) VALUES ('{txtFolderManageGroup.Text}','Group')");
            MessageBox.Show($"Đã thêm chủ đề {txtFolderManageGroup.Text}");
            //sqlController.LoadDataIntoComboBoxManageGroup();
            Form1.cbbFolderManageGroup1.Items.Add($"{txtFolderManageGroup.Text}");
            this.Close();
        }

        private void btnCancelFolderManageGroup_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }
}
