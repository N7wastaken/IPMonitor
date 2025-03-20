using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;

namespace IPMonitor
{
    public partial class ChartForm : Form
    {
        public ChartForm()
        {
            InitializeComponent();
            InitializeChart();
        }

        private void InitializeChart()
        {
            // Inicjalizacja pustego wykresu
            this.chartStats.Series.Clear();
            this.chartStats.Titles.Clear();
            this.chartStats.Titles.Add("Historia pingów");

            // Dodaj obszar wykresu, jeśli nie istnieje
            if (this.chartStats.ChartAreas.Count == 0)
            {
                this.chartStats.ChartAreas.Add(new ChartArea());
            }

            // Skonfiguruj osie
            chartStats.ChartAreas[0].AxisX.Title = "Date"; // Domyślny opis osi X
            chartStats.ChartAreas[0].AxisY.Title = "Liczba pingów";
            chartStats.ChartAreas[0].AxisX.Interval = 1;
            chartStats.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd";
            chartStats.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chartStats.ChartAreas[0].AxisY.Interval = 1; // Ustawienie interwału na 1, aby uniknąć ułamków
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOUSEMENU = 0xF090; // Kliknięcie ikony okna
            const int SC_CLOSE = 0xF060; // Zamknięcie okna (podwójne kliknięcie na ikonie)

            if (m.Msg == WM_SYSCOMMAND)
            {
                int command = m.WParam.ToInt32() & 0xFFF0;

                if (command == SC_MOUSEMENU)
                {
                    return; // Blokuje tylko menu systemowe na ikonie
                }

                if (command == SC_CLOSE && Control.MousePosition.X < this.Left + 40)
                {
                    return; // Blokuje podwójne kliknięcie na ikonie, ale nie blokuje normalnego "X"
                }
            }

            base.WndProc(ref m);
        }


        
        // Przycisk "Pokaż"
        private async void btnShow_Click(object sender, EventArgs e)
        {
            string ipAddress = textBoxSearchIP.Text.Trim();

            // Walidacja adresu IP
            if (string.IsNullOrEmpty(ipAddress))
            {
                MessageBox.Show("Wpisz adres IP!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sprawdź, czy adres IP zawiera tylko cyfry i kropki
            if (!System.Text.RegularExpressions.Regex.IsMatch(ipAddress, @"^[0-9\.]+$"))
            {
                MessageBox.Show("Adres IP może zawierać tylko cyfry i kropki!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sprawdź długość adresu IP
            if (ipAddress.Length < 7 || ipAddress.Length > 15)
            {
                MessageBox.Show("Adres IP musi mieć od 7 do 15 znaków!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Jeśli walidacja przebiegła pomyślnie, wyświetl wykres
            await DisplayChartForIP(ipAddress);
        }

        private async Task DisplayChartForIP(string ipAddress)
        {
            try
            {
                DateTime startDate = dateTimePickerStart.Value.Date; // Ustawiamy czas na początek dnia
                DateTime endDate = dateTimePickerEnd.Value.Date.AddDays(1).AddSeconds(-1); // Ustawiamy czas na koniec dnia

                // Pobierz surowe dane z bazy
                var rawData = await GetPingDataFromDatabaseAsync(ipAddress, startDate, endDate);

                // Zlicz udane i nieudane pingi
                int successfulPings = 0;
                int failedPings = 0;

                foreach (var dataPoint in rawData)
                {
                    if (dataPoint.Item2 == 1)
                    {
                        successfulPings++;
                    }
                    else
                    {
                        failedPings++;
                    }
                }

                // Wyczyść wykres
                chartStats.Series.Clear();

                // Dodaj serię dla udanych pingów
                var successfulPingsSeries = new Series("Udane pingi")
                {
                    ChartType = SeriesChartType.Column,
                    Color = Color.Green,
                    BorderColor = Color.DarkGreen,
                    BorderWidth = 2,
                    IsValueShownAsLabel = true,
                    Label = "#VALY",
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    LabelAngle = -90,
                    ["PixelPointWidth"] = "150"
                };

                // Dodaj serię dla nieudanych pingów
                var failedPingsSeries = new Series("Nieudane pingi")
                {
                    ChartType = SeriesChartType.Column,
                    Color = Color.Red,
                    BorderColor = Color.DarkRed,
                    BorderWidth = 2,
                    IsValueShownAsLabel = true,
                    Label = "#VALY",
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    LabelAngle = -90,
                    ["PixelPointWidth"] = "150"
                };

                // Dodaj dane do serii
                successfulPingsSeries.Points.AddXY("", successfulPings);
                failedPingsSeries.Points.AddXY("", failedPings);

                // Dodaj serie do wykresu
                chartStats.Series.Add(successfulPingsSeries);
                chartStats.Series.Add(failedPingsSeries);

                // Ustaw opis osi X na wybrany okres z DateTimePicker
                chartStats.ChartAreas[0].AxisX.Title = $"Okres: {startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}";

                // Wyłącz etykiety na osi X
                chartStats.ChartAreas[0].AxisX.LabelStyle.Enabled = false;

                // Wyłącz etykiety na osi Y
                chartStats.ChartAreas[0].AxisY.LabelStyle.Enabled = false;

                // Dostosuj linie siatki na osi X
                chartStats.ChartAreas[0].AxisX.MajorGrid.Interval = 2; // Linie siatki co 2 jednostki
                chartStats.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray; // Kolor linii siatki

                // Dostosuj linie siatki na osi Y
                int maxPings = Math.Max(successfulPings, failedPings); // Najwyższa wartość na osi Y
                chartStats.ChartAreas[0].AxisY.MajorGrid.Interval = maxPings / 5; // Linie siatki co 1/5 zakresu
                chartStats.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray; // Kolor linii siatki

                // Dodaj legendę
                chartStats.Legends.Clear(); // Wyczyść istniejące legendy
                var legend = new Legend
                {
                    Name = "Legend1",
                    Docking = Docking.Bottom,
                    Alignment = StringAlignment.Center,
                    Title = "Legenda",
                    TitleAlignment = StringAlignment.Center
                };
                chartStats.Legends.Add(legend);

                // Dodaj opisy do legendy
                successfulPingsSeries.LegendText = "Udane pingi (zielone)";
                failedPingsSeries.LegendText = "Nieudane pingi (czerwone)";

                // Skonfiguruj osie
                chartStats.ChartAreas[0].AxisX.Interval = 1;
                chartStats.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                chartStats.ChartAreas[0].RecalculateAxesScale();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas pobierania danych: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Pobierz surowe dane z bazy
        private async Task<List<(DateTime, int)>> GetPingDataFromDatabaseAsync(string ipAddress, DateTime startDate, DateTime endDate)
        {
            var rawData = new List<(DateTime, int)>();

            try
            {
                using (var connection = await MySqlHelper.GetConnectionAsync())
                {
                    string query = @"
                        SELECT date, ping_status
                        FROM statystyki
                        WHERE id_ip_adress = (SELECT id FROM ip_adress WHERE ip_adress = @ipAddress)
                        AND date BETWEEN @startDate AND @endDate
                        ORDER BY date;";

                    Console.WriteLine($"Wykonuję zapytanie: {query}");
                    Console.WriteLine($"Parametry: IP={ipAddress}, StartDate={startDate}, EndDate={endDate}");

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ipAddress", ipAddress);
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                DateTime date = reader.GetDateTime(reader.GetOrdinal("date"));
                                int pingStatus = reader.GetInt32(reader.GetOrdinal("ping_status"));
                                rawData.Add((date, pingStatus));
                                Console.WriteLine($"Dodano punkt: {date}, {pingStatus}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas pobierania danych z bazy: {ex.Message}");
                throw;
            }

            return rawData;
        }

        private void dateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {
            // Logika zmiany daty początkowej
        }

        private void dateTimePickerEnd_ValueChanged(object sender, EventArgs e)
        {
            // Logika zmiany daty końcowej
        }
    }
}