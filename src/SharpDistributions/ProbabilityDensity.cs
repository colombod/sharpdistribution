namespace SharpDistributions;

/// <summary>
/// A probability function.
/// </summary>
/// <typeparam name="T">Source domain</typeparam>
/// <param name="p">The element</param>
/// <returns>The probability associated with the event.</returns>
public delegate double ProbabilityDensity<in T>(T p);