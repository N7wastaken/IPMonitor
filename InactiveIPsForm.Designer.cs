namespace IPMonitor
{
    partial class InactiveIPsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListBox listBoxInactiveIPs;
        private System.Windows.Forms.CheckBox checkBoxShowNonArp;
        private System.Windows.Forms.Button buttonChangeStatus;
        private System.Windows.Forms.ToolTip toolTip;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listBoxInactiveIPs = new System.Windows.Forms.ListBox();
            this.checkBoxShowNonArp = new System.Windows.Forms.CheckBox();
            this.buttonChangeStatus = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();

            // listBoxInactiveIPs
            this.listBoxInactiveIPs.FormattingEnabled = true;
            this.listBoxInactiveIPs.Location = new System.Drawing.Point(12, 40);
            this.listBoxInactiveIPs.Name = "listBoxInactiveIPs";
            this.listBoxInactiveIPs.Size = new System.Drawing.Size(260, 199);
            this.listBoxInactiveIPs.TabIndex = 0;

            // checkBoxShowNonArp
            this.checkBoxShowNonArp.AutoSize = true;
            this.checkBoxShowNonArp.Location = new System.Drawing.Point(12, 12);
            this.checkBoxShowNonArp.Name = "checkBoxShowNonArp";
            this.checkBoxShowNonArp.Size = new System.Drawing.Size(160, 17);
            this.checkBoxShowNonArp.TabIndex = 1;
            this.checkBoxShowNonArp.Text = "Wyświetl adresy nie będące w ARP (niedostępne)";
            this.checkBoxShowNonArp.UseVisualStyleBackColor = true;

            // buttonChangeStatus
            this.buttonChangeStatus.Location = new System.Drawing.Point(12, 245);
            this.buttonChangeStatus.Name = "buttonChangeStatus";
            this.buttonChangeStatus.Size = new System.Drawing.Size(260, 23);
            this.buttonChangeStatus.TabIndex = 2;
            this.buttonChangeStatus.Text = "Zmień status";
            this.toolTip.SetToolTip(this.buttonChangeStatus, "Manualna zmiana statusu w bazie na aktywny");
            this.buttonChangeStatus.UseVisualStyleBackColor = true;
            this.buttonChangeStatus.Click += new System.EventHandler(this.buttonChangeStatus_Click);

            // InactiveIPsForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 280);
            this.Controls.Add(this.buttonChangeStatus);
            this.Controls.Add(this.checkBoxShowNonArp);
            this.Controls.Add(this.listBoxInactiveIPs);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Icon = new System.Drawing.Icon("IPM_icon.ico");
            this.Name = "InactiveIPsForm";
            this.Text = "Nieaktywne adresy IP";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}