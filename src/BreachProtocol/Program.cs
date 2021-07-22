using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace BreachProtocol
{
    class Program
    {
        private static readonly ImmutableArray<byte> values = ImmutableArray.Create<byte>(0x1C, 0x55, 0xBD, 0xE9, 0xFF);
        private static readonly byte[,] matrix = new byte[4, 4];
        private static readonly byte[] buffer = new byte[8];
        private static int bufferSize = 0;
        private static readonly byte[] solution = new byte[4];

        static void Main(string[] args)
        {
            FillMatrix();
            CreateSolution();
            Print();
            Recursive(0, 0, 0);
        }

        private static byte GetRandomValue()
        {
            int index = Random.Shared.Next(values.Length);
            return values[index];
        }

        private static void FillMatrix()
        {
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    matrix[row, col] = GetRandomValue();
                }
            }
        }

        private static void Print()
        {
            Console.SetCursorPosition(0, 0);
            PrintBuffer();
            PrintMatrix();
        }

        private static void PrintBuffer()
        {
            foreach (byte value in buffer)
            {
                Console.Write($"{value:X2} ");
            }
            Console.Write("  ");
            Console.WriteLine();
        }

        private static void PrintMatrix()
        {
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    Console.Write($"{matrix[row, col]:X2} ");
                }
                Console.WriteLine();
            }
        }

        private static void CreateSolution()
        {
            byte[,] tempMatrix = new byte[matrix.GetLength(0), matrix.GetLength(1)];
            Array.Copy(matrix, tempMatrix, matrix.Length);

            byte[] fullSolution = new byte[buffer.Length];
            int row = 0;
            int col = 0;
            int dimension = 0;

            for (int i = 0; i < fullSolution.Length; i++)
            {
                do
                {
                    if (dimension == 0)
                    {
                        col = Random.Shared.Next(tempMatrix.GetLength(1));
                    }
                    else
                    {
                        row = Random.Shared.Next(tempMatrix.GetLength(0));
                    }
                } while (tempMatrix[row, col] == 0);

                fullSolution[i] = tempMatrix[row, col];
                tempMatrix[row, col] = 0;
                dimension ^= 1;
            }

            Array.Copy(fullSolution, Random.Shared.Next(fullSolution.Length - solution.Length), solution, 0, solution.Length);
        }

        private static bool ContainsSolution()
        {
            bool result = false;
            for (int i = 0; i < buffer.Length - solution.Length; i++)
            {
                result = true;
                for (int j = 0; j < solution.Length; j++)
                {
                    if (buffer[i + j] != solution[j])
                    {
                        result = false;
                        break;
                    }
                }

                if (result)
                {
                    break;
                }
            }

            return result;
        }

        private static void Recursive(int row, int col, int dimension)
        {
            if (bufferSize >= buffer.Length)
                return;

            if (dimension == 0)
                col = 0;
            else
                row = 0;

            while (row < matrix.GetLength(0) && col < matrix.GetLength(1))
            {
                if (matrix[row, col] != 0)
                {
                    bufferSize++;
                    buffer[bufferSize - 1] = matrix[row, col];
                    matrix[row, col] = 0;
                    Print();
                    Recursive(row, col, dimension ^ 1);
                    matrix[row, col] = buffer[bufferSize - 1];
                    bufferSize--;
                }

                if (dimension == 0)
                    col++;
                else
                    row++;
            }
        }
    }
}
