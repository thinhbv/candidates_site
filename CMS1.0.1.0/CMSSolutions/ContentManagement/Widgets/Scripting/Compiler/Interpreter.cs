namespace CMSSolutions.ContentManagement.Widgets.Scripting.Compiler
{
    public class Interpreter
    {
        public EvaluationResult Evalutate(EvaluationContext context)
        {
            return new InterpreterVisitor(context).Evaluate();
        }
    }
}