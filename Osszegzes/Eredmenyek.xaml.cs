using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Osszegzes
{
    public partial class Eredmenyek : Page
    {
        private string connectionString = "datasource=127.0.0.1;port=3306;database=teszt_adatok;username=root;password=;charset=utf8;";
        private DataTable dataTable;

        public Eredmenyek()
        {
            InitializeComponent();
            LoadData();
            LoadFilters();
        }

        private void LoadData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM teszt_osszegzes ORDER BY id DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            dgResults.ItemsSource = dataTable.DefaultView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az adatok betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFilters()
        {
            try
            {
                // Add empty option for no filter
                cmbFilterTeacher.Items.Add(string.Empty);
                cmbFilterClass.Items.Add(string.Empty);

                // Add unique teachers
                var teachers = dataTable.AsEnumerable()
                    .Select(row => row.Field<string>("tanar"))
                    .Distinct()
                    .Where(t => !string.IsNullOrEmpty(t))
                    .OrderBy(t => t);

                foreach (var teacher in teachers)
                {
                    cmbFilterTeacher.Items.Add(teacher);
                }

                // Add unique classes
                var classes = dataTable.AsEnumerable()
                    .Select(row => row.Field<string>("osztaly"))
                    .Distinct()
                    .Where(c => !string.IsNullOrEmpty(c))
                    .OrderBy(c => c);

                foreach (var className in classes)
                {
                    cmbFilterClass.Items.Add(className);
                }

                // Select first (empty) option
                cmbFilterTeacher.SelectedIndex = 0;
                cmbFilterClass.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a szűrők betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            string teacherFilter = cmbFilterTeacher.SelectedItem?.ToString() ?? string.Empty;
            string classFilter = cmbFilterClass.SelectedItem?.ToString() ?? string.Empty;

            DataView dv = dataTable.DefaultView;

            string filterExpression = "";

            if (!string.IsNullOrEmpty(teacherFilter))
            {
                filterExpression = $"tanar = '{teacherFilter}'";
            }

            if (!string.IsNullOrEmpty(classFilter))
            {
                if (!string.IsNullOrEmpty(filterExpression))
                {
                    filterExpression += " AND ";
                }
                filterExpression += $"osztaly = '{classFilter}'";
            }

            dv.RowFilter = filterExpression;
            dgResults.ItemsSource = dv;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.Content = new MainWindow().Content;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            LoadFilters();
        }


        private void ShowStatistics(DataView dataView)
        {
            try
            {
                // Convert DataView to a workable collection
                var rows = dataView.ToTable().AsEnumerable();

                // Calculate statistics
                double avgDifficulty = rows.Average(r => Convert.ToDouble(r.Field<int>("nehezseg")));

                var megerthetosegStats = rows.GroupBy(r => r.Field<string>("megerthetoseg"))
                    .Select(g => new { Category = g.Key, Count = g.Count(), Percentage = Math.Round((double)g.Count() / rows.Count() * 100, 1) })
                    .OrderByDescending(x => x.Count);

                var elmenyStat = rows.GroupBy(r => r.Field<string>("elmeny"))
                    .Select(g => new { Category = g.Key, Count = g.Count(), Percentage = Math.Round((double)g.Count() / rows.Count() * 100, 1) })
                    .OrderByDescending(x => x.Count);

                var idoStat = rows.GroupBy(r => r.Field<string>("ido"))
                    .Select(g => new { Category = g.Key, Count = g.Count(), Percentage = Math.Round((double)g.Count() / rows.Count() * 100, 1) })
                    .OrderByDescending(x => x.Count);

                // Build message
                string message = $"Statisztikák ({rows.Count()} értékelés alapján):\n\n";

                message += $"Átlagos nehézség: {avgDifficulty:F1}/6\n\n";

                message += "Megérthetőség:\n";
                foreach (var stat in megerthetosegStats)
                {
                    message += $"- {stat.Category}: {stat.Count} ({stat.Percentage}%)\n";
                }

                message += "\nÉlmény:\n";
                foreach (var stat in elmenyStat)
                {
                    message += $"- {stat.Category}: {stat.Count} ({stat.Percentage}%)\n";
                }

                message += "\nIdőráfordítás:\n";
                foreach (var stat in idoStat)
                {
                    message += $"- {stat.Category}: {stat.Count} ({stat.Percentage}%)\n";
                }

                // Show statistics
                MessageBox.Show(message, "Statisztikák", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a statisztikák számításakor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnStatistics_Click(object sender, RoutedEventArgs e)
        {
            // Filter current data view
            DataView currentView = (DataView)dgResults.ItemsSource;

            if (currentView.Count == 0)
            {
                MessageBox.Show("Nincs megjeleníthető adat a statisztikához.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Create and show the statistics window
            StatisticsWindow statsWindow = new StatisticsWindow(currentView);
            statsWindow.Owner = Window.GetWindow(this);
            statsWindow.ShowDialog();
        }
    }
}