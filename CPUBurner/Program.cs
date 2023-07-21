using System;
using System.Diagnostics;
using System.Threading;

namespace CPUBurner
{
    class Program
    {
        static PerformanceCounter cpuCounter;
        static PerformanceCounter ramCounter;

        static void Main()
        {
            Console.WriteLine("CPU Stress Test - Press any key to stop.");

            // Initialize PerformanceCounters for CPU and RAM usage.
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            // Get the number of logical processors to utilize (you can change this as needed).
            int numProcessors = Environment.ProcessorCount;

            // Create threads to utilize all logical processors.
            Thread[] threads = new Thread[numProcessors];
            for (int i = 0; i < numProcessors; i++)
            {
                threads[i] = new Thread(CpuStress);
                threads[i].Start();
            }

            // Start displaying CPU and RAM usage in a separate thread.
            Thread monitorThread = new Thread(MonitorUsage);
            monitorThread.Start();

            // Wait for any key press to stop the stress test.
            Console.ReadKey();

            // Stop the stress test by terminating the threads.
            for (int i = 0; i < numProcessors; i++)
            {
                threads[i].Abort();
            }

            // Stop the monitor thread.
            monitorThread.Abort();
        }

        static void CpuStress()
        {
            try
            {
                while (true)
                {
                    // Perform some CPU-intensive calculations.
                    // You can adjust the load by changing the complexity of calculations and loop duration.
                    // For example, you can try adding nested loops and mathematical operations.
                    for (int i = 0; i < 100000; i++)
                    {
                        for (int j = 0; j < 100000; j++)
                        {
                            Math.Pow(i, j);
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // Thread was aborted; exit gracefully.
            }
        }

        static void MonitorUsage()
        {
            try
            {
                while (true)
                {
                    // Display CPU and RAM usage.
                    float cpuUsage = cpuCounter.NextValue();
                    float ramUsage = ramCounter.NextValue();

                    Console.WriteLine($"CPU Usage: {cpuUsage:F2}%\tRAM Available: {ramUsage:F2} MB");

                    // Wait for 1 second before checking again.
                    Thread.Sleep(1000);
                }
            }
            catch (ThreadAbortException)
            {
                // Thread was aborted; exit gracefully.
            }
        }
    }
}
