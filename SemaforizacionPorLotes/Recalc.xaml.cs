using SemaforoPorLotes.Models;
using System;
using System.Collections.Generic;
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
using SemaforoPorLotes;
using SemaforoPorLotes.Repository;

namespace SemaforizacionPorLotes
{
    /// <summary>
    /// Interaction logic for Recalc.xaml
    /// </summary>
    public partial class Recalc : Window
    {
        public Recalc()
        {
            InitializeComponent();
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(new SemaforoPorLotes.Models.User("CLE"));
            this.Close();
            mainWindow.Show();
        }

        private void recalc_Btn_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateSelected = (DateTime)initialDate.SelectedDate;
            int isDateBefore2024 = DateTime.Compare(dateSelected, new DateTime(2024, 1, 1));
            if (isDateBefore2024 < 0)
            {
                MessageBox.Show("Selecciona una fecha mayor o igual al 1 de enero del 2024", "Fecha incorrecta", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if  (dateSelected != null)
            {
                string xmlResponse = ReporteRecalculo.GenerateReportForRecalcInformation(dateSelected);
                if (xmlResponse != null)
                {
                    // Borrar informacion a partir de la fecha seleccionada
                    IEnumerable<RowData> rowDatas = ReporteRecalculo.getAllRowDataFromXmlResponse(xmlResponse);
                    ReporteRecalculo.ProcessAllRowData(rowDatas);
                    MessageBox.Show("Información actualizada correctamente", "Operación exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
            }            
            else
            {
                MessageBox.Show("Selecciona una fecha", "Campo obligatorio", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void initialDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
