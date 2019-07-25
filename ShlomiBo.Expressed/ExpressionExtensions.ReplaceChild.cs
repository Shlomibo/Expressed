using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ShlomiBo.Expressed
{
	partial class ExpressionExtensions
	{
		#region Methods

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
				.ReplaceSubExpression(new[] { leafToReplace }
				.ToDictionary(
					exp => exp,
					exp => replacement));
		}

		#endregion Methods
	}
}
