using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Core.DataPrepare
{
    public class MemoryPrepare:IPrepare
    {
        List<byte[]> list = new List<byte[]>();
        ImageFormat imageFormat;

        public MemoryPrepare(ImageFormat imageFormat)
        {
            this.imageFormat = imageFormat;
        }

        public void Add(Bitmap bmp)
        {
            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, imageFormat);

            byte[] data;
            data = stream.ToArray();

            stream.Close();
            list.Add(data);
        }

        public IEnumerator<byte[]> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }
    }
}
