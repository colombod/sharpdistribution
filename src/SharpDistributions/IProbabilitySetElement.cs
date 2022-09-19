namespace SharpDistributions;

public interface IProbabilitySetElement<T>
{
    Distribution<T> Distribution { get; set; }
}