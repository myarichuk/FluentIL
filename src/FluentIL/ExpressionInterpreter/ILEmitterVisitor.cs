﻿using System;
using System.Linq.Expressions;
using FluentIL.Emitters;

namespace FluentIL.ExpressionInterpreter
{
    public class ILEmitterVisitor : ExpressionVisitor
    {
// ReSharper disable InconsistentNaming
        private readonly DynamicMethodBody IL;
// ReSharper restore InconsistentNaming

        public ILEmitterVisitor(DynamicMethodBody dmb)
        {
            IL = dmb;
        }


        protected override Expression VisitParameter(ParameterExpression node)
        {
            Expression result = base.VisitParameter(node);
            IL.LdLocOrArg(node.Name);
            return result;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Expression result;

            if (node.NodeType == ExpressionType.AndAlso)
            {
                string firstConditionIsFalse = Guid.NewGuid().ToString();
                string jump = Guid.NewGuid().ToString();

                result = node;
                Visit(node.Left);
                IL.Brfalse(firstConditionIsFalse);
                Visit(node.Right);
                IL
                    .Br_S(jump)
                    .MarkLabel(firstConditionIsFalse)
                    .Ldc(0)
                    .MarkLabel(jump);
            }
            else if (node.NodeType == ExpressionType.OrElse)
            {
                string firstConditionIsTrue = Guid.NewGuid().ToString();
                string jump = Guid.NewGuid().ToString();

                result = node;
                Visit(node.Left);
                IL.Brtrue(firstConditionIsTrue);
                Visit(node.Right);
                IL
                    .Br_S(jump)
                    .MarkLabel(firstConditionIsTrue)
                    .Ldc(1)
                    .MarkLabel(jump);
            }
            else
            {
                result = base.VisitBinary(node);
                if (node.NodeType == ExpressionType.Add)
                    IL.Add();
                else if (node.NodeType == ExpressionType.Divide)
                    IL.Div();
                else if (node.NodeType == ExpressionType.Multiply)
                    IL.Mul();
                else if (node.NodeType == ExpressionType.Subtract)
                    IL.Sub();
                else if (node.NodeType == ExpressionType.GreaterThan)
                    IL.Cgt();
                else if (node.NodeType == ExpressionType.GreaterThanOrEqual)
                    IL.Cge();
                else if (node.NodeType == ExpressionType.LessThan)
                    IL.Clt();
                else if (node.NodeType == ExpressionType.LessThanOrEqual)
                    IL.Cle();
                else if (node.NodeType == ExpressionType.Equal)
                    IL.Ceq();
                else
                    throw new NotSupportedException();
            }

            return result;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            Expression result = base.VisitConstant(node);

            if (node.Type == typeof (int))
                IL.Ldc((int) node.Value);
            else if (node.Type == typeof (double))
                IL.Ldc((double) node.Value);
            else if (node.Type == typeof (float))
                IL.Ldc((float) node.Value);
            else if (node.Type == typeof (bool))
                IL.Ldc((bool) node.Value ? 1 : 0);
            else
                throw new NotSupportedException();

            return result;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            Expression result = base.VisitUnary(node);
            if (node.NodeType == ExpressionType.Not)
                IL.Ldc(0).Ceq();
            return result;
        }
    }
}