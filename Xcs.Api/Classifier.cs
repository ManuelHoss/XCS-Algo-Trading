using System;

namespace XCS.Api
{
    public class Classifier
    {
        public char[] Condition { get; set; }
        public XcsAction Action { get; set; }
        /// <summary>
        /// Prediction
        /// </summary>
        public double Prediction { get; set; }
        /// <summary>
        /// Prediction error
        /// </summary>
        public double Epsilon { get; set; }
        public double Fitness { get; set; }
        public int Experience { get; set; }
        /// <summary>
        /// Actual time
        /// </summary>
        public DateTime Ts { get; set; }
        /// <summary>
        /// Actionset size estimate
        /// </summary>
        public int As { get; set; }
        public int Numerosity { get; set; }
    }
}