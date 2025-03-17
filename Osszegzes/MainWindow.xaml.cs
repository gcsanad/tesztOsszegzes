using MySql.Data.MySqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Osszegzes
{
    public partial class MainWindow : Window
    {
        private string connectionString = "datasource=127.0.0.1;port=3306;database=teszt_adatok;username=root;password=;charset=utf8;";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnMentes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "INSERT INTO teszt_osszegzes (tanar, osztaly, megerthetoseg, erthetoseg, tablaiMunka, vetitettDiasor, oktatofilmek, egyeniGyak, haziFeladatok, ido, gyakorisag, erzes, elmeny, megjegyzes) " +
                                   "VALUES (@tanar, @osztaly, @megerthetoseg, @ertetoseg, @tablaiMunka, @diasor, @filmek, @gyakFeladatok, @haziFeladatok, @ido, @gyakorisag, @erzes, @elmeny, @megjegyzes)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@tanar", txtTanar.Text);
                        cmd.Parameters.AddWithValue("@osztaly", txtOsztaly.Text);
                        cmd.Parameters.AddWithValue("@megerthetoseg", ((ComboBoxItem)cmbMegerthetoseg.SelectedItem)?.Content.ToString());
                        cmd.Parameters.AddWithValue("@ertetoseg", ((ComboBoxItem)cmbErtetoseg.SelectedItem)?.Content.ToString());
                        cmd.Parameters.AddWithValue("@tablaiMunka", int.Parse(((ComboBoxItem)cmbTablaiMunka.SelectedItem)?.Content.ToString()));
                        cmd.Parameters.AddWithValue("@diasor", int.Parse(((ComboBoxItem)cmbDiasor.SelectedItem)?.Content.ToString()));
                        cmd.Parameters.AddWithValue("@filmek", int.Parse(((ComboBoxItem)cmbFilmek.SelectedItem)?.Content.ToString()));
                        cmd.Parameters.AddWithValue("@gyakFeladatok", int.Parse(((ComboBoxItem)cmbGyak.SelectedItem)?.Content.ToString()));
                        cmd.Parameters.AddWithValue("@haziFeladatok", int.Parse(((ComboBoxItem)cmbHaziFeladatok.SelectedItem)?.Content.ToString()));
                        cmd.Parameters.AddWithValue("@ido", ((ComboBoxItem)cmbIdo.SelectedItem)?.Content.ToString());
                        cmd.Parameters.AddWithValue("@gyakorisag", ((ComboBoxItem)cmbGyakorisag.SelectedItem)?.Content.ToString());
                        cmd.Parameters.AddWithValue("@erzes", ((ComboBoxItem)cmbErzes.SelectedItem)?.Content.ToString());
                        cmd.Parameters.AddWithValue("@elmeny", ((ComboBoxItem)cmbElmeny.SelectedItem)?.Content.ToString());
                        cmd.Parameters.AddWithValue("@megjegyzes", txtMegjegyzes.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Adat mentése sikeres!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnResults_Click(object sender, RoutedEventArgs e)
        {
            Eredmenyek resultsPage = new Eredmenyek();
            this.Content = resultsPage;
        }

    }
}