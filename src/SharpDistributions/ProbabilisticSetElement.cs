namespace SharpDistributions
{
    public interface IProbabilitySetElement<T>
    {
        Distribution<T> Distribution { get; set; }
    }

    public class ProbabilitySetElement<T> : IProbabilitySetElement<T>
    {
        public T Element { get; set; }


        public Distribution<T> Distribution { get; set; }

    }
}