using BreachProtocol.Collections;
using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace BreachProtocol
{
    /// <summary>
    /// Represents a Breach Protocol puzzle.
    /// </summary>
    public class Puzzle
    {
        /// <summary>
        /// Contains all possible values used within the puzzle.
        /// </summary>
        public static readonly ImmutableArray<byte> ValidValues = ImmutableArray.Create<byte>(0x1C, 0x55, 0xBD, 0xE9, 0xFF);

        private readonly ArrayStack<PuzzleItem> _buffer;
        private readonly byte[,] _matrix;
        private readonly byte[] _sequence;

        /// <summary>
        /// Gets the maximum number of elements that can be stored in the buffer.
        /// </summary>
        public int BufferCapacity => _buffer.Capacity;

        /// <summary>
        /// Gets the current number of elements stored in the buffer.
        /// </summary>
        public int BufferCount => _buffer.Count;

        /// <summary>
        /// Gets the total number of positions in the matrix.
        /// </summary>
        public int MatrixCount => _matrix.Length;

        /// <summary>
        /// Gets the number of rows in the matrix.
        /// </summary>
        public int MatrixRows => _matrix.GetLength(0);

        /// <summary>
        /// Gets the number of columns in the matrix.
        /// </summary>
        public int MatrixColumns => _matrix.GetLength(1);

        /// <summary>
        /// Gets the currently selected row in the matrix.
        /// </summary>
        public int CurrentRow { get; private set; }

        /// <summary>
        /// Gets the currently selected column in the matrix.
        /// </summary>
        public int CurrentColumn { get; private set; }

        /// <summary>
        /// Gets the current axis of the matrix.
        /// </summary>
        public PuzzleAxis CurrentAxis { get; private set; }

        /// <summary>
        /// Gets the current winning sequence that must be found.
        /// </summary>
        public ReadOnlyCollection<byte> Sequence { get; }

        /// <summary>
        /// Creates a new instance of <see cref="Puzzle"/>.
        /// </summary>
        /// <param name="rows">The number or rows in the matrix.</param>
        /// <param name="columns">The number of columns in the matrix.</param>
        /// <param name="bufferCapacity">The capacity of the buffer.</param>
        /// <param name="sequenceLength">The length of the sequence.</param>
        public Puzzle(int rows, int columns, int bufferCapacity, int sequenceLength)
        {
            if (rows < 0)
                throw new ArgumentOutOfRangeException(nameof(rows));
            if (columns < 0)
                throw new ArgumentOutOfRangeException(nameof(columns));
            if (bufferCapacity < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferCapacity));
            if (sequenceLength < 0 || sequenceLength > bufferCapacity)
                throw new ArgumentOutOfRangeException(nameof(sequenceLength));

            _buffer = new(bufferCapacity);
            _matrix = new byte[rows, columns];
            _sequence = new byte[sequenceLength];
            Sequence = new ReadOnlyCollection<byte>(_sequence);
        }

        /// <summary>
        /// Gets the value in the buffer at the specified index.
        /// </summary>
        /// <param name="index">The index of the value to get.</param>
        /// <returns>The value at the specified index.</returns>
        public byte GetBufferValue(int index)
        {
            if (index < 0 || index >= _buffer.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _buffer[index].Value;
        }

        /// <summary>
        /// Gets the value in the matrix at the specified row and column.
        /// </summary>
        /// <param name="row">The row of the value to get.</param>
        /// <param name="column">The column of the value to get.</param>
        /// <returns>The value at the specified row and column.</returns>
        public byte GetMatrixValue(int row, int column)
        {
            if (row < 0 || row > _matrix.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(row));
            if (column < 0 || column > _matrix.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(column));

            return _matrix[row, column];
        }

        /// <summary>
        /// Removes a value from the matrix at the current row and column and pushes it onto the buffer.
        /// </summary>
        /// <returns>Returns true if the buffer contains the sequence. Otherwise returns false.</returns>
        public bool Push()
        {
            if (_buffer.Count == _buffer.Capacity)
                throw new InvalidOperationException("The buffer is full.");
            if (_matrix[CurrentRow, CurrentColumn] == 0)
                throw new InvalidOperationException("The specified location is already used.");

            _buffer.Push(new PuzzleItem(CurrentRow, CurrentColumn, _matrix[CurrentRow, CurrentColumn]));
            _matrix[CurrentRow, CurrentColumn] = 0;
            CurrentAxis = SwitchAxis(CurrentAxis);

            return IsSolved();
        }

        /// <summary>
        /// Pops the last value from the buffer, stores it back in the matrix, and sets the current row and
        /// column to match the position of the value. This is effectively an "undo".
        /// </summary>
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

        /// <summary>
        /// Moves the current row or column to the specified row or column. A row can only be changed if the
        /// current axis is vertical, and a column can only be changed if the current axis is horizontal.
        /// </summary>
        /// <param name="row">The new row.</param>
        /// <param name="column">The new column</param>
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

        /// <summary>
        /// Initializes the puzzle with a random matrix, clears the buffer, generates a new sequence, resets
        /// the current row and column to zero, and resets the axis to horizontal.
        /// </summary>
        public void Initialize()
        {
            _buffer.Clear();
            FillMatrix();
            CreateSequence();

            CurrentRow = 0;
            CurrentColumn = 0;
            CurrentAxis = PuzzleAxis.Horizontal;
        }

        public void Reset()
        {
            while (BufferCount > 0)
            {
                Pop();
            }
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

        private void CreateSequence()
        {
            byte[,] matrix = new byte[MatrixRows, MatrixColumns];
            Array.Copy(_matrix, matrix, matrix.Length);

            byte[] fullSequence = new byte[BufferCapacity];
            int row = 0;
            int col = 0;
            PuzzleAxis axis = PuzzleAxis.Horizontal;

            for (int i = 0; i < fullSequence.Length; i++)
            {
                do
                {
                    if (axis == PuzzleAxis.Horizontal)
                        col = Random.Shared.Next(MatrixColumns);
                    else
                        row = Random.Shared.Next(MatrixRows);
                } while (matrix[row, col] == 0);

                fullSequence[i] = matrix[row, col];
                matrix[row, col] = 0;
                axis = SwitchAxis(axis);
            }

            Array.Copy(fullSequence, Random.Shared.Next(fullSequence.Length - _sequence.Length), _sequence, 0, _sequence.Length);
        }

        public bool IsSolved()
        {
            // Short cirtcuit since there's no way for the buffer to contain the sequence if it
            // doesn't have enough items.
            if (_sequence.Length > _buffer.Count)
                return false;

            bool result = false;
            for (int i = 0; i <= _buffer.Count - _sequence.Length; i++)
            {
                result = true;
                for (int j = 0; j < _sequence.Length; j++)
                {
                    if (_buffer[i + j].Value != _sequence[j])
                    {
                        result = false;
                        break;
                    }
                }

                if (result)
                    break;
            }

            return result;
        }
    }
}
