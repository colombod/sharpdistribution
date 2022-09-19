using System.Numerics;

namespace SharpDistributions;

public interface IProbabilitySetElement<T, TProbability> where TProbability : IFloatingPoint<TProbability>
{
    Distribution<T, TProbability> Distribution { get; set; }
}