using System;
using System.Linq;
using System.Runtime.CompilerServices;
using XCS.Api;

namespace Xcs.Api.GeneticAlgorithm
{
    /// <summary>
    /// Subsumtion implemented according to http://opim.wharton.upenn.edu/~sok/papers/b/XCSAlgDesc01202001.pdf
    /// </summary>
    public static class Subsumtion
    {
        public static bool DoesSubsume(Classifier subsumingClassifier, Classifier classifierToSubsume)
        {
            if (subsumingClassifier.Action == classifierToSubsume.Action)
            {
                if (CouldSubsume(subsumingClassifier))
                {
                    if (IsMoreGeneral(subsumingClassifier, classifierToSubsume))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool CouldSubsume(Classifier classifier)
        {
            if (classifier.Experience > XcsParamerter.ThetaSubsumption)
            {
                if (classifier.Epsilon < XcsParamerter.EpsilonZero)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsMoreGeneral(Classifier generalClassifier, Classifier specificClassifier)
        {
            if (generalClassifier.Condition.Count(c => c == '#') <= specificClassifier.Condition.Count(c => c == '#'))
            {
                return false;
            }

            for (int i = 0; i < generalClassifier.Condition.Length; i++)
            {
                if (generalClassifier.Condition[i] != '#' &&
                    generalClassifier.Condition[i] != specificClassifier.Condition[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
