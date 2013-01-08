using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Linq;
using ELW.Library.Math;
using ELW.Library.Math.Exceptions;
using ELW.Library.Math.Expressions;
using ELW.Library.Math.Tools;

namespace iSpreadsheets.Helpers
{
    [Serializable]
    public class SpreadsheetCellCustomInfo
    {
        public string Formula { get; set; }

        public string Value { get; set; }

        public CellName CellName { get; set; }

        public List<CellName> DependentCells { get; set; }

        public List<CellName> ConsequentialCells { get; set; } 

        private SpreadsheetCellCustomInfo() { }

        public SpreadsheetCellCustomInfo(CellName cellName)
        {
            this.CellName = cellName;
            this.DependentCells = new List<CellName>();
            this.ConsequentialCells = new List<CellName>();
            this.Formula = string.Empty;
            this.Value = string.Empty;
        }

        public void Calculate(string formula, DataTable dataTable)
        {
            if (formula.StartsWith("="))
            {
                string expression = formula.Substring(1);
                string result;

                try
                {
                    result = this.EvaluateExpression(expression, dataTable).ToString();
                    Logger.WriteLogInfo(string.Format("Expression \"{0}\" was successfully calculated, result: {1}", expression, result));
                }
                catch (Exception ex)
                {
                    result = "##" + ex.GetType().Name + ": " + ex.Message;
                    Logger.WriteLogException(ex.GetType().Name + ": " + ex.Message);
                }


                this.Formula = formula;
                this.Value = result;
            }
            else
            {
                this.Formula = string.Empty;
                this.Value = formula;
            }
        }

        private double EvaluateExpression(string expression, DataTable dataTable)
        {
            // replace B1:B7 => B1,B2,B3,B4,B5,B6,B7
            expression = RegexReplacer.ReplaceRangeOperators(expression);

            // Creating list of variables specified
            List<VariableValue> variables = new List<VariableValue>();

            const string patternForMatchVariables = @"(?<Cell>(?<Column>[a-zA-Z]{1,3})(?<Row>\d{1,3}))";

            foreach (Match match in Regex.Matches(expression, patternForMatchVariables))
            {
                var cellName = match.Groups["Cell"].Value;
                var col = SSColumns.Parse(match.Groups["Column"].Value);
                var row = int.Parse(match.Groups["Row"].Value);
                var spreadsheetCell = dataTable.GetSpreadsheetCell(row, col);

                this.CheckForRecursion(spreadsheetCell, dataTable, this.CellName.FullName);

                double cellValue;

                if (spreadsheetCell != null)
                {
                    if (!this.DependentCells.Contains(spreadsheetCell.Tag.CellName))
                    {
                        this.DependentCells.Add(spreadsheetCell.Tag.CellName);
                        Logger.WriteLogInfo(string.Format("Added [{0}] cell to dependent list of cell [{1}]",
                                                          spreadsheetCell.Tag.CellName.FullName, this.CellName.FullName));
                    }
                    if (this.ConsequentialCells.Contains(spreadsheetCell.Tag.CellName))
                    {
                        throw new RecursiveCellCallException("Infinity recursive loop detected!");
                    }
                    if (!spreadsheetCell.Tag.ConsequentialCells.Contains(this.CellName))
                    {
                        spreadsheetCell.Tag.ConsequentialCells.Add(this.CellName);
                        Logger.WriteLogInfo(string.Format("Added [{0}] cell to consequential list of cell [{1}]",
                                                          this.CellName.FullName, spreadsheetCell.Tag.CellName.FullName));
                    }


                    if (spreadsheetCell.Content == string.Empty)
                    {
                        cellValue = 0;
                    }
                    else
                    {
                        if (!double.TryParse(spreadsheetCell.Content, out cellValue))
                        {
                            throw new CellContensIsNotNumericException(string.Format("Cell [{0}] content must be numeric for using in formulas", cellName));
                        }
                    }
                }
                else
                {
                    throw new CellIndexIsOutOfRange(string.Format("Cell [{0}] is not present in table", cellName));
                }

                if (!variables.Any(v => v.VariableName == cellName))
                {
                    variables.Add(new VariableValue(cellValue, cellName));
                }
            }

            return this.EvaluateExpressionWithVariables(expression, variables);
        }

        private double EvaluateExpressionWithVariables(string expression, List<VariableValue> variables)
        {
            try
            {
                // Compiling an expression
                PreparedExpression preparedExpression = ToolsHelper.Parser.Parse(expression);
                CompiledExpression compiledExpression = ToolsHelper.Compiler.Compile(preparedExpression);


                // Do it !
                double res = ToolsHelper.Calculator.Calculate(compiledExpression, variables);

                return res;
            }
            catch (CompilerSyntaxException ex)
            {
                //MessageBox.Show(String.Format("Compiler syntax error: {0}", ex.Message));
                throw;
            }
            catch (MathProcessorException ex)
            {
                //MessageBox.Show(String.Format("Error: {0}", ex.Message));
                throw;
            }
            catch (ArgumentException)
            {
                //MessageBox.Show("Error in input data.");
                throw;
            }
            catch (Exception)
            {
                //MessageBox.Show("Unexpected exception.");
                throw;
            }
        }

        private void CheckForRecursion(SpreadsheetCell cell, DataTable dataTable, string cellNameToCheck)
        {
            if (cell.Tag.CellName.FullName == cellNameToCheck)
            {
                throw new RecursiveCellCallException("Infinity recursive loop detected!");
            }

            foreach (var consCellName in cell.Tag.DependentCells)
            {
                var consCell = dataTable.GetSpreadsheetCell(consCellName.Row, consCellName.Col);
                CheckForRecursion(consCell, dataTable, cellNameToCheck);
            }
        }
    }

    public class CellContensIsNotNumericException : Exception
    {
        public CellContensIsNotNumericException(string message) 
            : base(message)
        {
        }
    }

    public class CellIndexIsOutOfRange : Exception
    {
        public CellIndexIsOutOfRange(string message)
            : base(message)
        {
        }
    }

    public class RecursiveCellCallException : Exception
    {
        public RecursiveCellCallException(string message) 
            : base(message)
        {
            
        }
    }
}
