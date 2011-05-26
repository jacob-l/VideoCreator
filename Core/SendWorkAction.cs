using System;
using System.Collections.Generic;
using CCP.Core.Jobs;
using CCP.Core.Jobs.ScheduleActions;
using CCP.Core.Messaging;
using CCP.Extensions.SelectStrategies.XPathStrategy;
using Core.Rendering;
using CCP.Core.Tasks;

namespace Core
{
    [Serializable]
    class SendWorkAction : ScheduleAction
    {
        int minStep = 10;
        int quantity;
        String baseVideoJobName = "videoJob";
        ManagerVideoJob job;
        List<String> tasksAddr = new List<string>();
        IRender render;
        Queue<WorkId> initWork = new Queue<WorkId>();

        public SendWorkAction(ManagerVideoJob job, IRender render, int quantity, IEnumerator<String> tasksAddr, IEnumerator<WorkId> initWorkIter)
        {
            this.job = job;
            tasksAddr.Reset();
            while (tasksAddr.MoveNext())
                this.tasksAddr.Add(tasksAddr.Current);
            initWorkIter.Reset();
            while (initWorkIter.MoveNext())
                this.initWork.Enqueue(initWorkIter.Current);
            this.quantity = quantity;
            this.render = render;
        }

        public override void Handle()
        {
            for (int i = 0; i < tasksAddr.Count; i++)
            {
                int curWorkCount = (int)Math.Ceiling((double)initWork.Count / (tasksAddr.Count - i));
                List<WorkId> curWorkList = new List<WorkId>();
                for (int j = 0; j < curWorkCount; j++)
                    curWorkList.Add(initWork.Dequeue());
                String nameForJob = baseVideoJobName + Guid.NewGuid().ToString();
                Message msg = new VideoProcessorJobMsg(nameForJob, "/" + job.Owner.Owner.Owner.Name + "/" + (job.Owner.Owner as NamedTask).Name + "/" + job.Name, render, curWorkList.GetEnumerator());
                msg.Target = new XPathSelectStrategy(tasksAddr[i]);
                this.job.Owner.Owner.Owner.Router.Send(msg);
            }
        }
    }
}
