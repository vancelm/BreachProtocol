using System;
using System.Diagnostics;

namespace BreachProtocol
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            PrintMenu();
            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.D1:
                    PlayGame();
                    break;
                case ConsoleKey.D2:
                    TestAlgorithms();
                    break;
                default:
                    break;
            }
        }

        private static void PlayGame()
        {
            Puzzle puzzle = new(8, 8, 8, 4);
            puzzle.Initialize();
            PlayGame(puzzle);
        }

        private static void PlayGame(Puzzle puzzle)
        {
            Console.Clear();
            PrintPuzzle(puzzle);

            bool solved = false;

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
                            solved = puzzle.Push();
                            PrintPuzzle(puzzle);
                        }
                        break;
                    case ConsoleKey.Backspace:
                        if (puzzle.BufferCount > 0)
                        {
                            puzzle.Pop();
                            PrintPuzzle(puzzle);
                        }
                        break;
                    case ConsoleKey.F1:
                        solved = Solve_BruteForce(puzzle);
                        PrintPuzzle(puzzle);
                        break;
                    case ConsoleKey.F2:
                        solved = Solve_BetterButUgly(puzzle);
                        PrintPuzzle(puzzle);
                        break;
                    case ConsoleKey.Escape:
                        return;
                    default:
                        break;
                }

                if (solved)
                    PlayerWins(puzzle);

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
            Console.Clear();

            int iterations = 100;

            for (int size = 2; size < 15; size++)
            {
                Puzzle puzzle = new(size, size, size, size / 2);
                double totalElapsed1 = 0;
                double minElapsed1 = double.MaxValue;
                double maxElapsed1 = 0;
                double totalElapsed2 = 0;
                double minElapsed2 = double.MaxValue;
                double maxElapsed2 = 0;
                for (int i = 0; i < iterations; i++)
                {
                    puzzle.Initialize();
                    double elapsed1 = TimeAlgorithm(() =>
                    {
                        Solve_BruteForce(puzzle);
                    });
                    totalElapsed1 += elapsed1;
                    minElapsed1 = Math.Min(minElapsed1, elapsed1);
                    maxElapsed1 = Math.Max(maxElapsed1, elapsed1);

                    puzzle.Reset();
                    double elapsed2 = TimeAlgorithm(() =>
                    {
                        Solve_BetterButUgly(puzzle, 0);
                    });
                    totalElapsed2 += elapsed2;
                    minElapsed2 = Math.Min(minElapsed2, elapsed2);
                    maxElapsed2 = Math.Max(maxElapsed2, elapsed2);
                }

                double avgElapsed1 = totalElapsed1 / iterations;
                double avgElapsed2 = totalElapsed2 / iterations;
                Console.Write($"{size,2}, {totalElapsed1,8:0.00}, {minElapsed1,8:0.00}, {maxElapsed1,8:0.00}, {avgElapsed1,8:0.00}, ");
                Console.WriteLine($"{totalElapsed2,8:0.00}, {minElapsed2,8:0.00}, {maxElapsed2,8:0.00}, {avgElapsed2,8:0.00}");
            }
        }

        private static double TimeAlgorithm(Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds;
        }

        private static bool Solve_BruteForce(Puzzle puzzle)
        {
            if (puzzle.BufferCount == puzzle.BufferCapacity)
                return false;

            if (puzzle.CurrentAxis == PuzzleAxis.Horizontal)
                puzzle.Move(puzzle.CurrentRow, 0);
            else
                puzzle.Move(0, puzzle.CurrentColumn);

            while (puzzle.CurrentRow < puzzle.MatrixRows && puzzle.CurrentColumn < puzzle.MatrixColumns)
            {
                if (puzzle.GetMatrixValue(puzzle.CurrentRow, puzzle.CurrentColumn) != 0)
                {
                    if (puzzle.Push())
                        return true;
                    if (Solve_BruteForce(puzzle))
                        return true;
                    puzzle.Pop();
                }

                if (puzzle.CurrentAxis == PuzzleAxis.Horizontal)
                    puzzle.Move(puzzle.CurrentRow, puzzle.CurrentColumn + 1);
                else
                    puzzle.Move(puzzle.CurrentRow + 1, puzzle.CurrentColumn);
            }

            return false;
        }

        private static bool Solve_BetterButUgly(Puzzle puzzle, int index = 0)
        {
            if (puzzle.BufferCount == puzzle.BufferCapacity)
                return false;

            if (puzzle.CurrentAxis == PuzzleAxis.Horizontal)
                puzzle.Move(puzzle.CurrentRow, 0);
            else
                puzzle.Move(0, puzzle.CurrentColumn);

            // First limit searching to just the next sequence value
            while (puzzle.CurrentRow < puzzle.MatrixRows && puzzle.CurrentColumn < puzzle.MatrixColumns)
            {
                byte value = puzzle.GetMatrixValue(puzzle.CurrentRow, puzzle.CurrentColumn);
                if (value == puzzle.Sequence[index])
                {
                    if (puzzle.Push())
                        return true;
                    if (Solve_BetterButUgly(puzzle, index + 1))
                        return true;
                    puzzle.Pop();
                }

                if (puzzle.CurrentAxis == PuzzleAxis.Horizontal)
                    puzzle.Move(puzzle.CurrentRow, puzzle.CurrentColumn + 1);
                else
                    puzzle.Move(puzzle.CurrentRow + 1, puzzle.CurrentColumn);
            }

            // Now we have to dig deeper so exclude 0's and also sequence value since we already checked those
            // We only do this if we haven't already found the start of the sequence

            if (puzzle.CurrentAxis == PuzzleAxis.Horizontal)
                puzzle.Move(puzzle.CurrentRow, 0);
            else
                puzzle.Move(0, puzzle.CurrentColumn);

            if (index == 0)
            {
                while (puzzle.CurrentRow < puzzle.MatrixRows && puzzle.CurrentColumn < puzzle.MatrixColumns)
                {
                    byte value = puzzle.GetMatrixValue(puzzle.CurrentRow, puzzle.CurrentColumn);
                    if (value != 0 && value != puzzle.Sequence[index])
                    {
                        if (puzzle.Push())
                            return true; // Won't happen lol
                        if (Solve_BetterButUgly(puzzle, index))
                            return true; // This can still happen
                        puzzle.Pop();
                    }

                    if (puzzle.CurrentAxis == PuzzleAxis.Horizontal)
                        puzzle.Move(puzzle.CurrentRow, puzzle.CurrentColumn + 1);
                    else
                        puzzle.Move(puzzle.CurrentRow + 1, puzzle.CurrentColumn);
                }
            }

            return false;
        }

        private static void PrintMenu()
        {
            Console.Clear();
            Console.WriteLine(@"__________                              .__      __________                __                      .__   ");
            Console.WriteLine(@"\______   \_______   ____ _____    ____ |  |__   \______   \_______  _____/  |_  ____   ____  ____ |  |  ");
            Console.WriteLine(@" |    |  _/\_  __ \_/ __ \\__  \ _/ ___\|  |  \   |     ___/\_  __ \/  _ \   __\/  _ \_/ ___\/  _ \|  |  ");
            Console.WriteLine(@" |    |   \ |  | \/\  ___/ / __ \\  \___|   Y  \  |    |     |  | \(  <_> )  | (  <_> )  \__(  <_> )  |__");
            Console.WriteLine(@" |______  / |__|    \___  >____  /\___  >___|  /  |____|     |__|   \____/|__|  \____/ \___  >____/|____/");
            Console.WriteLine(@"        \/              \/     \/     \/     \/                                            \/            ");
            Console.WriteLine();
            Console.WriteLine("Breach Protocol is adapted from the hacking minigame in the game Cyberpunk 2077.");
            Console.WriteLine("You have to match the given sequence by selecting values within the matrix.");
            Console.WriteLine("You are limited to selecting values within either a row or column, which switches after every selection.");
            Console.WriteLine();
            Console.WriteLine("------------------- Keys -------------------");
            Console.WriteLine("Move: <UP> <DOWN> <LEFT> <RIGHT> arrow keys");
            Console.WriteLine("Select: <ENTER>");
            Console.WriteLine("Undo: <BACKSPACE>");
            Console.WriteLine("Solve: <F1> algorithm1 <F2> algorithm2");
            Console.WriteLine("Exit: <ESC>");
            Console.WriteLine();
            Console.WriteLine("------------------- Menu -------------------");
            Console.WriteLine("1) Play Breach Protocol");
            Console.WriteLine("2) Run Algorithm Tests");
            Console.WriteLine("Press any other key to exit.");
        }
    }
}