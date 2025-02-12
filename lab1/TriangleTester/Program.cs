using System.Diagnostics;

namespace TriangleTester
{
	internal class Program
	{
		private static readonly string TestsFilePath = "tests.txt";
		private static readonly string ExePath = "Triangle.exe";
		private static readonly string ResultsFilePath = "results.txt";

		const string Error = "error";
		const string Success = "success";

		static void Main()
		{
			if (!File.Exists(TestsFilePath))
			{
				Console.WriteLine($"Tests file not exists: \"{TestsFilePath}\"");
				return;
			}

			if (!File.Exists(ExePath))
			{
				Console.WriteLine($"Exe file not exists: \"{ExePath}\"");
				return;
			}
			
			using (StreamReader reader = new StreamReader(TestsFilePath))
			using (StreamWriter writer = new StreamWriter(ResultsFilePath))
			{
				string? line;
				while ((line = reader.ReadLine()) != null)
				{
					try
					{
						string[] parts = line.Split(' ');
						if (parts.Length != 4)
						{
							writer.WriteLine(Error);
							continue;
						}

						string[] numbers = parts.Take(3).ToArray();

						string expectedProgramResult = parts.Last();

						string arguments = $"{numbers[0]} {numbers[1]} {numbers[2]}";

						string programResult = RunProgram(ExePath, arguments);

						writer.WriteLine($"{(programResult == expectedProgramResult ? Success : Error)}");
						Console.WriteLine($"Value: {line}, result: {programResult}, expected: {expectedProgramResult}");
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Exception: {ex.Message}");
						writer.WriteLine(Error);
					}
				}
			}
		}

		static string RunProgram(string exePath, string arguments)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo(exePath, arguments)
			{
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using (Process? process = Process.Start(startInfo))
			{
				if (process == null)
				{
					Console.WriteLine($"Failed to start process: \"{exePath}\"");
					return Error;
				}
				process.WaitForExit();
				return process.StandardOutput.ReadToEnd().Trim();
			}
		}
	}
}
