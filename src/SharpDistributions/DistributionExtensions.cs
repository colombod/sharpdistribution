using System;
using System.Linq;
using System.Numerics;

namespace SharpDistributions;

public static class DistributionExtensions
{
    public static double Expectation<T>(this Distribution<T> source, int samplesCount) where T : INumber<T>,IDivisionOperators<T,double,double>
    {
        double ret = 0;
        double n =  samplesCount;
        ProbabilityDensity <T> density = source.Density;
        var samples = source.Take(samplesCount);
        foreach(var sample in samples)
        {
            var prob = sample;
            if (density is { })
            {
                ret += density(prob) / n;
            }
            else
            {
                ret += prob / n;
            }
        }
        
        return ret;
    }

    public static double GeneralExpectation<T>(this Distribution<T> source, int samplesCount)
    {
        double ret = 0;
        double n = samplesCount;
        ProbabilityDensity<T> density = source.Density;

        if (density is null)
        {
            throw new InvalidOperationException();
        }
        var samples = source.Take(samplesCount);
        foreach (var sample in samples)
        {
            var prob = sample;  
            ret += density(prob) / n;            
        }

        return ret;
    }
}