using System;
using System.Collections.Generic;

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
public class Distribution<T> : IEnumerable<T>
{
    /// <summary>
    /// Sample function corresponding to this distribution.
    /// </summary>
    private readonly IEnumerator<T> _samplingFunction;

    /// <summary>
    /// Probability function. If null the identity is assumed.
    /// </summary>
    private readonly ProbabilityDensity<T> _density;

    /// <summary>
    /// Sampling function associated with the distribution.
    /// </summary>
    public IEnumerator<T> SamplingFunction => _samplingFunction;

    /// <summary>
    /// Return the density function to be used.
    /// </summary>
    public ProbabilityDensity<T> Density => _density;

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
    public Distribution(IEnumerator<T> samplingFunction, ProbabilityDensity<T> d)
    {
        _density = d;
        _samplingFunction = samplingFunction;
        _samplingFunction.MoveNext();
    }
    
    /// <summary>
    /// Return the next sample from the distribution.
    /// </summary>
    /// <returns>A sample from the distribution.</returns>
    public T NextSample()
    {
        _samplingFunction.MoveNext();
        return _samplingFunction.Current;
    }

    /// <summary>
    /// Return the current sample from the sampling function.
    /// </summary>
    public T Sample => _samplingFunction.Current;

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
        return _samplingFunction;
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
        return _samplingFunction;
    }

    #endregion
}
