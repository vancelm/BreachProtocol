using BreachProtocol.Collections;
using System;

namespace BreachProtocol
{
    public class Puzzle
    {
        private readonly byte[,] _matrix;
        private readonly ArrayStack<PuzzleItem> _buffer;

        public int BufferCapacity => _buffer.Capacity;
        public int BufferCount => _buffer.Count;
        public int MatrixCount => _matrix.Length;
        public int MatrixRows => _matrix.GetLength(0);
        public int MatrixColumns => _matrix.GetLength(1);

        public int CurrentRow { get; private set; }
        public int CurrentColumn { get; private set; }
        public PuzzleAxis CurrentAxis { get; private set; }

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

        public Puzzle(int rows, int columns, int bufferCapacity)
        {
            if (rows < 0)
                throw new ArgumentOutOfRangeException(nameof(rows));
            if (columns < 0)
                throw new ArgumentOutOfRangeException(nameof(columns));
            if (bufferCapacity < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferCapacity));

            _matrix = new byte[rows, columns];
            _buffer = new(bufferCapacity);
        }

        public void Push()
        {
            if (_buffer.Count == _buffer.Capacity)
                throw new InvalidOperationException("The buffer is full.");
            if (_matrix[CurrentRow, CurrentColumn] == 0)
                throw new InvalidOperationException("The specified location is already used.");

            _buffer.Push(new PuzzleItem(CurrentRow, CurrentColumn, _matrix[CurrentRow, CurrentColumn]));
            _matrix[CurrentRow, CurrentColumn] = 0;
        }

        public void Pop()
        {
            if (_buffer.Count == 0)
                throw new InvalidOperationException("The buffer is empty.");
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

        private void ChangeDirection()
        {
            if (CurrentAxis == PuzzleAxis.Horizontal)
                CurrentAxis = PuzzleAxis.Vertical;
            else
                CurrentAxis = PuzzleAxis.Horizontal;
        }
    }
}
