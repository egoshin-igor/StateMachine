using System;
using System.Collections.Generic;
using System.Text;

namespace Lekser
{
    public class UndefinedTerm
    {
        public string Id { get; private set; }
        public int RowPosition { get; private set; }
        public int ColumnPosition { get; private set; }

        public UndefinedTerm( string id, int rowPosition, int columnPosition )
        {
            Id = id;
            RowPosition = rowPosition;
            ColumnPosition = columnPosition;
        }
    }
}
