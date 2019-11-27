using System;
using System.Collections.Generic;
using Lekser;
using Lekser.Enums;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax
{
    public class AriphmeticalOperationsController
    {
        public List<Term> Numbers { get; } = new List<Term>();

		public TermType? TermType = null;

		public void AddNewNumber( Term number )
        {
			if ( !TermType.HasValue )
			{
				TermType = number.Type;
			}

			Numbers.Add( number );

            bool numbersIsUnique = number.Type == TermType.Value;

            if ( !numbersIsUnique )
                throw new ApplicationException( $"Not all number types are equal. Number:{number.Value} in row {number.RowPosition}." );
		}

		public void AddNewVariable( Term variable, TermType variableType )
		{
			if ( !TermType.HasValue )
			{
				TermType = variableType;
			}

			Numbers.Add( variable );

			bool numbersIsUnique = variableType == TermType.Value;

			if ( !numbersIsUnique )
				throw new ApplicationException($"Not all variable types are equal. Number:{variable.Value} in row {variable.RowPosition}.");
		}

		public void Clear()
        {
            Numbers.Clear();
        }
    }
}
