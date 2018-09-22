using CMSSolutions.ContentManagement.Widgets.Scripting.Compiler;

namespace CMSSolutions.ContentManagement.Widgets.Scripting.Ast
{
    public interface IAstNodeWithToken
    {
        Token Token { get; }
    }
}