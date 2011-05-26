using System;
using CCP.Core.Jobs.ScheduleActions;
using Core.Coders;

namespace Core
{
    class GlueVideoAction : ScheduleAction
    {
        ICoder coder;
        FileWorker fileWorker;
        String videoName;
        Statistica stat;

        public GlueVideoAction(ICoder coder, FileWorker fileWorker, String videoName, Statistica stat)
        {
            this.coder = coder;
            this.fileWorker = fileWorker;
            this.videoName = videoName;
            this.stat = stat;
        }

        public override void Handle()
        {
            coder.Process(fileWorker.GetTemplateName(), videoName);
            stat.Stop();
            fileWorker.Dispose();
        }
    }
}
