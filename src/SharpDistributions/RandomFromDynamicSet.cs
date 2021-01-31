namespace SharpDistributions
{
    /// <summary>
    /// The Dynamic Set is defined by the population Size
    /// Every time the GetNext is called the population is reduced and the 
    /// distribution updated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RandomFromDynamicSet<T> : RandomFromStaticSet<T> where T : IProbabilitySetElement<T>
    {
        public RandomFromDynamicSet(int populationSize, params T[] elements)
            : base(elements)
        {
            PopulationSize = populationSize;
        }

        public int PopulationSize { get; set; }

        /// <summary>
        /// If the populationSize is 0
        /// null is returned
        /// </summary>
        /// <returns></returns>
        public override object[] GetNext()
        {
            if (PopulationSize <= 0)
            {
                return null;
            }

            return null;
        }
    }
}