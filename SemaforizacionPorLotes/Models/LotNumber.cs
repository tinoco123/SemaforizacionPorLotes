namespace SemaforoPorLotes.Models
{
    public class LotNumber
    {
        public string LotNumberName { get; set; }
        public int ItemId { get; set; }
        public int VendorId { get; set; }
        public string ExpirationDate { get; set; }

        public LotNumber(string lotNumberName, int itemId, int vendorId, string expirationDate)
        {
            LotNumberName = lotNumberName;
            ItemId = itemId;
            VendorId = vendorId;
            ExpirationDate = expirationDate;
        }

        public LotNumber(string lotNumberName, int itemId, string expirationDate)
        {
            LotNumberName = lotNumberName;
            ItemId = itemId;
            ExpirationDate = expirationDate;
        }
    }
}
