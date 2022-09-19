using System;
using System.Linq;
using System.Numerics;

namespace SharpDistributions;

public static class DistributionExtensions
{
    public static double Expectation<T>(this Distribution<T,double> source, int samplesCount)
        where T : INumber<T>, IDivisionOperators<T, double, double>
    {
       return source.Expectation<T,double>(samplesCount); 
    }
    public static TProbability Expectation<T, TProbability>(this Distribution<T, TProbability> source, int samplesCount)
        where TProbability :  IFloatingPoint<TProbability>
        where T : INumber<T>,IDivisionOperators<T, TProbability, TProbability>
    {
        if (source.Density is { })
        {
            return source.ExpectationUsingDensityFunction<T, TProbability>(samplesCount);
        }

        var n = TProbability.CreateSaturating( samplesCount);
        var samples = source.Take(samplesCount);
        return samples.Aggregate(TProbability.Zero, (current, sample) => (current + sample / n));
    }


    public static TProbability ExpectationUsingDensityFunction<T, TProbability>(this Distribution<T, TProbability> source, int samplesCount)
        where TProbability : IFloatingPoint<TProbability>
    {
        var density = source.Density ?? throw new ArgumentNullException("source.Density");
        var n = TProbability.CreateSaturating(samplesCount);

        var samples = source.Take(samplesCount).Select(s => TProbability.CreateSaturating(density(s)));
        return samples.Aggregate(TProbability.Zero, (current, sample) => (current + sample / n));
    }

    public static double ExpectationUsingDensityFunction<T>(this Distribution<T, double> source, int samplesCount)
    {
       return source.ExpectationUsingDensityFunction<T,double>(samplesCount); 
    }
}
