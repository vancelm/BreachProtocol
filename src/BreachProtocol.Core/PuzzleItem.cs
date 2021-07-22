namespace BreachProtocol
{
    internal struct PuzzleItem
    {
        public readonly int Row;
        public readonly int Column;
        public readonly byte Value;

        internal PuzzleItem(int row, int column, byte value)
        {
            Row = row;
            Column = column;
            Value = value;
        }
    }
}
