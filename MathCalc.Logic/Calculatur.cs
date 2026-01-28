using MathCalc.Logic.Internal;
using System;
using System.Linq.Expressions;
using System.Text;

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

        public CalcObj EvaluateExpression(string expression)
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

        private CalcObj EvaluateExpression(ref int pos, string expression, EvalMod evalMod)
        {
            string buildingValue = string.Empty;
            CalcObj? currentValue = null;
            var readingMod = ReadingMod.Value;

            void MakeBuildNumber()
            {
                buildingValue = buildingValue.Replace(',', '.');
                if (!double.TryParse("0" + buildingValue.TrimEnd('.'), out double currentValueDouble))
                    throw new Exception("Invalid identifier: " + buildingValue);

                currentValue = new CalcObj(CalcObjType.Number, currentValueDouble);
                readingMod = ReadingMod.Operator;
                buildingValue = string.Empty;
            }

            var subExpressions = new List<(CalcObj Value, Operator[] Operator)>(4); // Value, Operation
            var argValues = new List<CalcObj>(4);
            var startPos = pos;

            CalcObj CalcCurrentVal(int pos)
            {
                if (currentValue == null)
                    throw new Exception("Invalid value: " + Sub(expression, startPos, pos));

                if (subExpressions.Count == 0)
                    return currentValue;

                subExpressions.Add((currentValue, []));

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
                            else if (c == '_' || c == '\'' || c == '’')
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
                            else
                            {
                                ValueForm? vf = null;
                                foreach (var x in ValueForm.ValueForms)
                                    if (SubStringEq(x.StartStr, expression, ref pos))
                                    {
                                        vf = x;
                                        break;
                                    }

                                if (vf != null)
                                {
                                    pos++;
                                    currentValue = new CalcObj(vf.Type, vf.Parse(ReadTill(expression, vf.EndStr, vf.EscapeChar, ref pos)));
                                    readingMod = ReadingMod.Operator;
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

                            var valTuple = new ValueTuple<int>(pos);
                            var ops = Operator.Operators
                                .Where(x =>
                                {
                                    int pos2 = valTuple.Item1;
                                    var same = x.First == currentValue!.Type && SubStringEq(x.Expression, expression, ref pos2);
                                    valTuple.Item1 = pos2;
                                    return same;
                                })
                                .ToArray();
                            pos = valTuple.Item1;

                            if (!ops.Any())
                                throw new Exception("Invalid operator: " + Sub(expression, pos, pos + 1));

                            subExpressions.Add((currentValue!, ops));

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
                                    if (StringComparer.OrdinalIgnoreCase.Equals(x.Name, buildingValue) && x.Attributes.SameValue(argValues, (a, b) => a == b.Type))
                                    {
                                        fn = x;
                                        break;
                                    }

                                if (fn == null)
                                    throw new Exception($"Function not found with type arguments: {buildingValue} ({string.Join(", ", argValues)}");

                                currentValue = fn?.Action.Invoke(argValues!);
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


        private string ReadTill(string expression, string endStr, char escapeChar, ref int pos)
        {
            var valueStr = string.Empty;
            var isNextEscaped = false;
            var chars = new StringBuilder(8);
            
            for (; pos < expression.Length; pos++)
            {
                var c = expression[pos];
                if (!isNextEscaped && c == escapeChar)
                    isNextEscaped = true;
                else if (!isNextEscaped && SubStringEq(endStr, expression, ref pos))
                    return chars.ToString();
                else
                {
                    isNextEscaped = false;
                    chars.Append(c);
                }
            }

            throw new Exception("Invalid format value: " + chars.ToString());
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

        private CalcObj CalcCurrentValIterations(List<(CalcObj Value, Operator[] Operator)> subExpressions)
        {
            IEnumerable<(CalcObj Value, Operator[] Operator)> Iter(IEnumerable<(CalcObj Value, Operator[] Operator)> items, int prio)
            {
                var enumerator = items.GetEnumerator();
                enumerator.MoveNext(); // Always true, because check in subExpressions.Count == 0

                var prevItem = enumerator.Current;

                while (enumerator.MoveNext())
                {
                    var currentItem = enumerator.Current;

                    var op = prevItem.Operator.FirstOrDefault(x => x.Second == currentItem.Value.Type);

                    if (op == null)
                        throw new Exception($"Invalid operator. No operator for {prevItem.Value} and {currentItem.Value}");

                    if (op.Prio < prio)
                    {
                        yield return prevItem!;
                        prevItem = currentItem;
                    }
                    else
                    {
                        prevItem = (op.Action.Invoke(prevItem.Value, currentItem.Value), currentItem.Operator);
                    }
                }

                yield return prevItem!;
            }

            IEnumerable<(CalcObj Value, Operator[] Operator)> iter = subExpressions;
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
