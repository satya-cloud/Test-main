using System;
using System.Collections.Generic;

namespace Test
{
    public class Projector
    {
        #region Properties
        public Inputs Inputs;//move the properties/variables to a business entities/models
        #endregion

        #region Constructor
        public Projector(Inputs inputs)
        {
            Inputs = inputs;
        }
        #endregion

        #region Public Methods
        public double[] GetInflatedCost()
        {   
            double[] projections = new double[Inputs.Time];// use list<double> instead of double[] array
            double factor = (Inputs.Inflation + 100) / 100;

            for (int i = 0; i < Inputs.Time; i++)
            {
                projections[i] = Inputs.Cost * GetProjectionFactor(factor, i);//change the list item insertion after changing the array as list
            }

            return projections;
        }
        
        public double[] GetInflatedCostWuthDecrement()
        {
            double[] projections = GetInflatedCost();//user list

            double[] projectionsWithDecrement = new double[Inputs.Time];//use list
            double factor = (100 + Inputs._annualChangeInYearlyPayments)/100;

            for (int i = 0; i < projections.Length; i++)//use foreach instead of for
            {
                projectionsWithDecrement[i] = projections[i] * Math.Pow(factor, i + 1);//change the item insertion into list after changing the array as a list
            }

            return projectionsWithDecrement;
        }

        public void UpdateInflatedCostWithDecrement(double[] salaryProjectionsWithoutDecrement)//convert array into list
        {
            for (int i = 0; i < Inputs.Time; i++)
            {
                salaryProjectionsWithoutDecrement[i] *= Inputs.GetAvgPercentOfAnnualChangeInPayments(i);//see if we can replace this loop with extention method
            }
        }
        #endregion

        #region Private Methods
        private static double GetProjectionFactor(double factor, int cashFlowYear)
        {
            return (Math.Pow(factor, cashFlowYear));
        }
        #endregion
    }
}