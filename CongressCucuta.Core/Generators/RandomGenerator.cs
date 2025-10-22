namespace CongressCucuta.Core.Generators;

public class RandomGenerator (int? seed = null) : IGenerator {
    private readonly Random _random = seed is int s ? new (s) : new ();

    public int Choose (int maximum) => maximum > 0 ? _random.Next (maximum) : throw new ArgumentException ("Maximum should be positive", nameof (maximum));

    public int Roll () => _random.Next (1, 7);
}
