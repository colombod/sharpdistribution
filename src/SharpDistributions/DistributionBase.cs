using System.Collections.Generic;
using System.Numerics;

namespace SharpDistributions;

/// <summary>
/// This class represents a distribution of objects of a given domain.
/// A sampling function is represented by an enumerator, a distribution
/// is based on a sampling function and provides more services than a
/// simple enumerator. A distribution can be used with the foreach construct
/// of C#, and provides additional services.
/// <example>
/// Distribution&lt;double&gt; = new Distribution&lt;double&gt;(Distributions.BernoulliDistribution(0.5));
/// </example>
/// </summary>
public class Distribution<T,TProbability> : IEnumerable<T>
where TProbability : IFloatingPoint<TProbability>
{
    /// <summary>
    /// Sampling function associated with the distribution.
    /// </summary>
    public IEnumerator<T> SamplingFunction { get; }

    /// <summary>
    /// Return the density function to be used.
    /// </summary>
    public ProbabilityDensity<T, TProbability> Density { get; }

    /// <summary>
    /// It builds a distribution given a sampling function.
    /// </summary>
    /// <param name="samplingFunction">Sampling function that defines the distribution.</param>
    /// <param name="d">
    /// Probability function associated with the elements (if known).
    /// If null the identity distribution is assumed.
    /// </param>
    public Distribution(IEnumerator<T> samplingFunction) : this(samplingFunction, null) {}

    /// <summary>
    /// It builds a distribution given a sampling function.
    /// </summary>
    /// <param name="samplingFunction">Sampling function that defines the distribution.</param>
    /// <param name="d">
    /// Probability function associated with the elements (if known).
    /// If null the identity distribution is assumed.
    /// </param>
    public Distribution(IEnumerator<T> samplingFunction, ProbabilityDensity<T, TProbability> d)
    {
        Density = d;
        SamplingFunction = samplingFunction;
        SamplingFunction.MoveNext();
    }
    
    /// <summary>
    /// Return the next sample from the distribution.
    /// </summary>
    /// <returns>A sample from the distribution.</returns>
    public T NextSample()
    {
        SamplingFunction.MoveNext();
        return SamplingFunction.Current;
    }

    /// <summary>
    /// Return the current sample from the sampling function.
    /// </summary>
    public T Sample => SamplingFunction.Current;

    #region IEnumerable<T> Members

    /// <summary>
    /// Return the sampling function.
    /// <remarks>
    /// The enumerator is always the same object since the
    /// sampling function is infinite. Also resetting the returned enumerator
    /// does not provide any benefit or effect.
    /// </remarks>
    /// </summary>
    /// <returns>The enumerator associated with the sampling function.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        return SamplingFunction;
    }

    #endregion

    #region IEnumerable Members

    /// <summary>
    /// Return the enumerator associated with the sampling function.
    /// <see>GetEnumerator</see>
    /// </summary>
    /// <returns>The enumerator associated with the sampling function.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return SamplingFunction;
    }

    #endregion
}
