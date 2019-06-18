using System.Collections.Generic;
using System.Linq;

namespace SLR.Table
{
    public sealed class TableOfFirsts
    {
        /// <summary>
        /// headers of table columns
        /// </summary>
        public List<string> Column = new List<string>();
        public List<Token> TokensColumn => Column.Select( s => new Token( s ) ).ToList();

        /// <summary>
        /// headers of table rows
        /// </summary>
        public List<Cell> Row = new List<Cell>();

        /// <summary>
        /// Start index
        /// </summary>
        public string StartToken => Column.Count > 0 ? Column[ 0 ] : SpecialWords.End;

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

        public void AddRuleInTable( Cell cell, string rule )
        {
            var columnIndex = Column.IndexOf( cell.Value );
            var tempCell = new Cell( cell );
            foreach ( var value in tempCell.Values )
            {
                value.Value = rule;
                value.Type = TokenType.Rule;
            }

            Table[ _currentIndex ][ columnIndex ] = tempCell;
        }

        public void AddInTable( List<Cell> cells )
        {
            foreach ( var cell in cells )
            {
                var columnIndex = Column.IndexOf( cell.Value );
                if ( Table[ _currentIndex ][ columnIndex ] != null )
                {
                    foreach ( var item in cell.Values )
                    {
                        Table[ _currentIndex ][ columnIndex ].Values.Add( item );
                    }
                }
                else
                {
                    Table[ _currentIndex ][ columnIndex ] = cell;
                }
            }
        }
    }
}
