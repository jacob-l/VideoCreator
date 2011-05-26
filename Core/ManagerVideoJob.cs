using System;
using System.Collections.Generic;
using CCP.Core.Jobs;
using CCP.Core.Jobs.ScheduleActions;
using CCP.Core.Messaging;
using Core.Coders;
using Core.Rendering;
using System.Configuration;
using CCP.Extensions.SelectStrategies.XPathStrategy;
using CCP.Core.Utils;

namespace Core
{   
    public class ManagerVideoJob : NamedJob
    {
        List<String> tasksAddr = new List<String>();
        List<ScheduleAction> actions = new List<ScheduleAction>();
        Queue<WorkId> works = new Queue<WorkId>();
        int quantity;
        int nameNumberLength;
        int answered = 0;
        ICoder coder;
        FileWorker fileWorker = null;
        String resultName;
        IRender render;
        Statistica stat;
        int initWorkQuantity = 2;

        public ManagerVideoJob(String nameJob, List<String> listTasksAddr, IRender render, int quantity, ICoder coder, String resultName)
            : base(nameJob)
        {
            IEnumerator<String> tasksAddr = listTasksAddr.GetEnumerator();
            tasksAddr.Reset();
            while (tasksAddr.MoveNext())
                this.tasksAddr.Add(tasksAddr.Current);

            if (quantity <= 0)
                throw new ArgumentException("Quantity should be positive");
            this.quantity = quantity;
            this.render = render;
            this.nameNumberLength = quantity.ToString().Length;
            this.coder = coder;
            this.resultName = resultName;

            FillWorks();
            FillActions();
        }

        private FileWorker InitialFileWorker(String extension)
        {
            return new FileWorker(extension, quantity);
        }

        private void FillActions()
        {
            List<WorkId> initialWork = new List<WorkId>();
            for (int i = 0; i < tasksAddr.Count * initWorkQuantity; i++)
                if (works.Count != 0)
                    initialWork.Add(works.Dequeue());
            actions.Add(new SendWorkAction(this, render, quantity, tasksAddr.GetEnumerator(), initialWork.GetEnumerator()));
        }

        private void FillWorks()
        {
            int step = int.Parse(ConfigurationManager.AppSettings["workSize"]);

            int startCapture = 0;
            int i = 0;
            int fullWorkNumber = (int)Math.Ceiling((double)quantity / step);
            stat = new Statistica(quantity, tasksAddr.Count);
            stat.Start();
            while (startCapture < quantity)
            {
                int endCapture = startCapture + step;
                if (endCapture > quantity)
                    endCapture = quantity;
                works.Enqueue(new WorkId(i, fullWorkNumber, startCapture, endCapture));
                i++;
                startCapture += step;
            }
        }

        private void WriteImages(ImagesMessage message)
        {
            if (fileWorker == null)
                fileWorker = InitialFileWorker(message.Extension);
            List<byte[]> imagesList = message.Images;
            for (int i = 0; i < imagesList.Count; i++)
            {
                fileWorker.Write(imagesList[i], message.WorkId.StartCapture + i);
            }
        }

        public override ScheduleAction Next()
        {
            ScheduleAction act;
            if (actions.Count != 0)
            {
                act = actions[0];
                actions.RemoveAt(0);
            }
            else
            {
                act = new WaitScheduleAction(this);
            }
            
            return act;
        }

        public override void Consume(Message msg)
        {
            ImagesMessage message = msg as ImagesMessage;
            if (message == null)
                return;
            
            WriteImages(message);

            Message answerMsg;

            if (works.Count != 0)
            {
                answerMsg = new WorkMessage(works.Dequeue());
            }
            else
            {
                answerMsg = new StopJobSchedullerMessage(ExitType.Normal);
            }

            answerMsg.Target = msg["source"] as ISelectReceiversStrategy;
            this.Send(answerMsg);

            answered++;
            if (answered == message.WorkId.FullNumber)
            {
                actions.Add(new GlueVideoAction(coder, fileWorker, resultName, stat));
                actions.Add(new FinishScheduleAction(this));
                if (!this.IsPlanning)
                {
                    this.MoveToPlanningJobs();
                }
            }
        }
    }
}
