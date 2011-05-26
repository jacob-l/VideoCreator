using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.DataPrepare
{
    public class ComparerByteArray : IEqualityComparer<byte[]>
    {
        bool IEqualityComparer<byte[]>.Equals(byte[] x, byte[] y)
        {
            if (x.Length != y.Length)
                return false;
            for (int i = 0; i < x.Length; i++)
                if (x[i] != y[i])
                    return false;
            return true;
        }

        int IEqualityComparer<byte[]>.GetHashCode(byte[] obj)
        {
            throw new NotImplementedException();
        }
    }

}
