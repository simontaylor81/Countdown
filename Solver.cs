using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countdown
{
	class DecisionPoint
	{
		public DecisionPoint(int[] inputs, IEnumerable<int> availableIndices)
		{
			this.Expression = null;
			this.inputs = inputs;
			this.availableIndices = availableIndices.ToArray();
		}

		public bool Next()
		{
			// Make sure we don't accidentally use the old expression.
			Expression = null;

			if (literalIndex < availableIndices.Length)
			{
				var inputIndex = availableIndices[literalIndex];
				Expression = new LiteralExpression(inputs[inputIndex], inputIndex);
				literalIndex++;
				return true;
			}

			// Try new sub-expressions.
			if (lhsDP != null && rhsDP != null)
			{
				if (rhsDP.Next())
				{
					Expression = new CompoundExpression(lhsDP.Expression, rhsDP.Expression, op);
					return true;
				}

				// RHS exhausted, so try new LHS.
				if (lhsDP.Next())
				{
					// reset rhs decision point
					rhsDP = new DecisionPoint(inputs, lhsDP.UnusedIndices);
					if (rhsDP.Next())
					{
						Expression = new CompoundExpression(lhsDP.Expression, rhsDP.Expression, op);
						return true;
					}
				}

				// Sub-expressions exhausted, so move to the next op.
				op++;
			}

			// Try new operator, if we have enough literals.
			if ((int)op < Enum.GetValues(typeof(Operator)).Length && availableIndices.Length >= 2)
			{
				// Create two new decision points.
				lhsDP = new DecisionPoint(inputs, availableIndices);
				var lhsRes = lhsDP.Next();

				rhsDP = new DecisionPoint(inputs, lhsDP.UnusedIndices);
				var rhsRes = rhsDP.Next();

				Debug.Assert(lhsRes);
				Debug.Assert(rhsRes);

				Expression = new CompoundExpression(lhsDP.Expression, rhsDP.Expression, op);

				return true;
			}

			return false;
		}

		public IExpression Expression { get; private set; }

		public IEnumerable<int> UnusedIndices { get { return availableIndices.Except(Expression.UsedIndices); } }

		private int[] inputs;
		private int[] availableIndices;
		private int literalIndex;
		private Operator op;
		private DecisionPoint lhsDP;
		private DecisionPoint rhsDP;
	}

	class NoSolutionException : Exception
	{}

	static class Solver
	{
		public static IExpression Solve(int target, IEnumerable<int> inputs)
		{
			var inputsArray = inputs.ToArray();
			var root = new DecisionPoint(inputsArray, Enumerable.Range(0, inputsArray.Length));

			while (root.Next())
			{
				try
				{
					if (root.Expression.Evaluate() == target)
						return root.Expression;
				}
				catch (DivideByZeroException)
				{
					// Skip solutions that cause div by zero.
				}
			}

			throw new NoSolutionException();
		}
	}
}
