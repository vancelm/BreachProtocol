namespace BreachProtocol
{
    /// <summary>
    /// Represents a value from a row and column position in a matrix.
    /// </summary>
    internal struct PuzzleItem
    {
        /// <summary>
        /// Gets the row of this <see cref="PuzzleItem"/>.
        /// </summary>
        internal readonly int Row;

        /// <summary>
        /// Gets the column of this <see cref="PuzzleItem"/>.
        /// </summary>
        internal readonly int Column;

        /// <summary>
        /// Gets the value of this <see cref="PuzzleItem"/>.
        /// </summary>
        internal readonly byte Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="PuzzleItem"/> struct with the specified row, column, and value.
        /// </summary>
        /// <param name="row">The row of this <see cref="PuzzleItem"/></param>
        /// <param name="column">The column of this <see cref="PuzzleItem"/></param>
        /// <param name="value">The value of this <see cref="PuzzleItem"/></param>
        internal PuzzleItem(int row, int column, byte value)
        {
            Row = row;
            Column = column;
            Value = value;
        }
    }
}
