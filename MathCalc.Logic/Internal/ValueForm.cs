using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;

namespace MathCalc.Logic.Internal
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class ValueForm(string startStr, string endStr, char escapeChar, CalcObjType type, Func<string, object> parse)
    {
        /* Start with:
         * anyNumber => number
         * " => string
         * d" => date
         * // dt" => datetime
         * // t" => time
         * s" => span
         */

        public static readonly ValueForm[] ValueForms = [..(new ValueForm[]
        {
            new ("\"", "\"", '\\',CalcObjType.String, x => x),
            new ("d\"", "\"", '\\', CalcObjType.DateTime, x => DateTime.Parse(x)),
            new ("s\"", "\"", '\\', CalcObjType.TimeSpan, x => TimeSpan.Parse(x)),
        }).OrderByDescending(x => x.StartStr.Length)];

        public string StartStr { get; set; } = startStr;
        public string EndStr { get; set; } = endStr;
        public char EscapeChar { get; set; } = escapeChar;
        public CalcObjType Type { get; set; } = type;
        public Func<string, object> Parse { get; set; } = parse;

        public string DebuggerDisplay => $"VF {Type}: {StartStr} {EndStr}";
        public override string ToString() => DebuggerDisplay;
    }
}
