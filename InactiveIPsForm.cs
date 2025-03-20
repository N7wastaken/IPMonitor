using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace IPMonitor
{
    public partial class InactiveIPsForm : Form
    {
        private string _selectedPeriod;

        public InactiveIPsForm(string selectedPeriod)
        {
            InitializeComponent();
            _selectedPeriod = selectedPeriod;
            LoadInactiveIPs();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOUSEMENU = 0xF090; // Kliknięcie ikony okna
            const int SC_CLOSE = 0xF060; // Zamknięcie okna (podwójne kliknięcie na ikonie)

            if (m.Msg == WM_SYSCOMMAND)
            {
                int command = m.WParam.ToInt32() & 0xFFF0;

                if (command == SC_MOUSEMENU || (command == SC_CLOSE && Control.MousePosition.X < this.Left + 40))
                {
                    return; // Blokuje menu systemowe i podwójne kliknięcie na ikonie
                }
            }

            base.WndProc(ref m);
        }

        private async void LoadInactiveIPs()
        {
            try
            {
                listBoxInactiveIPs.Items.Clear();

                // Zmień pierwsze wpisy ping_status na NULL
                await UpdateFirstPingStatusToNullAsync();

                // Pobierz nieaktywne adresy IP z bazy danych
                var inactiveIPs = await GetInactiveIPsAsync(_selectedPeriod);

                // Dodaj adresy IP do ListBox
                foreach (var ip in inactiveIPs)
                {
                    listBoxInactiveIPs.Items.Add(ip);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania nieaktywnych adresów IP: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task UpdateFirstPingStatusToNullAsync()
        {
            try
            {
                using (var connection = await MySqlHelper.GetConnectionAsync())
                {
                    // Zapytanie SQL do zmiany pierwszego wpisu ping_status na NULL dla każdego adresu IP
                    string query = @"
                        UPDATE statystyki 
                        SET ping_status = 2 
                        WHERE id IN (
                            SELECT id 
                            FROM (
                                SELECT MIN(id) as id 
                                FROM statystyki 
                                GROUP BY id_ip_adress
                            ) as first_entries
                        )";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas aktualizacji ping_status: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<List<string>> GetInactiveIPsAsync(string period)
        {
            var inactiveIPs = new List<string>();

            try
            {
                using (var connection = await MySqlHelper.GetConnectionAsync())
                {
                    int days = GetDaysFromPeriod(period);

                    // Zapytanie SQL do pobrania nieaktywnych adresów IP
                    string query = @"
                        SELECT ip_adress 
                        FROM ip_adress 
                        WHERE id IN (
                            SELECT id_ip_adress 
                            FROM statystyki 
                            WHERE date >= DATE_SUB(NOW(), INTERVAL @period DAY)
                            GROUP BY id_ip_adress 
                            HAVING SUM(ping_status) = 0
                        )
                        AND status IS NULL";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@period", days);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string ip = reader.GetString(0);
                                inactiveIPs.Add(ip);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas pobierania nieaktywnych adresów IP: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return inactiveIPs;
        }

        private int GetDaysFromPeriod(string period)
        {
            switch (period)
            {
                case "1 tydzień":
                    return 7;
                case "2 tygodnie":
                    return 14;
                case "3 tygodnie":
                    return 21;
                case "1 miesiąc":
                    return 30;
                case "2 miesiące":
                    return 60;
                case "kwartał":
                    return 90;
                default:
                    return 7; // Domyślnie 1 tydzień
            }
        }

        private async void buttonChangeStatus_Click(object sender, EventArgs e)
        {
            if (listBoxInactiveIPs.SelectedItem == null)
            {
                MessageBox.Show("Proszę wybrać adres IP z listy.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedIP = listBoxInactiveIPs.SelectedItem?.ToString() ?? string.Empty;

            try
            {
                using (var connection = await MySqlHelper.GetConnectionAsync())
                {
                    string query = "UPDATE ip_adress SET status = 1 WHERE ip_adress = @ip";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ip", selectedIP);
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Status adresu IP {selectedIP} został zmieniony na aktywny.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (selectedIP != null)
                            {
                                listBoxInactiveIPs.Items.Remove(selectedIP); // Usuń adres IP z listy
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Nie udało się zmienić statusu adresu IP {selectedIP}.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zmiany statusu adresu IP: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}