﻿namespace Tool_Facebook
{
    partial class FolderManageAccForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderManageAccForm));
            this.btnAddFolderManageAcc = new System.Windows.Forms.Button();
            this.btnCancelFolderManageAcc = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFolderManageAcc = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnAddFolderManageAcc
            // 
            this.btnAddFolderManageAcc.AutoEllipsis = true;
            this.btnAddFolderManageAcc.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnAddFolderManageAcc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnAddFolderManageAcc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddFolderManageAcc.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddFolderManageAcc.ForeColor = System.Drawing.Color.Black;
            this.btnAddFolderManageAcc.Image = ((System.Drawing.Image)(resources.GetObject("btnAddFolderManageAcc.Image")));
            this.btnAddFolderManageAcc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddFolderManageAcc.Location = new System.Drawing.Point(36, 140);
            this.btnAddFolderManageAcc.Name = "btnAddFolderManageAcc";
            this.btnAddFolderManageAcc.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnAddFolderManageAcc.Size = new System.Drawing.Size(120, 49);
            this.btnAddFolderManageAcc.TabIndex = 5;
            this.btnAddFolderManageAcc.Text = "Thêm";
            this.btnAddFolderManageAcc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddFolderManageAcc.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddFolderManageAcc.UseVisualStyleBackColor = false;
            this.btnAddFolderManageAcc.Click += new System.EventHandler(this.btnAddFolderManageAcc_Click);
            // 
            // btnCancelFolderManageAcc
            // 
            this.btnCancelFolderManageAcc.AutoEllipsis = true;
            this.btnCancelFolderManageAcc.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnCancelFolderManageAcc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnCancelFolderManageAcc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelFolderManageAcc.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelFolderManageAcc.ForeColor = System.Drawing.Color.Black;
            this.btnCancelFolderManageAcc.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelFolderManageAcc.Image")));
            this.btnCancelFolderManageAcc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelFolderManageAcc.Location = new System.Drawing.Point(231, 140);
            this.btnCancelFolderManageAcc.Name = "btnCancelFolderManageAcc";
            this.btnCancelFolderManageAcc.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnCancelFolderManageAcc.Size = new System.Drawing.Size(98, 49);
            this.btnCancelFolderManageAcc.TabIndex = 6;
            this.btnCancelFolderManageAcc.Text = "Hủy";
            this.btnCancelFolderManageAcc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelFolderManageAcc.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelFolderManageAcc.UseVisualStyleBackColor = false;
            this.btnCancelFolderManageAcc.Click += new System.EventHandler(this.btnCancelFolderManageAcc_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(33, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "Tên Thư Mục:";
            // 
            // txtFolderManageAcc
            // 
            this.txtFolderManageAcc.Location = new System.Drawing.Point(161, 61);
            this.txtFolderManageAcc.Name = "txtFolderManageAcc";
            this.txtFolderManageAcc.Size = new System.Drawing.Size(168, 20);
            this.txtFolderManageAcc.TabIndex = 12;
            // 
            // FolderManageAccForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(417, 255);
            this.Controls.Add(this.txtFolderManageAcc);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancelFolderManageAcc);
            this.Controls.Add(this.btnAddFolderManageAcc);
            this.Name = "FolderManageAccForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FolderManageAccForm";
            this.Load += new System.EventHandler(this.FolderManageAccForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAddFolderManageAcc;
        private System.Windows.Forms.Button btnCancelFolderManageAcc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFolderManageAcc;
    }
}