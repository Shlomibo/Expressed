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

			return Implementation(expression);

			TraversalResult Implementation(Expression exp)
			{
				if (exp is null)
				{
					return new TraversalResult(null, true);
				}

				ExpressionTraversal traversalResult = traversal(exp) ?? ExpressionTraversal.Default;

				exp = traversalResult.NewExpression ?? exp;

				if (traversalResult.Break || !traversalResult.TraverseSubTree)
				{
					return new TraversalResult(exp, !traversalResult.Break);
				}

				var unary = exp as UnaryExpression;
				var binary = exp as BinaryExpression;
				var conditional = exp as ConditionalExpression;
				var invocation = exp as InvocationExpression;
				var methodCall = exp as MethodCallExpression;
				var lambda = exp as LambdaExpression;
				var listInit = exp as ListInitExpression;
				var member = exp as MemberExpression;
				var @new = exp as NewExpression;
				var newArray = exp as NewArrayExpression;
				var typeBinary = exp as TypeBinaryExpression;
				var block = exp as BlockExpression;
				var @dynamic = exp as DynamicExpression;
				var @goto = exp as GotoExpression;
				var index = exp as IndexExpression;
				var label = exp as LabelExpression;
				var runtimeVariables = exp as RuntimeVariablesExpression;
				var loop = exp as LoopExpression;
				var @switch = exp as SwitchExpression;
				var @try = exp as TryExpression;

				switch (exp.NodeType)
				{
					case ExpressionType.Add:
						return Binary(binary, Add);

					case ExpressionType.AddChecked:
						return Binary(binary, AddChecked);

					case ExpressionType.And:
						return Binary(binary, And);

					case ExpressionType.AndAlso:
						return Binary(binary, AndAlso);

					case ExpressionType.ArrayLength:
						return Unary(unary, ArrayLength);

					case ExpressionType.ArrayIndex:
						return Binary(binary, ArrayIndex);

					case ExpressionType.Call:
						return MethodCall(methodCall, Call);

					case ExpressionType.Coalesce:
						return Binary(binary, Coalesce);

					case ExpressionType.Conditional:
						return Conditional(conditional, Condition);

					case ExpressionType.Convert:
						return Unary(unary, ex => Convert(ex, unary.Type));

					case ExpressionType.ConvertChecked:
						return Unary(unary, ex => ConvertChecked(ex, unary.Type));

					case ExpressionType.Divide:
						return Binary(binary, Divide);

					case ExpressionType.Equal:
						return Binary(binary, Equal);

					case ExpressionType.ExclusiveOr:
						return Binary(binary, ExclusiveOr);

					case ExpressionType.GreaterThan:
						return Binary(binary, GreaterThan);

					case ExpressionType.GreaterThanOrEqual:
						return Binary(binary, GreaterThanOrEqual);

					case ExpressionType.Invoke:
						return Invocation(invocation, Invoke);

					case ExpressionType.Lambda:
						return LambdaExpression(lambda, Lambda);

					case ExpressionType.LeftShift:
						return Binary(binary, LeftShift);

					case ExpressionType.LessThan:
						return Binary(binary, LessThan);

					case ExpressionType.LessThanOrEqual:
						return Binary(binary, LessThanOrEqual);

					case ExpressionType.ListInit:
						return ListInitExpression(listInit, ListInit);

					case ExpressionType.MemberAccess:
						return MemberExpression(member, MakeMemberAccess);

					case ExpressionType.Modulo:
						return Binary(binary, Modulo);

					case ExpressionType.Multiply:
						return Binary(binary, Multiply);

					case ExpressionType.MultiplyChecked:
						return Binary(binary, MultiplyChecked);

					case ExpressionType.Negate:
						return Unary(unary, Negate);

					case ExpressionType.UnaryPlus:
						return Unary(unary, UnaryPlus);

					case ExpressionType.NegateChecked:
						return Unary(unary, NegateChecked);

					case ExpressionType.New:
						return NewExpression(@new, New);

					case ExpressionType.NewArrayInit:
						return NewArrayExpression(newArray, NewArrayInit);

					case ExpressionType.NewArrayBounds:
						return NewArrayExpression(newArray, NewArrayBounds);

					case ExpressionType.Not:
						return Unary(unary, Not);

					case ExpressionType.NotEqual:
						return Binary(binary, NotEqual);

					case ExpressionType.Or:
						return Binary(binary, Or);

					case ExpressionType.OrElse:
						return Binary(binary, OrElse);

					case ExpressionType.Power:
						return Binary(binary, Power);

					case ExpressionType.Quote:
						return Unary(unary, Quote);

					case ExpressionType.RightShift:
						return Binary(binary, RightShift);

					case ExpressionType.Subtract:
						return Binary(binary, Subtract);

					case ExpressionType.SubtractChecked:
						return Binary(binary, SubtractChecked);

					case ExpressionType.TypeAs:
						return Unary(unary, ex => TypeAs(ex, unary.Type));

					case ExpressionType.TypeIs:
						return TypeBinaryExpression(typeBinary, TypeIs);

					case ExpressionType.Assign:
						return Binary(binary, Assign);

					case ExpressionType.Block:
						return BlockExpression(block, Block);

					case ExpressionType.Decrement:
						return Unary(unary, Decrement);

					case ExpressionType.Dynamic:
						return DynamicExpression(dynamic, Dynamic);

					case ExpressionType.Goto:
						return GotoExpression(@goto, Goto);

					case ExpressionType.Increment:
						return Unary(unary, Increment);

					case ExpressionType.Index:
						return IndexExpression(index, MakeIndex);

					case ExpressionType.Label:
						return LabelExpression(label, Label);

					case ExpressionType.RuntimeVariables:
						return RuntimeVariablesExpression(runtimeVariables, RuntimeVariables);

					case ExpressionType.Loop:
						return LoopExpression(loop, Loop);

					case ExpressionType.Switch:
						return SwitchExpression(@switch, Switch);

					case ExpressionType.Throw:
						return Unary(unary, ex => Throw(ex, unary.Type));

					case ExpressionType.Try:
						return TryExpression(@try, TryCatchFinally);

					case ExpressionType.Unbox:
						return Unary(unary, ex => Unbox(ex, unary.Type));

					case ExpressionType.AddAssign:
						return Binary(binary, AddAssign);

					case ExpressionType.AndAssign:
						return Binary(binary, AndAssign);

					case ExpressionType.DivideAssign:
						return Binary(binary, DivideAssign);

					case ExpressionType.ExclusiveOrAssign:
						return Binary(binary, ExclusiveOrAssign);

					case ExpressionType.LeftShiftAssign:
						return Binary(binary, LeftShiftAssign);

					case ExpressionType.ModuloAssign:
						return Binary(binary, ModuloAssign);

					case ExpressionType.MultiplyAssign:
						return Binary(binary, MultiplyAssign);

					case ExpressionType.OrAssign:
						return Binary(binary, OrAssign);

					case ExpressionType.PowerAssign:
						return Binary(binary, PowerAssign);

					case ExpressionType.RightShiftAssign:
						return Binary(binary, RightShiftAssign);

					case ExpressionType.SubtractAssign:
						return Binary(binary, SubtractAssign);

					case ExpressionType.AddAssignChecked:
						return Binary(binary, AddAssignChecked);

					case ExpressionType.MultiplyAssignChecked:
						return Binary(binary, MultiplyAssignChecked);

					case ExpressionType.SubtractAssignChecked:
						return Binary(binary, SubtractAssignChecked);

					case ExpressionType.PreIncrementAssign:
						return Unary(unary, PreIncrementAssign);

					case ExpressionType.PreDecrementAssign:
						return Unary(unary, PreDecrementAssign);

					case ExpressionType.PostIncrementAssign:
						return Unary(unary, PostIncrementAssign);

					case ExpressionType.PostDecrementAssign:
						return Unary(unary, PostIncrementAssign);

					case ExpressionType.TypeEqual:
						return TypeBinaryExpression(typeBinary, TypeEqual);

					case ExpressionType.OnesComplement:
						return Unary(unary, OnesComplement);

					case ExpressionType.IsTrue:
						return Unary(unary, IsTrue);

					case ExpressionType.IsFalse:
						return Unary(unary, IsFalse);

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

			TraversalResult Unary(
				UnaryExpression unary,
				Func<Expression, UnaryExpression> create)
			{
				var (exp, whole) = Implementation(unary.Operand);

				var result = create(exp);

				return new TraversalResult(result, whole);
			}

			TraversalResult Binary(
				BinaryExpression binary,
				Func<Expression, Expression, BinaryExpression> create)
			{
				var (left, wholeLeft) = Implementation(binary.Left);
				var (right, wholeRight) = Implementation(binary.Right);

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

		public struct TraversalResult
		{
			#region Properties

			public Expression Result { get; }
			public bool TraversedWholeExpression { get; }
			#endregion Properties

			#region Constructors

			public TraversalResult(Expression result, bool traversedWholeExpression)
			{
				this.Result = result;
				this.TraversedWholeExpression = traversedWholeExpression;
			}

			#endregion Constructors

			#region Methods

			public static implicit operator TraversalResult(Expression result) =>
				result is null
					? default
					: new TraversalResult(result, true);

			public void Deconstruct(out Expression expression, out bool traversedWholeExpression)
			{
				expression = this.Result;
				traversedWholeExpression = this.TraversedWholeExpression;
			}

			#endregion Methods
		}

		#endregion Structs

		#region Classes

		public sealed class ExpressionTraversal
		{
			#region Properties

			public static ExpressionTraversal Default { get; } = new ExpressionTraversal
			{
				TraverseSubTree = true,
			};

			public bool Break { get; set; }
			public Expression NewExpression { get; set; }
			public bool TraverseSubTree { get; set; } = true;
			#endregion Properties
		}

		#endregion Classes
	}
}
