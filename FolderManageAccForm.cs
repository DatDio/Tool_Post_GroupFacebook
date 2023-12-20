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
        SqlController sqlController;
        public FolderManageAccForm()
        {
            InitializeComponent();
        }
        private void FolderManageAccForm_Load(object sender, EventArgs e)
        {
            sqlController = new SqlController();
        }
        private void btnAddFolderManageAcc_Click(object sender, EventArgs e)
        {
            sqlController.excuteSQL($"INSERT INTO tbl_folders(C_Folder,C_Type) VALUES ('{txtFolderManageAcc.Text}','Account')");
            MessageBox.Show($"Đã thêm chủ đề {txtFolderManageAcc.Text}");
            //sqlController.LoadDataIntoComboBoxManageAcc();
            Form1.cbbFolderManageAcc1.Items.Add($"{txtFolderManageAcc.Text}");
            this.Close();
        }

        private void btnCancelFolderManageAcc_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
