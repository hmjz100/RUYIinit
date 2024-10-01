namespace RUYIinit2
{
    using System;
    using System.Linq.Dynamic;
    using System.Linq.Expressions;

    public class ParseCondition
    {
        public bool ConditionResult = false;

        public bool Parse(string condition, string tokenLP, string tokenOSMODE)
        {
            bool conditionResult;
            if (condition.Equals("WW"))
            {
                conditionResult = true;
            }
            else
            {
                ParameterExpression expression = Expression.Parameter(typeof(Tokens), "Tokens");
                ParameterExpression[] parameters = new ParameterExpression[] { expression };
                Tokens tokens1 = new Tokens();
                tokens1.LP = tokenLP;
                tokens1.OSMODE = tokenOSMODE;
                tokens1.INDEX = 1;
                Tokens tokens = tokens1;
                object[] args = new object[] { tokens };
                object obj2 = System.Linq.Dynamic.DynamicExpression.ParseLambda(parameters, null, condition, Array.Empty<object>()).Compile().DynamicInvoke(args);
                Console.WriteLine(obj2.ToString());
                if (obj2.ToString().Equals("True"))
                {
                    this.ConditionResult = true;
                }
                conditionResult = this.ConditionResult;
            }
            return conditionResult;
        }
    }
}

