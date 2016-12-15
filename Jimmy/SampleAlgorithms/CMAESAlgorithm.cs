using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMAESharp;

namespace Jimmy.SampleAlgorithms
{
    public class CMAESAlgorithm : IAlgorithm
    {
        public CMAESAlgorithm(Jimmy evalHelper, string taskType)
        {
            this.evalHelper = evalHelper;
            this.taskType = taskType;
        }

        private Jimmy evalHelper;
        private string taskType;

        public void Run()
        {
            #region cma settings (auto generated)
            CMAEvolutionStrategy cma = new CMAEvolutionStrategy(this.taskType);

            //number of dimension (only those who have candidate value counted)
            int dimension = evalHelper.Dimensions();

            //initial X and STD (only those who have candidate value counted)
            List<double> initialX = new List<double>();
            List<double> initialStd = new List<double>();

            foreach (Parameter parameter in evalHelper.Parameters)
            {
                if (parameter.Source == ParameterSource.CandidateValue)
                {
                    initialX.Add((parameter.MaxCandidateValue() + parameter.MinCandidateValue()) / 2);
                    initialStd.Add((parameter.MaxCandidateValue() - parameter.MinCandidateValue()) / 3);
                }
            }

            double[] fitness = cma.init(dimension, initialX.ToArray(), initialStd.ToArray());

            #endregion

            while (cma.getNumber() == 0)
            {
                double[][] pop = cma.samplePopulation();
                for (int i = 0; i < pop.Length; ++i)
                {
                    //break the loop if canceled by user while in process
                    if (evalHelper.IsCanceled())
                    {
                        evalHelper.OnCanceled(EventArgs.Empty);
                        return;
                    }

                    while (!evalHelper.IsFeasible(pop[i]))
                        pop[i] = cma.resampleSingle(i);
                    fitness[i] = evalHelper.ValueOf(pop[i]);

                }
                cma.updateDistribution(fitness);

                //stopping criteria.        
                cma.options.stopTolFun = 0.000001 * cma.getBestFunctionValue();//default:0.02
                cma.options.stopTolFunHist = 0.1 * cma.options.stopTolFun;
            }
            cma.setFitnessOfMeanX(evalHelper.ValueOf(cma.getMeanX()));

            //final output
            string simpleTextReport;
            simpleTextReport = "Terminated due to" + Environment.NewLine;
            foreach (String s in cma.getMessages())
                simpleTextReport = simpleTextReport + ("  " + s + Environment.NewLine);
            simpleTextReport = simpleTextReport + "Best function value " + cma.getBestFunctionValue();

            evalHelper.OnOptimizationComplete(new OptimizationCompleteEventArgs { SimpleTextReport = simpleTextReport });
        }
    }
}
