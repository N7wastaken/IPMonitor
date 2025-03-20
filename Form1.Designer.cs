using System.ComponentModel;

namespace IPMonitor
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnLoadList;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStats;
        private System.Windows.Forms.ListBox listBoxIPs;
        private System.Windows.Forms.TextBox textBoxLogs;
        private System.Windows.Forms.Timer timerPing;
        private System.Windows.Forms.Label labelInactiveIPs;
        private System.Windows.Forms.ComboBox comboBoxInactivePeriod;
        private System.Windows.Forms.Button btnCheckInactive;

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
            this.btnLoadList = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStats = new System.Windows.Forms.Button();
            this.listBoxIPs = new System.Windows.Forms.ListBox();
            this.textBoxLogs = new System.Windows.Forms.TextBox();
            this.timerPing = new System.Windows.Forms.Timer(this.components);

            this.SuspendLayout();

            // listBoxIPs
            this.listBoxIPs.FormattingEnabled = true;
            this.listBoxIPs.Location = new System.Drawing.Point(12, 50);
            this.listBoxIPs.Name = "listBoxIPs";
            this.listBoxIPs.Size = new System.Drawing.Size(120, 200);
            this.listBoxIPs.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxIPs_MouseDoubleClick);

            // btnLoadList
            this.btnLoadList.Location = new System.Drawing.Point(140, 10);
            this.btnLoadList.Name = "btnLoadList";
            this.btnLoadList.Size = new System.Drawing.Size(160, 30);
            this.btnLoadList.Text = "Załaduj adresy IP";
            this.btnLoadList.Click += new System.EventHandler(this.btnLoadList_Click);

            // btnStart
            this.btnStart.Location = new System.Drawing.Point(140, 48);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 30);
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

            // btnStop
            this.btnStop.Location = new System.Drawing.Point(224, 48);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 30);
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);

            // btnStats
            this.btnStats.Location = new System.Drawing.Point(140, 86);
            this.btnStats.Name = "btnStats";
            this.btnStats.Size = new System.Drawing.Size(160, 30);
            this.btnStats.Text = "Statystyki";
            this.btnStats.Click += new System.EventHandler(this.btnStats_Click);

            // labelInactiveIPs
            this.labelInactiveIPs = new System.Windows.Forms.Label();
            this.labelInactiveIPs.Location = new System.Drawing.Point(140, 121);
            this.labelInactiveIPs.Name = "labelInactiveIPs";
            this.labelInactiveIPs.Size = new System.Drawing.Size(160, 15);
            this.labelInactiveIPs.Text = "Nieaktywne adresy:";

            // comboBoxInactivePeriod
            this.comboBoxInactivePeriod = new System.Windows.Forms.ComboBox();
            this.comboBoxInactivePeriod.Location = new System.Drawing.Point(141, 140);
            this.comboBoxInactivePeriod.Name = "comboBoxInactivePeriod";
            this.comboBoxInactivePeriod.Size = new System.Drawing.Size(159, 20);
            this.comboBoxInactivePeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInactivePeriod.Items.AddRange(new object[] {
                   "1 tydzień",
                   "2 tygodnie",
                   "3 tygodnie",
                   "1 miesiąc",
                   "2 miesiące",
                   "kwartał"
                              });
            this.comboBoxInactivePeriod.SelectedIndex = 0;

            // btnCheckInactive
            this.btnCheckInactive = new System.Windows.Forms.Button();
            this.btnCheckInactive.Location = new System.Drawing.Point(220, 175);
            this.btnCheckInactive.Name = "btnCheckInactive";
            this.btnCheckInactive.Size = new System.Drawing.Size(80, 25);
            this.btnCheckInactive.Text = "Sprawdź";
            this.btnCheckInactive.Click += new System.EventHandler(this.btnCheckInactive_Click);

            // listBoxIPs
            this.listBoxIPs.FormattingEnabled = true;
            this.listBoxIPs.Location = new System.Drawing.Point(12, 11);
            this.listBoxIPs.Name = "listBoxIPs";
            this.listBoxIPs.Size = new System.Drawing.Size(120, 200);

            // textBoxLogs
            this.textBoxLogs.Location = new System.Drawing.Point(12, 222);
            this.textBoxLogs.Multiline = true;
            this.textBoxLogs.Name = "textBoxLogs";
            this.textBoxLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLogs.Size = new System.Drawing.Size(285, 150);
            this.textBoxLogs.ReadOnly = true;
            this.textBoxLogs.HideSelection = true;

            // timerPing
            this.timerPing.Interval = 14400000; // 4 godziny

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 385);
            this.Controls.Add(this.textBoxLogs);
            this.Controls.Add(this.listBoxIPs);
            this.Controls.Add(this.btnStats);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnLoadList);
            this.Controls.Add(this.labelInactiveIPs);
            this.Controls.Add(this.comboBoxInactivePeriod);
            this.Controls.Add(this.btnCheckInactive);
            this.Icon = new System.Drawing.Icon("IPM_icon.ico");
            this.Name = "Form1";
            this.Text = "IP Monitor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}