using SemaforoPorLotes.Models;
using SemaforoPorLotes.Repository;
using SemaforoPorLotes.Utils;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SemaforizacionPorLotes
{
    /// <summary>
    /// Lógica de interacción para LoadingData.xaml
    /// </summary>
    public partial class LoadingData : Window
    {
        private User User { get; set; }
        public LoadingData(User user)
        {
            InitializeComponent();
            User = user;
        }

        public void start()
        {
            DateTime dateTime = LoadData.getLastSyncDate();
            string dataXml = InventoryValuationDetailReport.GenerateInventoryValuationDetailResponseXml(dateTime);
            if (dataXml != null)
            {
                DateTime currentDateTime = DateTime.Now;
                Sync currentSync = new Sync(1, currentDateTime);
                SyncRepositoryImpl syncRepositoryImpl = new SyncRepositoryImpl();
                syncRepositoryImpl.saveSync(currentSync);
                StatusTB.Text = "Insertando datos de QuickBooks";
                IEnumerable<RowData> rowDataList = LoadData.getAllRowDataFromXmlResponse(dataXml);
                LoadData.processAllDatFromXmlResponse(rowDataList);
            }
            
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            start();
            MainWindow mainWindow = new MainWindow(User);
            this.Close();
            mainWindow.Show();
        }
    }
}
