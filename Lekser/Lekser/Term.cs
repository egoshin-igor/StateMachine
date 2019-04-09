using Lekser.Enums;

namespace Lekser
{
    public class Term
    {
        public int Id { get; private set; }
        public TermType Type { get; private set; }
        public int RowPosition { get; private set; }
        public int ColumnPosition { get; private set; }
        public string NumberString { get; private set; }

        public Term( int id, TermType termType, int rowPosition, int columnPosition, string numberString = null )
        {
            Id = id;
            Type = termType;
            RowPosition = rowPosition;
            ColumnPosition = columnPosition;
            NumberString = numberString;
        }

        public void SetNumberString( string numberString )
        {
            NumberString = numberString;
        }
    }
}
