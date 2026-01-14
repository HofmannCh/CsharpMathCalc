using System.Diagnostics;

namespace MathCalc.Logic.Internal
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class Operator(int prio, string expression, CalcObjType first, CalcObjType second, Func<CalcObj, CalcObj, CalcObj> action)
    {
        public static readonly Operator[] Operators = [.. (new Operator[]
        {
            new (1, "+",
                CalcObjType.Number, CalcObjType.Number,
                (a,b) => new CalcObj(CalcObjType.Number, a.ToTypeDouble() + b.ToTypeDouble() )),

            new (2, "+",
                CalcObjType.String, CalcObjType.String,
                (a,b) => new CalcObj(CalcObjType.String, a.ToTypeString() + b.ToTypeString() )),

            new (1, "-",
                CalcObjType.Number, CalcObjType.Number,
                (a,b) => new CalcObj(CalcObjType.Number, a.ToTypeDouble() - b.ToTypeDouble() )),

            new (2, "*",
                CalcObjType.Number, CalcObjType.Number,
                (a,b) => new CalcObj(CalcObjType.Number, a.ToTypeDouble() * b.ToTypeDouble() )),

            new (1, "*",
                CalcObjType.String, CalcObjType.Number,
                (a,b) => new CalcObj(CalcObjType.String, string.Join(string.Empty, Enumerable.Range(0, b.ToTypeInt()).Select(x => a.ToTypeString()) ))),

            new (2, "/",
                CalcObjType.Number, CalcObjType.Number,
                (a,b) => new CalcObj(CalcObjType.Number, a.ToTypeDouble() / b.ToTypeDouble() )),

            new (2, "**",
                CalcObjType.Number, CalcObjType.Number,
                (a,b) =>new CalcObj(CalcObjType.Number, Math.Pow(a.ToTypeDouble(), b.ToTypeDouble()))),

            new (2, "^",
                CalcObjType.Number, CalcObjType.Number,
                (a,b) => new CalcObj(CalcObjType.Number, Math.Pow(a.ToTypeDouble(),b.ToTypeDouble()))),

            new (1, "+",
                CalcObjType.DateTime, CalcObjType.TimeSpan,
                (a,b) => new CalcObj(CalcObjType.DateTime, a.ToTypeDateTime() + b.ToTypeTimeSpan())),

            new (1, "-",
                CalcObjType.DateTime, CalcObjType.TimeSpan,
                (a,b) => new CalcObj(CalcObjType.DateTime, a.ToTypeDateTime() - b.ToTypeTimeSpan())),

            new (1, "+",
                CalcObjType.TimeSpan, CalcObjType.TimeSpan,
                (a,b) => new CalcObj(CalcObjType.TimeSpan, a.ToTypeTimeSpan() + b.ToTypeTimeSpan())),

            new (1, "-",
                CalcObjType.TimeSpan, CalcObjType.TimeSpan,
                (a,b) => new CalcObj(CalcObjType.TimeSpan, a.ToTypeTimeSpan() - b.ToTypeTimeSpan())),

        }).OrderByDescending(x => x.Expression.Length)];

        public int Prio { get; set; } = prio;
        public string Expression { get; set; } = expression;
        public CalcObjType First { get; set; } = first;
        public CalcObjType Second { get; set; } = second;
        public Func<CalcObj, CalcObj, CalcObj> Action { get; set; } = action;

        public string DebuggerDisplay => $"OP {{{Expression}}} ({Prio})";
        public override string ToString() => DebuggerDisplay;
    }
}
