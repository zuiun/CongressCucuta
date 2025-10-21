namespace CongressCucuta.Core.Generators;

public class RandomGenerator : IGenerator {
    private readonly Random _random = new ();

    public int Choose (int maximum) => maximum > 0 ? _random.Next (maximum) : -1;

    public int Roll () => _random.Next (1, 7);
}
