using System.Numerics;

namespace SharpDistributions;

public class ProbabilitySetElement<T, TProbability> : IProbabilitySetElement<T, TProbability> where TProbability : IFloatingPoint<TProbability>
{
    public T Element { get; set; }


    public Distribution<T,TProbability> Distribution { get; set; }

}