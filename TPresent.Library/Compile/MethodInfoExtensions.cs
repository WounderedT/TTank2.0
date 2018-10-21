using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class MethodInfoExtensions
    {
        public static TDelegate CreateDelegate<TDelegate>(this MethodInfo method) where TDelegate : class
        {
            return CreateDelegate<TDelegate>(method,
                (typeArguments, parameterExpressions) => Expression.Call(method, parameterExpressions));
        }

        private static TDelegate CreateDelegate<TDelegate>(MethodInfo method, Func<Type[], ParameterExpression[], MethodCallExpression> getCallExpression) where TDelegate : class
        {
            var parameterExpression = GetExpressionParametersFrom<TDelegate>();
            CheckParameterCountsAreEqual(parameterExpression, method.GetParameters());

            var expression = getCallExpression(new Type[] { }, parameterExpression);
            return Expression.Lambda<TDelegate>(expression, parameterExpression).Compile();
        }

        private static ParameterExpression[] GetExpressionParametersFrom<TDelegate>()
        {
            return typeof(TDelegate)
                .GetMethod("Invoke")
                .GetParameters()
                .Select(s => Expression.Parameter(s.ParameterType))
                .ToArray();
        }

        private static void CheckParameterCountsAreEqual(ParameterExpression[] parameterExpression, ParameterInfo[] methodParameters)
        {
            if(parameterExpression.Count() != methodParameters.Count())
            {
                throw new InvalidOperationException("The number of parameters of the requiest delegate does not match the number of parameters of the specify method.");
            }
        }
    }
}
