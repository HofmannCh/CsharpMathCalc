using System;
using System.Collections.Generic;
using System.Text;

namespace MathCalc.Logic.Internal
{
    public class CalcObj
    {
        public CalcObj()
        {

        }

        public CalcObj(CalcObjType type, object value)
        {
            Type = type;
            Value = value;
        }

        public CalcObjType Type { get; set; }
        public object Value { get; set; }

        private T Throw<T>(CalcObjType type) => throw new Exception($"Can't convert type {Type} to {type} (val: {Value})");

        public int ToTypeInt() => Type == CalcObjType.Number ? Convert.ToInt32(Value) : Throw<int>(Type);
        public double ToTypeDouble() => Type == CalcObjType.Number ? Convert.ToDouble(Value) : Throw<double>(Type);
        public DateTime ToTypeDateTime() => Type == CalcObjType.DateTime ? (DateTime)Value : Throw<DateTime>(Type);
        public TimeSpan ToTypeTimeSpan() => Type == CalcObjType.TimeSpan ? (TimeSpan)Value : Throw<TimeSpan>(Type);
        public string ToTypeString() => Type == CalcObjType.String ? (string)Value : Value?.ToString() ?? string.Empty;

        public override string ToString()
        {
            var type = ValueForm.ValueForms.FirstOrDefault(x => x.Type == Type);
            if (type == null || type.Type == CalcObjType.Number)
            {
                return ToTypeString();
            }
            else
            {
                if (Value == null) return "null";
                return type.StartStr + Value.ToString()!.Replace(type.EndStr, type.EscapeChar + type.EndStr) + type.EndStr;
            }
        }
    }

    public enum CalcObjType
    {
        Number,
        DateTime,
        TimeSpan,
        String
    }
}
