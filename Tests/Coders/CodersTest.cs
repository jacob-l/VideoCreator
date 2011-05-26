using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using NUnit.Framework;
using Core;
using Core.Coders;
using Core.DataPrepare;
using System.Threading;

namespace Tests.Coders
{
    [TestFixture]
    public class CodersTest
    {
        FileWorker fileWorker;
        String process = "../../../../../3rdParty/utility/ffmpeg.exe";

        [SetUp]
        public void SetUp()
        {
            int quantity = 1;
            fileWorker = new FileWorker(ImageFormat.Png.ToString(), quantity);
            MemoryPrepare prepare = new MemoryPrepare(ImageFormat.Png);
            prepare.Add(Tests.Resource.TestCoders);
            IEnumerator<byte[]> iter = prepare.GetEnumerator();
            iter.MoveNext();
            fileWorker.Write(iter.Current, quantity - 1);
        }

        [TearDown]
        public void TearDown()
        {
            fileWorker.Remove();
        }

        private void TestCoder(ICoder coder)
        {
            String name = FileWorker.GetRandomFileName() + ".avi";
            coder.Process(fileWorker.GetTemplateName(), name);
            Assert.IsTrue(File.Exists(name));
            File.Delete(name);
            
        }

        [Test]
        public void TestCoderFfmpeg()
        {
            ICoder coder = new CoderFfmpeg(process);
            TestCoder(coder);
        }
    }
}
