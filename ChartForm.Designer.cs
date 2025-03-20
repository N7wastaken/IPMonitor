namespace IPMonitor
{
    partial class ChartForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox textBoxSearchIP;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.Label labelStartDate;
        private System.Windows.Forms.Label labelEndDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartStats;

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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartStats = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.textBoxSearchIP = new System.Windows.Forms.TextBox();
            this.btnShow = new System.Windows.Forms.Button();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.labelStartDate = new System.Windows.Forms.Label();
            this.labelEndDate = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartStats)).BeginInit();
            this.SuspendLayout();

            // chartStats
            chartArea1.Name = "ChartArea1";
            this.chartStats.ChartAreas.Add(chartArea1);
            this.chartStats.Location = new System.Drawing.Point(12, 100);
            this.chartStats.Name = "chartStats";
            series1.ChartArea = "ChartArea1";
            series1.Name = "Udane pingi";
            series2.ChartArea = "ChartArea1";
            series2.Name = "Nieudane pingi";
            this.chartStats.Series.Add(series1);
            this.chartStats.Series.Add(series2);
            this.chartStats.Size = new System.Drawing.Size(410, 300);
            this.chartStats.TabIndex = 0;
            this.chartStats.Text = "chartStats";

            // textBoxSearchIP
            this.textBoxSearchIP.Location = new System.Drawing.Point(12, 12);
            this.textBoxSearchIP.Name = "textBoxSearchIP";
            this.textBoxSearchIP.Size = new System.Drawing.Size(200, 20);
            this.textBoxSearchIP.TabIndex = 1;
            this.textBoxSearchIP.PlaceholderText = "Wpisz adres IP";

            // btnShow
            this.btnShow.Location = new System.Drawing.Point(220, 10);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(75, 23);
            this.btnShow.TabIndex = 2;
            this.btnShow.Text = "Pokaż";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);

            // dateTimePickerStart
            this.dateTimePickerStart.Location = new System.Drawing.Point(12, 50);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerStart.TabIndex = 3;
            this.dateTimePickerStart.Value = DateTime.Now.AddDays(-7); // Domyślnie ustaw na ostatni tydzień
            this.dateTimePickerStart.ValueChanged += new System.EventHandler(this.dateTimePickerStart_ValueChanged);

            // dateTimePickerEnd
            this.dateTimePickerEnd.Location = new System.Drawing.Point(220, 50);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerEnd.TabIndex = 4;
            this.dateTimePickerEnd.Value = DateTime.Now; // Domyślnie ustaw na dzisiaj
            this.dateTimePickerEnd.ValueChanged += new System.EventHandler(this.dateTimePickerEnd_ValueChanged);

            // labelStartDate
            this.labelStartDate.AutoSize = true;
            this.labelStartDate.Location = new System.Drawing.Point(12, 34);
            this.labelStartDate.Name = "labelStartDate";
            this.labelStartDate.Size = new System.Drawing.Size(72, 13);
            this.labelStartDate.TabIndex = 5;
            this.labelStartDate.Text = "Data początkowa";

            // labelEndDate
            this.labelEndDate.AutoSize = true;
            this.labelEndDate.Location = new System.Drawing.Point(220, 34);
            this.labelEndDate.Name = "labelEndDate";
            this.labelEndDate.Size = new System.Drawing.Size(69, 13);
            this.labelEndDate.TabIndex = 6;
            this.labelEndDate.Text = "Data końcowa";

            // ChartForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 415);
            this.Controls.Add(this.labelEndDate);
            this.Controls.Add(this.labelStartDate);
            this.Controls.Add(this.dateTimePickerEnd);
            this.Controls.Add(this.dateTimePickerStart);
            this.Controls.Add(this.btnShow);
            this.Controls.Add(this.textBoxSearchIP);
            this.Controls.Add(this.chartStats);
            this.Icon = new System.Drawing.Icon("IPM_icon.ico");
            this.Name = "ChartForm";
            this.Text = "Statystyki pingów";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen; // Ustawienie na środku ekranu
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle; // Blokowanie skalowania
            this.MaximizeBox = false; // Wyłączenie przycisku maksymalizacji
            ((System.ComponentModel.ISupportInitialize)(this.chartStats)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}