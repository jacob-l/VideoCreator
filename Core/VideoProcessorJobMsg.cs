using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCP.Core.Messaging;
using Core.Rendering;

namespace Core
{
    [Serializable]
    public class VideoProcessorJobMsg : StartJobMessage
    {
        String name;
        String forAnswer;
        IRender render;
        List<WorkId> workList = new List<WorkId>();

        public VideoProcessorJobMsg(String name, String forAnswer, IRender render, IEnumerator<WorkId> workIter)
        {
            if ((name == null) || (forAnswer == null) || (render == null))
                throw new ArgumentNullException("Null parameter in VideoProcessorJobMsg");
            this.name = name;
            this.forAnswer = forAnswer;
            this.render = render;
            workIter.Reset();
            while (workIter.MoveNext())
                this.workList.Add(workIter.Current);
        }

        public override CCP.Core.Jobs.Job CreateJob()
        {
            
            Console.WriteLine("!!!!!!!!!!!!!!!!!");
            return new VideoProcessorJob(name, forAnswer, render, workList.GetEnumerator(), this.Target);
        }
    }
}
