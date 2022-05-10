using System.Collections.Generic;
using System.Linq;
using Test.Discounter;

namespace Test
{
    public class Calculator
    {
        public Calculator(List<Inputs> listOfAllInputs)
        {
            _listOfAllInputs = listOfAllInputs;
            TermForAllProjections = MaximumTermOfProjection.SetTermOfProjection(listOfAllInputs);

            ProjectedCashflowsForEachOutgoingType = new List<double[]>();
            ProjectedRollForwardCashflowsForEachOutgoingType = new List<double[]>();

            RunProjector();
            RunRollForward();
        }
        
        private readonly List<Inputs> _listOfAllInputs;

        public int TermForAllProjections { get; }
        public List<double[]> ProjectedCashflowsForEachOutgoingType { get; }
        public List<double[]> ProjectedRollForwardCashflowsForEachOutgoingType { get; }
        public double[] AggregatedCashflows { get; private set; }
        public double[] AggregatedRollForwardCashflows { get; private set; }

        public void RunProjector()
        {
            foreach (Inputs input in _listOfAllInputs)
            {
                Projector projector = new Projector(input);
                DiscounterContinuous discounter = new DiscounterContinuous(projector);

                double[] arrayOfDiscountedCashflows = discounter.GetDiscountedValue(projector.GetInflatedCostWuthDecrement());
                ProjectedCashflowsForEachOutgoingType.Add(arrayOfDiscountedCashflows);
            }

            double[] aggregatedCashflow = Aggregator.AggregateYearlyProjections(ProjectedCashflowsForEachOutgoingType, _listOfAllInputs);
            AggregatedCashflows = aggregatedCashflow;
        }

        public void RunRollForward()
        {
            foreach (Inputs input in _listOfAllInputs)
            {
                Projector projector = new Projector(input);
                RollForward rollForward = new RollForward(projector);

                double[] yearlyInflatedCashflowAtBaseDate = projector.GetInflatedCostWuthDecrement();

                double[] rollForwardProjections = rollForward.GetRollForwardProjections(projector.Inputs.Time, yearlyInflatedCashflowAtBaseDate);
                ProjectedRollForwardCashflowsForEachOutgoingType.Add(rollForwardProjections);
            }

            double[] aggregatedRollForwardCashflow = Aggregator.AggregateYearlyProjections(ProjectedRollForwardCashflowsForEachOutgoingType, _listOfAllInputs);
            AggregatedRollForwardCashflows = aggregatedRollForwardCashflow;
        }

        public double[] CalculateTotalInfWithDec()
        {
            List<double> sums = new List<double>();

            double[] cashFlow = new Projector(_listOfAllInputs.First()).GetInflatedCostWuthDecrement();

            int currentSumsLength = sums.Count;

            for (int i =0; i < cashFlow.Length; i++)
            {
                if (i > currentSumsLength-1)
                {
                    sums.Add(cashFlow[i]);
                }
                else
                {
                    sums[i] += cashFlow[i];
                }
            }

            double[] totalArray = sums.ToArray();

            return totalArray;
        }

        public double[] CalculateTotalInfCashflows()
        {
            List<double> sums = new List<double>();

            double[] cashFlow = new Projector(_listOfAllInputs.First()).GetInflatedCost();

            int currentSumsLength = sums.Count;

            for (int i = 0; i < cashFlow.Length; i++)
            {
                if (i > currentSumsLength - 1)
                {
                    sums.Add(cashFlow[i]);
                }
                else
                {
                    sums[i] += cashFlow[i];
                }
            }

            double[] totalArray = sums.ToArray();

            return totalArray;
        }
        
        public double[] CalculateTotalInfCashflows(bool withDecrement)
        {
            if (withDecrement)
            {
                List<double> sums = new List<double>();

                double[] cashFlow = new Projector(_listOfAllInputs.First()).GetInflatedCostWuthDecrement();

                int currentSumsLength = sums.Count;

                for (int i = 0; i < cashFlow.Length; i++)
                {
                    if (i > currentSumsLength - 1)
                    {
                        sums.Add(cashFlow[i]);
                    }
                    else
                    {
                        sums[i] += cashFlow[i];
                    }
                }

                double[] totalArray = sums.ToArray();

                return totalArray;
            }

            List<double> sumsNoDec = new List<double>();

            double[] cashFlowNoDec = new Projector(_listOfAllInputs.First()).GetInflatedCost();

            int currentSumsLengthNoDec = sumsNoDec.Count;

            for (int i = 0; i < cashFlowNoDec.Length; i++)
            {
                if (i > currentSumsLengthNoDec - 1)
                {
                    sumsNoDec.Add(cashFlowNoDec[i]);
                }
                else
                {
                    sumsNoDec[i] += cashFlowNoDec[i];
                }
            }

            double[] totalArrayNoDec = sumsNoDec.ToArray();

            return totalArrayNoDec;
        }
    }
}
