using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddMatrixApp
{
    internal class Program
    {
        static int[,] GenerateRandomMatrix(Random r, int rows, int columns, int min, int max)
        {
            int[,] matrix = new int[rows, columns];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = r.Next(min, max + 1);
                }
            }
            return matrix;
        }
        //однопоточный метод сложения двух матриц
        static int MatrixSum(int[,] matrix1, int[,] matrix2)
        {
            int res = 0;
            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix1.GetLength(1); j++)
                {
                    res += matrix1[i, j] + matrix2[i, j];
                }
            }
            return res;
        }
        //многопоточный метод сложения матриц
        static int MatrixSum(int[,] matrix1, int[,] matrix2, int n)//n колво потоков
        {
            object obj = new object();
            int sum = 0;

            int step = matrix1.GetLength(0) / n;//поток на кол-во строк
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < n; i++)
            {
                int start = i * step;
                int end = start + step;
                if (i == n - 1)
                {
                    end = matrix1.GetLength(0);
                }
                Thread thread = new Thread(() =>
                {
                    lock (obj)
                    {
                        int localsum = MatrixSum(matrix1, matrix2, start, end);
                        Interlocked.Add(ref sum, localsum);
                    }
                });
                thread.Start();
                threads.Add(thread);
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            return sum;
        }
        static int MatrixSum(int[,] m, int[,] m1, int start, int end)
        {
            int sum = 0;
            for (int i = start; i < end; i++)
            {
                for (int j = 0; j < m.GetLength(0); j++)
                {
                    sum += m[i, j];
                    sum += m1[i, j];
                }
            }
            return sum;
        }
        static void Main(string[] args)
        {
            int length = 500;
            Random r = new Random();
            int[,] matrix = GenerateRandomMatrix(r, length, length, 0, 10);
            int[,] matrix2 = GenerateRandomMatrix(r, length, length, 0, 10);

            int oneThreadResult = MatrixSum(matrix, matrix2);
            Console.WriteLine($"однопоточный результат:{oneThreadResult}");

            int multyThreadsResult = MatrixSum(matrix, matrix2, 16);
            Console.WriteLine($"многопоточный результат:{multyThreadsResult}");
        }
    }
}
