using System;
using System.Collections.Generic;
using CCP.Core.Messaging;

namespace Core
{
    [Serializable]
    class ImagesMessage : Message
    {
        List<byte[]> data = new List<byte[]>();
        WorkId workId;
        String extension;

        public ImagesMessage(IEnumerator<byte[]> iter, WorkId workId, String extension)
        {
            iter.Reset();
            while (iter.MoveNext())
            {
                byte[] imgData = new byte[iter.Current.Length];
                iter.Current.CopyTo(imgData, 0);
                data.Add(imgData);
            }
            this.workId = workId;
            this.extension = extension;
        }

        public List<byte[]> Images 
        {
            get
            {
                List<byte[]> result = new List<byte[]>();
                for (int i = 0; i < data.Count; i++)
                {
                    byte[] copy = new byte[data[i].Length];
                    data[i].CopyTo(copy, 0);
                    result.Add(copy);
                }
                return result;
            }
        }

        public WorkId WorkId
        {
            get
            {
                return workId;
            }
        }

        public String Extension
        {
            get
            {
                return extension;
            }
        }
    }
}
