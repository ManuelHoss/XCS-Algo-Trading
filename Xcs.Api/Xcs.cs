using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xcs.Api;

namespace XCS.Api
{
    public class Xcs
    {
        private XcsParamerter _parameters;
        private List<Classifier> Population;
        private IEnvironment Environment;

        public Xcs(XcsParamerter parameters, IEnvironment environment)
        {
            _parameters = parameters;
            Population = new List<Classifier>();
            Environment = environment;
        }

        public void InitializeEnvironment()
        {

        }

        public void InitializeReinforcementComponent()
        {

        }

        public void InitializeXcs()
        {

        }

        public async Task RunExperiment(double reward)
        {
            char[] condition = await Environment.GetSituationAsync();
            List<Classifier> matchset = GenerateMatchset(condition);
            Dictionary<XcsAction, double> predictionArray = GeneratePredictionArray(matchset);
            XcsAction action = SelectAction(predictionArray);
            List<Classifier> actionset = GenerateActionSet(matchset, action);
            await Environment.ExecuteActionAsync(action);
            UpdateClassifierParameter();
            // TODO: Expand, because this is Single Step only!!
            UpdateSet(actionset, reward);
        }
        
        private List<Classifier> GenerateActionSet(List<Classifier> matchset, XcsAction action)
        {
            return matchset.Where(classifier => classifier.Action == action).ToList();
        }

        private XcsAction SelectAction(Dictionary<XcsAction, double> predictionArray)
        {
            if (new Random().NextDouble() < _parameters.Explorationrate)
            {
                return GetRandomXcsAction();
            }
            else
            {
                // TODO: Use Roulette Wheel Selection?!
                return predictionArray.FirstOrDefault(action => action.Value.Equals(predictionArray.Values.Max())).Key;
            }
        }

        private List<Classifier> GenerateMatchset(char[] condition)
        {
            List<Classifier> matchset = new List<Classifier>();
            foreach (var classifier in Population)
            {
                for (int i = 0; i < condition.Length; i++)
                {
                    if (condition[i] == classifier.Condition[i] ||
                        condition[i] == '#' || classifier.Condition[i] == '#')
                    {
                        matchset.Add(classifier);
                    }
                }

                // TODO: check if number of different actions is less then thetaMNA
            }
            return matchset;
        }

        private Classifier GenerateCoveringClassifier(List<Classifier> matchset, char[] condition)
        {
            Classifier cl = new Classifier();
            cl.Condition = condition;
            // Adding some generalization with wildcards
            for (int i = 0; i < condition.Length; i++)
            {
                if (new Random().NextDouble() < _parameters.WildcardProbability)
                {
                    cl.Condition[i] = '#';
                }
            }

            // Get random action not present in matchset [M]
            cl.Action = GetMissingActionsFrom(matchset)[new Random().Next(matchset.Count)];

            cl.Prediction = _parameters.Initial_p;
            cl.Epsilon = _parameters.Initial_epsilon;
            cl.Fitness = _parameters.Initial_F;
            cl.Experience = 0;
            cl.Ts = DateTime.Now;
            cl.As = 1;
            cl.Numerosity = 1;

            return cl;
        }
        
        private Dictionary<XcsAction, double> GeneratePredictionArray(List<Classifier> matchset)
        {
            // Initialize "Arrays"
            Dictionary<XcsAction, double> predictionArray = new Dictionary<XcsAction, double>();
            Dictionary<XcsAction, double> fitnessSumArray = new Dictionary<XcsAction, double>();
            foreach (var action in GetAllXcsActions())
            {
                predictionArray[action] = double.Epsilon;
                fitnessSumArray[action] = 0;
            }

            // Filling prediction Array and fitness sum array
            foreach (Classifier cl in matchset)
            {
                if (predictionArray[cl.Action].Equals(double.Epsilon))
                {
                    predictionArray[cl.Action] = cl.Prediction * cl.Fitness;
                }
                else
                {
                    predictionArray[cl.Action] += cl.Prediction * cl.Fitness;
                }
                fitnessSumArray[cl.Action] += cl.Fitness;
            }

            // Finalize prediction array
            foreach (var action in GetAllXcsActions())
            {
                if (fitnessSumArray[action] > 0)
                {
                    predictionArray[action] = predictionArray[action] / fitnessSumArray[action];
                }
            }

            return predictionArray;
        }

        // --------------- Update Methods ---------------
        // TODO: Expand, because this is Single Step only!!
        private void UpdateSet(List<Classifier> actionset, double reward)
        {
            foreach (Classifier classifier in actionset)
            {
                double sum = 0;
                foreach (var cl in actionset)
                {
                    sum += cl.Numerosity - cl.As;
                }

                classifier.Experience ++;
                // Update prediction (p)
                if (classifier.Experience < 1 / _parameters.Beta)
                {
                    // Update prediction (p)
                    classifier.Prediction += (reward - classifier.Prediction) / classifier.Experience;
                    // Update prediction error (epsilon)
                    classifier.Epsilon += (Math.Abs(reward - classifier.Prediction) - classifier.Epsilon) / classifier.Experience;
                    // Update action set size estimate classifier.as
                    classifier.As += (int)sum / classifier.Experience;
                }
                else
                {
                    // Update prediction (p)
                    classifier.Prediction += _parameters.Beta * (reward - classifier.Prediction);
                    // Update prediction error (epsilon)
                    classifier.Epsilon += _parameters.Beta * (Math.Abs(reward - classifier.Prediction) - classifier.Epsilon);
                    // Update action set size estimate classifier.as
                    classifier.As += (int)(_parameters.Beta * sum);
                }
            }
            UpdateFitness(actionset);

            if (_parameters.DoActionsetSubsumption)
            {
                throw new NotImplementedException();
            }
        }

        private void UpdateClassifierParameter()
        {
            throw new NotImplementedException();
        }

        private void UpdateFitness(List<Classifier> actionset)
        {
            double accuracySum = 0;
            double[] accuracyVector = new double[actionset.Count];

            // Fill accuracy vector and build accuracy sum
            for (int i = 0; i < accuracyVector.Length; i++)
            {
                if (actionset[i].Epsilon < _parameters.EpsilonZero)
                {
                    accuracyVector[i] = 1;
                }
                else
                {
                    accuracyVector[i] = _parameters.Alpha *
                                        Math.Pow((actionset[i].Epsilon / _parameters.EpsilonZero), -_parameters.V);
                }

                // Increase accuracy sum
                accuracySum += accuracyVector[i] * actionset[i].Numerosity;
            }

            // Update Fitness of each classifier in the actionset
            for (int i = 0; i < accuracyVector.Length; i++)
            {
                actionset[i].Fitness += _parameters.Beta * (accuracyVector[i] * actionset[i].Numerosity / accuracySum - actionset[i].Fitness);
            }
        }

        #region Helper methods

        private List<XcsAction> GetMissingActionsFrom(List<Classifier> matchset)
        {
            List<XcsAction> actions = GetAllXcsActions();

            foreach (var action in actions)
            {
                // Check if matchset contains a classifier with "action"
                if (matchset.Any(cl => cl.Action == action))
                {
                    actions.Remove(action);
                }
            }

            return actions;
        }

        private List<XcsAction> GetAllXcsActions()
        {
            return Enum.GetValues(typeof(XcsAction))
                            .Cast<XcsAction>()
                            .ToList();
        }

        private XcsAction GetRandomXcsAction()
        {
            return GetAllXcsActions()[new Random().Next(GetAllXcsActions().Count)];
        }
        


        #endregion
    }
}
