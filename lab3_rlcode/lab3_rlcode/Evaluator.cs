using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Lab3
{
    public class Evaluator
    {
        public static RLNumber EvaluateExpression(string expression)
        {
            // Parse the expression and build the expression tree
            var expressionTree = ParseExpression(expression);

            // Compile and evaluate the expression
            var compiledExpression = Expression.Lambda<Func<RLNumber>>(expressionTree).Compile();
            return compiledExpression.Invoke();
        }

        private static readonly Char[] Ops = new Char[] { '+', '-', '*', '/', '=', '>', '<' };

        private static Expression ParseExpression(string expression)
        {
            // Use regular expression to split the expression into tokens
            var regex = new Regex(@"([+*/=<>]|\s+-\s+)");
            var tokens = regex.Split(expression).Where(token => !string.IsNullOrWhiteSpace(token)).ToArray();

            // Convert tokens to expressions
            var operands = new Stack<Expression>();
            var operators = new Stack<char>();
            
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i].Trim();
                if (Ops.Contains(token[0]) && token.Length == 1)//token is an operator
                {
                    // розраховуємо приорітети операторів
                    while (operators.Count > 0 && Priority(operators.Peek()) >= Priority(token[0]))
                    {
                        PopAndApplyOperator(operands, operators);
                    }
                    operators.Push(token[0]);
                }
                else
                {
                    operands.Push(ParseToken(token));
                }
            }

            while (operators.Count > 0)
            {
                PopAndApplyOperator(operands, operators);//викидаємо стек і рахуємо операцію
            }

            return operands.Peek();
        }

        private static void PopAndApplyOperator(Stack<Expression> operands, Stack<char> operators)
        {
            char op = operators.Pop();
            Expression right = operands.Pop();
            Expression left = operands.Pop();
            Expression result = CombineExpressions(left, op, right);
            operands.Push(result);
        }

        private static Expression ParseToken(string token)
        {
            if (double.TryParse(token, out double doubleValue))
            {
                return Expression.Constant(new RLNumber(doubleValue));//десяткове число
            }
            else if (int.TryParse(token, out int intValue))
            {
                return Expression.Constant(new RLNumber(intValue));//ціле число
            }
            else
            {
                // Remove quotes if any
                return Expression.Constant(new RLNumber(token.Trim('"')));//РЛ-число
            }
        }

        private static Expression CombineExpressions(Expression left, char operatorToken, Expression right)
        {
            switch (operatorToken)
            {
                case '+':
                    return Expression.Add(left, right);
                case '-':
                    return Expression.Subtract(left, right);
                case '*':
                    return Expression.Multiply(left, right);
                case '/':
                    return Expression.Divide(left, right);
                case '=':
                    return Expression.Equal(left, right);
                case '>':
                    return Expression.GreaterThan(left, right);
                case '<':
                    return Expression.LessThan(left, right);
                default:
                    throw new ArgumentException($"Unsupported operator: {operatorToken}");
            }
        }

        private static int Priority(char op)
        {
            //PEMDAS
            switch (op)
            {
                case '+':
                case '-':
                    return 1;
                case '*':
                case '/':
                    return 2;
                case '=':
                case '>':
                case '<':
                    return 3;
                default:
                    return 0;
            }
        }
    }
}
