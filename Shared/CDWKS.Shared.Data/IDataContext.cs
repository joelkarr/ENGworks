using System;

namespace RDC.Shared.Framework.Data
{
    public interface IDataContext : IDisposable
    {
        void SaveChanges();
    }
}
