using BreachProtocol.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace BreachProtocol
{
    public class Puzzle
    {
        public static readonly ImmutableArray<byte> ValidValues = ImmutableArray.Create<byte>(0x1C, 0x55, 0xBD, 0xE9, 0xFF);

        private readonly ArrayStack<PuzzleItem> _buffer;
        private readonly byte[,] _matrix;
        private readonly byte[] _solution;

        public int BufferCapacity => _buffer.Capacity;
        public int BufferCount => _buffer.Count;
        public int MatrixCount => _matrix.Length;
        public int MatrixRows => _matrix.GetLength(0);
        public int MatrixColumns => _matrix.GetLength(1);
        public int CurrentRow { get; private set; }
        public int CurrentColumn { get; private set; }
        public PuzzleAxis CurrentAxis { get; private set; }
        public ReadOnlyCollection<byte> Solution { get; }

        public Puzzle(int rows, int columns, int bufferCapacity, int solutionLength)
        {
            if (rows < 0)
                throw new ArgumentOutOfRangeException(nameof(rows));
            if (columns < 0)
                throw new ArgumentOutOfRangeException(nameof(columns));
            if (bufferCapacity < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferCapacity));
            if (solutionLength < 0 || solutionLength > bufferCapacity)
                throw new ArgumentOutOfRangeException(nameof(solutionLength));

            _buffer = new(bufferCapacity);
            _matrix = new byte[rows, columns];
            _solution = new byte[solutionLength];
            Solution = new ReadOnlyCollection<byte>(_solution);
        }

        public byte GetBufferValue(int index)
        {
            if (index < 0 || index >= _buffer.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _buffer[index].Value;
        }

        public byte GetMatrixValue(int row, int column)
        {
            if (row < 0 || row > _matrix.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(row));
            if (column < 0 || column > _matrix.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(column));

            return _matrix[row, column];
        }

        public bool Push()
        {
            if (_buffer.Count == _buffer.Capacity)
                throw new InvalidOperationException("The buffer is full.");
            if (_matrix[CurrentRow, CurrentColumn] == 0)
                throw new InvalidOperationException("The specified location is already used.");

            _buffer.Push(new PuzzleItem(CurrentRow, CurrentColumn, _matrix[CurrentRow, CurrentColumn]));
            _matrix[CurrentRow, CurrentColumn] = 0;
            CurrentAxis = SwitchAxis(CurrentAxis);

            return ContainsSolution();
        }

        public void Pop()
        {
            if (_buffer.Count == 0)
                throw new InvalidOperationException("The buffer is empty.");

            PuzzleItem item = _buffer.Pop();
            _matrix[item.Row, item.Column] = item.Value;
            CurrentRow = item.Row;
            CurrentColumn = item.Column;
            CurrentAxis = SwitchAxis(CurrentAxis);
        }

        public void Move(int row, int column)
        {
            if (CurrentAxis == PuzzleAxis.Horizontal && row != CurrentRow)
                throw new InvalidOperationException("Can only change column when direction is horizontal.");
            if (CurrentAxis == PuzzleAxis.Vertical && column != CurrentColumn)
                throw new InvalidOperationException("Can only change row when direction is vertical.");
            if (row < 0 || row > _matrix.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(row));
            if (column < 0 || column > _matrix.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(column));

            CurrentRow = row;
            CurrentColumn = column;
        }

        public void Initialize()
        {
            _buffer.Clear();
            FillMatrix();
            CreateSolution();

            CurrentRow = 0;
            CurrentColumn = 0;
            CurrentAxis = PuzzleAxis.Horizontal;
        }

        private void FillMatrix()
        {
            for (int row = 0; row < MatrixRows; row++)
                for (int col = 0; col < MatrixColumns; col++)
                    _matrix[row, col] = GetRandomValue();
        }

        private static PuzzleAxis SwitchAxis(PuzzleAxis axis)
        {
            if (axis == PuzzleAxis.Horizontal)
                return PuzzleAxis.Vertical;
            else
                return PuzzleAxis.Horizontal;
        }

        private byte GetRandomValue()
        {
            int index = Random.Shared.Next(ValidValues.Length);
            return ValidValues[index];
        }

        private void CreateSolution()
        {
            byte[,] matrix = new byte[MatrixRows, MatrixColumns];
            Array.Copy(_matrix, matrix, matrix.Length);

            byte[] fullSolution = new byte[BufferCapacity];
            int row = 0;
            int col = 0;
            PuzzleAxis axis = PuzzleAxis.Horizontal;

            for (int i = 0; i < fullSolution.Length; i++)
            {
                do
                {
                    if (axis == PuzzleAxis.Horizontal)
                    {
                        col = Random.Shared.Next(MatrixColumns);
                    }
                    else
                    {
                        row = Random.Shared.Next(MatrixRows);
                    }
                } while (matrix[row, col] == 0);

                fullSolution[i] = matrix[row, col];
                matrix[row, col] = 0;
                axis = SwitchAxis(axis);
            }

            Array.Copy(fullSolution, Random.Shared.Next(fullSolution.Length - _solution.Length), _solution, 0, _solution.Length);
        }

        private bool ContainsSolution()
        {
            bool result = false;

            for (int i = 0; i <= _buffer.Count - _solution.Length; i++)
            {
                result = true;
                for (int j = 0; j < _solution.Length; j++)
                {
                    if (_buffer[i + j].Value != _solution[j])
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
    }
}
