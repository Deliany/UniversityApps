using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ELW.Library.Math;
using ELW.Library.Math.Expressions;
using ELW.Library.Math.Tools;


namespace by_Deliany
{
    public static class MyParser
    {
        static public double calculate(string input, double value)
        {

            PreparedExpression preparedExpression = ToolsHelper.Parser.Parse(input);

            CompiledExpression compiledExpression = ToolsHelper.Compiler.Compile(preparedExpression);

            List<VariableValue> variables = new List<VariableValue>();
            variables.Add(new VariableValue(value, "x"));
            double res = ToolsHelper.Calculator.Calculate(compiledExpression, variables);

            return res;

        }
    }
}
