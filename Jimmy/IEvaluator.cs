using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jimmy
{
    public interface IEvaluator
    {
        double Run(List<CandidateValue> x);
    }
}
