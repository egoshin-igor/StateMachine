using Lekser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax
{
	public class VariablesTableController : IVariablesTableController
	{
		private List<List<Term>> _tables = new List<List<Term>>();
		private int _lastTableIndex = -1;

		public void CreateTable()
		{
			_tables.Add( new List<Term>() );
			_lastTableIndex = _tables.Count - 1;
		}

		public void DestroyLastTable()
		{
			_tables.RemoveAt( _lastTableIndex );
			_lastTableIndex--;
		}

		public Term GetTerm(int id)
		{
			var tableIndex = _lastTableIndex;

			Term term = null;
			while ( tableIndex != -1 )
			{
				term = _tables[ tableIndex ].FirstOrDefault( t => t.Id == id );
				tableIndex--;

				if ( term != null ) break;
			}

			return term;
		}

		public void InsertToCurrentTable(Term term)
		{
			var duplicate = _tables[ _lastTableIndex ].FirstOrDefault( t => t.Id == term.Id );

			if ( duplicate != null )
			{
				throw new Exception($"Found duplicate: on string { term.NumberString } on position { term.RowPosition }");
			}

			_tables[ _lastTableIndex ].Add( term );
		}
	}
}
