using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCP.Core.Messaging;

namespace Core
{
    [Serializable]
    public class JustTest : StartJobMessage
    {
        public JustTest()
        {
        }

        public override CCP.Core.Jobs.Job CreateJob()
        {
            Console.WriteLine("!!!!!!!!!!");
            Console.WriteLine("");
            return null;
        }
    }
}
