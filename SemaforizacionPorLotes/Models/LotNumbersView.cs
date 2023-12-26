using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SemaforoPorLotes.Models
{
    public class LotNumbersView: INotifyPropertyChanged
    {
        
        public String ItemName { get; set; }
        
        public String ItemDesc { get; set; }
        
        public String LotNumber { get; set; }
        
        public int Quantity { get; set; }
        
        public String Vendor { get; set; }
        
        public string ExpirationDate { get; set; }
        
        public int? DaysToExpire { get; set; }

        public string Color { get; set; }

        public LotNumbersView()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
