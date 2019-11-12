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

        public Variable GetVariable( int id )
        {
            var tableIndex = _lastTableIndex;

            Variable result = null;
            while ( tableIndex != -1 )
            {
                result = _tables[ tableIndex ].FirstOrDefault( t => t.Identifier?.Id == id );
                tableIndex--;

                if ( result != null ) break;
            }

            return result;
        }

        /// <summary>
        /// Определяет тип для новой переменной
        /// </summary>
        /// <param name="type"></param>
        public void DefineNewType( Term type )
        {
            if ( _lastTableIndex == -1 )
                throw new ApplicationException( "Not exist any scope" );

            var lastVariable = _tables[ _lastTableIndex ].LastOrDefault();
            if ( lastVariable != null && lastVariable.Identifier == null )
                throw new ApplicationException( $"You try define new type:{type.Type.ToString()} without defining previous" );


            _tables[ _lastTableIndex ].Add( new Variable { Type = type } );
        }

        /// <summary>
        /// Определяет идентификатор для уже объявленной переменной
        /// </summary>
        /// <param name="identifier"></param>
        public void DefineIdentifier( Term identifier )
        {
            if ( _lastTableIndex == -1 )
                throw new ApplicationException( "Not exist any scope" );

            Variable duplicate = _tables[ _lastTableIndex ]
                .Where( t => t.Identifier != null )
                .FirstOrDefault( t => t.Identifier.Id == identifier.Id );

            if ( duplicate != null )
                throw new ApplicationException( $"Found duplicate: on row { identifier.RowPosition }" );
            Variable lastVariable = _tables[ _lastTableIndex ].LastOrDefault();
            if ( lastVariable == null || lastVariable?.Identifier != null )
                throw new ApplicationException( $"Can't define identifier on row { identifier.RowPosition }" );

            lastVariable.Identifier = identifier;
        }
    }
}
