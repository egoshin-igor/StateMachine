using Lekser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax
{
    public class VariablesTableController : IVariablesTableController
    {
        private List<List<Variable>> _tables = new List<List<Variable>>();
        private int _lastTableIndex = -1;

        public void CreateTable()
        {
            _tables.Add( new List<Variable>() );
            _lastTableIndex = _tables.Count - 1;
        }

        public void DestroyLastTable()
        {
            _tables.RemoveAt( _lastTableIndex );
            _lastTableIndex--;
        }

        public Term GetTerm( int id )
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

        public void DefineNewType( Term type )
        {
            if ( _lastTableIndex == -1 )
                throw new Exception( "Not exist any scope" );

            var lastVariable = _tables[ _lastTableIndex ].LastOrDefault();
            if ( lastVariable?.Identifier == null )
                throw new Exception( $"You try define new type:{type.Type.ToString()} without defining previous" );


            _tables[ _lastTableIndex ].Add( new Variable { Type = type } );
        }

        public void DefineIdentifier( Term identifier )
        {
            if ( _lastTableIndex == -1 )
                throw new Exception( "Not exist any scope" );

            Variable duplicate = _tables[ _lastTableIndex ]
                .Where( t => t.Identifier != null )
                .FirstOrDefault( t => t.Identifier.Id == identifier.Id );

            if ( duplicate != null )
                throw new Exception( $"Found duplicate: on string { identifier.NumberString } on position { identifier.RowPosition }" );
            Variable lastVariable = _tables[ _lastTableIndex ].LastOrDefault();
            if ( lastVariable == null || lastVariable?.Identifier != null )
                throw new Exception( $"Can't define identifier on string { identifier.NumberString } on position { identifier.RowPosition }" );

            lastVariable.Identifier = identifier;
        }
    }
}
