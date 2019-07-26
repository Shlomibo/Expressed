using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ShlomiBo.Expressed
{
	partial class ExpressionExtensions
	{
		#region Methods

		/// <summary>
		/// Copies this expression, running a function against each sub-expression, and replaces it
		/// whenever that function returns none-null expression
		/// </summary>
		/// <param name="expression">The expression to copy and replace its side expressions</param>
		/// <param name="replacementSelector">
		/// A function that returns new expression to replace. If that function returns an expression,
		/// it is the subexpression in the result expression;
		/// otherwise, it that function returns null, then the result subexpression is the
		/// subExpression.ReplaceSubExression(replacementSelector) (i.e. recursivly copy-repalcing the subexpression)
		/// </param>
		/// <returns>
		/// A copy of this expression, having each sub-expression that 'replacementSelector' returned
		/// non-null for it replaced with the result returned from 'replacementSelector'
		/// </returns>
		public static Expression ReplaceSubExpression(
			this Expression expression,
			Func<Expression, Expression> replacementSelector)
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}
			if (replacementSelector == null)
			{
				throw new ArgumentNullException(nameof(replacementSelector));
			}

			return expression.Traverse(
				exp =>
				{
					var resultExp = replacementSelector(exp);

					return new ExpressionTraversal
					{
						NewExpression = resultExp,
						TraverseSubTree = resultExp == null,
					};
				})
				.Result;
		}

		/// <summary>
		/// Replaces each occurance of key expression with its value expression, in this expression
		/// </summary>
		/// <param name="expression">This expression</param>
		/// <param name="replacements">A dictionary of expressions, paired with their replacements</param>
		/// <returns>
		/// A copy of this expression, having each occurance of a key from 'replacements' dictionary
		/// replaced with its associated value
		/// </returns>
		public static Expression ReplaceSubExpression(
			this Expression expression,
			IDictionary<Expression, Expression> replacements)
		{
			if (replacements == null)
			{
				throw new ArgumentNullException(nameof(replacements));
			}

			return expression.ReplaceSubExpression(ex =>
				replacements.TryGetValue(ex, out var rep)
					? rep
					: null);
		}

		/// <summary>
		/// Replaces each occurance of 'leafToReplace' in this 'expression' with 'replacement'
		/// </summary>
		/// <param name="expression">This expression</param>
		/// <param name="leafToReplace">An expression to replace with 'replacement'</param>
		/// <param name="replacement">
		/// The expression that replaces 'leafToReplace' in the result expression
		/// </param>
		/// <returns>A copy of this expression, where each intance of 'leafToReplace' is repalced with 'replacement'</returns>
		/// 
		public static Expression ReplaceSubExpression(
			this Expression expression,
			Expression leafToReplace,
			Expression replacement)
		{
			if (leafToReplace == null)
			{
				throw new ArgumentNullException(nameof(leafToReplace));
			}
			if (replacement == null)
			{
				throw new ArgumentNullException(nameof(replacement));
			}

			return expression
				.ReplaceSubExpression(
					new Dictionary<Expression, Expression>
					{
						[leafToReplace] = replacement
					});
		}

		#endregion Methods
	}
}
