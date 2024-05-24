using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;
using ProgressBar = System.Windows.Forms.ProgressBar;

namespace Lab11
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<ulong> task = Newton_Task.Calculate(25, 12);
            task.Wait();
            Console.WriteLine(task.Result);

            Task<ulong> task2 = Newton_Delegate.Calculate(25, 12);
            Console.WriteLine(task2.Result);

            Task<ulong> task3 = Newton_Async.Calculate(25, 12);
            task2.Wait();
            Console.WriteLine(task3.Result);

            ProgressBar progressBar = new ProgressBar();
            progressBar.Width = 285;
            FibonacciCalculator calculator = new FibonacciCalculator(progressBar);
            calculator.Calculate(50);
            Application.Run(new Form { Controls = { progressBar } });

            //FileCompressor.CompressFiles("C:\\Users\\Adam\\Studia\\4 semestr\\Platformy Technologiczne\\Platformy_Technologiczne\\Lab11\\compress");
            //FileCompressor.DecompressFiles("C:\\Users\\Adam\\Studia\\4 semestr\\Platformy Technologiczne\\Platformy_Technologiczne\\Lab11\\compress");
        }
    }
    public class Newton_Task
    {
        public static async Task<ulong> Calculate(ulong N, ulong K)
        {
            var numeratorTask = Task.Run(() => FactorialRange(N, N - K + 1));
            var denominatorTask = Task.Run(() => Factorial(K));

            await Task.WhenAll(numeratorTask, denominatorTask);

            ulong numerator = numeratorTask.Result;
            ulong denominator = denominatorTask.Result;

            return numerator / denominator;
        }

        private static ulong Factorial(ulong n)
        {
            ulong result = 1;
            for (ulong i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        private static ulong FactorialRange(ulong start, ulong end)
        {
            ulong result = 1;
            for (ulong i = start; i >= end; i--)
            {
                result *= i;
            }
            return result;
        }
    }

    public class Newton_Delegate
    {
        public delegate Task<ulong> FactorialDelegate(ulong n);
        public delegate Task<ulong> FactorialRangeDelegate(ulong start, ulong end);

        public static async Task<ulong> Calculate(ulong N, ulong K)
        {
            FactorialDelegate factorialDelegate = new FactorialDelegate(Factorial);
            FactorialRangeDelegate factorialRangeDelegate = new FactorialRangeDelegate(FactorialRange);

            Task<ulong> numeratorTask = factorialRangeDelegate(N, N - K + 1);
            Task<ulong> denominatorTask = factorialDelegate(K);

            ulong[] results = await Task.WhenAll(numeratorTask, denominatorTask);

            return results[0] / results[1];
        }

        private static Task<ulong> Factorial(ulong n)
        {
            return Task.Run(() =>
            {
                ulong result = 1;
                for (ulong i = 1; i <= n; i++)
                {
                    result *= i;
                }
                return result;
            });
        }

        private static Task<ulong> FactorialRange(ulong start, ulong end)
        {
            return Task.Run(() =>
            {
                ulong result = 1;
                for (ulong i = start; i >= end; i--)
                {
                    result *= i;
                }
                return result;
            });
        }
    }

    public class Newton_Async
    {
        public static async Task<ulong> Calculate(ulong N, ulong K)
        {
            ulong numerator = await FactorialRange(N, N - K + 1);
            ulong denominator = await Factorial(K);

            return numerator / denominator;
        }

        private static Task<ulong> Factorial(ulong n)
        {
            return Task.Run(() =>
            {
                ulong result = 1;
                for (ulong i = 1; i <= n; i++)
                {
                    result *= i;
                }
                return result;
            });
        }

        private static Task<ulong> FactorialRange(ulong start, ulong end)
        {
            return Task.Run(() =>
            {
                ulong result = 1;
                for (ulong i = start; i >= end; i--)
                {
                    result *= i;
                }
                return result;
            });
        }
    }

    public class FibonacciCalculator
    {
        private BackgroundWorker worker;
        private ProgressBar progressBar;

        public FibonacciCalculator(ProgressBar progressBar)
        {
            this.progressBar = progressBar;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
        }

        public void Calculate(int n)
        {
            worker.RunWorkerAsync(n);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int n = (int)e.Argument;
            ulong[] fib = new ulong[n + 1];
            fib[0] = 0;
            fib[1] = 1;
            for (int i = 2; i <= n; i++)
            {
                fib[i] = fib[i - 1] + fib[i - 2];
                Thread.Sleep(5);
                worker.ReportProgress((i * 100) / n, fib[i]);
            }
            e.Result = fib[n];
            Console.WriteLine(fib[n]);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
    }

    public class FileCompressor
    {
        public static void CompressFiles(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath);
            Parallel.ForEach(files, (file) =>
            {
                using (FileStream originalFileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    string compressedFileName = file + ".gz";
                    using (FileStream compressedFileStream = new FileStream(compressedFileName, FileMode.Create))
                    using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                    {
                        originalFileStream.CopyTo(compressionStream);
                    }
                }
            });
        }

        public static void DecompressFiles(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath, "*.gz");
            Parallel.ForEach(files, (file) =>
            {
                using (FileStream compressedFileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    string decompressedFileName = Path.ChangeExtension(file, null);
                    using (FileStream decompressedFileStream = new FileStream(decompressedFileName, FileMode.Create))
                    using (GZipStream decompressionStream = new GZipStream(compressedFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            });
        }
    }
}