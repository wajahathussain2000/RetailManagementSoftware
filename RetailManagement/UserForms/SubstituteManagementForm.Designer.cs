namespace RetailManagement.UserForms
{
    partial class SubstituteManagementForm
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
            this.dgvSubstitutes = new System.Windows.Forms.DataGridView();
            this.cmbSubstituteItem = new System.Windows.Forms.ComboBox();
            this.txtSubstituteReason = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblItemName = new System.Windows.Forms.Label();
            this.lblSubstituteItem = new System.Windows.Forms.Label();
            this.lblReason = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubstitutes)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSubstitutes
            // 
            this.dgvSubstitutes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSubstitutes.Location = new System.Drawing.Point(12, 60);
            this.dgvSubstitutes.Name = "dgvSubstitutes";
            this.dgvSubstitutes.Size = new System.Drawing.Size(676, 250);
            this.dgvSubstitutes.TabIndex = 0;
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblItemName.Location = new System.Drawing.Point(12, 15);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(96, 20);
            this.lblItemName.TabIndex = 1;
            this.lblItemName.Text = "Item Name:";
            // 
            // lblSubstituteItem
            // 
            this.lblSubstituteItem.AutoSize = true;
            this.lblSubstituteItem.Location = new System.Drawing.Point(12, 330);
            this.lblSubstituteItem.Name = "lblSubstituteItem";
            this.lblSubstituteItem.Size = new System.Drawing.Size(86, 13);
            this.lblSubstituteItem.TabIndex = 2;
            this.lblSubstituteItem.Text = "Substitute Item:";
            // 
            // cmbSubstituteItem
            // 
            this.cmbSubstituteItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSubstituteItem.Location = new System.Drawing.Point(104, 327);
            this.cmbSubstituteItem.Name = "cmbSubstituteItem";
            this.cmbSubstituteItem.Size = new System.Drawing.Size(200, 21);
            this.cmbSubstituteItem.TabIndex = 3;
            // 
            // lblReason
            // 
            this.lblReason.AutoSize = true;
            this.lblReason.Location = new System.Drawing.Point(320, 330);
            this.lblReason.Name = "lblReason";
            this.lblReason.Size = new System.Drawing.Size(47, 13);
            this.lblReason.TabIndex = 4;
            this.lblReason.Text = "Reason:";
            // 
            // txtSubstituteReason
            // 
            this.txtSubstituteReason.Location = new System.Drawing.Point(373, 327);
            this.txtSubstituteReason.Name = "txtSubstituteReason";
            this.txtSubstituteReason.Size = new System.Drawing.Size(200, 20);
            this.txtSubstituteReason.TabIndex = 5;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(12, 370);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 30);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(100, 370);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 30);
            this.btnRemove.TabIndex = 7;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(613, 370);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 30);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // SubstituteManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 450);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtSubstituteReason);
            this.Controls.Add(this.lblReason);
            this.Controls.Add(this.cmbSubstituteItem);
            this.Controls.Add(this.lblSubstituteItem);
            this.Controls.Add(this.lblItemName);
            this.Controls.Add(this.dgvSubstitutes);
            this.Name = "SubstituteManagementForm";
            this.Text = "Substitute Management";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubstitutes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.DataGridView dgvSubstitutes;
        private System.Windows.Forms.ComboBox cmbSubstituteItem;
        private System.Windows.Forms.TextBox txtSubstituteReason;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.Label lblSubstituteItem;
        private System.Windows.Forms.Label lblReason;

        #endregion
    }
}
