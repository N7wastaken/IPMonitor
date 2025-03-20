using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Runtime.InteropServices;

namespace IPMonitor
{
    public partial class Form1 : Form
    {
        private const int EM_HIDESELECTION = 0x0103;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private CancellationTokenSource? _cancellationTokenSource;

        public Form1()
        {
            InitializeComponent();

            // Ustawienia textBoxLogs
            textBoxLogs.ReadOnly = true; // Zablokuj edycję
            textBoxLogs.ScrollBars = ScrollBars.Vertical; // Włącz przewijanie (dla TextBox)
            textBoxLogs.BackColor = System.Drawing.SystemColors.Window; // Zachowaj kolor tła
            textBoxLogs.Enter += new System.EventHandler(this.textBoxLogs_Enter); // Zapobiegaj pojawianiu się kursora
            this.Shown += new EventHandler(Form1_Shown);

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IPM_icon.ico");
            if (File.Exists(path))
            {
                this.Icon = new Icon(path);
            }
            else
            {
                MessageBox.Show("Nie znaleziono pliku ikony: " + path);
            }

        }
        private void Form1_Shown(object? sender, EventArgs e)
        {
            btnLoadList.Focus();  // Ustaw focus po pokazaniu formularza
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

        private void textBoxLogs_Enter(object? sender, EventArgs e)
        {
            HideCaret(textBoxLogs.Handle);
        }

        private void HideCaret(IntPtr handle)
        {
            SendMessage(handle, EM_HIDESELECTION, (IntPtr)1, IntPtr.Zero);
        }

        // Przycisk "Załaduj zastrzeżenia"
        private void btnLoadList_Click(object sender, EventArgs e)
        {
            using (var dhcpForm = new DhcpForm())
            {
                if (dhcpForm.ShowDialog() == DialogResult.OK)
                {
                    string dhcpServerIp = dhcpForm.DhcpServerIp;
                    SaveDhcpServerIp(dhcpServerIp); // Zapisz adres DHCP do pliku
                    LoadReservedIps(dhcpServerIp);
                }
            }
        }

        private void SaveDhcpServerIp(string dhcpServerIp)
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dhcp_config.txt");
            File.WriteAllText(configFilePath, dhcpServerIp);
        }

        private string? LoadDhcpServerIp()
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dhcp_config.txt");
            if (File.Exists(configFilePath))
            {
                return File.ReadAllText(configFilePath);
            }
            return null;
        }

        private void listBoxIPs_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Sprawdź, czy kliknięto na element w ListBox
            int index = listBoxIPs.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                // Pobierz zaznaczony adres IP
                string? selectedIp = listBoxIPs.Items[index]?.ToString();

                if (selectedIp != null)
                {
                    // Skopiuj adres IP do schowka
                    Clipboard.SetText(selectedIp);

                    // Opcjonalnie: Wyświetl komunikat o skopiowaniu
                    MessageBox.Show($"Skopiowano adres IP: {selectedIp}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async void LoadReservedIps(string dhcpServerIp)
        {
            try
            {
                if (listBoxIPs == null)
                {
                    throw new InvalidOperationException("listBoxIPs nie został zainicjalizowany.");
                }

                listBoxIPs.Items.Clear();

                // Uruchom PowerShell, aby zapisać adresy IP do pliku
                string outputFile = await Task.Run(() => RunPowerShellScript(dhcpServerIp));

                // Parsuj plik z adresami IP
                var reservedIps = ParseReservedIpsFromFile(outputFile);

                // Dodaj zastrzeżone adresy IP do listy
                foreach (var ip in reservedIps)
                {
                    listBoxIPs.Items.Add(ip);
                }

                MessageBox.Show("Zastrzeżone adresy IP zostały załadowane.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania adresów IP: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                listBoxIPs.Items.Clear(); // Wyczyść listę IP
            }
        }

        private string RunPowerShellScript(string dhcpServerIp)
        {
            string outputFile = Path.Combine(Path.GetTempPath(), "dhcp_reservations.txt");

            // Sprawdź, czy folder istnieje, a jeśli nie, utwórz go
            string? directory = Path.GetDirectoryName(outputFile);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"$DhcpServer = '{dhcpServerIp}'; " +
                            $"$OutputFile = '{outputFile}'; " +
                            "$directory = Split-Path -Path $OutputFile -Parent; " +
                            "if (-not (Test-Path -Path $directory)) { New-Item -ItemType Directory -Path $directory; } " +
                            "try { " +
                            "$Scopes = Get-DhcpServerv4Scope -ComputerName $DhcpServer -ErrorAction Stop; " +
                            "$Reservations = foreach ($Scope in $Scopes) { " +
                            "Get-DhcpServerv4Reservation -ComputerName $DhcpServer -ScopeId $Scope.ScopeId -ErrorAction Stop " +
                            "}; " +
                            "$Reservations | Select-Object -ExpandProperty IPAddress | Out-File -FilePath $OutputFile -ErrorAction Stop; " +
                            "Write-Output 'Plik został pomyślnie utworzony: $OutputFile'; " +
                            "} catch { " +
                            "Write-Error 'Błąd podczas wykonywania skryptu: $_'; " +
                            "throw; " +
                            "}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Verb = "runas" // Uruchom jako administrator
            };

            try
            {
                using (var process = Process.Start(processStartInfo))
                {
                    if (process == null)
                    {
                        throw new Exception("Nie udało się uruchomić procesu PowerShell.");
                    }

                    if (!process.WaitForExit(45000)) // 45 sekund
                    {
                        process.Kill();
                        throw new TimeoutException("Polecenie PowerShell przekroczyło limit czasu.");
                    }

                    using (var reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.WriteLine("PowerShell Output:");
                        Console.WriteLine(result);

                        if (process.ExitCode != 0)
                        {
                            using (var errorReader = process.StandardError)
                            {
                                string error = errorReader.ReadToEnd();
                                Console.WriteLine("PowerShell Error:");
                                Console.WriteLine(error);
                                throw new Exception($"Błąd podczas wykonywania polecenia PowerShell: {error}");
                            }
                        }

                        return outputFile;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas uruchamiania skryptu PowerShell: {ex.Message}");
            }
        }

        private string[] ParseReservedIpsFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Plik {filePath} nie istnieje.");
                }

                // Wczytaj zawartość pliku
                var fileContent = File.ReadAllText(filePath);

                // Użyj wyrażenia regularnego, aby wyodrębnić adresy IP
                var regex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                var matches = regex.Matches(fileContent);

                var reservedIps = new System.Collections.Generic.List<string>();
                foreach (Match match in matches)
                {
                    reservedIps.Add(match.Value);
                }

                return reservedIps.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas parsowania pliku: {ex.Message}");
            }
        }

        // Przycisk "Start"
        private async void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBoxIPs?.Items.Count == 0)
                {
                    MessageBox.Show("Lista adresów IP jest pusta.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                btnStart.Enabled = false;
                btnStop.Enabled = true;

                _cancellationTokenSource = new CancellationTokenSource(); // Utwórz nowy CancellationTokenSource
                var cancellationToken = _cancellationTokenSource.Token; // Pobierz token

                timerPing.Tick += async (s, ev) => await PingIPsAsync(cancellationToken);
                timerPing.Start();

                await PingIPsAsync(cancellationToken); // Uruchom pierwsze pingowanie z tokenem
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd w btnStart_Click: {ex.Message}\n\nSzczegóły:\n{ex.StackTrace}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Przycisk "Stop"
        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                timerPing.Stop();
                btnStart.Enabled = true;
                btnStop.Enabled = false;

                // Anuluj operację pingowania, jeśli CancellationTokenSource istnieje
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose(); // Zwolnij zasób
                    _cancellationTokenSource = null; // Ustaw na null, aby uniknąć ponownego użycia
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd w btnStop_Click: {ex.Message}\n\nSzczegóły:\n{ex.StackTrace}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Przycisk "Statystyki"
        private void btnStats_Click(object sender, EventArgs e)
        {
            try
            {
                var chartForm = new ChartForm();
                chartForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas generowania wykresu: {ex.Message}\n\nSzczegóły:\n{ex.StackTrace}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCheckInactive_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedPeriod = comboBoxInactivePeriod.SelectedItem?.ToString() ?? string.Empty;
                var inactiveIPsForm = new InactiveIPsForm(selectedPeriod);
                inactiveIPsForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas otwierania okna nieaktywnych adresów IP: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task PingIPsAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (listBoxIPs == null)
                {
                    MessageBox.Show("listBoxIPs jest null.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (var ip in listBoxIPs.Items)
                {
                    // Sprawdź, czy operacja została anulowana
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return; // Przerwij pętlę, jeśli operacja została anulowana
                    }

                    string? ipAddress = ip?.ToString();
                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        bool pingStatus = await PingAsync(ipAddress);

                        // Dodaj nową wiadomość z poprawnym przejściem do nowej linii
                        textBoxLogs.AppendText($"{DateTime.Now}: {ipAddress} - {(pingStatus ? "Success" : "Failed")}{Environment.NewLine}");

                        // Ukryj kursor
                        HideCaret(textBoxLogs.Handle);

                        await SavePingResultAsync(ipAddress, pingStatus);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd w PingIPsAsync: {ex.Message}\n\nSzczegóły:\n{ex.StackTrace}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<bool> PingAsync(string ip)
        {
            try
            {
                using (var ping = new System.Net.NetworkInformation.Ping())
                {
                    var reply = await ping.SendPingAsync(ip, 1000); // Timeout 1 sekunda
                    return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        private async Task SavePingResultAsync(string ip, bool pingStatus)
        {
            try
            {
                using (var connection = await MySqlHelper.GetConnectionAsync())
                {
                    // Sprawdź, czy adres IP już istnieje w tabeli ip_adress
                    var checkIpQuery = "SELECT id FROM ip_adress WHERE ip_adress = @ip";
                    using (var checkIpCommand = new MySqlCommand(checkIpQuery, connection))
                    {
                        checkIpCommand.Parameters.AddWithValue("@ip", ip);
                        var ipId = await checkIpCommand.ExecuteScalarAsync();

                        // Jeśli adres IP nie istnieje, dodaj go do tabeli ip_adress
                        if (ipId == null)
                        {
                            var insertIpQuery = "INSERT INTO ip_adress (ip_adress) VALUES (@ip)";
                            using (var insertIpCommand = new MySqlCommand(insertIpQuery, connection))
                            {
                                insertIpCommand.Parameters.AddWithValue("@ip", ip);
                                await insertIpCommand.ExecuteNonQueryAsync();
                                ipId = await checkIpCommand.ExecuteScalarAsync(); // Pobierz nowo dodane ID
                            }
                        }

                        // Zapisz wynik pingowania do tabeli statystyki
                        var insertStatsQuery = "INSERT INTO statystyki (ping_status, date, id_ip_adress) VALUES (@pingStatus, @date, @ipId)";
                        using (var insertStatsCommand = new MySqlCommand(insertStatsQuery, connection))
                        {
                            insertStatsCommand.Parameters.AddWithValue("@pingStatus", pingStatus ? 1 : 0);
                            insertStatsCommand.Parameters.AddWithValue("@date", DateTime.Now);
                            insertStatsCommand.Parameters.AddWithValue("@ipId", ipId);
                            await insertStatsCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania wyniku ping: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}