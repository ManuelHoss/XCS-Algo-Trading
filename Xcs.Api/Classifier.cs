using System;
using System.Reflection;

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
        /// Indicator how many rounds the Classifier "survived"
        /// </summary>
        public long Ts { get; set; }
        /// <summary>
        /// Actionset size estimate
        /// </summary>
        public int As { get; set; }
        public int Numerosity { get; set; }
        
        public Classifier Clone(Classifier classifier)
        {
            return (Classifier) this.MemberwiseClone();
        }
    }
}