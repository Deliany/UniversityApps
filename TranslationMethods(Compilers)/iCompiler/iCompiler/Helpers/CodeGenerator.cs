using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace iCompiler.Helpers
{
    public sealed class Generator
    {
        private ILGenerator ilgenerator;
        private Dictionary<string, LocalBuilder> symbolsTable;

        public Generator(Statement stmt, string moduleName)
        {
            var path = Path.GetFileNameWithoutExtension(moduleName);
            var assemblyName = new AssemblyName(path);
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
            var typeBuilder = moduleBuilder.DefineType(moduleName);

            var methodBuilder = typeBuilder.DefineMethod("Main", MethodAttributes.Static, typeof(void), Type.EmptyTypes);

            // CodeGenerator
            ilgenerator = methodBuilder.GetILGenerator();
            symbolsTable = new Dictionary<string, LocalBuilder>();

            // Go Compile!
            GenerateStatements(stmt);

            ilgenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("ReadKey", BindingFlags.Public | BindingFlags.Static, null, new Type[] { }, null));
            ilgenerator.Emit(OpCodes.Ret);
            typeBuilder.CreateType();
            moduleBuilder.CreateGlobalFunctions();
            assemblyBuilder.SetEntryPoint(methodBuilder);
            assemblyBuilder.Save(moduleName);

            symbolsTable = null;
            ilgenerator = null;
        }

        private void GenerateStatements(Statement stmt)
        {
            if (stmt is StatementList)
            {
                var seq = (StatementList)stmt;

                GenerateStatements(seq.First);
                GenerateStatements(seq.Second);
            }
            else if (stmt is DeclareVariable)
            {
                var declare = (DeclareVariable)stmt;

                symbolsTable[declare.Ident] = ilgenerator.DeclareLocal(TypeOfExpr(declare.Expr));

                var assign = new Assign { Ident = declare.Ident, Expr = declare.Expr };

                GenerateStatements(assign);
            }
            else if (stmt is Assign)
            {
                var assign = (Assign)stmt;

                GenerateExpression(assign.Expr, TypeOfExpr(assign.Expr));
                Store(assign.Ident, TypeOfExpr(assign.Expr));
            }
            else if (stmt is Print)
            {
                GenerateExpression(((Print)stmt).Expr, typeof(string));
                ilgenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));
            }

            else if (stmt is ReadInteger)
            {
                ilgenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("ReadLine", BindingFlags.Public | BindingFlags.Static, null, new Type[] { }, null));
                ilgenerator.Emit(OpCodes.Call, typeof(int).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null));
                Store(((ReadInteger)stmt).Ident, typeof(int));
            }
            else if (stmt is ForLoop)
            {
                var forLoop = (ForLoop)stmt;
                var assign = new Assign { Ident = forLoop.Ident, Expr = forLoop.From };
                GenerateStatements(assign);

                var test = ilgenerator.DefineLabel();
                ilgenerator.Emit(OpCodes.Br, test);

                var body = ilgenerator.DefineLabel();
                ilgenerator.MarkLabel(body);
                GenerateStatements(forLoop.Body);

                ilgenerator.Emit(OpCodes.Ldloc, symbolsTable[forLoop.Ident]);
                ilgenerator.Emit(OpCodes.Ldc_I4, 1);
                ilgenerator.Emit(OpCodes.Add);
                Store(forLoop.Ident, typeof(int));

                ilgenerator.MarkLabel(test);
                ilgenerator.Emit(OpCodes.Ldloc, symbolsTable[forLoop.Ident]);
                GenerateExpression(forLoop.To, typeof(int));
                ilgenerator.Emit(OpCodes.Blt, body);
            }
            else
            {
                throw new CodeGeneratorException("Cant generate a " + stmt.GetType().Name);
            }
        }

        private void GenerateExpression(Expression expr, Type expectedType)
        {
            Type deliveredType;

            if (expr is StringLiteral)
            {
                deliveredType = typeof(string);
                ilgenerator.Emit(OpCodes.Ldstr, ((StringLiteral)expr).Value);
            }
            else if (expr is IntegerLiteral)
            {
                deliveredType = typeof(int);
                ilgenerator.Emit(OpCodes.Ldc_I4, ((IntegerLiteral)expr).Value);
            }
            else if (expr is Variable)
            {
                var ident = ((Variable)expr).Ident;
                deliveredType = TypeOfExpr(expr);

                if (!symbolsTable.ContainsKey(ident))
                {
                    throw new CodeGeneratorException("Undeclared variable '" + ident + "'");
                }

                ilgenerator.Emit(OpCodes.Ldloc, symbolsTable[ident]);
            }
            else if (expr is BinaryExpression)
            {
                deliveredType = typeof(int);

                GenerateExpression(((BinaryExpression)expr).Left, typeof(int));
                GenerateExpression(((BinaryExpression)expr).Right, typeof(int));

                var binExpr = (BinaryExpression)expr;

                switch (binExpr.Op)
                {
                    case BinaryOperator.Add:
                        ilgenerator.Emit(OpCodes.Add);
                        break;
                    case BinaryOperator.Sub:
                        ilgenerator.Emit(OpCodes.Sub);
                        break;
                    case BinaryOperator.Mul:
                        ilgenerator.Emit(OpCodes.Mul);
                        break;
                    case BinaryOperator.Div:
                        ilgenerator.Emit(OpCodes.Div);
                        break;
                }
            }
            else
            {
                throw new CodeGeneratorException("Unable to generate " + expr.GetType().Name);
            }

            if (deliveredType != expectedType)
            {
                if (deliveredType == typeof(int) && expectedType == typeof(string))
                {
                    ilgenerator.Emit(OpCodes.Box, typeof(int));
                    ilgenerator.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
                }
                else
                {
                    throw new CodeGeneratorException("Can't convert type " + deliveredType.Name + " to " + expectedType.Name);
                }
            }
        }

        private Type TypeOfExpr(Expression expr)
        {
            if (expr is StringLiteral)
            {
                return typeof(string);
            }

            if (expr is IntegerLiteral)
            {
                return typeof(int);
            }

            if (expr is Variable)
            {
                var var = (Variable)expr;

                if (symbolsTable.ContainsKey(var.Ident))
                {
                    return symbolsTable[var.Ident].LocalType;
                }

                throw new CodeGeneratorException("Undeclared variable '" + var.Ident + "'");
            }

            if (expr is BinaryExpression)
            {
                return typeof(int);
            }

            throw new CodeGeneratorException("Unable to find the type of " + expr.GetType().Name);
        }

        private void Store(string name, Type type)
        {
            if (!symbolsTable.ContainsKey(name))
            {
                throw new CodeGeneratorException("Undeclared variable '" + name + "'");
            }

            var localBuilder = symbolsTable[name];

            if (localBuilder.LocalType == type)
            {
                ilgenerator.Emit(OpCodes.Stloc, symbolsTable[name]);
            }
            else
            {
                throw new CodeGeneratorException("'" + name + "' is of type " + localBuilder.LocalType.Name + " but attempted to store value of type " + type.Name);
            }
        }
    }
}
