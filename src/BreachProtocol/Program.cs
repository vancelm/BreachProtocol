using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace BreachProtocol
{
    class Program
    {
        private struct MatrixItem
        {
            public readonly int Row;
            public readonly int Col;
            public readonly byte Value;

            public MatrixItem(int row, int col, byte value)
            {
                Row = row;
                Col = col;
                Value = value;
            }
        }

        private static readonly ImmutableArray<byte> values = ImmutableArray.Create<byte>(0x1C, 0x55, 0xBD, 0xE9, 0xFF);
        private static readonly byte[,] matrix = new byte[6, 6];
        private static readonly MatrixItem[] buffer = new MatrixItem[6];
        private static int bufferSize = 0;
        private static readonly byte[] solution = new byte[4];
        private static readonly List<MatrixItem[]> solutions = new();

        static void Main(string[] args)
        {
            FillMatrix();
            CreateSolution();
            Print();
            FindSolutions_BruteForce();
            Print();
            PrintSolutions();
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
            foreach (MatrixItem item in buffer)
            {
                Console.Write($"{item.Value:X2} ");
            }
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

        private static void PrintSolutions()
        {
            foreach (MatrixItem[] sol in solutions.OrderBy(s => s.Length))
            {
                foreach (MatrixItem item in sol)
                {
                    Console.Write($"({item.Row}, {item.Col}, {item.Value:X2}), ");
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
                    if (buffer[i + j].Value != solution[j])
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

        private static void FindSolutions_BruteForce()
        {
            BruteForce_Recursive(0, 0, 0);
        }

        private static void BruteForce_Recursive(int row, int col, int dimension)
        {
            if (ContainsSolution())
            {
                solutions.Add(buffer.Take(bufferSize).ToArray());
            }

            if (bufferSize >= buffer.Length)
            {
                return;
            }

            if (dimension == 0)
            {
                col = 0;
            }
            else
            {
                row = 0;
            }

            while (row < matrix.GetLength(0) && col < matrix.GetLength(1))
            {
                if (matrix[row, col] != 0)
                {
                    bufferSize++;
                    buffer[bufferSize - 1] = new MatrixItem(row, col, matrix[row, col]);
                    matrix[row, col] = 0;
                    //Print();
                    BruteForce_Recursive(row, col, dimension ^ 1);
                    matrix[row, col] = buffer[bufferSize - 1].Value;
                    buffer[bufferSize - 1] = default;
                    bufferSize--;
                }

                if (dimension == 0)
                {
                    col++;
                }
                else
                {
                    row++;
                }
            }
        }
    }
}
