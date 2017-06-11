using System;
using System.Collections.Generic;
using System.Linq;
using XCS.Api;

namespace Xcs.Api.GeneticAlgorithm
{
    public static class GeneticAlgorithm
    {
        public static void RunGeneticAlgorithm(List<Classifier> actionset, char[] condition, List<Classifier> population)
        {
            // Calculate sum over all Classifier in [A] with Timestamp * Numerosity
            double sumTsNumerosity = 0 ;
            double sumNumerosity = 0;
            foreach (var classifier in actionset)
            {
                sumTsNumerosity += classifier.Ts * classifier.Numerosity;
                sumNumerosity += classifier.Numerosity;
            }
            if (DateTime.Now.Ticks - sumTsNumerosity / sumNumerosity < XcsParamerter.ThetaGA)
            {
                // Update Timestamp of classifiers in [A]
                foreach (var classifier in actionset)
                {
                    classifier.Ts = DateTime.Now.Ticks;
                }

                Classifier parentOne = RouletteWheelSelection.SelectOffspring(actionset);
                Classifier parentTwo = RouletteWheelSelection.SelectOffspring(actionset);

                Classifier childOne = parentOne.Clone(parentOne);
                Classifier childTwo = parentTwo.Clone(parentTwo);

                childOne.Numerosity = childTwo.Numerosity = 1;
                childOne.Experience = childTwo.Experience = 0;

                // Do crossover
                if (Helper.RandomGenerator.NextDouble() < XcsParamerter.Crossoverprobability)
                {
                    OnePointCrossover.ApplyCrossover(childOne, childTwo);
                    childOne.Prediction = childTwo.Prediction = (parentOne.Prediction + parentTwo.Prediction) / 2;
                    childOne.Epsilon = childTwo.Epsilon = (parentOne.Epsilon + parentTwo.Epsilon) / 2;
                    childOne.Fitness = childTwo.Epsilon = (parentOne.Fitness + parentTwo.Fitness) / 2;
                }

                childOne.Fitness = childOne.Fitness * 0.1;
                childTwo.Fitness = childTwo.Fitness * 0.1;

                List<Classifier> children = new List<Classifier>() {childOne, childTwo};

                foreach (var child in children)
                {
                    Mutation.ApplyMutation(child, condition);
                    if (XcsParamerter.DoGaSubsumption)
                    {
                        if (Subsumtion.DoesSubsume(parentOne, child))
                        {
                            parentOne.Numerosity++;
                        }
                        else if (Subsumtion.DoesSubsume(parentTwo, child))
                        {
                            parentTwo.Numerosity++;
                        }
                        else
                        {
                            InsertClassifierIntoPopulation(population, child);
                        }
                    }
                    else
                    {
                        InsertClassifierIntoPopulation(population, child);
                    }
                    DeleteFromPopulation(population);
                }
            }
        }

        private static void DeleteFromPopulation(List<Classifier> population)
        {
            if (population.Sum(item => item.Numerosity) < XcsParamerter.N)
            {
                return;
            }

            double averageFitnessInPopulation = population.Sum(item => item.Fitness) / population.Sum(item => item.Numerosity);
            double voteSum = 0;
            foreach (var classifier in population)
            {
                voteSum += DeletionVote(classifier, averageFitnessInPopulation);
            }
            double choicePoint = Helper.RandomGenerator.NextDouble() * voteSum;
            voteSum = 0;
            foreach (var classifier in population)
            {
                voteSum += DeletionVote(classifier, averageFitnessInPopulation);

                if (voteSum > choicePoint)
                {
                    if (classifier.Numerosity > 1)
                    {
                        classifier.Numerosity--;
                    }
                    else
                    {
                        population.Remove(classifier);
                    }
                    return;
                }
            }
        }

        private static void InsertClassifierIntoPopulation(List<Classifier> population, Classifier classifier)
        {
            foreach (var cl in population)
            {
                if (cl.Equals(classifier))
                {
                    cl.Numerosity++;
                    return;
                }
            }
            population.Add(classifier);
        }

        private static double DeletionVote(Classifier classifier, double averageFitnessInPopulation)
        {
            double vote = classifier.As * classifier.Numerosity;
            if (classifier.Experience > XcsParamerter.ThetaDelete &&
                ((classifier.Fitness / classifier.Numerosity) < XcsParamerter.Delta * averageFitnessInPopulation))
            {
                vote *= averageFitnessInPopulation / (classifier.Fitness / classifier.Numerosity);
            }
            return vote;
        }
    }
}
