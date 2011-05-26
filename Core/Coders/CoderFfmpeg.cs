using System;
using System.Diagnostics;

namespace Core.Coders
{
    [Serializable]
    public class CoderFfmpeg:ICoder
    {
        String processName;
        //Default settings:
        String arguments = "-f image2 -i {0} -s 800x600 -y -sameq -an -r 25 -vcodec mpeg4 {1}";

        public CoderFfmpeg(String processName, String arguments)
        {
            this.processName = processName;
            if (arguments != null)
                this.arguments = arguments;
        }

        public CoderFfmpeg(String processName)
        {
            this.processName = processName;
        }

        public void Process(String filesName, String videoName)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = this.processName;
            p.StartInfo.Arguments = String.Format(this.arguments, filesName, videoName);
            p.Start();
            p.WaitForExit();
        }
    }
}
