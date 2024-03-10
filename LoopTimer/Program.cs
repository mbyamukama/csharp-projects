
using System;
using System.Diagnostics;

namespace LoopTimer
{
	
	class Program
	{
		static void Main()
		{
			List<double> measurementTimes = new List<double>();

			for (int j = 0; j < 100; j++)
			{
				Console.Write("Running loop {0}: ", j);
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();

				// Your for loop
				for (int i = 1; i <= 100000000; ++i)
				{
					// Do some computation here
				}

				stopwatch.Stop();
				double elapsedTime = stopwatch.Elapsed.TotalMilliseconds;
				Console.WriteLine(elapsedTime.ToString());

				// Add the time taken to the list
				measurementTimes.Add(elapsedTime);
			}

			// Calculate the average time
			double averageTime = measurementTimes.Average();
			Console.WriteLine("Average time taken: " + averageTime + " ms");
		}
	}
}
