using System;
using System.Collections.Generic;
using System.Linq;
using SharpDistributions;
using SharpDistributions.Sample01;

// Use directly of Sampling Functions
// We don't need Distribution extra facilities;
var b = Distributions.Bernoulli(0.3);
var count = 0;
for (var i = 0; i < 100000; i++)
{
    if (b.NextSample()) count++;
}
Console.WriteLine(count);

Console.WriteLine(Distributions.NormalExponential().Expectation(200));

var a = Army.ArmyElements().Take(20000).ToList();
            
var sa = a.OfType<SoldierTypeA>().Count();
var sb = a.OfType<SoldierTypeB>().Count();
var ta = a.OfType<TankA>().Count();
var tb = a.OfType<TankB>().Count();
           
Console.WriteLine($"Soldiers/Tanks {a.OfType<Soldier>().Count()/ (double)a.OfType<Tank>().Count()}");
Console.WriteLine($"Soldiers A({sa}) B({sb}), Tanks A({ta}) B({tb})");

namespace SharpDistributions.Sample01
{
    internal interface IArmyElement
    {
        
    }

    internal class Tank : IArmyElement { }

    internal class Soldier : IArmyElement { }

    internal class SoldierTypeA : Soldier
    {
    }

    internal class SoldierTypeB : Soldier
    {
    }

    internal class TankA : Tank
    {
    }

    internal class TankB : Tank
    {
    }

    internal class Army
    {
        public List<Tank> Tanks;
        public List<Soldier> Soldiers;

        public Army()
        {
            Tanks = new List<Tank>();
            Soldiers = new List<Soldier>();
        }
        public static Army Generate(int size)
        {
            var ret = new Army();
            var b = Distributions.Bernoulli(0.2);
            var tg = new Distribution<Tank>(TankSamplingFunction());
            var sg = new Distribution<Soldier>(SoldierSamplingFunction());
            for (var i = 0; i < size; i++)
            {
                if (b.NextSample())
                    ret.Tanks.Add(tg.NextSample());
                else
                    ret.Soldiers.Add(sg.NextSample());
            }
            return ret;
        }

        public static IEnumerable<IArmyElement> ArmyElements()
        {
            var sampler = ArmySamplingFunction();
            while (sampler.MoveNext())
            {
                yield return sampler.Current;
            }
        }

        public static IEnumerator<IArmyElement> ArmySamplingFunction()
        {
            var b = Distributions.Bernoulli(0.2);
            var tg = new Distribution<Tank>(TankSamplingFunction());
            var sg = new Distribution<Soldier>(SoldierSamplingFunction());
            while (true)
            {
                if (b.NextSample())
                    yield return tg.NextSample();
                else
                    yield return sg.NextSample();
            }
        }

        static IEnumerator<Tank> TankSamplingFunction()
        {
            return Distributions.Bernoulli(0.4).Select(b => b ? new TankA() : new TankB() as Tank).GetEnumerator();
        }
        static IEnumerator<Soldier> SoldierSamplingFunction()
        {
            return Distributions.Bernoulli(0.6).Select(b => b ? new SoldierTypeA() : new SoldierTypeB() as Soldier).GetEnumerator();
        }
    }
}