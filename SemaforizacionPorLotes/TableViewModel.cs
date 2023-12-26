using SemaforoPorLotes.Models;
using SemaforoPorLotes.Repository;
using System.Collections;
using System.Collections.ObjectModel;

namespace SemaforoPorLotes
{
    public class TableViewModel
    {
        public ObservableCollection<LotNumbersView> lotNumbersViews { get; set; }        
        public TableViewModel(Hashtable parameters) 
        {
            lotNumbersViews = LotNumbersTableData.GetLotNumbers(parameters);
        }

        public TableViewModel(ObservableCollection<LotNumbersView> lotNumbersList)
        {
            lotNumbersViews = lotNumbersList;
        }
    }
}
