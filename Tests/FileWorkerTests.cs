using System;
using System.IO;
using NUnit.Framework;
using Core;

namespace Tests
{
    [TestFixture]
    public class FileWorkerTests
    {
        int dataSize = 777;
        
        byte[] GetRandomData()
        {
            byte[] data = new byte[dataSize];
            Random rand = new Random(DateTime.Now.Second);
            for (int i = 0; i < dataSize; i++)
                data[i] = (byte)rand.Next();
            return data;
        }

        bool isEqual(byte[] data1, byte[] data2)
        {
            if (data1.Length != data2.Length)
                return false;
            for (int i = 0; i < data1.Length; i++)
                if (data1[i] != data2[i])
                    return false;
            return true;
        }

        [Test]
        public void ReadWriteTest()
        {
            int quantity = 1;
            byte[] data, dataFromFile;

            FileWorker worker = new FileWorker("", quantity);

            data = GetRandomData();
            worker.Write(data, quantity - 1);
            dataFromFile = worker.Read(quantity - 1);
            try
            {
                Assert.IsTrue(isEqual(data, dataFromFile));
            }
            finally
            {
                worker.Remove();
            }
        }

        [Test]
        public void CreateFolderTest()
        {
            String folderName = null;
            try
            {
                folderName = FileWorker.MakeRandomFolder();
                Assert.IsTrue(Directory.Exists(folderName));
            }
            finally
            {
                if (folderName != null)
                    Directory.Delete(folderName);
            }
        }
    }
}
