using System.Diagnostics;

namespace MathCalc.Logic.Internal
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class Function(string name, int countAttributes, Func<List<double>, double> action)
    {
        private static readonly Random Random = new Random();

        public static readonly Function[] Functions = [.. (new Function[]
        {
            new ("sqrt", 1, attrs => Math.Sqrt(attrs[0])),
            new ("pow", 2, attrs => Math.Pow(attrs[0], attrs[1])),
            new ("pi", 0, attrs => Math.PI),
            new ("log", 1, attrs => Math.Log(attrs[0])),
            new ("log", 2, attrs => Math.Log(attrs[0], attrs[1])),
            new ("log2", 1, attrs => Math.Log2(attrs[0])),
            new ("log10", 1, attrs => Math.Log10(attrs[0])),
            new ("min", 2, attrs => Math.Min(attrs[0], attrs[1])),
            new ("max", 2, attrs => Math.Max(attrs[0], attrs[1])),
            new ("floor", 1, attrs => Math.Floor(attrs[0])),
            new ("roundd", 1, attrs => Math.Floor(attrs[0])),
            new ("ceil", 1, attrs => Math.Ceiling(attrs[0])),
            new ("roundu", 1, attrs => Math.Ceiling(attrs[0])),
            new ("round", 1, attrs => Math.Round(attrs[0])),
            new ("abs", 1, attrs => Math.Abs(attrs[0])),
            new ("sin", 1, attrs => Math.Sin(attrs[0])),
            new ("cos", 1, attrs => Math.Cos(attrs[0])),
            new ("tan", 1, attrs => Math.Tan(attrs[0])),
            new ("tan", 1, attrs => Math.Tan(attrs[0])),
            new ("rnd", 0, attrs => Random.NextDouble()),
            new ("rndi", 0, attrs => Random.Next()),
            new ("rndi", 1, attrs => Random.Next((int)attrs[0])),
            new ("rndi", 2, attrs => Random.Next((int)attrs[0], (int)attrs[1])),
        }).OrderByDescending(x => x.Name.Length)];

        public string Name { get; set; } = name;
        public int CountAttributes { get; set; } = countAttributes;
        public Func<List<double>, double> Action { get; set; } = action;

        public string DebuggerDisplay => $"FN {name}({CountAttributes}x)";
        public override string ToString() => DebuggerDisplay;
    }
}
