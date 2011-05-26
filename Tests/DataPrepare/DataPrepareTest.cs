using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using Core.DataPrepare;

namespace Tests.DataPrepare
{
    
    [TestFixture]
    public class DataPrepareTest
    {
        ImageFormat format = ImageFormat.Bmp;

        private List<Bitmap> GetBitmaps()
        {
            List<Bitmap> list = new List<Bitmap>();
            list.Add(Tests.Resource.TestCoders);
            list.Add(Tests.Resource.TestCoders2);
            
            return list;
        }

        private void PrepareTest(IPrepare prepare)
        {
            List<Bitmap> list = GetBitmaps();
            for (int i = 0; i < list.Count; i++)
                prepare.Add(list[i]);

            List<byte[]> listByte = new List<byte[]>();
            for (int i = 0; i < list.Count; i++)
            {
                MemoryStream stream = new MemoryStream();
                list[i].Save(stream, format);
                listByte.Add(stream.ToArray());
                stream.Close();
                stream.Dispose();
            }

            Assert.AreEqual(prepare.Count, list.Count, prepare.ToString() + " Problem with count");

            IEnumerator<byte[]> iter = prepare.GetEnumerator();
            iter.Reset();
            while (iter.MoveNext())
            {
                Assert.IsTrue(listByte.Contains(iter.Current, new ComparerByteArray()));
            }
        }

        [Test]
        public void MemoryPrepareTets()
        {
            MemoryPrepare prepare = new MemoryPrepare(format);
            PrepareTest(prepare);
        }

        [Test]
        public void DiskPrepareTets()
        {
            HardDiskPrepare prepare = new HardDiskPrepare(format, GetBitmaps().Count);
            try
            {
                PrepareTest(prepare);
            }
            finally
            {
                prepare.Dispose();
            }
        }

        [Test]
        public void ComparerTest()
        {
            int quantity = 10;
            byte[] array1 = new byte[quantity];
            byte[] array2 = new byte[quantity];

            for (int i = 0; i < quantity; i++)
            {
                array1[i] = (byte)i;
                array2[i] = (byte)i;
            }

            List<byte[]> list = new List<byte[]>();
            list.Add(array1);
            Assert.IsTrue(list.Contains(array2, new ComparerByteArray()), "Can't find existing");

            for (int i = 0; i < quantity; i++)
            {
                array1[i] = (byte)i;
                array2[i] = (byte)(quantity - i);
            }

            list = new List<byte[]>();
            list.Add(array1);
            Assert.IsFalse(list.Contains(array2, new ComparerByteArray()), "Find not existing element");
        }
    }
}
