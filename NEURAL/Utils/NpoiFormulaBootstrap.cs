using NPOI.SS.Formula.Eval;

namespace NEURAL.Utils
{
    public static class NpoiFormulaBootstrap
    {
        private static int _done;

        public static void EnsureRegistered()
        {
            if (Interlocked.Exchange(ref _done, 1) == 1) return; 

            FunctionEval.RegisterFunction("N", new NFunctionImpl());
        }
    }
}
