using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeSequencer
{
    internal class BaseStep
    {
        public BaseStep()
        {
        }
    }

    public enum Result
    {
        Fail,
        Pass,
        Abort,
        NoExecute,
    }
}