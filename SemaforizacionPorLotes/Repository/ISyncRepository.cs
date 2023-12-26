using SemaforoPorLotes.Models;
using System;

namespace SemaforoPorLotes.Repository
{
    public interface ISyncRepository
    {
        DateTime getLastSync();
        bool saveSync(Sync sync);
    }
}
