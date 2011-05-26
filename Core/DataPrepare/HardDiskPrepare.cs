using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Core.DataPrepare
{

    public class HardDiskPrepare:IPrepare,IDisposable
    {
        ImageFormat imageFormat;
        FileWorker fileWorker;
        int count = 0;

        public HardDiskPrepare(ImageFormat imageFormat, int quantity)
        {
            this.imageFormat = imageFormat;
            this.fileWorker = new FileWorker(imageFormat.ToString(), quantity);
        }

        public void Add(Bitmap bmp)
        {
            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, imageFormat);

            byte[] data;
            data = stream.ToArray();

            fileWorker.Write(data, count);
            count++;
            stream.Close();
        }

        public IEnumerator<byte[]> GetEnumerator()
        {
            List<byte[]> list = new List<byte[]>();
            for (int i = 0; i < count; i++)
            {
                list.Add(fileWorker.Read(i));
            }
            return list.GetEnumerator();
        }

        public int Count
        {
            get 
            { 
                return count; 
            }
        }

        public void Dispose()
        {
            fileWorker.Dispose();
        }
    }
}
