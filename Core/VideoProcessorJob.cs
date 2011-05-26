using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CCP.Core.Jobs;
using CCP.Core.Jobs.ScheduleActions;
using CCP.Extensions.SelectStrategies.XPathStrategy;
using Core.Rendering;
using Core.DataPrepare;
using CCP.Extensions.Serializers;
using CCP.Core.Messaging;

namespace Core
{
   
    class VideoProcessorJob : NamedJob
    {
        Queue<WorkId> works = new Queue<WorkId>();
        IRender render;
        String forAnswer;
        ImageFormat imageFormat = ImageFormat.Png;
        ISelectReceiversStrategy strategy;

        public VideoProcessorJob(String name, String forAnswer, IRender render, IEnumerator<WorkId> workIter, ISelectReceiversStrategy strategy)
            : base(name)
        {
            this.forAnswer = forAnswer;
            workIter.Reset();
            while (workIter.MoveNext())
                this.works.Enqueue(workIter.Current);
            this.render = render;
            this.strategy = strategy;
        }

        private void Save(Bitmap bmp, IPrepare prepare, int number)
        {
            prepare.Add(bmp);

            Console.WriteLine("-----------------" + number + " saved" + "-----------------");
        }

        private IPrepare Draw(WorkId workId)
        {
            IPrepare prepare = new MemoryPrepare(imageFormat);
            for (int i = workId.StartCapture; i < workId.EndCapture; i++)
            {
                Save(render.GetBitmap(i), prepare, i);
            }
            return prepare;
        }

        public override ScheduleAction Next()
        {
            if (works.Count == 0)
                return new WaitScheduleAction(this);

            WorkId workId = works.Dequeue();
            IPrepare prepare = Draw(workId);
            ImagesMessage msg = new ImagesMessage(prepare.GetEnumerator(), workId, imageFormat.ToString());
            msg.Target = new XPathSelectStrategy(forAnswer);
            msg["source"] = strategy;
            this.Owner.Owner.Owner.Router.Send(msg);
               
            return new EmptyScheduleAction();
        }

        public override void Consume(Message msg)
        {
            if (msg is WorkMessage)
            {
                works.Enqueue((msg as WorkMessage).WorkId);
                if (!this.IsPlanning)
                    this.MoveToPlanningJobs();
            }
        }
    }
}
