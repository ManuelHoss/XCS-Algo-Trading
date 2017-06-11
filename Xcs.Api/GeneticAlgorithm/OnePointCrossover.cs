using System.Collections.Generic;
using System.Linq;
using XCS.Api;

namespace Xcs.Api.GeneticAlgorithm
{
    public static class OnePointCrossover
    {
        public static void ApplyCrossover(Classifier childOne, Classifier childTwo)
        {
            Classifier[] classifiers = new Classifier[2];

            int crossoverPoint = Helper.RandomGenerator.Next(1, childOne.Condition.Length - 1);

            char[] childOneStart = new List<char>(childOne.Condition).GetRange(0, crossoverPoint-1).ToArray();
            char[] childTwoStart = new List<char>(childTwo.Condition).GetRange(0, crossoverPoint-1).ToArray();

            char[] childOneEnd = new List<char>(childOne.Condition).GetRange(crossoverPoint, childOne.Condition.Length).ToArray();
            char[] childTwoEnd = new List<char>(childTwo.Condition).GetRange(crossoverPoint, childTwo.Condition.Length).ToArray();
            
            childOne.Condition = childOneStart.Union(childOneEnd).ToArray();
            childTwo.Condition = childTwoStart.Union(childTwoEnd).ToArray();

            classifiers[0] = childOne;
            classifiers[1] = childTwo;
        }
    }
}
