using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SharpDistributions;

/// <summary>
/// This class defines a set of classic distributions.
/// </summary>
[SuppressMessage("ReSharper", "IteratorNeverReturns")]
[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "By Design")]
public class Distributions
{
    /// <summary>
    /// This is the unit of the sampling functions. This is hidden inside the class
    /// to ensure that is not reset. This ensures that all the sampling functions
    /// will use the same source of randomness.
    /// </summary>
    private static readonly Distribution<double> Unit;

    /// <summary>
    /// Static constructor. It prepares the internal generator.
    /// </summary>
    static Distributions()
    {
        Unit = new Distribution<double>(CoreUniformFunction());
    }

    /// <summary>
    /// Definition of the core uniform function used to generate the samples.
    /// </summary>
    /// <returns>The unit sampling function</returns>
    private static IEnumerator<double> CoreUniformFunction()
    {
        var rnd = new Random();
        while (true)
            yield return rnd.NextDouble();
    }

    #region Sampling Functions
    /// <summary>
    /// The uniform sampling function between 0 and 1.
    /// </summary>
    /// <returns>The unit sampling function</returns>
    public static IEnumerator<double> UnitUniformFunction()
    {
        // Note that we define a new enumeration to ensure that the uniform
        // distribution is untouched.
        return Unit.GetEnumerator();
    }

    /// <summary>
    /// Sampling function defining the classic binomial distribution.
    /// </summary>
    /// <param name="p">Probability of the Bernoulli used</param>
    /// <param name="n0">Number of trials</param>
    /// <returns>The binomial sampling function</returns>
    public static IEnumerator<double> BinomialFunction(double p, int n0)
    {
        var bernoulli = Bernoulli(p);
        while (true)
        {
            double ret = 0;
            for (int i = 0; i < n0; i++)
                if (bernoulli.NextSample()) ret++;
            yield return ret;
            ret = 0;
        }
    }

    /// <summary>
    /// Uniform sampling function for values in the given interval.
    /// </summary>
    /// <param name="a">Minimum value of the interval.</param>
    /// <param name="b">Maximum value of the interval.</param>
    /// <returns>The Uniform sampling function.</returns>
    public static IEnumerator<double> UniformFunction(double a, double b)
    {
        foreach (double v in Unit)
            yield return a + v * (b - a);
    }

    /// <summary>
    /// A point uniform sampling function.
    /// <remarks>
    /// Included only as a sampling function that cannot be expressed by
    /// means of a discrete or continuous distribution
    /// </remarks>
    /// </summary>
    /// <returns>The point uniform sampling function.</returns>
    public static IEnumerator<double> PointUniformFunction()
    {
        foreach (double v in Unit)
            yield return v < 0.5 ? 0.0 : v;
    }

    /// <summary>
    /// Sampling function defining the Bernoulli distribution.
    /// </summary>
    /// <param name="p">Probability of the Bernoulli toss.</param>
    /// <returns>The Bernoulli sampling function.</returns>
    public static IEnumerator<bool> BernoulliFunction(double p)
    {
        return Unit.Select(v => v <= p).GetEnumerator();
    }

    /// <summary>
    /// Normal exponential sampling function.
    /// </summary>
    /// <returns>The normal exponential sampling function.</returns>
    public static IEnumerator<double> NormalExponentialFunction()
    {
        return Unit.Select(v => -Math.Log(v)).GetEnumerator();
    }

    /// <summary>
    /// Gaussian sampling function as defined by Box and Mueller.
    /// </summary>
    /// <param name="mean">Mean of the distribution</param>
    /// <param name="variance">Variance of the distribution.</param>
    /// <returns></returns>
    public static IEnumerator<double> GaussianBoxMuellerFunction(double mean, double variance)
    {
        while (true)
        {
            var u = Unit.NextSample();
            var v = Unit.NextSample();
            yield return mean + variance * Math.Sqrt(-2.0 * Math.Log(u)) * Math.Cos(2.0 * Math.PI * v);
        }
    }

    /// <summary>
    /// A sampling function for Gaussian distribution based on the central limit theorem.
    /// </summary>
    /// <param name="mean">Mean of the distribution.</param>
    /// <param name="variance">Variance of the distribution.</param>
    /// <returns>The gaussian distribution.</returns>
    public static IEnumerator<double> GaussianCentralFunction(double mean, double variance)
    {
        while (true)
        {
            var acc = -6.0;
            for (var i = 0; i < 12; i++)
            {
                acc += Unit.NextSample();
            }
            yield return mean + variance * acc;
        }
    }

    /// <summary>
    /// The Geometric sampling function.
    /// </summary>
    /// <param name="p">The probability of the Bernoulli used.</param>
    /// <returns>The geometric sampling function.</returns>
    public static IEnumerator<double> GeometricFunction(double p)
    {
        var bernoulli = Distributions.Bernoulli(p);
        while (true)
        {
            var ret = 0.0;
            while (bernoulli.NextSample())
            {
                ret++;
            }
            yield return ret;
        }
    }

    /// <summary>
    /// The gaussian sampling function defined as rejection.
    /// </summary>
    /// <param name="mean">Mean of the distribution.</param>
    /// <param name="variance">Variance of the distribution.</param>
    /// <returns>The gaussian distribution.</returns>
    public static IEnumerator<double> GaussianRejectionFunction(double mean, double variance)
    {
        var exp = NormalExponential();
        var bernoulliFair = Bernoulli(0.5);
        while (true)
        {
            var y1 = exp.NextSample();
            var y2 = exp.NextSample();
            if (y2 >= ((y1 - 1.0) * (y1 - 1.0)) / 2.0)
            {
                yield return mean + (bernoulliFair.NextSample() ? variance * y1 : -variance * y1);
            }
        }
    }

    /// <summary>
    /// A Bayes sampling function of a generic domain A. A probability function
    /// is required in order to define the probability of each element.
    /// </summary>
    /// <typeparam name="A">The domain of values.</typeparam>
    /// <param name="p">The probability function over A</param>
    /// <param name="c">Normalization factor (so that p can map over R+)</param>
    /// <param name="q">The elements to select from.</param>
    /// <returns>A sampling function over the given domain.</returns>
    public static IEnumerator<A> BayesRejectionFunction<A>(ProbabilityDensity<A> p, double c, IEnumerator<A> q)
    {
        foreach (var v in Unit)
        {
            if (v < (p(q.Current) / c))
            {
                yield return q.Current;
            }
            q.MoveNext();
        }
    }
    #endregion

    #region Distributions
    /// <summary>
    /// Unit uniform distribution.
    /// </summary>
    /// <returns>A distribution object that represents the unit distribution.</returns>
    public static Distribution<double> UnitUniform()
    {
        return new Distribution<double>(UnitUniformFunction());
    }

    /// <summary>
    /// Binomial distribution.
    /// </summary>
    /// <param name="p">Probability of success of each toss.</param>
    /// <param name="n0">Number of tosses.</param>
    /// <returns>A distribution object that represents the Binomial distribution.</returns>
    public static Distribution<double> Binomial(double p, int n0)
    {
        return new Distribution<double>(BinomialFunction(p, n0));
    }

    /// <summary>
    /// Uniform distribution over an interval[a, b].
    /// </summary>
    /// <param name="a">Minimum value of the interval.</param>
    /// <param name="b">Maximum value of the interval.</param>
    /// <returns>A distribution object that represents the Uniform distribution.</returns>
    public static Distribution<double> Uniform(double a, double b)
    {
        return new Distribution<double>(UniformFunction(a, b));
    }

    /// <summary>
    /// This distribution is built on top of the PointUniformFunction sampling function.
    /// </summary>
    /// <returns>A distribution object that represents the point uniform distribution.</returns>
    public static Distribution<double> PointUniform()
    {
        return new Distribution<double>(PointUniformFunction());
    }

    /// <summary>
    /// Bernoulli distribution.
    /// </summary>
    /// <param name="p">Probability of a win</param>
    /// <returns>A distribution object that represents the Bernoulli distribution.</returns>
    public static Distribution<bool> Bernoulli(double p)
    {
        return new Distribution<bool>(BernoulliFunction(p));
    }

    /// <summary>
    /// Normal exponential distribution.
    /// </summary>
    /// <returns>A distribution object that represents the normal exponential distribution.</returns>
    public static Distribution<double> NormalExponential()
    {
        return new Distribution<double>(NormalExponentialFunction());
    }

    /// <summary>
    /// Gaussian distribution based on GaussianBoxMuellerFunction sampling function.
    /// </summary>
    /// <param name="mean">Mean of the distribution</param>
    /// <param name="variance">Variance of the distribution.</param>
    /// <returns>A distribution object that represents the Gaussian distribution.</returns>
    public static Distribution<double> GaussianBoxMueller(double mean, double variance)
    {
        return new Distribution<double>(GaussianBoxMuellerFunction(mean, variance));
    }

    /// <summary>
    /// Gaussian distribution based on GaussianCentralFunction sampling function.
    /// </summary>
    /// <param name="mean">Mean of the distribution</param>
    /// <param name="variance">Variance of the distribution.</param>
    /// <returns>A distribution object that represents the Gaussian distribution.</returns>
    public static Distribution<double> GaussianCentral(double mean, double variance)
    {
        return new Distribution<double>(GaussianCentralFunction(mean, variance));
    }

    /// <summary>
    /// Geometric distribution.
    /// </summary>
    /// <param name="p">Probability of success of each toss.</param>
    /// <returns>A distribution object that represents the Geometric distribution.</returns>
    public static Distribution<double> Geometric(double p)
    {
        return new Distribution<double>(GeometricFunction(p));
    }

    /// <summary>
    /// Gaussian distribution based on GaussianBoxMuellerFunction sampling function.
    /// </summary>
    /// <param name="mean">Mean of the distribution</param>
    /// <param name="variance">Variance of the distribution.</param>
    /// <returns>A distribution object that represents the Gaussian distribution.</returns>
    public static Distribution<double> GaussianRejection(double mean, double variance)
    {
        return new Distribution<double>(GaussianRejectionFunction(mean, variance));
    }

    /// <summary>
    /// Bayes distribution based on the rejection method.
    /// </summary>
    /// <typeparam name="TDomain">The domain of values.</typeparam>
    /// <param name="p">The probability function over Domain</param>
    /// <param name="c">Normalization factor (so that p can map over R+)</param>
    /// <param name="q">The elements to select from.</param>
    /// <returns>A distribution object that represents the Bayes distribution over a domain A.</returns>
    public static Distribution<TDomain> BayesRejection<TDomain>(ProbabilityDensity<TDomain> p, double c, IEnumerator<TDomain> q)
    {
        return new Distribution<TDomain>(BayesRejectionFunction(p, c, q));
    }
    #endregion

}