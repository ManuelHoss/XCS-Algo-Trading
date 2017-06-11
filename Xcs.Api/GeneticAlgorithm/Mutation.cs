using System;
using System.Collections.Generic;
using System.Linq;
using XCS.Api;

namespace Xcs.Api.GeneticAlgorithm
{
    public static class Mutation
    {
        public static void ApplyMutation(Classifier classifier, char[] condition)
        {
            for (int i = 0; i < classifier.Condition.Length; i++)
            {
                if (Helper.RandomGenerator.NextDouble() < XcsParamerter.MutationProbability)
                {
                    if (classifier.Condition[i] == '#')
                    {
                        classifier.Condition[i] = condition[i];
                    }
                    else
                    {
                        classifier.Condition[i] = '#';
                    }
                }
            }

            if (Helper.RandomGenerator.NextDouble() < XcsParamerter.MutationProbability)
            {
                classifier.Action = GetRandomXcsAction();
            }
        }

        private static XcsAction GetRandomXcsAction()
        {
            List<XcsAction> actions = Enum.GetValues(typeof(XcsAction))
                .Cast<XcsAction>()
                .ToList();
            return actions[new Random().Next(actions.Count)];
        }
    }
}
