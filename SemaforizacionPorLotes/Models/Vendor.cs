namespace SemaforoPorLotes.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        public string VendorName { get; set; }

        public Vendor(int id, string vendorName)
        {
            Id = id;
            VendorName = vendorName;
        }

        public Vendor(string vendorName)
        {
            VendorName = vendorName;
        }
    }
}
