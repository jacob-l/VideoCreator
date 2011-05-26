using System;
using System.Text;
using System.IO;

namespace Core
{
    public class FileWorker:IDisposable
    {
        String folderName;
        String fileBase;
        String extension;
        int quantity;
        int dim;

        public FileWorker(String filesName, String extension, int quantity)
        {
            this.folderName = Directory.GetCurrentDirectory() + "\\" + FileWorker.MakeRandomFolder();
            this.fileBase = folderName + "\\" + filesName;
            this.extension = extension;
            this.quantity = quantity;
            dim = quantity.ToString().Length;
        }

        public FileWorker(String extension, int quantity)
        {
            this.folderName = Directory.GetCurrentDirectory() + "\\" + FileWorker.MakeRandomFolder();
            this.fileBase = folderName + "\\";
            this.extension = extension;
            this.quantity = quantity;
            dim = quantity.ToString().Length;
        }

        public byte[] Read(int i)
        {
            if ((i >= quantity) || (quantity < 0))
                throw new ArgumentException("Bad index of file");

            String fileName = FileWorker.GetFileName(fileBase, extension, i, dim);
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            byte[] data = new byte[fs.Length]; 
            fs.Read(data, 0, data.Length);

            fs.Close();

            return data;
        }

        public void Write(byte[] data, int i)
        {
            String fileName = FileWorker.GetFileName(fileBase, extension, i, dim);
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

            fs.Write(data, 0, data.Length);

            fs.Close();
        }

        public String GetTemplateName()
        {
            return fileBase + "%" + dim + "d." + extension;
        }

        public void Remove()
        {
            Directory.Delete(folderName, true);
        }

        public static String MakeRandomFolder()
        {
            String directoryName;
            do
            {
                directoryName = Guid.NewGuid().ToString();
            }
            while (Directory.Exists(directoryName));
            Directory.CreateDirectory(directoryName);

            return directoryName;
        }

        public static String GetRandomFileName()
        {
            String fileName;
            do
            {
                fileName = Guid.NewGuid().ToString();
            }
            while (File.Exists(fileName));

            return fileName;
        }

        public static String GetFileName(String baseName, String ext, int i, int dim)
        {
            StringBuilder res = new StringBuilder();
            res.Append(baseName);
            for (int j = 0; j < dim - i.ToString().Length; j++)
                res.Append("0");
            res.Append(i.ToString());
            res.Append(".");
            res.Append(ext);
            return res.ToString();
        }

        public void Dispose()
        {
            this.Remove();
        }
    }
}
