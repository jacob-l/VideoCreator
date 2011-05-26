using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Core
{
    public class Statistica
    {
        int imagesQuantity;
        int taskQuantity;
        DateTime startTime;
        DateTime endTime;

        public Statistica(int imagesQuantity, int taskQuantity)
        {
            this.imagesQuantity = imagesQuantity;
            this.taskQuantity = taskQuantity;
        }

        public void Start()
        {
            startTime = DateTime.Now;
        }

        public void Stop()
        {
            endTime = DateTime.Now;

            FileStream fStream = new FileStream(String.Format("{0}-{1}-{2}.txt", endTime.Hour, endTime.Minute, endTime.Second), FileMode.Create);
            StreamWriter writer = new StreamWriter(fStream);
            writer.WriteLine("Start time: " + startTime.ToString());
            writer.WriteLine("End time: " + endTime.ToString());
            TimeSpan workTime = endTime - startTime;
            writer.WriteLine("Work time: " + workTime.Hours + " hours, " + workTime.Minutes + " minutes, " + workTime.Seconds + " seconds.");
            writer.WriteLine("Tasks quantity: " + taskQuantity);
            writer.WriteLine("Images quantity: " + imagesQuantity);

            writer.Close();
            fStream.Close();
        }
    }
}
