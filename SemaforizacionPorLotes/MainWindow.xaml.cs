using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CsvHelper;
using Microsoft.Win32;
using SemaforoPorLotes;
using SemaforoPorLotes.Models;
using SemaforoPorLotes.Repository;

namespace SemaforizacionPorLotes
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User User { get; set; }
        
        private static ObservableCollection<LotNumbersView> tempTableData;
        private static ObservableCollection<LotNumbersView> currentTableDate;
        public MainWindow(User user)
        {
            InitializeComponent();
            if (user.Username == "CLE")
            {
                AdjustInventoryReportBtn.Visibility = Visibility.Visible;
                Recalc_Btn.Visibility = Visibility.Visible;
            }
            User = user;
            UserName.Content = User.Username;
            TableViewModel tableViewModel = new TableViewModel(new Hashtable() { });
            DataContext = tableViewModel;
            tempTableData = tableViewModel.lotNumbersViews;
            currentTableDate = tableViewModel.lotNumbersViews;
            vendorsComboBox.ItemsSource = LotNumbersTableData.GetVendors();
            itemsComboBox.ItemsSource = LotNumbersTableData.GetItems();
            colorsComboBox.ItemsSource = new List<string>() { "Seleccionar color", "Blanco", "Rojo", "Tomate", "Verde", "Morado" };
        }

        private void lotNumbersTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                LotNumbersView lotNumberView = e.Row.Item as LotNumbersView;
                if (lotNumberView != null)
                {
                    if (lotNumberView.ExpirationDate.Equals("Sin caducidad"))
                    {
                        e.Row.Background = Brushes.White;
                    }
                    else if (lotNumberView.ExpirationDate.Equals("No definido"))
                    {
                        e.Row.Background = new SolidColorBrush(Color.FromRgb(195, 158, 255));

                    }
                    else if (lotNumberView.DaysToExpire <= 30)
                    {
                        e.Row.Background = new SolidColorBrush(Color.FromRgb(255, 116, 82));
                    }
                    else if (lotNumberView.DaysToExpire <= 90)
                    {
                        e.Row.Background = new SolidColorBrush(Color.FromRgb(255, 160, 82));
                    }
                    else
                    {
                        e.Row.Background = new SolidColorBrush(Color.FromRgb(20, 255, 149));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void filterBtn_Click(object sender, RoutedEventArgs e)
        {

            Hashtable parameters = new Hashtable();

            Vendor vendorSelected = (Vendor)vendorsComboBox.SelectedItem;
            if (vendorSelected != null && vendorSelected.Id > 0)
            {
                parameters.Add("@vendor_id", vendorSelected.Id);
            }

            Item itemSelected = (Item)itemsComboBox.SelectedItem;
            if (itemSelected != null && itemSelected.Id > 0)
            {
                parameters.Add("@item_id", itemSelected.Id);
            }

            string colorSelected = (string)colorsComboBox.SelectedValue;
            if (colorSelected == "Blanco")
            {
                parameters.Add("@expiration_date", "Sin caducidad");
            }
            else if (colorSelected == "Morado")
            {
                parameters.Add("@expiration_date", "No definido");
            }
            else if (colorSelected == "Rojo")
            {
                parameters.Add("@days_to_expire", 30);
            }
            else if (colorSelected == "Tomate")
            {
                parameters.Add("@days_to_expire", 90);
            }
            else if (colorSelected == "Verde")
            {
                parameters.Add("@days_to_expire", 91);
            }

            if (initialDate.SelectedDate != null)
            {
                DateTime initialDateSelected = (DateTime)initialDate.SelectedDate;
                parameters.Add("initial_date", initialDateSelected.ToString("yyyy-MM-dd"));
            }
            if (finalDate.SelectedDate != null)
            {
                DateTime finalDateSelected = (DateTime)finalDate.SelectedDate;
                parameters.Add("final_date", finalDateSelected.ToString("yyyy-MM-dd"));
            }

            setTableData(parameters);
            searchTextBox.Text = "";
        }

        private void setTableData(Hashtable parameters)
        {
            TableViewModel tableViewModel = new TableViewModel(parameters);
            DataContext = tableViewModel;
            tempTableData = tableViewModel.lotNumbersViews;
            currentTableDate = tableViewModel.lotNumbersViews;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
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
                        csv.WriteRecords(currentTableDate);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string textToSearch = searchTextBox.Text;
            if (textToSearch.Length >= 0)
            {
                var newData = from lot_number in tempTableData
                              where lot_number.ItemName.Contains(textToSearch)
                                    || lot_number.ItemDesc.Contains(textToSearch)
                                    || lot_number.LotNumber.Contains(textToSearch)
                              select lot_number;
                ObservableCollection<LotNumbersView> lotNumbersList = new ObservableCollection<LotNumbersView>(newData);
                TableViewModel tableViewModel = new TableViewModel(lotNumbersList);
                DataContext = tableViewModel;
                currentTableDate = tableViewModel.lotNumbersViews;
            }
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            this.Close();
            login.Show();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Height >= 800)
            {
                lotNumbersTable.Height = 600;
            }
        }

        private void CleanFilter_Btn_Click(object sender, RoutedEventArgs e)
        {
            vendorsComboBox.SelectedIndex = 0;
            itemsComboBox.SelectedIndex = 0;
            colorsComboBox.SelectedIndex = 0;
            initialDate.SelectedDate = null;
            finalDate.SelectedDate = null;
            searchTextBox.Text = "";
            filterBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void AdjustInventoryReportBtn_Click(object sender, RoutedEventArgs e)
        {
            GenerateAdjustInventoryReport generateAdjustInventoryReport = new GenerateAdjustInventoryReport();
            this.Close();
            generateAdjustInventoryReport.Show();
        }

        private void lotNumbersTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            LotNumbersView row = (LotNumbersView)e.Row.Item;
            if(User.Username != "CLE")
            {
                MessageBox.Show("Sin permisos suficientes para ajustar la cantidad de inventario", "Permisos insuficientes", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                
            }
            else
            {
                int colIndex = e.Column.DisplayIndex;
                if (colIndex == 3)
                {
                    var editedValue = (e.EditingElement as TextBox).Text;
                    if (Convert.ToString(row.Quantity) != editedValue)
                    {
                        ItemRepositoryImpl itemRepositoryImpl = new ItemRepositoryImpl();
                        LotNumberRepositoryImpl lotNumberRepositoryImpl = new LotNumberRepositoryImpl();
                        int itemId = itemRepositoryImpl.GetItemId(row.ItemName);
                        int lotNumberId = lotNumberRepositoryImpl.GetLotNumberId(itemId, row.LotNumber);
                        lotNumberRepositoryImpl.UpdateLotNumberQuantity(lotNumberId, Convert.ToInt32(editedValue), "");
                        MessageBox.Show("Cantidad ajustada correctamente", "Operación exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                    }                                        
                }
            }
        }

        private void Recalc_Btn_Click(object sender, RoutedEventArgs e)
        {
            Recalc recalc = new Recalc();
            this.Close();
            recalc.Show();
        }
    }
}
