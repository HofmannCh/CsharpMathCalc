using System.Diagnostics;

namespace MathCalc.Logic
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
            new (2, "**", Math.Pow),
            new (2, "^", Math.Pow),
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

        private static readonly Random Random = new Random();

        private static readonly Function[] Functions = [.. (new Function[]
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

        [DebuggerDisplay("{DebuggerDisplay,nq}")]
        public class Function(string name, int countAttributes, Func<List<double>, double> action)
        {
            public string Name { get; set; } = name;
            public int CountAttributes { get; set; } = countAttributes;
            public Func<List<double>, double> Action { get; set; } = action;

            public string DebuggerDisplay => $"FN {name}({CountAttributes}x)";
            public override string ToString() => DebuggerDisplay;
        }

        // 1 + 1 * 2
        // (1 + 1) * 2

        public double EvaluateExpression(string expression)
        {
            try
            {
                int pos = 0;
                return EvaluateExpression(ref pos, expression, EvalMod.Root);
            }
            catch (Exception ex)
            {
                throw new Exception("Calculate Exception: " + ex.Message);
            }
        }

        // 4 + 2 * (3 + 1) / 4 = 3

        // 1: 4+, 2*(3+1)/4
        // 2: 2*, (3+1)/, 4
        // 3: 3+, 1

        public enum ReadingMod
        {
            Value,
            Brackets,
            Operator,
            FunctionName,
            FunctionArgs,
            FunctionArgsComma,
        }

        public enum EvalMod
        {
            Root,
            Brackets,
            FunctionArgs
        }

        private double EvaluateExpression(ref int pos, string expression, EvalMod evalMod)
        {
            string buildingValue = string.Empty;
            double? currentValue = null;
            var readingMod = ReadingMod.Value;

            void MakeBuildNumber()
            {
                if (!double.TryParse("0" + buildingValue.Replace(',', '.').TrimEnd('.'), out double currentValue2))
                    throw new Exception("Invalid identifier: " + buildingValue);

                currentValue = currentValue2;
                readingMod = ReadingMod.Operator;
                buildingValue = string.Empty;
            }

            var subExpressions = new List<(double Value, Operator? Operator)>(4); // Value, Operation
            var argValues = new List<double>(4);
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

                        if (prevItem.Operator!.Prio < prio)
                        {
                            yield return prevItem!;
                            prevItem = currentItem;
                        }
                        else
                        {
                            prevItem = (prevItem.Operator.Action.Invoke(prevItem.Value, currentItem.Value), currentItem.Operator);
                        }
                    }

                    yield return prevItem!;
                }

                IEnumerable<(double Value, Operator? Operator)> iter = subExpressions;
                var maxPrio = Operators.Max(x => x.Prio);
                var minPrio = Operators.Min(x => x.Prio) - 1;
                for (int i = maxPrio; i >= minPrio; i--)
                {
                    iter = Iter(iter, i)
#if DEBUG
                        .ToArray() // Easier to debug when evaluated everting simultaneously 
#endif
                        ;
                }

                var (val, op) = iter.First();
                return val;
            }

            char[] endChars = evalMod switch
            {
                EvalMod.Root => [],
                EvalMod.Brackets => [')'],
                EvalMod.FunctionArgs => [',', ')'],
                _ => [],
            };

            for (; pos < expression.Length; pos++)
            {
                char c = expression[pos];

                if (char.IsWhiteSpace(c))
                    continue;

                AssureLower(ref c);

                switch (readingMod)
                {
                    case ReadingMod.Brackets:
                        {
                            if (c == ')')
                            {
                                readingMod = ReadingMod.Operator;
                            }
                            else
                            {
                                throw new Exception("Brackets not closed");
                            }
                            break;
                        }
                    case ReadingMod.Value:
                        {
                            if (c == '(')
                            {
                                pos++;
                                currentValue = EvaluateExpression(ref pos, expression, EvalMod.Brackets);
                                readingMod = ReadingMod.Brackets;

                                //if (pos >= expression.Length)
                                //    throw new Exception("Brackets not closed");

                                pos -= 2;

                            }
                            else if (endChars.Contains(c)) // For (1 + 1) + 1
                            {
                                if (buildingValue != string.Empty)
                                    MakeBuildNumber();

                                pos++;
                                return CalcCurrentVal(pos - 1);
                            }
                            else if (c == '_' || c == '\'')
                            {
                                // continue;
                            }
                            else if (char.IsAsciiDigit(c) || c == '.' || c == ',')
                            {
                                buildingValue += c;
                            }
                            else if (buildingValue != string.Empty)
                            {
                                MakeBuildNumber();
                                pos--;
                            }
                            else if (char.IsAsciiLetterLower(c))
                            {
                                buildingValue = string.Empty + c;
                                readingMod = ReadingMod.FunctionName;
                            }
                            else
                            {
                                throw new Exception("Invalid syntax: " + Sub(expression, startPos, pos));
                            }

                            break;
                        }

                    case ReadingMod.Operator:
                        {
                            if (endChars.Contains(c)) // For ((1 + 1)) + 1
                            {
                                if (buildingValue != string.Empty)
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

                            subExpressions.Add((currentValue!.Value, op));

                            currentValue = null;
                            readingMod = ReadingMod.Value;
                            break;
                        }

                    case ReadingMod.FunctionName:
                        {
                            if (char.IsAsciiLetterLower(c))
                            {
                                buildingValue += c;
                            }
                            else if (c == '(')
                            {
                                argValues.Clear();
                                readingMod = ReadingMod.FunctionArgs;
                            }
                            else
                            {
                                throw new Exception("Invalid function: " + buildingValue + c);
                            }
                            break;
                        }

                    case ReadingMod.FunctionArgs:
                        {
                            if (c == ')')
                                goto case ReadingMod.FunctionArgsComma;

                            argValues.Add(EvaluateExpression(ref pos, expression, EvalMod.FunctionArgs));
                            readingMod = ReadingMod.FunctionArgsComma;
                            pos -= 2;
                            break;
                        }
                    case ReadingMod.FunctionArgsComma:
                        {
                            if (c == ',')
                            {
                                readingMod = ReadingMod.FunctionArgs;
                            }
                            else if (c == ')')
                            {

                                Function? fn = null;
                                foreach (var x in Functions)
                                    if (StringComparer.OrdinalIgnoreCase.Equals(x.Name, buildingValue) && x.CountAttributes == argValues.Count)
                                    {
                                        //pos += x.Name.Length;
                                        fn = x;
                                        break;
                                    }

                                if (fn == null)
                                    throw new Exception($"Function not found with count arguments: {buildingValue} ({argValues.Count}x)");

                                currentValue = fn?.Action.Invoke(argValues);
                                buildingValue = string.Empty;
                                readingMod = ReadingMod.Operator;
                            }
                            else
                            {
                                throw new Exception("Invalid function ending: " + c);
                            }
                            break;
                        }
                }
            }

            if (evalMod != EvalMod.Root)
                throw new Exception("Invalid syntax, symbol missing: " + string.Join(",", endChars));

            if (buildingValue != string.Empty)
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
