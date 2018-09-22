using System;
using System.Linq.Expressions;

namespace CMSSolutions.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression Visit<TExpression>(this Expression expression, Func<TExpression, Expression> visitor)
            where TExpression : Expression
        {
            return ExpressionVisitor<TExpression>.Visit(expression, visitor);
        }

        public static TReturn Visit<TExpression, TReturn>(this TReturn expression, Func<TExpression, Expression> visitor)
            where TExpression : Expression
            where TReturn : Expression
        {
            return (TReturn)ExpressionVisitor<TExpression>.Visit(expression, visitor);
        }

        public static Expression<TDelegate> Visit<TExpression, TDelegate>(this Expression<TDelegate> expression, Func<TExpression, Expression> visitor)
            where TExpression : Expression
        {
            return ExpressionVisitor<TExpression>.Visit(expression, visitor);
        }

        public static bool TryParsePath(this Expression expression, out string path)
        {
            path = null;
            Expression expression2 = expression.RemoveConvert();
            var expression3 = expression2 as MemberExpression;
            var expression4 = expression2 as MethodCallExpression;
            if (expression3 != null)
            {
                string str2;
                string name = expression3.Member.Name;
                if (!TryParsePath(expression3.Expression, out str2))
                {
                    return false;
                }
                path = (str2 == null) ? name : (str2 + "." + name);
            }
            else if (expression4 != null)
            {
                if ((expression4.Method.Name == "Select") && (expression4.Arguments.Count == 2))
                {
                    string str3;
                    if (!TryParsePath(expression4.Arguments[0], out str3))
                    {
                        return false;
                    }
                    if (str3 != null)
                    {
                        var expression5 = expression4.Arguments[1] as LambdaExpression;
                        if (expression5 != null)
                        {
                            string str4;
                            if (!TryParsePath(expression5.Body, out str4))
                            {
                                return false;
                            }
                            if (str4 != null)
                            {
                                path = str3 + "." + str4;
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            return true;
        }

        public static Expression RemoveConvert(this Expression expression)
        {
            while ((expression != null) && ((expression.NodeType == ExpressionType.Convert) || (expression.NodeType == ExpressionType.ConvertChecked)))
            {
                expression = ((UnaryExpression)expression).Operand.RemoveConvert();
            }
            return expression;
        }
    }

    public class ExpressionVisitor<TExpression> : ExpressionVisitor where TExpression : Expression
    {
        private readonly Func<TExpression, Expression> _visitor;

        public ExpressionVisitor(Func<TExpression, Expression> visitor)
        {
            _visitor = visitor;
        }

        public override Expression Visit(Expression expression)
        {
            if (expression is TExpression && _visitor != null)
                expression = _visitor(expression as TExpression);

            return base.Visit(expression);
        }

        public static Expression Visit(Expression expression, Func<TExpression, Expression> visitor)
        {
            return new ExpressionVisitor<TExpression>(visitor).Visit(expression);
        }

        public static Expression<TDelegate> Visit<TDelegate>(Expression<TDelegate> expression, Func<TExpression, Expression> visitor)
        {
            return (Expression<TDelegate>)new ExpressionVisitor<TExpression>(visitor).Visit(expression);
        }
    }
}