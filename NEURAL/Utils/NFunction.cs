
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;


namespace NEURAL.Utils
{
    public sealed class NFunctionImpl : Function
    {
        public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
        {
            if (args == null || args.Length == 0) return new NumberEval(0d);

            var val = OperandResolver.GetSingleValue(args[0], srcRowIndex, srcColumnIndex);

            if (val is BoolEval be) return new NumberEval(be.BooleanValue ? 1d : 0d);
            if (val is ErrorEval || val is BlankEval || val is StringEval) return new NumberEval(0d);

            try
            {
                double d = OperandResolver.CoerceValueToDouble(val);
                if (double.IsNaN(d) || double.IsInfinity(d)) return new NumberEval(0d);
                return new NumberEval(d);
            }
            catch
            {
                return new NumberEval(0d);
            }
        }
    }
}
