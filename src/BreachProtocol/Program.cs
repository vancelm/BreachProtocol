using System;

namespace BreachProtocol
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string mode = args.Length > 0 ? args[0].ToLowerInvariant() : "play";

            switch (mode)
            {
                case "play":
                    PlayGame();
                    break;
                case "test":
                    TestAlgorithms();
                    break;
                default:
                    break;
            }
        }

        private static void PlayGame()
        {
            Puzzle puzzle = new Puzzle(8, 8, 8, 4);
            puzzle.Initialize();
            PrintPuzzle(puzzle);

            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (puzzle.CurrentAxis == PuzzleAxis.Vertical && puzzle.CurrentRow > 0)
                            puzzle.Move(puzzle.CurrentRow - 1, puzzle.CurrentColumn);
                        break;
                    case ConsoleKey.DownArrow:
                        if (puzzle.CurrentAxis == PuzzleAxis.Vertical && puzzle.CurrentRow < puzzle.MatrixRows - 1)
                            puzzle.Move(puzzle.CurrentRow + 1, puzzle.CurrentColumn);
                        break;
                    case ConsoleKey.LeftArrow:
                        if (puzzle.CurrentAxis == PuzzleAxis.Horizontal && puzzle.CurrentColumn > 0)
                            puzzle.Move(puzzle.CurrentRow, puzzle.CurrentColumn - 1);
                        break;
                    case ConsoleKey.RightArrow:
                        if (puzzle.CurrentAxis == PuzzleAxis.Horizontal && puzzle.CurrentColumn < puzzle.MatrixColumns - 1)
                            puzzle.Move(puzzle.CurrentRow, puzzle.CurrentColumn + 1);
                        break;
                    case ConsoleKey.Enter:
                        if (puzzle.BufferCount == puzzle.BufferCapacity)
                        {
                            break;
                        }

                        if (puzzle.GetMatrixValue(puzzle.CurrentRow, puzzle.CurrentColumn) != 0)
                        {
                            bool isWinner = puzzle.Push();
                            PrintPuzzle(puzzle);

                            if (isWinner)
                            {
                                PlayerWins(puzzle);
                                return;
                            }
                        }
                        break;
                    case ConsoleKey.Backspace:
                        if (puzzle.BufferCount > 0)
                        {
                            puzzle.Pop();
                            PrintPuzzle(puzzle);
                        }
                        break;
                    case ConsoleKey.Escape:
                        return;
                    default:
                        break;
                }

                SetCursorPosition(puzzle);
            }
        }

        private static void PlayerWins(Puzzle puzzle)
        {
            Console.SetCursorPosition(0, puzzle.MatrixRows + 1);
            Console.WriteLine("*********************************");
            Console.WriteLine("* WINNER WINNER CHICKEN DINNER! *");
            Console.WriteLine("*********************************");
        }

        private static void SetCursorPosition(Puzzle puzzle)
        {
            Console.SetCursorPosition(puzzle.CurrentColumn * 3, puzzle.CurrentRow);
        }

        private static void PrintPuzzle(Puzzle puzzle)
        {
            PrintMatrix(puzzle);
            PrintBuffer(puzzle);
            PrintSolution(puzzle);
            SetCursorPosition(puzzle);
        }

        private static void PrintSolution(Puzzle puzzle)
        {
            Console.SetCursorPosition(puzzle.MatrixColumns * 3 + 1, 2);
            Console.Write("Sequence: ");
            for (int i = 0; i < puzzle.Sequence.Count; i++)
            {
                Console.Write($"{puzzle.Sequence[i]:X2} ");
            }
        }

        private static void PrintBuffer(Puzzle puzzle)
        {
            Console.SetCursorPosition(puzzle.MatrixColumns * 3 + 1, 0);
            Console.Write("Buffer: ");
            for (int i = 0; i < puzzle.BufferCapacity; i++)
            {
                if (i < puzzle.BufferCount)
                    Console.Write($"{puzzle.GetBufferValue(i):X2} ");
                else
                    Console.Write("__ ");
            }
        }

        private static void PrintMatrix(Puzzle puzzle)
        {
            Console.SetCursorPosition(0, 0);
            for (int row = 0; row < puzzle.MatrixRows; row++)
            {
                for (int col = 0; col < puzzle.MatrixColumns; col++)
                {
                    if (puzzle.CurrentAxis == PuzzleAxis.Horizontal)
                    {
                        if (row == puzzle.CurrentRow)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else
                    {
                        if (col == puzzle.CurrentColumn)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    Console.Write($"{puzzle.GetMatrixValue(row, col):X2} ");
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void TestAlgorithms()
        {

        }



        //private static void PrintSolutions()
        //{
        //    Console.WriteLine("Solutions:");
        //    foreach (MatrixItem[] sol in solutions.OrderBy(s => s.Length))
        //    {
        //        for (int i = 0; i < sol.Length; i++)
        //        {
        //            Console.Write($"({sol[i].Row}, {sol[i].Col}, {sol[i].Value:X2})");

        //            if (i < sol.Length - 1)
        //            {
        //                Console.Write(", ");
        //            }
        //        }

        //        Console.WriteLine();
        //    }
        //}




        //private static void FindSolutions_BruteForce()
        //{
        //    BruteForce_Recursive(0, 0, 0);
        //}

        //private static void BruteForce_Recursive(int row, int col, int dimension)
        //{
        //    if (ContainsSolution())
        //    {
        //        solutions.Add(buffer.Take(bufferSize).ToArray());
        //    }

        //    if (bufferSize >= buffer.Length)
        //    {
        //        return;
        //    }

        //    if (dimension == 0)
        //    {
        //        col = 0;
        //    }
        //    else
        //    {
        //        row = 0;
        //    }

        //    while (row < matrix.GetLength(0) && col < matrix.GetLength(1))
        //    {
        //        if (matrix[row, col] != 0)
        //        {
        //            bufferSize++;
        //            buffer[bufferSize - 1] = new MatrixItem(row, col, matrix[row, col]);
        //            matrix[row, col] = 0;
        //            //Print();
        //            BruteForce_Recursive(row, col, dimension ^ 1);
        //            matrix[row, col] = buffer[bufferSize - 1].Value;
        //            buffer[bufferSize - 1] = default;
        //            bufferSize--;
        //        }

        //        if (dimension == 0)
        //        {
        //            col++;
        //        }
        //        else
        //        {
        //            row++;
        //        }
        //    }
        //}

        //private static void FindSolutions()
        //{
        //    int index = solution.Length - 1;
        //    for (int row = 0; row < matrix.GetLength(0); row++)
        //    {
        //        for (int col = 0; col < matrix.GetLength(1); col++)
        //        {
        //            if (matrix[row, col] == solution[index])
        //            {
        //                buffer[]
        //                FindSolutions_Recursive(index, row, col, 0);
        //                FindSolutions_Recursive(index, row, col, 1);
        //            }
        //        }
        //    }
        //}

        //private static void FindSolutions_Recursive(int index, int row, int col, int dimension)
        //{
        //    if (index <= 0)
        //    {
        //        return;
        //    }

        //    index--;

        //    while (row < matrix.GetLength(0) && col < matrix.GetLength(1))
        //    {
        //        if (matrix[row, col] == )
        //    }

        //}
    }
}
