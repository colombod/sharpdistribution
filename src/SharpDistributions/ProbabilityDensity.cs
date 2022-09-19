using System.Numerics;

namespace SharpDistributions;

/// <summary>
/// A probability function.
/// </summary>
/// <typeparam name="T">Source domain</typeparam>
/// <typeparam name="TProbability">The probability numeric type</typeparam>
/// <param name="p">The element</param>
/// <returns>The probability associated with the event.</returns>
public delegate TProbability ProbabilityDensity<in T, out TProbability>(T p) where TProbability : IFloatingPoint<TProbability>;