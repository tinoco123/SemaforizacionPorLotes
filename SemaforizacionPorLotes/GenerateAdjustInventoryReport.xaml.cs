using CsvHelper;
using Microsoft.Win32;
using SemaforizacionPorLotes.Models;
using SemaforoPorLotes.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SemaforizacionPorLotes
{
    /// <summary>
    /// Interaction logic for GenerateAdjustInventoryReport.xaml
    /// </summary>
    public partial class GenerateAdjustInventoryReport : Window
    {
        public GenerateAdjustInventoryReport()
        {
            InitializeComponent();
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(new User("CLE"));
            this.Close();
            mainWindow.Show();
        }

        private void initialDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void finalDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void filterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (initialDate.SelectedDate != null && finalDate.SelectedDate != null)
            {
                DateTime initialDateSelected = (DateTime)initialDate.SelectedDate;
                DateTime finalDateSelected = (DateTime)finalDate.SelectedDate;
                IEnumerable<InventoryAdjustmentLineRet> adjustInventories =  InventoryAdjustmentQuery.GetLotNumberInventoryAdjustment(initialDateSelected, finalDateSelected);

                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.InitialDirectory = @"C:\";
                saveFile.Title = "Guardar archivo CSV";
                saveFile.DefaultExt = "csv";
                saveFile.Filter = "CSV (*.csv)|*.csv";
                saveFile.RestoreDirectory = true;
                bool? result = saveFile.ShowDialog();
                if (result == true)
                {
                    try
                    {
                        using (var writer = new StreamWriter(saveFile.FileName, false, Encoding.GetEncoding("ISO-8859-1")))
                        using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
                        {
                            csv.WriteRecords(adjustInventories);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }

            }
            else
            {
                MessageBox.Show("Debes seleccionar una fecha inicial y una fecha final", "Seleccionar rango de fechas", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
