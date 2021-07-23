namespace BreachProtocol
{
    internal struct PuzzleItem
    {
        internal readonly int Row;
        internal readonly int Column;
        internal readonly byte Value;

        internal PuzzleItem(int row, int column, byte value)
        {
            Row = row;
            Column = column;
            Value = value;
        }
    }
}
