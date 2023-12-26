using System;

namespace SemaforoPorLotes.Models
{
    public class Sync
    {
        public int Id { get; set; }
        public DateTime LastSync { get; set; }

        public Sync(int id, DateTime lastSync)
        {
            Id = id;
            LastSync = lastSync;
        }

        public Sync(DateTime lastSync)
        {
            LastSync = lastSync;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
