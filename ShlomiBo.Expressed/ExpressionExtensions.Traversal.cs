using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ShlomiBo.Expressed
{
	using static Expression;

	partial class ExpressionExtensions
	{
		#region Methods

		/// <summary>
		/// Traverse an expression tree, allowing generation of new expressions from it
		/// </summary>
		/// <param name="expression">The expression to traverse</param>
		/// <param name="traversal">A function that run against each sub-expression</param>
		/// <returns>
		/// Traversal result that determines the result of the traversal,
		/// if subexpressions should be traversed as well, and if traversal should continue
		/// </returns>
		public static TraversalResult Traverse(
			this Expression expression,
			Func<Expression, ExpressionTraversal> traversal)
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}
			if (traversal == null)
			{
				throw new ArgumentNullException(nameof(traversal));
			}

			bool didBreak = false;
			return Implementation(expression);

			TraversalResult Implementation(Expression exp)
			{
				if (exp is null)
				{
					return new TraversalResult(null, true);
				}
				if (didBreak)
				{
					return new TraversalResult(exp, false);
				}

				ExpressionTraversal traversalResult = traversal(exp) ?? ExpressionTraversal.Default;

				exp = traversalResult.NewExpression ?? exp;

				if (traversalResult.Break || !traversalResult.TraverseSubTree)
				{
					if (traversalResult.Break)
					{
						didBreak = true;
					}

					return new TraversalResult(exp, !traversalResult.Break);
				}

				switch (exp)
				{
					case UnaryExpression unary:
						return UnaryExpressions(unary);

					case BinaryExpression binary:
						return BinaryExpressions(binary);

					case MethodCallExpression methodCall:
						return MethodCall(methodCall, Call);

					case ConditionalExpression conditional:
						return Conditional(conditional, Condition);

					case InvocationExpression invocation:
						return Invocation(invocation, Invoke);

					case LambdaExpression lambda:
						return LambdaExpression(lambda, Lambda);

					case ListInitExpression listInit:
						return ListInitExpression(listInit, ListInit);

					case MemberExpression member:
						return MemberExpression(member, MakeMemberAccess);

					case NewExpression @new:
						return NewExpression(@new, New);

					case NewArrayExpression newArray:
						return NewArrayExpressions(newArray);

					case TypeBinaryExpression typeBinary:
						return TypeBinaryExpressions(typeBinary);

					case BlockExpression block:
						return BlockExpression(block, Block);

					case DynamicExpression dynamic:
						return DynamicExpression(dynamic, Dynamic);

					case GotoExpression @goto:
						return GotoExpression(@goto, Goto);

					case IndexExpression index:
						return IndexExpression(index, MakeIndex);

					case LabelExpression label:
						return LabelExpression(label, Label);

					case RuntimeVariablesExpression runtimeVariables:
						return RuntimeVariablesExpression(runtimeVariables, RuntimeVariables);

					case LoopExpression loop:
						return LoopExpression(loop, Loop);

					case SwitchExpression @switch:
						return SwitchExpression(@switch, Switch);

					case TryExpression @try:
						return TryExpression(@try, TryCatchFinally);

					default:
						return exp;
				}
			}

			TraversalResult Invocation(
				InvocationExpression invocation,
				Func<Expression, IEnumerable<Expression>, InvocationExpression> create)
			{
				var (exp, wholeExp) = Implementation(invocation.Expression);
				var args = invocation.Arguments
					.Select(arg => Implementation(arg))
					.ToArray();

				var result = create(
					exp,
					args.Select(arg => arg.Result));

				return new TraversalResult(result, wholeExp && args.All(arg => arg.TraversedWholeExpression));
			}

			TraversalResult Conditional(
				ConditionalExpression conditional,
				Func<Expression, Expression, Expression, ConditionalExpression> create)
			{
				var (test, wholeTest) = Implementation(conditional.Test);
				var (ifTrue, wholeTrue) = Implementation(conditional.IfTrue);
				var (ifFaluse, wholeFalse) = Implementation(conditional.IfFalse);

				var result = create(
					test,
					ifTrue,
					ifFaluse);

				return new TraversalResult(result, wholeTest && wholeTrue && wholeFalse);
			}

			TraversalResult MethodCall(
				MethodCallExpression methodCall,
				Func<Expression, MethodInfo, IEnumerable<Expression>, MethodCallExpression> create)
			{
				var (instance, wholeInstance) = Implementation(methodCall.Object);
				var args = methodCall.Arguments
					.Select(arg => Implementation(arg))
					.ToArray();

				var result = create(instance, methodCall.Method, args.Select(arg => arg.Result));

				return new TraversalResult(result, wholeInstance && args.All(arg => arg.TraversedWholeExpression));
			}

			TraversalResult UnaryExpressions(
				UnaryExpression unary)
			{
				switch (unary.NodeType)
				{
					case ExpressionType.ArrayLength:
						return Unary(unary, ArrayLength);

					case ExpressionType.Convert:
						return Unary(unary, ex => Convert(ex, unary.Type));

					case ExpressionType.ConvertChecked:
						return Unary(unary, ex => ConvertChecked(ex, unary.Type));

					case ExpressionType.Negate:
						return Unary(unary, Negate);

					case ExpressionType.UnaryPlus:
						return Unary(unary, UnaryPlus);

					case ExpressionType.NegateChecked:
						return Unary(unary, NegateChecked);

					case ExpressionType.Not:
						return Unary(unary, Not);

					case ExpressionType.Quote:
						return Unary(unary, Quote);

					case ExpressionType.TypeAs:
						return Unary(unary, ex => TypeAs(ex, unary.Type));

					case ExpressionType.Decrement:
						return Unary(unary, Decrement);

					case ExpressionType.Increment:
						return Unary(unary, Increment);

					case ExpressionType.Throw:
						return Unary(unary, ex => Throw(ex, unary.Type));

					case ExpressionType.Unbox:
						return Unary(unary, ex => Unbox(ex, unary.Type));

					case ExpressionType.PreIncrementAssign:
						return Unary(unary, PreIncrementAssign);

					case ExpressionType.PreDecrementAssign:
						return Unary(unary, PreDecrementAssign);

					case ExpressionType.PostIncrementAssign:
						return Unary(unary, PostIncrementAssign);

					case ExpressionType.PostDecrementAssign:
						return Unary(unary, PostIncrementAssign);

					case ExpressionType.OnesComplement:
						return Unary(unary, OnesComplement);

					case ExpressionType.IsTrue:
						return Unary(unary, IsTrue);

					case ExpressionType.IsFalse:
						return Unary(unary, IsFalse);

					default:
						throw new InvalidOperationException($"Invalid unary expression: {unary.NodeType}");
				}
			}

			TraversalResult Unary(
				UnaryExpression unary,
				Func<Expression, UnaryExpression> create)
			{
				var (exp, whole) = Implementation(unary.Operand);

				var result = create(exp);

				return new TraversalResult(result, whole);
			}

			TraversalResult BinaryExpressions(BinaryExpression binaryExpression)
			{
				switch (binaryExpression.NodeType)
				{
					case ExpressionType.Add:
						return Binary(binaryExpression, Add);

					case ExpressionType.AddChecked:
						return Binary(binaryExpression, AddChecked);

					case ExpressionType.And:
						return Binary(binaryExpression, And);

					case ExpressionType.AndAlso:
						return Binary(binaryExpression, AndAlso);

					case ExpressionType.ArrayIndex:
						return Binary(binaryExpression, ArrayIndex);

					case ExpressionType.Coalesce:
						return Binary(binaryExpression, Coalesce);

					case ExpressionType.Divide:
						return Binary(binaryExpression, Divide);

					case ExpressionType.Equal:
						return Binary(binaryExpression, Equal);

					case ExpressionType.ExclusiveOr:
						return Binary(binaryExpression, ExclusiveOr);

					case ExpressionType.GreaterThan:
						return Binary(binaryExpression, GreaterThan);

					case ExpressionType.GreaterThanOrEqual:
						return Binary(binaryExpression, GreaterThanOrEqual);

					case ExpressionType.LeftShift:
						return Binary(binaryExpression, LeftShift);

					case ExpressionType.LessThan:
						return Binary(binaryExpression, LessThan);

					case ExpressionType.LessThanOrEqual:
						return Binary(binaryExpression, LessThanOrEqual);

					case ExpressionType.Modulo:
						return Binary(binaryExpression, Modulo);

					case ExpressionType.Multiply:
						return Binary(binaryExpression, Multiply);

					case ExpressionType.MultiplyChecked:
						return Binary(binaryExpression, MultiplyChecked);

					case ExpressionType.NotEqual:
						return Binary(binaryExpression, NotEqual);

					case ExpressionType.Or:
						return Binary(binaryExpression, Or);

					case ExpressionType.OrElse:
						return Binary(binaryExpression, OrElse);

					case ExpressionType.Power:
						return Binary(binaryExpression, Power);

					case ExpressionType.RightShift:
						return Binary(binaryExpression, RightShift);

					case ExpressionType.Subtract:
						return Binary(binaryExpression, Subtract);

					case ExpressionType.SubtractChecked:
						return Binary(binaryExpression, SubtractChecked);

					case ExpressionType.Assign:
						return Binary(binaryExpression, Assign);

					case ExpressionType.AddAssign:
						return Binary(binaryExpression, AddAssign);

					case ExpressionType.AndAssign:
						return Binary(binaryExpression, AndAssign);

					case ExpressionType.DivideAssign:
						return Binary(binaryExpression, DivideAssign);

					case ExpressionType.ExclusiveOrAssign:
						return Binary(binaryExpression, ExclusiveOrAssign);

					case ExpressionType.LeftShiftAssign:
						return Binary(binaryExpression, LeftShiftAssign);

					case ExpressionType.ModuloAssign:
						return Binary(binaryExpression, ModuloAssign);

					case ExpressionType.MultiplyAssign:
						return Binary(binaryExpression, MultiplyAssign);

					case ExpressionType.OrAssign:
						return Binary(binaryExpression, OrAssign);

					case ExpressionType.PowerAssign:
						return Binary(binaryExpression, PowerAssign);

					case ExpressionType.RightShiftAssign:
						return Binary(binaryExpression, RightShiftAssign);

					case ExpressionType.SubtractAssign:
						return Binary(binaryExpression, SubtractAssign);

					case ExpressionType.AddAssignChecked:
						return Binary(binaryExpression, AddAssignChecked);

					case ExpressionType.MultiplyAssignChecked:
						return Binary(binaryExpression, MultiplyAssignChecked);

					case ExpressionType.SubtractAssignChecked:
						return Binary(binaryExpression, SubtractAssignChecked);

					default:
						throw new InvalidOperationException($"Unknown expression type: [{binaryExpression.NodeType}]");
				}
			}

			TraversalResult Binary(
				BinaryExpression binaryExpression,
				Func<Expression, Expression, BinaryExpression> create)
			{
				var (left, wholeLeft) = Implementation(binaryExpression.Left);
				var (right, wholeRight) = Implementation(binaryExpression.Right);

				var result = create(
					left,
					right);

				return new TraversalResult(result, wholeLeft && wholeRight);
			}

			TraversalResult LambdaExpression(
				LambdaExpression lambda,
				Func<Type, Expression, string, bool, IEnumerable<ParameterExpression>, LambdaExpression> create)
			{
				var (body, wholeBody) = Implementation(lambda.Body);
				var prms = lambda.Parameters
					.Select(p => Implementation(p))
					.ToArray();

				var result = create(
					lambda.Type,
					body,
					lambda.Name,
					lambda.TailCall,
					prms
						.Select(prm => prm.Result)
						.Cast<ParameterExpression>());

				return new TraversalResult(result, wholeBody && prms.All(prm => prm.TraversedWholeExpression));
			}

			TraversalResult ListInitExpression(
				ListInitExpression listInit,
				Func<NewExpression, MethodInfo, IEnumerable<Expression>, ListInitExpression> create)
			{
				var (@new, wholeNew) = Implementation(listInit.NewExpression);
				var initArgs = listInit.Initializers
						.SelectMany(init => init.Arguments)
						.Select(arg => Implementation(arg))
						.ToArray();

				var result = create(
					(NewExpression)@new,
					listInit.Initializers
						.Select(init => init.AddMethod)
						.FirstOrDefault(),
					initArgs.Select(arg => arg.Result));

				return new TraversalResult(result, wholeNew && initArgs.All(arg => arg.TraversedWholeExpression));
			}

			TraversalResult MemberExpression(
				MemberExpression member,
				Func<Expression, MemberInfo, MemberExpression> create)
			{
				var (ex, whole) = Implementation(member.Expression);

				var result = create(
					ex,
					member.Member);

				return new TraversalResult(result, whole);
			}

			TraversalResult NewExpression(
				NewExpression @new,
				Func<ConstructorInfo, IEnumerable<Expression>, IEnumerable<MemberInfo>, NewExpression> create)
			{
				var args = @new.Arguments
					.Select(arg => Implementation(arg))
					.ToArray();

				var result = create(
					@new.Constructor,
					args.Select(arg => arg.Result),
					@new.Members);

				return new TraversalResult(result, args.All(arg => arg.TraversedWholeExpression));
			}

			TraversalResult NewArrayExpressions(
				NewArrayExpression newArray)
			{
				switch (newArray.NodeType)
				{
					case ExpressionType.NewArrayInit:
						return NewArrayExpression(newArray, NewArrayInit);

					case ExpressionType.NewArrayBounds:
						return NewArrayExpression(newArray, NewArrayBounds);


					default:
						throw new InvalidOperationException($"Invalid NewArrayExpression [{newArray.NodeType}]");
				}
			}

			TraversalResult NewArrayExpression(
				NewArrayExpression newArray,
				Func<Type, IEnumerable<Expression>, NewArrayExpression> create)
			{
				var expressions = newArray.Expressions
					.Select(exp => Implementation(exp))
					.ToArray();

				var result = create(
					newArray.Type,
					expressions.Select(e => e.Result));

				return new TraversalResult(result, expressions.All(e => e.TraversedWholeExpression));
			}

			TraversalResult TypeBinaryExpressions(
				TypeBinaryExpression typeBinary)
			{
				switch (typeBinary.NodeType)
				{
					case ExpressionType.TypeIs:
						return TypeBinaryExpression(typeBinary, TypeIs);

					case ExpressionType.TypeEqual:
						return TypeBinaryExpression(typeBinary, TypeEqual);

					default:
						throw new InvalidOperationException($"Invalid TypeBinaryExpression [{typeBinary.NodeType}]");
				}
			}

			TraversalResult TypeBinaryExpression(
				TypeBinaryExpression typeBinary,
				Func<Expression, Type, TypeBinaryExpression> create)
			{
				var (exp, whole) = Implementation(typeBinary.Expression);

				var result = create(
					exp,
					typeBinary.TypeOperand);

				return new TraversalResult(result, whole);
			}

			TraversalResult BlockExpression(
				BlockExpression block,
				Func<Type, IEnumerable<ParameterExpression>, IEnumerable<Expression>, BlockExpression> create)
			{
				var vars = block.Variables
						.Select(var => Implementation(var))
						.ToArray();
				var blocks = block.Expressions
					.Select(exp => Implementation(exp))
					.ToArray();

				var result = create(
					block.Type,
					vars
						.Select(var => var.Result)
						.Cast<ParameterExpression>(),
					blocks.Select(b => b.Result));

				return new TraversalResult(
					result,
					vars.All(v => v.TraversedWholeExpression) &&
						blocks.All(b => b.TraversedWholeExpression));
			}

			TraversalResult DynamicExpression(
				DynamicExpression dynamic,
				Func<CallSiteBinder, Type, IEnumerable<Expression>, DynamicExpression> create)
			{
				var args = dynamic.Arguments
					.Select(arg => Implementation(arg))
					.ToArray();

				var result = create(
					dynamic.Binder,
					dynamic.Type,
					args.Select(arg => arg.Result));

				return new TraversalResult(result, args.All(arg => arg.TraversedWholeExpression));
			}

			TraversalResult GotoExpression(
				GotoExpression @goto,
				Func<LabelTarget, Expression, Type, GotoExpression> create)
			{
				var (val, wholeVal) = Implementation(@goto.Value);

				var result = create(
					@goto.Target,
					val,
					@goto.Type);

				return new TraversalResult(result, wholeVal);
			}

			TraversalResult IndexExpression(
				IndexExpression index,
				Func<Expression, PropertyInfo, IEnumerable<Expression>, IndexExpression> create)
			{
				var (obj, wholeObj) = Implementation(index.Object);
				var args = index.Arguments
					.Select(arg => Implementation(arg))
					.ToArray();

				var result = create(
					obj,
					index.Indexer,
					args.Select(arg => arg.Result));

				return new TraversalResult(result, wholeObj && args.All(arg => arg.TraversedWholeExpression));
			}

			TraversalResult LabelExpression(
				LabelExpression label,
				Func<LabelTarget, Expression, LabelExpression> create)
			{
				var (defValue, wholeLabel) = Implementation(label.DefaultValue);

				var result = create(
					label.Target,
					defValue);

				return new TraversalResult(defValue, wholeLabel);
			}

			TraversalResult RuntimeVariablesExpression(
				RuntimeVariablesExpression runtimeVariables,
				Func<IEnumerable<ParameterExpression>, RuntimeVariablesExpression> create)
			{
				var vars = runtimeVariables.Variables
						.Select(var => Implementation(var))
						.ToArray();

				var result = create(vars.Select(varResult => (ParameterExpression)varResult.Result));

				return new TraversalResult(result, vars.All(v => v.TraversedWholeExpression));
			}

			TraversalResult LoopExpression(
				LoopExpression loop,
				Func<Expression, LabelTarget, LabelTarget, LoopExpression> create)
			{
				var (body, wholeBody) = Implementation(loop.Body);
				var result = create(
					body,
					loop.BreakLabel,
					loop.ContinueLabel);

				return new TraversalResult(result, wholeBody);
			}

			TraversalResult SwitchExpression(
				SwitchExpression @switch,
				Func<Type, Expression, Expression, MethodInfo, IEnumerable<SwitchCase>, SwitchExpression> create)
			{
				var (value, wholeVal) = Implementation(@switch.SwitchValue);
				var (defaultBody, wholeDefault) = Implementation(@switch.DefaultBody);
				var switches = @switch.Cases
					.Select(@case =>
					{
						var values = @case.TestValues
							.Select(switchValue => Implementation(switchValue))
							.ToArray();

						var (caseBody, wholeCaseBody) = Implementation(@case.Body);

						var resultCase = @case.Update(
							   values.Select(val => val.Result),
							   caseBody);

						return (resultCase, wholeCase: values.All(val => val.TraversedWholeExpression) && wholeCaseBody);
					})
					.ToArray();

				var result = create(
					@switch.Type,
					value,
					defaultBody,
					@switch.Comparison,
					switches.Select(@case => @case.resultCase));

				return new TraversalResult(result, wholeVal && wholeDefault && switches.All(s => s.wholeCase));
			}

			TraversalResult TryExpression(
				TryExpression @try,
				 Func<Expression, Expression, CatchBlock[], TryExpression> create)
			{
				var (body, wholeBody) = Call(@try.Body);
				var (@finally, wholeFinally) = Call(@try.Finally);
				var catchBlocks = @try.Handlers
						.Select(h =>
						{
							var (variable, wholeVar) = Call(h.Variable);
							var (filter, wholeFilter) = Call(h.Filter);
							var (catchBody, wholeCatchBody) = Call(h.Body);

							return (
								catchBlock: h.Update(
									(ParameterExpression)variable,
									filter,
									catchBody),
								whole: wholeVar && wholeFilter && wholeCatchBody);
						})
						.ToArray();

				var resultExp = create(
					body,
					@finally,
					catchBlocks
						.Select(block => block.catchBlock)
						.ToArray());

				return new TraversalResult(resultExp, wholeBody && wholeFinally && catchBlocks.All(b => b.whole));

				TraversalResult Call(Expression exp) =>
					Implementation(exp);
			}
		}

		#endregion Methods

		#region Structs

		/// <summary>
		/// A type represnts the result of subexression traversal
		/// </summary>
		public struct TraversalResult
		{
			#region Properties

			/// <summary>
			/// Gets the result subexpression
			/// </summary>
			public Expression Result { get; }
			/// <summary>
			/// Gets a value that indicates if all of the input expression was traversed
			/// </summary>
			public bool TraversedWholeExpression { get; }
			#endregion Properties

			#region Constructors

			/// <summary>
			/// Creates new intance
			/// </summary>
			/// <param name="result">The result subexpression</param>
			/// <param name="traversedWholeExpression">
			/// A value that indicates if all of the input expression was traversed
			/// </param>
			public TraversalResult(Expression result, bool traversedWholeExpression)
			{
				this.Result = result;
				this.TraversedWholeExpression = traversedWholeExpression;
			}

			#endregion Constructors

			#region Methods

			/// <summary>
			/// </summary>
			/// <param name="result"></param>
			public static implicit operator TraversalResult(Expression result) =>
				result is null
					? default
					: new TraversalResult(result, true);

			/// <summary>
			/// </summary>
			/// <param name="expression"></param>
			/// <param name="traversedWholeExpression"></param>
			public void Deconstruct(out Expression expression, out bool traversedWholeExpression)
			{
				expression = this.Result;
				traversedWholeExpression = this.TraversedWholeExpression;
			}

			#endregion Methods
		}

		#endregion Structs

		#region Classes

		/// <summary>
		/// 
		/// </summary>
		public sealed class ExpressionTraversal
		{
			#region Properties

			/// <summary>
			/// Gets the default traversal which is - keep the input expression,
			/// but traverse its subexpressions
			/// </summary>
			public static ExpressionTraversal Default => new ExpressionTraversal
			{
				TraverseSubTree = true,
			};

			/// <summary>
			/// Gets a value indicates if traversal should continue
			/// </summary>
			public bool Break { get; set; }
			/// <summary>
			/// Gets the result expression for the traversal, or null, to keep the input expression
			/// </summary>
			public Expression NewExpression { get; set; }
			/// <summary>
			/// A value indicates if traversal of current expression should continue to its subexpressions
			/// or siblings
			/// </summary>
			public bool TraverseSubTree { get; set; } = true;
			#endregion Properties
		}

		#endregion Classes
	}
}
