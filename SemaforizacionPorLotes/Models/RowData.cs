namespace SemaforoPorLotes.Models
{
    public struct RowData
    {
        public string ItemName { get; set; }
        public string ItemDesc { get; set; }
        public string Quantity { get; set; }
        public string LotNumber { get; set; }
        public string Type { get; set; }
        public string Vendor { get; set; }
        public string TxnId { get; set; }
        public string Date { get; set; }

        public RowData(string itemName, string itemDesc, string quantity, string lotNumber, string type, string vendor, string date, string txnId)
        {
            ItemName = itemName;
            ItemDesc = itemDesc;
            Quantity = quantity;
            LotNumber = lotNumber;
            Type = type;
            Vendor = vendor;
            TxnId = txnId;
            Date = date;
        }

        public RowData(string itemName, string quantity, string lotNumber) : this()
        {
            ItemName = itemName;
            Quantity = quantity;
            LotNumber = lotNumber;
        }

        public static bool operator ==(RowData rowData1, RowData rowData2)
        {
            return rowData1.Equals(rowData2);
        }
        public static bool operator !=(RowData rowData1, RowData rowData2)
        {
            return !rowData1.Equals(rowData2);
        }
    }
}
