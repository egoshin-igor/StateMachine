using System.Collections.Generic;

namespace SLR.Table
{
    public sealed class TableOfFirsts
    {
        /// <summary>
        /// headers of table columns
        /// </summary>
        public List<string> Column = new List<string>();

        /// <summary>
        /// headers of table rows
        /// </summary>
        public List<Cell> Row = new List<Cell>();

        /// <summary>
        /// Start index
        /// </summary>
        public string StartToken => Column.Count > 0 ? Column[ 0 ] : "[END]";

        /// <summary>
        /// table grid
        /// </summary>
        public List<List<Cell>> Table { get; } = new List<List<Cell>>();

        private int _currentIndex = 0;

        public void ExpandTable( Cell cell, bool firstCall = false )
        {
            Row.Add( cell );
            var row = new List<Cell>( Column.Count );
            for ( int i = 0; i < Column.Count; i++ ) row.Add( null );
            Table.Add( row );
            if ( !firstCall )
            {
                _currentIndex++;
            }
        }

        public void AddInTable( Cell cell )
        {
            var columnIndex = Column.IndexOf( cell.Value );
            Table[ _currentIndex ][ columnIndex ] = cell;
        }
    }
}
