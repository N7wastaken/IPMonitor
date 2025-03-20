using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

namespace IPMonitor
{
    public partial class DhcpForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DhcpServerIp { get; private set; } = string.Empty; // Inicjalizacja pustym ciągiem

        private ComboBox comboBoxDhcpServers;
        private TextBox textBoxNewDhcpServer;
        private Button btnLoad;
        private Button btnCancel;

        public DhcpForm()
        {
            comboBoxDhcpServers = new ComboBox();
            textBoxNewDhcpServer = new TextBox();
            btnLoad = new Button();
            btnCancel = new Button();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Ustawienia podstawowe formularza
            this.Text = "Załaduj adresy IP";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new System.Drawing.Size(225, 135); // Zwiększono rozmiar okna

            this.Shown += (sender, e) =>
                {
                    btnLoad.Focus();  // Ustawienie focusu na btnLoad po załadowaniu formularza
                };

            // Etykieta z opisem
            var label = new Label
            {
                Text = "Aktualny adres serwera DHCP:",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true
            };

            // ComboBox zapisany adres DHCP
            comboBoxDhcpServers = new ComboBox
            {
                Location = new System.Drawing.Point(10, 30),
                Width = 204,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            // Pole tekstowe do wprowadzenia nowego adresu IP
            textBoxNewDhcpServer = new TextBox
            {
                Location = new System.Drawing.Point(10, 60),
                Width = 204,
                PlaceholderText = "Wprowadź adres IP"
            };

            // Przycisk OK
            btnLoad = new Button
            {
                Text = "Załaduj zastrzeżenia",
                Location = new System.Drawing.Point(10, 100),
                Size = new System.Drawing.Size(120, 23),
                DialogResult = DialogResult.None // Ustawiamy na None, aby ręcznie kontrolować zamknięcie
            };

            // Przycisk Cancel
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(140, 100),
                DialogResult = DialogResult.Cancel
            };

            // Obsługa zdarzenia kliknięcia przycisku OK
            btnLoad.Click += (sender, e) =>
            {
                string ipAddress;

                // Sprawdź, czy użytkownik wpisał nowy adres IP
                if (!string.IsNullOrEmpty(textBoxNewDhcpServer.Text.Trim()))
                {
                    ipAddress = textBoxNewDhcpServer.Text.Trim();
                }
                else if (comboBoxDhcpServers.SelectedItem != null)
                {
                    ipAddress = comboBoxDhcpServers.SelectedItem?.ToString() ?? string.Empty;
                }
                else
                {
                    MessageBox.Show("Wpisz adres IP serwera DHCP lub wybierz go z listy!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Nie zamykaj formularza
                }

                // Walidacja adresu IP
                if (!Regex.IsMatch(ipAddress, @"^[0-9\.]+$"))
                {
                    MessageBox.Show("Adres IP może zawierać tylko cyfry i kropki!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Nie zamykaj formularza
                }

                if (ipAddress.Length < 7 || ipAddress.Length > 15)
                {
                    MessageBox.Show("Adres IP musi mieć od 7 do 15 znaków!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Nie zamykaj formularza
                }

                if (!IPAddress.TryParse(ipAddress, out _))
                {
                    MessageBox.Show("Nieprawidłowy format adresu IP!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Nie zamykaj formularza
                }

                // Użyj wpisanego adresu IP
                DhcpServerIp = ipAddress;

                // Dodaj nowy adres IP do listy, jeśli nie istnieje
                if (!comboBoxDhcpServers.Items.Contains(ipAddress))
                {
                    comboBoxDhcpServers.Items.Add(ipAddress);
                    SaveDhcpServerIp(ipAddress); // Zapisz adres DHCP do pliku
                }

                // Zamknij formularz z wynikiem OK
                this.DialogResult = DialogResult.OK;
                this.Close();
            };



            // Dodanie kontrolek do formularza
            this.Controls.Add(label);
            this.Controls.Add(comboBoxDhcpServers);
            this.Controls.Add(textBoxNewDhcpServer);
            this.Controls.Add(btnLoad);
            this.Controls.Add(btnCancel);

            // Wczytaj zapisane adresy DHCP
            LoadSavedDhcpServers();
            btnLoad.Focus(); // Ustawienie focusu na btnLoad po załadowaniu formularza
        }

        private void LoadSavedDhcpServers()
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dhcp_config.txt");
            if (File.Exists(configFilePath))
            {
                string[] savedDhcpServers = File.ReadAllLines(configFilePath);
                comboBoxDhcpServers.Items.AddRange(savedDhcpServers);
                if (comboBoxDhcpServers.Items.Count > 0)
                {
                    comboBoxDhcpServers.SelectedIndex = 0;
                }
            }
        }

        private void SaveDhcpServerIp(string dhcpServerIp)
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dhcp_config.txt");
            if (!File.Exists(configFilePath))
            {
                File.WriteAllText(configFilePath, dhcpServerIp);
            }
            else
            {
                var savedDhcpServers = File.ReadAllLines(configFilePath).ToList();
                if (!savedDhcpServers.Contains(dhcpServerIp))
                {
                    savedDhcpServers.Add(dhcpServerIp);
                    File.WriteAllLines(configFilePath, savedDhcpServers);
                }
            }
        }

        private void RemoveDhcpServerIp(string dhcpServerIp)
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dhcp_config.txt");
            if (File.Exists(configFilePath))
            {
                var savedDhcpServers = File.ReadAllLines(configFilePath).ToList();
                savedDhcpServers.Remove(dhcpServerIp);
                File.WriteAllLines(configFilePath, savedDhcpServers);
            }
        }
    }
}