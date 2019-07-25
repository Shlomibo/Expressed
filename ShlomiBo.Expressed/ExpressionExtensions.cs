using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ShlomiBo.Expressed
{
	using static Expression;

	public static partial class ExpressionExtensions
	{
		#region Methods

		public static Expression<Func<TInput, TOutput>> CombineBodyWith<TInput, TMiddle, TOutput>(
			this Expression<Func<TInput, TMiddle>> first,
			Expression<Func<TInput, TMiddle, TOutput>> second)
		{
			if (first == null)
			{
				throw new ArgumentNullException(nameof(first));
			}
			if (second == null)
			{
				throw new ArgumentNullException(nameof(second));
			}

			var firstParam = first.Parameters.Single();
			var firstBody = first.Body;

			var secondInputParam = second.Parameters.First();
			var secondMiddleResultParam = second.Parameters[1];

			var body = second.Body
				.ReplaceSubExpression(secondInputParam, firstParam)
				.ReplaceSubExpression(secondMiddleResultParam, firstBody);

			return Lambda<Func<TInput, TOutput>>(body, firstParam);
		}

		public static Expression<Func<TInput, TOutput>> CombineWith<TInput, TMiddle, TOutput>(
			this Expression<Func<TInput, TMiddle>> first,
			Expression<Func<TMiddle, TOutput>> second)
		{
			if (first == null)
			{
				throw new ArgumentNullException(nameof(first));
			}
			if (second == null)
			{
				throw new ArgumentNullException(nameof(second));
			}

			var input = first.Parameters.Single();

			var body = second.Body.ReplaceSubExpression(
				second.Parameters.Single(),
				first.Body);

			return Lambda<Func<TInput, TOutput>>(body, input);
		}

		public static Expression<Func<TInput, TOutput>> PrependWith<TInput, TMiddle, TOutput>(
			this Expression<Func<TMiddle, TOutput>> second,
			Expression<Func<TInput, TMiddle>> first)
		{
			return first.CombineWith(second);
		}

		public static Expression<Func<(T1, T1), T>> ToMultiArry<T1, T2, T>(
			this Expression<Func<T1, T1, T>> twoArgsExpression)
		{
			if (twoArgsExpression == null)
			{
				throw new ArgumentNullException(nameof(twoArgsExpression));
			}

			var arg2 = twoArgsExpression.Parameters[1];

			var tupleType = typeof((T1, T2));
			var tupleArg = Parameter(typeof((T1, T2)));

			var firstAccess = MakeMemberAccess(
				tupleArg,
				tupleType.GetProperty(nameof(ValueTuple<T1, T2>.Item1)));
			var secondsAccess = MakeMemberAccess(
				tupleArg,
				tupleType.GetProperty(nameof(ValueTuple<T1, T2>.Item2)));

			return Lambda<Func<(T1, T1), T>>(
				twoArgsExpression.Body.ReplaceSubExpression(new Dictionary<Expression, Expression>
				{
					[twoArgsExpression.Parameters[0]] = firstAccess,
					[twoArgsExpression.Parameters[1]] = secondsAccess,
				}),
				tupleArg);
		}

		public static Expression<Func<T1, T1, T>> ToOneArry<T1, T2, T>(
			this Expression<Func<(T1, T1), T>> pairExpression)
		{
			if (pairExpression == null)
			{
				throw new ArgumentNullException(nameof(pairExpression));
			}

			var arg1 = Parameter(typeof(T1));
			var arg2 = Parameter(typeof(T2));
			var tupleType = typeof(ValueTuple<T1, T2>);

			return Lambda<Func<T1, T1, T>>(
				pairExpression.Body.ReplaceSubExpression(TupleReplacement),
				arg1,
				arg2);

			Expression TupleReplacement(Expression exp)
			{
				if (!(exp is MemberExpression member) ||
					member.Type != tupleType)
				{
					return null;
				}

				return member.Member == tupleType.GetProperty(nameof(ValueTuple<T1, T2>.Item1)) ? arg1 :
					member.Member == tupleType.GetProperty(nameof(ValueTuple<T1, T2>.Item2)) ? arg2 :
					null;
			}
		}

		#endregion Methods
	}
}
