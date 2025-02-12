namespace TriangleType
{
	enum Result
	{
		Error, Equilateral, Isosceles, Scalene, NotTriangle
	}

	public class Triangle
	{
		private double[] _sides;

		public Triangle(double side1, double side2, double side3)
		{
			_sides = new double[3];
			_sides[0] = side1;
			_sides[1] = side2;
			_sides[2] = side3;
		}

		public string GetTriangleType()
		{
			if (IsInvalidSides())
				return Result.Error.ToString();

			if (IsNotTriangle())
				return Result.NotTriangle.ToString();

			if (IsEquilateralTriangle())
				return Result.Equilateral.ToString();

			if (IsIsoscelesTriangle())
				return Result.Isosceles.ToString();

			return Result.Scalene.ToString();
		}

		private bool IsInvalidSides()
		{
			return _sides[0] <= 0 && _sides[1] <= 0 && _sides[2] <= 0;
		}

		private bool IsNotTriangle()
		{
			return _sides[0] + _sides[1] <= _sides[2]
				|| _sides[0] + _sides[2] <= _sides[1]
				|| _sides[1] + _sides[2] <= _sides[0];
		}

		private bool IsEquilateralTriangle()
		{
			return _sides[0] == _sides[1] && _sides[1] == _sides[2];
		}

		private bool IsIsoscelesTriangle()
		{
			return _sides[0] == _sides[1] || _sides[1] == _sides[2] || _sides[0] == _sides[2];
		}
	}
	internal class Program
	{
		static bool ParseArgs(string[] args, out Triangle? triangle)
		{
			triangle = null;
			if (args.Length != 3)
				return false;

			if (double.TryParse(args[0], out double side1) && side1 > 0 &&
				double.TryParse(args[1], out double side2) && side2 > 0 &&
				double.TryParse(args[2], out double side3) && side3 > 0)
			{
				triangle = new Triangle(side1, side2, side3);
				return true;
			}

			return false;
		}

		static void Main(string[] args)
		{
			if (!ParseArgs(args, out Triangle? triangle) || triangle is null)
			{
				Console.WriteLine(Result.Error.ToString());
				return;
			}

			Console.WriteLine(triangle!.GetTriangleType());
		}
	}
}
