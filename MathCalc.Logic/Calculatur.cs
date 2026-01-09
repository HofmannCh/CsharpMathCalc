using MathCalc.Logic.Internal;
using System;
using System.Linq.Expressions;

namespace MathCalc.Logic
{
    public class Calculatur
    {
        public Calculatur()
        {

        }

        private static int UpperToLowerDiff = 'a' - 'A';

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

                subExpressions.Add((currentValue.Value, null));

                return CalcCurrentValIterations(subExpressions);
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
                            foreach (var x in Operator.Operators)
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
                                foreach (var x in Function.Functions)
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

        private double CalcCurrentValIterations(List<(double Value, Operator? Operator)> subExpressions)
        {

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
            var maxPrio = Operator.Operators.Max(x => x.Prio);
            var minPrio = Operator.Operators.Min(x => x.Prio) - 1;
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
    }
}
