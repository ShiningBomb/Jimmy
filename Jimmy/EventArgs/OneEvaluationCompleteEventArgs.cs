using System;
using System.Collections.Generic;

namespace Jimmy
{
    public class OneEvaluationCompleteEventArgs : EventArgs
    {
        public List<string> EvaluationResult;
        public object Tag;
    }
}
