namespace RUYIinit
{
    using System;
    using System.Linq.Dynamic;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;

    public class ParseCondition
    {
        public bool ConditionResult = false;

        public bool Parse(string condition, string tokenLP, string tokenOSMODE)
        {
            bool flag2;
            string str = condition;
            string replacement = "token.LANGUAGE==";
            condition = new Regex(@"\bLANGUAGE==").Replace(condition, replacement);
            replacement = "token.OSMODE==";
            condition = new Regex(@"\bOSMODE==").Replace(condition, replacement);
            if (condition.Equals("WW"))
            {
                flag2 = true;
            }
            else
            {
                ParameterExpression expression = Expression.Parameter(typeof(RUYIinit.Token), "Token");
                try
                {
                    ParameterExpression[] parameters = new ParameterExpression[] { expression };
                    RUYIinit.Token token1 = new RUYIinit.Token();
                    token1.LANGUAGE = tokenLP;
                    token1.OSMODE = tokenOSMODE;
                    RUYIinit.Token token = token1;
                    object[] args = new object[] { token };
                    flag2 = System.Linq.Dynamic.DynamicExpression.ParseLambda(parameters, null, condition, Array.Empty<object>()).Compile().DynamicInvoke(args).ToString().Equals("True");
                }
                catch (ParseException exception)
                {
                    Console.WriteLine("Exception caught: {0}", exception);
                    flag2 = false;
                }
            }
            return flag2;
        }
    }
}

