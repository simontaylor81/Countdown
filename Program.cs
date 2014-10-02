using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countdown
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 3)
			{
				Console.WriteLine("Usage: target_number input_number_0 input_number_1 ...");
				return;
			}

			int target = int.Parse(args[0]);
			var inputs = from input in args.Skip(1) select int.Parse(input);

			var solution = Solver.Solve(target, inputs);
			Debug.Assert(solution.Evaluate() == target);

			Console.WriteLine("Solution found: {0} = {1}", solution.ToString(), target);
		}
	}
}
