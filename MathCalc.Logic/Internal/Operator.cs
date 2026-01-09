using System.Diagnostics;

namespace MathCalc.Logic.Internal
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class Operator(int prio, string expression, Func<double, double, double> action)
    {
        public static readonly Operator[] Operators = [.. (new Operator[]
        {
            new (1, "+", (a,b) => a + b ),
            new (1, "-", (a,b) => a - b ),
            new (2, "*", (a,b) => a * b ),
            new (2, "/", (a,b) => a / b ),
            new (2, "**", Math.Pow),
            new (2, "^", Math.Pow),
        }).OrderByDescending(x => x.Expression.Length)];

        public int Prio { get; set; } = prio;
        public string Expression { get; set; } = expression;
        public Func<double, double, double> Action { get; set; } = action;

        public string DebuggerDisplay => $"OP {{{Expression}}} ({Prio})";
        public override string ToString() => DebuggerDisplay;
    }
}
