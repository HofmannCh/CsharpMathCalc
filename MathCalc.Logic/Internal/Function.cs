using System.Diagnostics;
using System.Linq;

namespace MathCalc.Logic.Internal
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class Function(string name, CalcObjType[] attributes, Func<List<CalcObj>, CalcObj> action)
    {
        private static readonly Random Random = new Random();

        public static readonly Function[] Functions = [.. (new Function[]
        {
            new (
                "sqrt",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Sqrt(attrs[0].ToTypeDouble()))),
            new (
                "pow",
                [CalcObjType.Number,CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Pow(attrs[0].ToTypeDouble(), attrs[1].ToTypeDouble()))),

            new (
                "pi",
                [],
                attrs => new CalcObj(CalcObjType.Number, Math.PI)),

            new (
                "log",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Log(attrs[0].ToTypeDouble()))),

            new (
                "log",
                [CalcObjType.Number,CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Log(attrs[0].ToTypeDouble(), attrs[1].ToTypeDouble()))),

            new (
                "log2",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Log2(attrs[0].ToTypeDouble()))),

            new (
                "log10",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Log10(attrs[0].ToTypeDouble()))),

            new (
                "min",
                [CalcObjType.Number,CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Min(attrs[0].ToTypeDouble(), attrs[1].ToTypeDouble()))),

            new (
                "max",
                [CalcObjType.Number,CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Max(attrs[0].ToTypeDouble(), attrs[1].ToTypeDouble()))),

            new (
                "floor",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Floor(attrs[0].ToTypeDouble()))),

            new (
                "roundd",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Floor(attrs[0].ToTypeDouble()))),

            new (
                "ceil",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Ceiling(attrs[0].ToTypeDouble()))),

            new (
                "roundu",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Ceiling(attrs[0].ToTypeDouble()))),

            new (
                "round",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Round(attrs[0].ToTypeDouble()))),

            new (
                "abs",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Abs(attrs[0].ToTypeDouble()))),

            new (
                "sin",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Sin(attrs[0].ToTypeDouble()))),

            new (
                "cos",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Cos(attrs[0].ToTypeDouble()))),

            new (
                "tan",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Math.Tan(attrs[0].ToTypeDouble()))),

            new (
                "rnd",
                [],
                attrs => new CalcObj(CalcObjType.Number, Random.NextDouble())),

            new (
                "rnd",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Random.Next(attrs[0].ToTypeInt()))),

            new (
                "rnd",
                [CalcObjType.Number, CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.Number, Random.Next(attrs[0].ToTypeInt(), attrs[1].ToTypeInt()))),

            new (
                "guid",
                [],
                attrs => new CalcObj(CalcObjType.String, Guid.NewGuid().ToString())),

            new (
                "guid",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.String, string.Join(Environment.NewLine, Enumerable.Range(0, attrs[0].ToTypeInt()).Select(x => Guid.NewGuid())))),

            new (
                "guid",
                [CalcObjType.Number,CalcObjType.String],
                attrs => new CalcObj(CalcObjType.String, string.Join(attrs[1].ToTypeString(), Enumerable.Range(0, attrs[0].ToTypeInt()).Select(x => Guid.NewGuid())))),

            new (
                "range",
                [CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.String, string.Join(Environment.NewLine, Enumerable.Range(0, attrs[0].ToTypeInt()).Select((x,i) => i)))),

            new (
                "range",
                [CalcObjType.Number, CalcObjType.String],
                attrs => new CalcObj(CalcObjType.String, string.Join(attrs[1].ToTypeString(), Enumerable.Range(0, attrs[0].ToTypeInt()).Select((x,i) => i)))),

            new (
                "range",
                [CalcObjType.Number, CalcObjType.Number],
                attrs => new CalcObj(CalcObjType.String, string.Join(Environment.NewLine, Enumerable.Range(attrs[0].ToTypeInt(), attrs[1].ToTypeInt()).Select((x,i) => i+attrs[0].ToTypeInt())))),

            new (
                "range",
                [CalcObjType.Number, CalcObjType.Number, CalcObjType.String],
                attrs => new CalcObj(CalcObjType.String, string.Join(attrs[2].ToTypeString(), Enumerable.Range(attrs[0].ToTypeInt(), attrs[1].ToTypeInt()).Select((x,i) => i+attrs[0].ToTypeInt())))),

            new (
                "now",
                [],
                attrs => new CalcObj(CalcObjType.DateTime, DateTime.Now)
                ),

            new (
                "nowUtc",
                [],
                attrs => new CalcObj(CalcObjType.DateTime, DateTime.UtcNow)
                ),

            new (
                "today",
                [],
                attrs => new CalcObj(CalcObjType.DateTime, DateTime.Today)
                ),
        }).OrderByDescending(x => x.Name.Length)];

        public string Name { get; set; } = name;
        public CalcObjType[] Attributes { get; set; } = attributes;
        public Func<List<CalcObj>, CalcObj> Action { get; set; } = action;

        public string DebuggerDisplay => $"FN {name}({string.Join(",", attributes)}x)";
        public override string ToString() => DebuggerDisplay;
    }
}
