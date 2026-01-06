using System.Diagnostics;

namespace MathCalc
{
    public class Calculatur
    {
        public Calculatur()
        {

        }

        private static int UpperToLowerDiff = 'a' - 'A';

        private readonly Operator[] Operators = [.. (new Operator[]
        {
            new (1, "+", (a,b) => a + b ),
            new (1, "-", (a,b) => a - b ),
            new (2, "*", (a,b) => a * b ),
            new (2, "/", (a,b) => a / b ),
            new (2, "**", (a,b) => Math.Pow(a, b) ),
        }).OrderByDescending(x => x.Expression.Length)];

        [DebuggerDisplay("{DebuggerDisplay,nq}")]
        public class Operator(int prio, string expression, Func<double, double, double> action)
        {
            public int Prio { get; set; } = prio;
            public string Expression { get; set; } = expression;
            public Func<double, double, double> Action { get; set; } = action;

            public string DebuggerDisplay => $"OP {{{Expression}}} ({Prio})";
            public override string ToString() => DebuggerDisplay;
        }

        // 1 + 1 * 2
        // (1 + 1) * 2

        public double EvaluateExpression(string expression)
        {
            int pos = 0;
            return EvaluateExpression(ref pos, expression);
        }

        // 4 + 2 * (3 + 1) / 4 = 3

        // 1: 4+, 2*(3+1)/4
        // 2: 2*, (3+1)/, 4
        // 3: 3+, 1
        public double EvaluateExpression(ref int pos, string expression)
        {

            string buildingNumber = string.Empty;
            double? currentValue = null;

            void MakeBuildNumber()
            {
                currentValue = Convert.ToDouble("0" + buildingNumber.Replace(',', '.').TrimEnd('.'));
                buildingNumber = string.Empty;
            }

            var subExpressions = new List<(double Value, Operator? Operator)>(); // Value, Operation

            var startPos = pos;

            double CalcCurrentVal(int pos)
            {
                if (!currentValue.HasValue)
                    throw new Exception("Invalid value: " + Sub(expression, startPos, pos));

                if (subExpressions.Count == 0)
                    return currentValue.Value;

                // 4+ 2* 4/
                // 4+ 8/

                subExpressions.Add((currentValue.Value, null));

                IEnumerable<(double Value, Operator Operator)> Iter(IEnumerable<(double Value, Operator? Operator)> items, int prio)
                {
                    var enumerator = items.GetEnumerator();
                    enumerator.MoveNext(); // Always true, because check in subExpressions.Count == 0

                    var prevItem = enumerator.Current;

                    while (enumerator.MoveNext())
                    {
                        var currentItem = enumerator.Current;

                        if (prevItem.Operator.Prio < prio)
                        {
                            yield return prevItem;
                            prevItem = currentItem;
                        }
                        else
                        {
                            prevItem = (prevItem.Operator.Action.Invoke(prevItem.Value, currentItem.Value), currentItem.Operator);
                        }
                    }

                    yield return prevItem;
                }

                IEnumerable<(double Value, Operator? Operator)> iter = subExpressions;
                var maxPrio = Operators.Max(x => x.Prio);
                var minPrio = Operators.Min(x => x.Prio) - 1;
                for (int i = maxPrio; i >= minPrio; i--)
                {
                    iter = Iter(iter, i).ToArray();
                }

                var (val, op) = iter.First();
                return val;
            }

            for (; pos < expression.Length; pos++)
            {
                char c = expression[pos];

                if (char.IsWhiteSpace(c))
                    continue;

                AssureLower(ref c);

                if (!currentValue.HasValue)
                {
                    if (c == '(')
                    {
                        pos++;
                        currentValue = EvaluateExpression(ref pos, expression);
                        pos--;
                    }
                    else if (c == ')')
                    {
                        if (buildingNumber != string.Empty)
                            MakeBuildNumber();

                        pos++;
                        return CalcCurrentVal(pos - 1);
                    }
                    else if (char.IsAsciiDigit(c) || c == '.' || c == ',')
                    {
                        buildingNumber += c;
                    }
                    else if (buildingNumber != string.Empty)
                    {
                        MakeBuildNumber();
                        pos--;
                    }
                    //else if (char.IsAsciiLetterLower(c))
                    //{ 
                    //    // Function like pow, sqrt, rnd or so
                    //}
                    else
                    {
                        throw new Exception("Invalid syntax: " + Sub(expression, startPos, pos));
                    }
                }
                else
                {
                    if (c == ')')
                    {
                        if (buildingNumber != string.Empty)
                            MakeBuildNumber();

                        pos++;
                        return CalcCurrentVal(pos - 1);
                    }

                    Operator? op = null;
                    foreach (var x in Operators)
                        if (SubStringEq(x.Expression, expression, ref pos))
                        {
                            op = x;
                            break;
                        }

                    if (op == null)
                        throw new Exception("Invalid operator: " + Sub(expression, pos, pos + 1));

                    subExpressions.Add((currentValue.Value, op));

                    currentValue = null;
                }
            }

            if (buildingNumber != string.Empty)
                MakeBuildNumber();

            return CalcCurrentVal(pos);
        }

        private void AssureLower(ref char c)
        {
            if (char.IsAsciiLetterUpper(c))
                c = (char)(c + UpperToLowerDiff);
        }

        private bool SubStringEq(string subString, string expression, ref int cI)
        {
            var sI = 0;
            var startCi = cI;
            for (; cI < expression.Length; cI++)
            {
                char exC = expression[cI];
                char ssC = subString[sI++];
                AssureLower(ref exC);
                AssureLower(ref ssC);

                if (exC != ssC)
                    break;

                if (sI >= subString.Length)
                    return true;
            }
            cI = startCi;
            return false;
        }

        private static string Sub(string src, int startPos, int endPos)
        {
            return src.Substring(startPos, endPos - startPos);
        }
    }
}
