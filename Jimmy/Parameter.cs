using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.IO;

namespace Jimmy
{
    [Serializable]
    public class Parameter
    {
        public Parameter(double min, double max, double precision)
        {
            double candidate = min;
            while (true)
            {
                _CandidateValues.Add(new CandidateValue() { Numerical = candidate });
                candidate += precision;
                if (candidate > max)
                {
                    break;
                }
            }
        }

        public Parameter() { }

        public string Name { get; set; }
        public ParameterType Type { get; set; }
        public ParameterSource Source { get; set; }

        private BindingList<CandidateValue> _CandidateValues = new BindingList<CandidateValue>();
        public BindingList<CandidateValue> CandidateValues
        {
            get
            {
                return _CandidateValues;
            }
            set
            {
                _CandidateValues = value;
            }
        }

        public string Formula { get; set; }//if any

        public double MaxCandidateValue()//add some gap to overcome round down
        {
            if (this.Type != ParameterType.Text)
            {
                List<double> numericalCandidateValues = GetNumericalCandidateValues();
                return numericalCandidateValues.Last() + (numericalCandidateValues.Last() - numericalCandidateValues[numericalCandidateValues.Count() - 2]);
            }
            else
            {
                return _CandidateValues.Count();
            }
        }

        public double MinCandidateValue()
        {
            if (this.Type != ParameterType.Text)
            {
                List<double> numericalCandidateValues = GetNumericalCandidateValues();
                return numericalCandidateValues.First();
            }
            else
            {
                return 0;
            }
        }

        public List<double> GetNumericalCandidateValues()
        {
            IEnumerable<double> sortedNumericalCandidateValue = from candidate in _CandidateValues
                                                                orderby candidate.Numerical ascending
                                                                select candidate.Numerical;
            return sortedNumericalCandidateValue.ToList();
        }
    }

    public enum ParameterType
    {
        Real = 0,
        Integer = 1,
        Text = 2,
    }

    public enum ParameterSource
    {
        CandidateValue = 0,
        Formula = 1
    }

}
