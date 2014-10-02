using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countdown
{
	enum Operator
	{
		Add,
		Sub,
		Mul,
		Div,
	}

	interface IExpression
	{
		int Evaluate();
		IEnumerable<int> UsedIndices { get; }
	}

	class LiteralExpression : IExpression
	{
		public LiteralExpression(int literal, int index)
		{
			this.literal = literal;
			this.index = index;
		}

		public int Evaluate()
		{
			return literal;
		}

		public override string ToString()
		{
			return literal.ToString();
		}

		public IEnumerable<int> UsedIndices { get { return new[] { index }; } }

		private int literal;
		private int index;
	}

	class CompoundExpression : IExpression
	{
		public CompoundExpression(IExpression lhs, IExpression rhs, Operator op)
		{
			this.lhs = lhs;
			this.rhs = rhs;
			this.op = op;
		}

		public int Evaluate()
		{
			var x = lhs.Evaluate();
			var y = rhs.Evaluate();

			switch (op)
			{
				case Operator.Add: return x + y;
				case Operator.Sub: return x - y;
				case Operator.Mul: return x * y;
				case Operator.Div: return x / y;
			}

			throw new InvalidOperationException();
		}

		public override string ToString()
		{
			var opStrings = new[] { "+", "-", "*", "/" };
			return string.Format("({0} {1} {2})", lhs.ToString(), opStrings[(int)op], rhs.ToString());
		}

		public IEnumerable<int> UsedIndices
		{
			get
			{
				return lhs.UsedIndices.Concat(rhs.UsedIndices);
			}
		}

		private IExpression lhs;
		private IExpression rhs;
		private Operator op;
	}
}
