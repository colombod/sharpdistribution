using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpDistributions
{
    /// <summary>
    /// A probability function.
    /// </summary>
    /// <typeparam name="T">Source domain</typeparam>
    /// <param name="p">The element</param>
    /// <returns>The probability associated with the event.</returns>
    public delegate double Prob<T>(T p);

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
        private readonly Prob<T> _density;

        /// <summary>
        /// Sampling function associated with the distribution.
        /// </summary>
        public IEnumerator<T> SamplingFunction => _samplingFunction;

        /// <summary>
        /// Return the density function to be used.
        /// </summary>
        public Prob<T> Density => _density;

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
        public Distribution(IEnumerator<T> samplingFunction, Prob<T> d)
        {
            _density = d;
            _samplingFunction = samplingFunction;
            _samplingFunction.MoveNext();
        }
        
        public double Expectation(int n)
        {
            double ret = 0;

            for (int i = 0; i < n; i++)
            {
                if (_density == null)
                    ret += (double)(object)_samplingFunction.Current;
                else
                    ret += _density(_samplingFunction.Current);
                _samplingFunction.MoveNext();
            }
            return ret / n;
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

    /// <summary>
    /// This class defines a set of classic distributions.
    /// </summary>
    public class Distributions
    {
        /// <summary>
        /// This is the unit of the sampling functions. This is hidden inside the class
        /// to ensure that is not reset. This ensures that all the sampling functions
        /// will use the same source of randomness.
        /// </summary>
        private static Distribution<double> unit;

        /// <summary>
        /// Static constructor. It prepares the internal generator.
        /// </summary>
        static Distributions()
        {
            unit = new Distribution<double>(CoreUniformFunction());
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
            return unit.GetEnumerator();
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
            foreach (double v in unit)
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
            foreach (double v in unit)
                yield return v < 0.5 ? 0.0 : v;
        }

        /// <summary>
        /// Sampling function defining the Bernoulli distribution.
        /// </summary>
        /// <param name="p">Probability of the Bernoulli toss.</param>
        /// <returns>The Bernoulli sampling function.</returns>
        public static IEnumerator<bool> BernoulliFunction(double p)
        {
            return unit.Select(v => v <= p).GetEnumerator();
        }

        /// <summary>
        /// Normal exponential sampling function.
        /// </summary>
        /// <returns>The normal exponential sampling function.</returns>
        public static IEnumerator<double> NormalExponentialFunction()
        {
            return unit.Select(v => -Math.Log(v)).GetEnumerator();
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
                var u = unit.NextSample();
                var v = unit.NextSample();
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
                    acc += unit.NextSample();
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
        public static IEnumerator<A> BayesRejectionFunction<A>(Prob<A> p, double c, IEnumerator<A> q)
        {
            foreach (var v in unit)
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
        /// <typeparam name="A">The domain of values.</typeparam>
        /// <param name="p">The probability function over A</param>
        /// <param name="c">Normalization factor (so that p can map over R+)</param>
        /// <param name="q">The elements to select from.</param>
        /// <returns>A distribution object that represents the Bayes distribution over a domain A.</returns>
        public static Distribution<A> BayesRejection<A>(Prob<A> p, double c, IEnumerator<A> q)
        {
            return new Distribution<A>(BayesRejectionFunction<A>(p, c, q));
        }
        #endregion

    }
}
