using SemaforoPorLotes.Models;


namespace SemaforoPorLotes.Repository
{
    public interface IUserRepository
    {
        bool login(User usuario);
    }
}
