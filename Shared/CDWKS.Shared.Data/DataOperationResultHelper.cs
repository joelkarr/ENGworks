using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDC.Shared.Framework.Data
{
    public class RepositoryOperationException : Exception
    {
        public RepositoryOperationException( string msg ) : base(msg)
        {
        }

        public RepositoryOperationException( string msg, Exception ex ) : base(msg, ex)
        {
        }
    }
}
