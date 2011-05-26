using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Core.DataPrepare
{
    public interface IPrepare
    {
        void Add(Bitmap bmp);
        IEnumerator<byte[]> GetEnumerator();
        int Count
        {
            get;
        }
    }
}
