using System.Linq.Dynamic.Core;
using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
namespace Lab3;

public class Evaluator
{
    public static RLNumber EvaluateExpression(string expression)
    {
        // Parse the expression
        var parsedExpression = ParseExpression(expression);

        // Compile and evaluate the expression
        var compiledExpression = parsedExpression.Compile();
        var result = compiledExpression.Invoke();

        // Return the result as RLNumber
        return result;
    }

    private static Expression<Func<RLNumber>> ParseExpression(string expression)
    {
        // Use regular expression to split the expression into tokens
        var regex = new Regex(@"([+*/]|\s+-\s+)");
        var tokens = regex.Split(expression).Where(token => !string.IsNullOrWhiteSpace(token)).ToArray();

        // Convert tokens to expressions
        var expressions = new List<Expression>();
        var operators = new List<char>();
        for (int i = 0; i < tokens.Length; i++)
        {
            string token = tokens[i].Trim();
            if( token == "+" || token == "-" || token == "*" || token == "/"){
                operators.Add(token[0]);
                continue;
            }
            expressions.Add(ParseToken(token));
        }

        // Combine the expressions with operators
        int operatorIndex = 0;
        Expression combinedExpression = expressions[0];
        for (int i = 0; i < expressions.Count-1; i ++)
        {
            char operatorToken = operators[operatorIndex++];
            Expression rightOperand = expressions[i + 1];
            combinedExpression = CombineExpressions(combinedExpression, operatorToken, rightOperand);
        }

        // Create a lambda expression
        return Expression.Lambda<Func<RLNumber>>(combinedExpression);
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
            //remove quotes if any
            return Expression.Constant(new RLNumber(token.Trim('"')));
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
            default:
                throw new ArgumentException($"Unsupported operator: {operatorToken}");
        }
    }
}
