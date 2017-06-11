namespace Xcs.Api
{
    public static class XcsParamerter
    {
        /// <summary>
        /// PopulationSize
        /// </summary>
        public static int N { get; set; }

        /// <summary>
        /// LearningRate for p, epsilon, f and as.
        /// Common value range: 0.1 - 0.2
        /// </summary>
        public static double Beta { get; set; }

        /// <summary>
        /// Paramerter for fitness calculation
        /// </summary>
        public static double Alpha { get; set; }
        /// <summary>
        /// Paramerter for fitness calculation. (Predictionerror)
        /// </summary>
        public static double EpsilonZero { get; set; }
        /// <summary>
        /// Paramerter for fitness calculation.
        /// Common value: 5
        /// </summary>
        public static double V { get; set; }

        /// <summary>
        /// Discount factor for multistep problems.
        /// Common value: 0.71
        /// </summary>
        public static double Gamma { get; set; }

        /// <summary>
        /// Genetic Algorithm threshold.
        /// Common value range: 25 - 50
        /// </summary>
        public static int ThetaGA { get; set; }

        /// <summary>
        /// Crossoverprobability.
        /// Common value range: 0.5 - 1.0
        /// </summary>
        public static double Crossoverprobability { get; set; }

        /// <summary>
        /// Probability of mutating an allele in the offspring.
        /// Common value range: 0.01 - 0.05
        /// </summary>
        public static double MutationProbability { get; set; }

        /// <summary>
        /// Deletion threshold.
        /// Common value: 20
        /// </summary>
        public static int ThetaDelete { get; set; }

        /// <summary>
        /// Mean fitness value of the population [P].
        /// Common value: 0.1
        /// </summary>
        public static double Delta { get; set; }

        /// <summary>
        /// Subsumtion threshold.
        /// Common value: 20
        /// </summary>
        public static double ThetaSubsumption { get; set; }

        /// <summary>
        /// Probability of using a wildcard in one attribute of an Classifier.
        /// Common value: 0.33 (Larger values reduce the need for covering.)
        /// </summary>
        public static double WildcardProbability { get; set; }

        /// <summary>
        /// p used when creating a new Classifier.
        /// Common values: very small, eventually zero.
        /// </summary>
        public static double Initial_p { get; set; }
        /// <summary>
        /// Epsilon value when creating a new Classifier.
        /// Common values: very small, eventually zero.
        /// </summary>
        public static double Initial_epsilon { get; set; }
        /// <summary>
        /// Fitness value wenn creating a new Classifier.
        /// Common values: very small, eventually zero.
        /// </summary>
        public static double Initial_F { get; set; }

        /// <summary>
        /// Probability of choosing a random action during action selection.
        /// Common values: 0.5 (Very dependant on the problem.)
        /// </summary>
        public static double Explorationrate { get; set; }

        /// <summary>
        /// Minimum number of actions, that have to be present for each Condition in the Matchset [M].
        /// Common value: Equal to the number of available actions.
        /// </summary>
        public static int ThetaMna { get; set; }

        /// <summary>
        /// Boolean parameter that specifies if offsprings are to be tested for possible logical subsumption by parents.
        /// </summary>
        public static bool DoGaSubsumption { get; set; }

        /// <summary>
        /// Boolean parameter that specifies if Actionsets are to be tested for subsuming Classifiers.
        /// </summary>
        public static bool DoActionsetSubsumption { get; set; }
    }
}
