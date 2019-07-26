using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ShlomiBo.Expressed
{
	using static Expression;

	/// <summary>
	/// Provides Expression extensions
	/// </summary>
	public static partial class ExpressionExtensions
	{
		#region Methods

		/// <summary>
		/// Combines the body from an expression, into a second expression, having access to the input
		/// </summary>
		/// <typeparam name="TInput">Type of expression input</typeparam>
		/// <typeparam name="TMiddle">Result type of this expression</typeparam>
		/// <typeparam name="TOutput">Result type of result expression</typeparam>
		/// <param name="first">This expression arg</param>
		/// <param name="second">Seconds expression arg</param>
		/// <returns>
		/// An expression from that gets TInput, and returns the seconds expression result,
		/// by inlining the first expression and the input into the second expression
		/// </returns>
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

		/// <summary>
		/// Creates an expression from the input of this expression, to the output of the other expression,
		/// by inlining the body of the first expression into the second
		/// </summary>
		/// <seealso cref="PrependTo{TInput, TMiddle, TOutput}(Expression{Func{TInput, TMiddle}}, Expression{Func{TMiddle, TOutput}})"/>
		/// <typeparam name="TInput">The input type</typeparam>
		/// <typeparam name="TMiddle">The output of the first expression, and input of the second</typeparam>
		/// <typeparam name="TOutput">The output of the seconds expression</typeparam>
		/// <param name="first">This expression</param>
		/// <param name="second">The expression to pipe the output to</param>
		/// <returns>An expression from the input of this expression, to the output of the other expression</returns>
		public static Expression<Func<TInput, TOutput>> Pipe<TInput, TMiddle, TOutput>(
			this Expression<Func<TInput, TMiddle>> first,
			Expression<Func<TMiddle, TOutput>> second)
		{
			return first.PrependTo(second);
		}

		/// <summary>
		/// Creates an expression from the input of this expression, to the output of the other expression,
		/// by inlining the body of the first expression into the second
		/// </summary>
		/// <seealso cref="Pipe{TInput, TMiddle, TOutput}(Expression{Func{TInput, TMiddle}}, Expression{Func{TMiddle, TOutput}})"/>
		/// <typeparam name="TInput">The input type</typeparam>
		/// <typeparam name="TMiddle">The output of the first expression, and input of the second</typeparam>
		/// <typeparam name="TOutput">The output of the seconds expression</typeparam>
		/// <param name="first">This expression</param>
		/// <param name="second">The expression to pipe the output to</param>
		/// <returns>An expression from the input of this expression, to the output of the other expression</returns>
		public static Expression<Func<TInput, TOutput>> PrependTo<TInput, TMiddle, TOutput>(
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

		/// <summary>
		/// Creates an expression from the input of the other expression, to the output of this expression,
		/// by inlining the body of the first expression into the second
		/// </summary>
		/// <seealso cref="AppendTo{TInput, TMiddle, TOutput}(Expression{Func{TMiddle, TOutput}}, Expression{Func{TInput, TMiddle}})"/>
		/// <typeparam name="TInput">The input type</typeparam>
		/// <typeparam name="TMiddle">The output of the first expression, and input of the second</typeparam>
		/// <typeparam name="TOutput">The output of the seconds expression</typeparam>
		/// <param name="first">This expression</param>
		/// <param name="second">The expression to compose with this expression</param>
		/// <returns>An expression from the input of the other expression, to the output of this expression</returns>
		public static Expression<Func<TInput, TOutput>> Compose<TInput, TMiddle, TOutput>(
			this Expression<Func<TMiddle, TOutput>> second,
			Expression<Func<TInput, TMiddle>> first)
		{
			return second.AppendTo(first);
		}

		/// <summary>
		/// Creates an expression from the input of the other expression, to the output of this expression,
		/// by inlining the body of the first expression into the second
		/// </summary>
		/// <seealso cref="Compose{TInput, TMiddle, TOutput}(Expression{Func{TMiddle, TOutput}}, Expression{Func{TInput, TMiddle}})"/>
		/// <typeparam name="TInput">The input type</typeparam>
		/// <typeparam name="TMiddle">The output of the first expression, and input of the second</typeparam>
		/// <typeparam name="TOutput">The output of the seconds expression</typeparam>
		/// <param name="first">This expression</param>
		/// <param name="second">The expression to compose with this expression</param>
		/// <returns>An expression from the input of the other expression, to the output of this expression</returns>
		public static Expression<Func<TInput, TOutput>> AppendTo<TInput, TMiddle, TOutput>(
			this Expression<Func<TMiddle, TOutput>> second,
			Expression<Func<TInput, TMiddle>> first)
		{
			return first.PrependTo(second);
		}

		#endregion Methods
	}
}
