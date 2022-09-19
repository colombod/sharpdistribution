using System;
using System.Collections.Generic;
using System.Numerics;

namespace SharpDistributions;

public class RandomFromStaticSet<T, TProbability> where T : IProbabilitySetElement<T, TProbability>
where TProbability : IFloatingPoint<TProbability>
{
    protected List<T> Elements;

    public RandomFromStaticSet(params T[] elements)
    {
        Elements = new List<T>();
        Elements.AddRange(elements);
        NormalizeSet();
    }

    public void NormalizeSet()
    {
        var total = 0.0;
        foreach (var element in Elements)
        {
            //total += element.Distribution;
        }
        if (Math.Abs(total - 1.0) > double.Epsilon)
        {
            foreach (T element in Elements)
            {
                //element.Distribution /= total;
            }
        }
    }

    public virtual object[] GetNext()
    {
        var max_prob = 0.0;
        var selection = default(T);
        foreach (T element in Elements)
        {
            var prob = element.Distribution.NextSample();
            //if (prob > max_prob)
            //{
            //    selection = element;
            //    max_prob = prob;
            //}
        }

        return null;
    }
}