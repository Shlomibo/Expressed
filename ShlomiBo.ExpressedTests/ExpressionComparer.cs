using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ShlomiBo.ExpressedTests
{
	internal sealed class ExpressionComparer : IEqualityComparer<Expression>
	{
		public static ExpressionComparer Intance { get; } = new ExpressionComparer();
		public bool Equals(Expression x, Expression y)
		{
			return x?.ToString() == y?.ToString();
		}

		public int GetHashCode(Expression obj)
		{
			return obj?.ToString()?.GetHashCode() ?? 0;
		}
	}
}
