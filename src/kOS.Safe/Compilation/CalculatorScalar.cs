using kOS.Safe.Encapsulation;

namespace kOS.Safe.Compilation
{
    public class CalculatorScalar : Calculator
    {
        public override object Add(OperandPair pair)
        {
            // It was just this one line:  I split it apart into pieces so I could profile each piece:
            //
            // return ScalarValue.Create(pair.Left) + ScalarValue.Create(pair.Right);
            //
            string keyPrefix = "  CalculatorScalar.Add("+pair.Left+", "+pair.Right+")";

            var watch = System.Diagnostics.Stopwatch.StartNew();

            ScalarValue leftScalar = ScalarValue.Create(pair.Left);

            watch.Stop();
            Utilities.Debug.DebugTimes[keyPrefix+" ScalarValue.Create("+pair.Left+")"] = watch.ElapsedTicks*1000D/System.Diagnostics.Stopwatch.Frequency;

            watch.Reset(); watch.Start();

            ScalarValue rightScalar = ScalarValue.Create(pair.Right);

            watch.Stop();
            Utilities.Debug.DebugTimes[keyPrefix+" ScalarValue.Create("+pair.Right+")"] = watch.ElapsedTicks*1000D/System.Diagnostics.Stopwatch.Frequency;

            watch.Reset(); watch.Start();

            object result = leftScalar + rightScalar;

            watch.Stop();
            Utilities.Debug.DebugTimes[keyPrefix+" +operator "] = watch.ElapsedTicks*1000D/System.Diagnostics.Stopwatch.Frequency;



            return result;
        }

        public override object Subtract(OperandPair pair)
        {
            return ScalarValue.Create(pair.Left) - ScalarValue.Create(pair.Right);
        }

        public override object Multiply(OperandPair pair)
        {
            return ScalarValue.Create(pair.Left) * ScalarValue.Create(pair.Right);
        }

        public override object Divide(OperandPair pair)
        {
            return ScalarValue.Create(pair.Left) / ScalarValue.Create(pair.Right);
        }

        public override object Power(OperandPair pair)
        {
            return ScalarValue.Create(pair.Left) ^ ScalarValue.Create(pair.Right);
        }

        public override object GreaterThan(OperandPair pair)
        {
            return ScalarValue.Create(pair.Left) > ScalarValue.Create(pair.Right);
        }

        public override object LessThan(OperandPair pair)
        {
            return ScalarValue.Create(pair.Left) < ScalarValue.Create(pair.Right);
        }

        public override object GreaterThanEqual(OperandPair pair)
        {
            return ScalarValue.Create(pair.Left) >= ScalarValue.Create(pair.Right);
        }

        public override object LessThanEqual(OperandPair pair)
        {
            return ScalarValue.Create(pair.Left) <= ScalarValue.Create(pair.Right);
        }

        public override object NotEqual(OperandPair pair)
        {
            return ScalarValue.Create(pair.Left) != ScalarValue.Create(pair.Right);
        }

        public override object Equal(OperandPair pair)
        {
            return ScalarValue.Create(pair.Left) == ScalarValue.Create(pair.Right);
        }

        public override object Min(OperandPair pair)
        {
            return ScalarValue.Min(ScalarValue.Create(pair.Left), ScalarValue.Create(pair.Right));
        }

        public override object Max(OperandPair pair)
        {
            return ScalarValue.Max(ScalarValue.Create(pair.Left), ScalarValue.Create(pair.Right));
        }
    }
}