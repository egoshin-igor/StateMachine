using System.Collections.Generic;
using System.Linq;
using SyntacticalAnalyzerGenerator.Words;

namespace SyntacticalAnalyzerGenerator
{
    public class SyntacticalAnalyzerGenerator
    {
        private List<Expression> _expressions;
        private List<List<Expression>> _groupedExpressions;
        private string _mainNoTermName;
        private List<TableRow> _result = new List<TableRow>();

        public SyntacticalAnalyzerGenerator( List<Expression> expressions, string mainNoTermName )
        {
            _expressions = expressions;
            _groupedExpressions = expressions.GroupBy( e => e.NoTerm.Name ).Select( g => g.ToList() ).ToList();
            _mainNoTermName = mainNoTermName;
        }

        public List<ResultTableRow> Generate()
        {
            _result.Clear();
            InitRows();
            for ( int i = 0; i < _result.Count; i++ )
            {
                TableRow row = _result[ i ];
                if ( row.Word.Type == WordType.RightNoTerm )
                {
                    row.GoTo = _result.FindIndex( r => r.Parent.Name == row.Word.Name );
                }
                else if ( i + 1 == _result.Count || row.Parent != _result[ i + 1 ].Parent )
                {
                    row.GoTo = -1;
                }
                else
                {
                    row.GoTo = i + 1;
                }
            }

            List<List<TableRow>> resultGroupedByParent = _result
                .GroupBy( r => r.Parent.Name )
                .Select( g => g.ToList() )
                .ToList();

            foreach ( List<TableRow> rows in resultGroupedByParent )
            {
                int current = rows[ 0 ].N;
                while ( current != -1 )
                {
                    _result[ current ].ShiftOnError = _result[ current ].NextParallelRow;
                    current = _result[ current ].NextParallelRow;
                }
            }
            for ( int i = 0; i < _result.Count; i++ )
            {
                TableRow row = _result[ i ];
                if ( row.Word.Name == Word.Epsilant )
                {
                    if ( row.IsFirst )
                    {
                        row.DirectingSet = row.Parent.DirectingSet;
                    }
                    else if ( row.IsLast )
                    {
                        row.DirectingSet = GetDirectingSetToEpsilant( GetParentRowByName( row.Parent.Name ).N );
                    }
                    else
                    {
                        TableRow nextRow = _result[ i + 1 ];
                        row.DirectingSet = nextRow.DirectingSet;
                    }
                }
            }

            return _result.ConvertAll( r => new ResultTableRow
            {
                DirectingSet = r.DirectingSet,
                GoTo = r.GoTo,
                IsEnd = r.IsEnd,
                IsPushToStack = r.IsPushToStack,
                IsShift = r.IsShift,
                N = r.N,
                Name = r.Name,
                TermType = r.TermType,
                ShiftOnError = r.ShiftOnError
            } );
        }

        private HashSet<string> GetDirectingSetToEpsilant( int tableIndex )
        {
            var result = new HashSet<string>();
            for ( int i = 0; i < _result.Count; i++ )
            {
                TableRow row = _result[ i ];
                if ( row.GoTo != tableIndex )
                    continue;

                if ( row.IsLast )
                {
                    AddRangeToSet( GetDirectingSetToEpsilant( GetParentRowByName( row.Parent.Name ).N ), result );
                }
                else
                {
                    Word nextWord = _result[ i + 1 ].Word;
                    AddRangeToSet( nextWord.DirectingSet, result );
                }
            }

            return result;
        }

        private void AddRangeToSet( HashSet<string> from, HashSet<string> to )
        {
            foreach ( string str in from )
            {
                to.Add( str );
            }
        }

        private void InitRows()
        {
            if ( _expressions.Count == 0 )
                return;

            List<Expression> mainExpressions = _expressions.Where( e => e.NoTerm.Name == _mainNoTermName ).ToList();
            int rowIndex = 0;
            for ( int j = 0; j < _expressions.Count; j++ )
            {
                Expression expression = _expressions[ j ];

                for ( int i = 0; i < expression.Words.Count; i++ )
                {
                    Word word = expression.Words[ i ];
                    var row = new TableRow
                    {
                        Name = word.Name,
                        TermType = word.TermType,
                        N = rowIndex,
                        DirectingSet = word.DirectingSet,
                        Word = word,
                        Parent = expression.NoTerm,
                        IsFirst = i == 0,
                        IsLast = i + 1 == expression.Words.Count
                    };

                    int nextParallelRow = row.N + expression.Words.Count - i;
                    if ( j + 1 == _expressions.Count || _expressions[ j + 1 ].NoTerm.Name != row.Parent.Name )
                    {
                        row.NextParallelRow = -1;
                    }
                    else
                    {
                        row.NextParallelRow = nextParallelRow;
                    }

                    row.IsShift = word.Type == WordType.Term;
                    row.IsPushToStack = word.Type == WordType.RightNoTerm && !row.IsLast;
                    _result.Add( row );
                    rowIndex++;
                }
            }

            int endIndex = mainExpressions.Sum( e => e.Words.Count ) - 1;
            _result[ endIndex ].IsEnd = true;
        }

        private TableRow GetParentRowByName( string name )
        {
            return _result.First( r => r.Parent.Name == name );
        }
    }
}
