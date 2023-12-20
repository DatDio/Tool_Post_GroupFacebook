namespace Tool_Facebook
{
    partial class FolderManageGroupForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderManageGroupForm));
            this.txtFolderManageGroup = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancelFolderManageGroup = new System.Windows.Forms.Button();
            this.btnAddFolderManageGroup = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtFolderManageGroup
            // 
            this.txtFolderManageGroup.Location = new System.Drawing.Point(178, 50);
            this.txtFolderManageGroup.Name = "txtFolderManageGroup";
            this.txtFolderManageGroup.Size = new System.Drawing.Size(168, 20);
            this.txtFolderManageGroup.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(50, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 16);
            this.label3.TabIndex = 15;
            this.label3.Text = "Tên Thư Mục:";
            // 
            // btnCancelFolderManageGroup
            // 
            this.btnCancelFolderManageGroup.AutoEllipsis = true;
            this.btnCancelFolderManageGroup.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnCancelFolderManageGroup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancelFolderManageGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelFolderManageGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelFolderManageGroup.ForeColor = System.Drawing.Color.Black;
            this.btnCancelFolderManageGroup.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelFolderManageGroup.Image")));
            this.btnCancelFolderManageGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelFolderManageGroup.Location = new System.Drawing.Point(264, 142);
            this.btnCancelFolderManageGroup.Name = "btnCancelFolderManageGroup";
            this.btnCancelFolderManageGroup.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnCancelFolderManageGroup.Size = new System.Drawing.Size(98, 49);
            this.btnCancelFolderManageGroup.TabIndex = 14;
            this.btnCancelFolderManageGroup.Text = "Hủy";
            this.btnCancelFolderManageGroup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelFolderManageGroup.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelFolderManageGroup.UseVisualStyleBackColor = false;
            this.btnCancelFolderManageGroup.Click += new System.EventHandler(this.btnCancelFolderManageGroup_Click);
            // 
            // btnAddFolderManageGroup
            // 
            this.btnAddFolderManageGroup.AutoEllipsis = true;
            this.btnAddFolderManageGroup.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnAddFolderManageGroup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnAddFolderManageGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddFolderManageGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddFolderManageGroup.ForeColor = System.Drawing.Color.Black;
            this.btnAddFolderManageGroup.Image = ((System.Drawing.Image)(resources.GetObject("btnAddFolderManageGroup.Image")));
            this.btnAddFolderManageGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddFolderManageGroup.Location = new System.Drawing.Point(41, 142);
            this.btnAddFolderManageGroup.Name = "btnAddFolderManageGroup";
            this.btnAddFolderManageGroup.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnAddFolderManageGroup.Size = new System.Drawing.Size(120, 49);
            this.btnAddFolderManageGroup.TabIndex = 13;
            this.btnAddFolderManageGroup.Text = "Thêm";
            this.btnAddFolderManageGroup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddFolderManageGroup.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddFolderManageGroup.UseVisualStyleBackColor = false;
            this.btnAddFolderManageGroup.Click += new System.EventHandler(this.btnAddFolderManageGroup_Click);
            // 
            // FolderManageGroupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(424, 269);
            this.Controls.Add(this.txtFolderManageGroup);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancelFolderManageGroup);
            this.Controls.Add(this.btnAddFolderManageGroup);
            this.Name = "FolderManageGroupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FolderManageGroupForm";
            this.Load += new System.EventHandler(this.FolderManageGroupForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFolderManageGroup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancelFolderManageGroup;
        private System.Windows.Forms.Button btnAddFolderManageGroup;
    }
}