namespace SemaforoPorLotes.Models
{
    public class LotNumber
    {
        public string LotNumberName { get; set; }
        public int Quantity { get; set; }
        public int ItemId { get; set; }
        public int VendorId { get; set; }
        public string ExpirationDate { get; set; }
        public string LastUpdate { get; set; }

        public LotNumber(string lotNumberName, int quantity, int itemId, int vendorId, string expirationDate, string lastUpdate)
        {
            LotNumberName = lotNumberName;
            Quantity = quantity;
            ItemId = itemId;
            VendorId = vendorId;
            ExpirationDate = expirationDate;
            LastUpdate = lastUpdate;
        }

        public LotNumber(string lotNumberName, int quantity, int itemId, string expirationDate, string lastUpdate)
        {
            LotNumberName = lotNumberName;
            Quantity = quantity;
            ItemId = itemId;
            ExpirationDate = expirationDate;
            LastUpdate = lastUpdate;
        }
    }
}
