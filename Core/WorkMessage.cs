using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCP.Core.Messaging;

namespace Core
{
    [Serializable]
    public class WorkMessage:Message
    {
        WorkId workId;

        public WorkMessage(WorkId workId)
        {
            if (workId == null)
                throw new ArgumentNullException("Work can't be null");
            this.workId = workId;
        }

        public WorkId WorkId
        {
            get
            {
                return workId;
            }
        }
    }
}
