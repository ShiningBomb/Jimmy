using System;
using System.Collections.Generic;
using System.Linq;
using NCalc;
using System.ComponentModel;

namespace Jimmy
{
    public class Jimmy
    {
        //Objective:
        //accomodate algorithms to easily evaluate floating number without considering other things.
        //Ability:
        //1.round down to nearest candidate value
        //2.prevent duplicate simulation        
        //3.range check (by calling isFeasible method)
        //4.converting doubles to simulator ready values
        //5.firing result to the UI
        //6.knows whether user hit stop button 

        public Jimmy(IEvaluator evaluator, List<Parameter> parameters)
        {
            this.evaluator = evaluator;
            this.parameters = parameters;
        }

        public List<Parameter> Parameters
        {
            get
            {
                return this.parameters.Where((x) => x.Source == ParameterSource.CandidateValue).ToList();
            }
        }

        private List<Parameter> parameters;
        private IEvaluator evaluator;
        private List<double[]> inputDatabase = new List<double[]>();
        private List<double> fitnessDatabase = new List<double>();

        public double ValueOf(double[] x)
        {
            //round down to nearest candidate value
            for (int i = 0; i < x.Length; i++)
            {
                if (Parameters[i].Type != ParameterType.Text)
                {
                    for (int j = 0; j < Parameters[i].CandidateValues.Count; j++)
                    {

                        if (Parameters[i].CandidateValues[j].Numerical > x[i])
                        {
                            x[i] = Parameters[i].CandidateValues[j - 1].Numerical;
                            break;
                        }

                        if (j == Parameters[i].CandidateValues.Count - 1) //if its bigger than any candidate value, round down to the biggest candidate value
                        {
                            x[i] = Parameters[i].CandidateValues.Last().Numerical;
                        }
                    }
                }
                else //if it is a text, round down by flooring as they dont have candidate values
                {
                    x[i] = Math.Floor(x[i]);
                }
            }

            //check if it already simulated
            for (int i = 0; i < inputDatabase.Count; i++)
            {
                int match = 0;
                for (int j = 0; j < x.Length; j++)
                {
                    if (inputDatabase[i][j] == x[j])
                    {
                        match++;
                    }
                }
                if (match == x.Length)
                {
                    return fitnessDatabase[i];
                }
            }

            //run simulator
            List<CandidateValue> toSimulator = ToEvaluator(x);
            double res = evaluator.Run(toSimulator);

            //update database
            inputDatabase.Add((double[])x.Clone());
            fitnessDatabase.Add(res);

            //firing result
            List<string> result = new List<string>();
            result.Add(fitnessDatabase.Count.ToString("D5"));
            result.Add(fitnessDatabase.Last().ToString("G7"));

            foreach (CandidateValue cv in toSimulator)
            {
                result.Add(cv.ToString());
            }

            OnOneEvaluationComplete(new OneEvaluationCompleteEventArgs() { EvaluationResult = result });
            return res;
        }

        public bool IsFeasible(double[] x)
        {
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] > Parameters[i].MaxCandidateValue() || x[i] < Parameters[i].MinCandidateValue())
                {
                    return false;
                }
            }
            return true;
        }

        private List<CandidateValue> ToEvaluator(double[] x)//convert double into simulator ready values, not included: duplicate check, round down to candidate value
        {
            List<CandidateValue> temp = new List<CandidateValue>();
            Dictionary<string, object> dict = new Dictionary<string, object>();

            for (int i = 0; i < x.Length; i++)
            {
                CandidateValue cv = new CandidateValue();
                if (Parameters[i].Type == ParameterType.Text)
                {
                    cv.Text = Parameters[i].CandidateValues[(int)x[i]].Text;
                    cv.Numerical = parameters[i].CandidateValues[(int)x[i]].Numerical;
                    temp.Add(cv);
                    dict[Parameters[i].Name] = cv.ToString();
                }
                else if (Parameters[i].Type == ParameterType.Integer)
                {
                    cv.Numerical = (int)x[i];
                    temp.Add(cv);
                    dict[Parameters[i].Name] = cv.ToString();
                }
                else if (Parameters[i].Type == ParameterType.Real)
                {
                    cv.Numerical = x[i];
                    temp.Add(cv);
                    dict[Parameters[i].Name] = cv.ToString();
                }
            }

            for (int i = 0; i < parameters.Count(); i++)
            {
                CandidateValue cv = new CandidateValue();
                if (parameters[i].Source == ParameterSource.Formula)
                {
                    Expression e = new Expression(parameters[i].Formula);
                    e.Parameters = dict;

                    if (parameters[i].Type == ParameterType.Text)
                    {
                        cv.Text = e.Evaluate().ToString();
                        temp.Insert(i, cv);
                        dict[parameters[i].Name] = cv.ToString();
                    }
                    else if (parameters[i].Type == ParameterType.Integer)
                    {
                        cv.Numerical = (int)double.Parse(e.Evaluate().ToString());
                        temp.Insert(i, cv);
                        dict[parameters[i].Name] = cv.ToString();
                    }
                    else if (parameters[i].Type == ParameterType.Real)
                    {
                        cv.Numerical = double.Parse(e.Evaluate().ToString());
                        temp.Insert(i, cv);
                        dict[parameters[i].Name] = cv.ToString();
                    }
                }
            }
            return temp;
        }

        public int Dimensions()
        {
            return this.Parameters.Count();
        }

        private Func<bool> lambdaIsCanceled;

        public Func<bool> IsCanceled
        {
            get
            {
                if (lambdaIsCanceled == null)
                    return () => false;

                return lambdaIsCanceled;
            }
            set { lambdaIsCanceled = value; }
        }

        public event EventHandler<OneEvaluationCompleteEventArgs> OneEvaluationComplete;
        public event EventHandler Canceled;
        public event EventHandler<OptimizationCompleteEventArgs> OptimizationComplete;

        public virtual void OnOneEvaluationComplete(OneEvaluationCompleteEventArgs e)
        {
            OneEvaluationComplete?.Invoke(this, e);
        }

        public virtual void OnCanceled(EventArgs e)
        {
            Canceled?.Invoke(this, e);
        }

        public virtual void OnOptimizationComplete(OptimizationCompleteEventArgs e)
        {
            OptimizationComplete?.Invoke(this, e);
        }
    }
}
