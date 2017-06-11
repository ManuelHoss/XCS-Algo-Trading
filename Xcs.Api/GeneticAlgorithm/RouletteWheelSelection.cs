using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCS.Api;

namespace Xcs.Api.GeneticAlgorithm
{
    /// <summary>
    ///     Implementation of Roulette Wheel Selection
    ///     <see href="http://en.wikipedia.org/wiki/Fitness_proportionate_selection">Wikipedia</see>
    /// </summary>
    public static class RouletteWheelSelection
    {
        public static Classifier SelectOffspring(List<Classifier> actionset)
        {
            try
            {
                double fitnessSum = 0;
                foreach (var classifier in actionset)
                {
                    fitnessSum += classifier.Fitness;
                }

                double crossoverPoint = Helper.RandomGenerator.NextDouble() * fitnessSum;

                fitnessSum = 0;
                foreach (var classifier in actionset)
                {
                    fitnessSum += classifier.Fitness;
                    if (fitnessSum > crossoverPoint)
                    {
                        return classifier;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Roulette Wheel Selection failed! - {ex.Message}");
                throw;
            }
            
        }
    }
}