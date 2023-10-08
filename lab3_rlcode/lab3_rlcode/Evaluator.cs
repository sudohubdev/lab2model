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
            expressionTree = Expression.Convert(expressionTree, typeof(object));
            var compiledExpression = Expression.Lambda<Func<object>>(expressionTree).Compile();
            if (compiledExpression.Invoke() is RLNumber result)
            {
                return result;
            }
            else if (compiledExpression.Invoke() is bool boolResult)//для логічних виразів виведемо 1 і 0
            {
                return new RLNumber(boolResult ? 1 : 0);
            }
            {
                throw new Exception("Invalid expression");
            }
        }

        private static readonly Char[] Ops = new Char[] { '+', '-', '*', '/', '=', '>', '<', '(', ')' };

        private static Expression ParseExpression(string expression)
        {
            var tokens = Tokenize(expression);

            // Convert tokens to expressions
            var operands = new Stack<Expression>();
            var operators = new Stack<char>();

            foreach (var token in tokens)
            {
                if (Ops.Contains(token[0]) && token.Length == 1)
                {
                    if (token[0] == '(')
                    {
                        operators.Push(token[0]);//дужка зразу в стек
                    }
                    else if (token[0] == ')')//закриваюча дужка
                    {
                        while (operators.Count > 0 && operators.Peek() != '(')//вкладені дужки як (a+(b+c))
                        {
                            PopAndApplyOperator(operands, operators);
                        }
                        if (operators.Count > 0 && operators.Peek() == '(') //викидаю вкладену дужку
                        {
                            operators.Pop();
                        }
                    }
                    else
                    {
                        // без дужок рахую приорітети і про-оджу по стеку
                        while (operators.Count > 0 && Priority(operators.Peek()) >= Priority(token[0]))
                        {
                            PopAndApplyOperator(operands, operators);
                        }
                        operators.Push(token[0]);
                    }
                }
                else
                {
                    operands.Push(ParseToken(token));
                }
            }

            while (operators.Count > 0)
            {
                PopAndApplyOperator(operands, operators);
            }

            return operands.Peek();
        }

        private static List<string> Tokenize(string expression)
        {
            var regex = new Regex(@"([+*/=<>()]|\s+-\s+)");
            var tokens = regex.Split(expression).Where(token => !string.IsNullOrWhiteSpace(token)).ToList();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] == "-" && (i == 0 || Ops.Contains(tokens[i - 1][0]) || tokens[i - 1] == "("))
                {
                    // Handle negative numbers
                    tokens[i] = "-" + tokens[i + 1];
                    tokens.RemoveAt(i + 1);
                }
            }

            return tokens;
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
                return Expression.Constant(new RLNumber(doubleValue));
            }
            else if (int.TryParse(token, out int intValue))
            {
                return Expression.Constant(new RLNumber(intValue));
            }
            else
            {
                // Remove quotes if any 
                return Expression.Constant(new RLNumber(token.Trim('"').Trim()));
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
                    return 0;
                default:
                    return 0;
            }
        }
    }
}
