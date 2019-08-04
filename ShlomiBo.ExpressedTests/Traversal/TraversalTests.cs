using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Xunit;
using ShlomiBo.Expressed;

namespace ShlomiBo.ExpressedTests.Traversal
{
	using static Expression;
	using TestLambda = Func<int, int, int>;
	using TestExpType = Expression<Func<int, int, int>>;

	public partial class TraversalTests
	{
		private TestExpType TestExpression => (x, y) => (x + y) * (x - y);
		private static ExpressionComparer Comparer => ExpressionComparer.Intance;

		[Fact]
		public void ReplaceLeaf()
		{
			var travResult = this.TestExpression.Body.Traverse(
				exp => exp is ParameterExpression p && p.Name == "x"
					? new ExpressionExtensions.ExpressionTraversal
					{
						NewExpression = Constant(5)
					}
					: null);
			var result = Lambda<TestLambda>(
				travResult.Result,
				this.TestExpression.Parameters);

			Assert.True(travResult.TraversedWholeExpression, "Did not traverse whole tree");
			Assert.Equal(
				ExpressionOf((x, y) => (5 + y) * (5 - y)),
				result,
				Comparer);
		}

		[Fact]
		public void ReplaceLeafThenBreak()
		{
			var travResult = this.TestExpression.Body.Traverse(
				exp => exp is ParameterExpression p && p.Name == "x"
					? new ExpressionExtensions.ExpressionTraversal
					{
						NewExpression = Constant(5),
						Break = true,
					}
					: null);
			var result = Lambda<TestLambda>(
				travResult.Result,
				this.TestExpression.Parameters);

			Assert.False(travResult.TraversedWholeExpression, "Did traverse whole tree");
			Assert.Equal(
				ExpressionOf((x, y) => (5 + y) * (x - y)),
				result,
				Comparer);
		}

		private static TestExpType ExpressionOf(TestExpType exp) =>
			exp;
	}
}
