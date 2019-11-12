using System;
using System.Collections.Generic;
using System.Linq;
using Lekser;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax
{
    public class AriphmeticalOperationsController
    {
        public List<Term> Numbers { get; } = new List<Term>();
        public void AddNewNumber( Term number )
        {
            Numbers.Add( number );

            bool numbersIsUnique = Numbers.Select( n => n.Type ).Distinct().Count() == 1;
            if ( !numbersIsUnique )
                throw new ApplicationException( $"Not all number types are equal. Number:{number.Value} in row {number.RowPosition}." );
        }

        public void Clear()
        {
            Numbers.Clear();
        }
    }
}
