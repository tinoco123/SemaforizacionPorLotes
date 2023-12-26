using SemaforoPorLotes.Models;

namespace SemaforoPorLotes.Repository
{
    public interface IVendorRepository
    {
        int GetVendorId(string vendorName);
        bool InsertVendor(Vendor vendor);
    }
}
