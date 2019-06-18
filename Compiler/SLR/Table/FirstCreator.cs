using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SLR.Utils;

namespace SLR.Table
{
    public sealed class FirstCreator
    {
        const string END_TOKEN = SpecialWords.End;

        public List<Sentence> Sentences { get; private set; } = new List<Sentence>();
        public TableOfFirsts TableOfFirsts => _tableOfFirsts;

        private Stack<Token> _stackOfVisited = new Stack<Token>();
        private Queue<Cell> _cellsQueue = new Queue<Cell>();
        private ISet<Token> _setOfVisited = new HashSet<Token>();

        private TableOfFirsts _tableOfFirsts = new TableOfFirsts();

        public FirstCreator( List<RawSentence> sentenses )
        {
            try
            {
                Sentences = SentenceConverter.ConvertRawSentences( sentenses );
                CreateHeaderRowOfTable();
                Create();
            }
            catch( StackOverflowException )
            {
                throw new ApplicationException( "Cicles exist" );
            }
        }

        public void WriteResult( TextWriter writer )
        {
            foreach ( var str in _tableOfFirsts.Column )
            {
                writer.Write( $"  { str } | " );
            }

            writer.WriteLine();

            for ( var i = 0; i < _tableOfFirsts.Row.Count; ++i )
            {
                foreach ( var x in _tableOfFirsts.Row[ i ].Values )
                {
                    writer.Write( $"{ x.Value } " );
                }

                writer.Write( $"| " );

                foreach ( var item in _tableOfFirsts.Table[ i ] )
                {
                    if ( item != null )
                    {
                        foreach ( var token in item.Values )
                        {
                            writer.Write( $"{ token.Value } " );
                        }
                    }
                    else
                    {
                        writer.Write( " " );
                    }

                    writer.Write( "| " );
                }

                writer.WriteLine();
            }
        }

        private void CreateHeaderRowOfTable()
        {
            foreach ( var sentence in Sentences )
            {
                foreach ( var token in sentence.Tokens )
                {
                    if ( !_tableOfFirsts.Column.Contains( token.Value ) )
                    {
                        _tableOfFirsts.Column.Add( token.Value );
                    }
                }
            }
        }

        private void Create()
        {
            var index = 0;
            var sentence = Sentences[ index ];
            var tokens = sentence.Tokens;
            if ( tokens.Count == 0 )
            {
                throw new ArgumentException( $"found empty sentence in row: { index }" );
            }

			Token firstToken = null;

			for ( var i = 0; i < tokens.Count; ++i )
            {
                if ( tokens[ i ].Type == TokenType.End )
                {
                    continue;
                }

                _setOfVisited.Add( tokens[ i ] );
                CreateFirstRowOfTable( tokens[ i ] );
				firstToken = tokens[i];
            }

			_tableOfFirsts.ExpandTable( new Cell( firstToken ) );
			var tokenListSecondRow = new List<Token> { tokens[1] };
			var cellsListSecondRow = CreateListOfCells(tokenListSecondRow);
			foreach (var c in cellsListSecondRow)
			{
				_tableOfFirsts.AddRuleInTable(c, $"[{ 0 }]");
			}
			

			while ( _cellsQueue.Count != 0 )
            {
                var cell = _cellsQueue.Dequeue();
                foreach ( var item in cell.Values )
                {
                    _setOfVisited.Add( item );
                }

                _tableOfFirsts.ExpandTable( cell );
                var tokensLists = new List<List<Token>>();
                foreach ( var value in cell.Values )
                {
                    _stackOfVisited.Push( value );
                    tokensLists.Add( GenerateFirsts( value ) );
                    _stackOfVisited.Pop();
                }

                for ( var i = 0; i < tokensLists.Count; ++i )
                {
                    var cells = CreateListOfCells( tokensLists[ i ] );
                    var isLast = !( cell.Values[ i ].ColIndex + 1 < Sentences[ cell.Values[ i ].RowIndex ].Tokens.Count );
                    if ( isLast )
                    {
                        foreach ( var c in cells )
                        {
                            _tableOfFirsts.AddRuleInTable( c, $"[{ cell.Values[ i ].RowIndex }]" );
                        }
                    }
                    else
                    {
                        _tableOfFirsts.AddInTable( cells );
                        AddCellsInQueue( cells );
                    }
                }
            }
        }

        private List<Token> GenerateFirsts( Token token )
        {
            var columnIndex = token.ColIndex;
            var rowIndex = token.RowIndex;
            var sentence = Sentences[ rowIndex ];
            var tokensList = new List<Token>();
            if ( columnIndex + 1 < sentence.Tokens.Count )
            {
                tokensList.Add( sentence.Tokens[ columnIndex + 1 ] );
                if ( sentence.Tokens[ columnIndex + 1 ].Type == TokenType.NonTerminal )
                {
                    _stackOfVisited.Push( sentence.Tokens[ columnIndex + 1 ] );
                    tokensList.AddRange( CountingInDepth( sentence.Tokens[ columnIndex + 1 ] ) );
                    _stackOfVisited.Pop();
                }
            }
            else
            {
                tokensList = ReverseCountInDepth( sentence.MainToken );
            }

            return tokensList;
        }

        private void CreateFirstRowOfTable( Token token )
        {
			var tokens = new List<Token>();
			if ( token.Type == TokenType.NonTerminal )
            {
                _stackOfVisited.Push( token );
                _tableOfFirsts.ExpandTable( new Cell( new Token( SpecialWords.Start, -1, -1 ) ), true );
				tokens.Add(token);
				tokens.AddRange( CountingInDepth( token ) );
                _stackOfVisited.Pop();

                var cells = CreateListOfCells( tokens );
                _tableOfFirsts.AddInTable( cells );
                AddCellsInQueue( cells );
            }
            else
            {
                throw new ArgumentException( "first token must be non terminal" );
            }
        }

        private void AddCellsInQueue( List<Cell> cells )
        {
            foreach ( var cell in cells )
            {
                var needToAddInQueue = true;
                foreach ( var value in cell.Values )
                {
                    if ( _setOfVisited.Contains( value ) )
                    {
                        needToAddInQueue = false;
                    }
                }

                if ( needToAddInQueue )
                {
                    _cellsQueue.Enqueue( cell );
                }
            }
        }

        private List<Cell> CreateListOfCells( IEnumerable<Token> list )
        {
            var distinctTokens = list.Distinct();

            var tokensGoups = distinctTokens.GroupBy( token => token.Value );

            var cells = new List<Cell>();
            foreach ( var item in tokensGoups )
            {
                var listTokens = item.ToList();
                cells.Add( new Cell( listTokens ) );
            }

            return cells;
        }

        private List<Token> CountingInDepth( Token token )
        {
            var tokensList = new List<Token>();

            for ( var i = 0; i < Sentences.Count; ++i )
            {
                if ( Sentences[ i ].MainToken != token.Value )
                {
                    continue;
                }

                if ( Sentences[ i ].Tokens[ 0 ].Value == token.Value )
                {
                    if ( _stackOfVisited.Contains( Sentences[ i ].Tokens[ 0 ] ) )
                    {
                        throw new Exception( $"loop detected! { token.Value } { token.ColIndex } { token.RowIndex }" );
                    }

                    tokensList.Add( Sentences[ i ].Tokens[ 0 ] );
                    continue;
                }

                if ( Sentences[ i ].Tokens[ 0 ].Type == TokenType.NonTerminal )
                {
                    tokensList.Add( Sentences[ i ].Tokens[ 0 ] );
                    _stackOfVisited.Push( token );
                    tokensList.AddRange( CountingInDepth( Sentences[ i ].Tokens[ 0 ] ) );
                    _stackOfVisited.Pop();
                }
                else
                {
                    tokensList.Add( Sentences[ i ].Tokens[ 0 ] );
                }
            }

            return tokensList;
        }

        private List<Token> ReverseCountInDepth( string value )
        {
            var tokensList = new List<Token>();
            for ( var i = 0; i < Sentences.Count; ++i )
            {
                var sentence = Sentences[ i ];
                if ( IsListContainsValue( value, sentence.Tokens ) )
                {
                    var tokens = sentence.Tokens;
                    for ( var j = 0; j < tokens.Count; ++j )
                    {
                        if ( tokens[ j ].Value == value )
                        {
                            var index = j + 1;
                            if ( index < tokens.Count )
                            {
                                if ( tokens[ index ].Type == TokenType.NonTerminal )
                                {
                                    if ( _stackOfVisited.Contains( tokens[ index ] ) )
                                    {
                                        throw new Exception( $"loop detected! { tokens[ index ].Value } { tokens[ index ].ColIndex } { tokens[ index ].RowIndex }" );
                                    }

                                    _stackOfVisited.Push( tokens[ index ] );
                                    tokensList.AddRange( CountingInDepth( tokens[ index ] ) );
                                    _stackOfVisited.Pop();
                                }
                                else
                                {
                                    tokensList.Add( tokens[ index ] );
                                }
                            }
                            else
                            {
                                tokensList.AddRange( ReverseCountInDepth( sentence.MainToken ) );
                            }
                        }
                    }
                }
            }

            return tokensList;
        }

        private bool IsListContainsValue( string value, List<Token> list )
        {
            IEnumerable<Token> result = list.Where( token => token.Value == value );

            return result.Count() != 0;
        }
    }
}
