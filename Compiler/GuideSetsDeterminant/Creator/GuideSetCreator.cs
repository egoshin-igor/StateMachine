using System.Collections.Generic;
using System.IO;

namespace GuideSetsDeterminant.Creator
{
    public sealed class GuideSetCreator
    {
        const string END_TOKEN = "[END]";
        const string START_LINK = "<";
        const string EMPTY_LINK = "[EPS]";

        private List<Sentence> _sentences = new List<Sentence>();
        private Stack<string> _stackOfEmpties = new Stack<string>();
        private string _startToken = "";

        public GuideSetCreator( List<Sentence> sentenses )
        {
            _sentences = sentenses;
            Create();
        }

        public void WriteResultToStream( TextWriter writer )
        {
            foreach ( var s in _sentences )
            {
                writer.WriteLine( $"{ s.MainToken } -> { TokensToString( s.Tokens, ' ' ) } / { TokensToString( s.ForwardSet, ',' ) }" );
            }
        }

        private string TokensToString( List<string> list, char delimeter )
        {
            var str = "";
            for ( var i = 0; i < list.Count; ++i )
            {
                str += list[ i ];
                if ( i < list.Count - 1 )
                {
                    str += delimeter;
                }
            }

            return str;
        }

        private void Create()
        {
            if ( _sentences.Count != 0 )
            {
                _startToken = _sentences[ 0 ].MainToken;
                var listOfTokens = new List<string>();
                listOfTokens.Add( END_TOKEN );
                _sentences.Add( new Sentence( _startToken, listOfTokens ) );
            }

            for ( var i = 0; i < _sentences.Count; ++i )
            {
                if ( _sentences[ i ].Tokens[ 0 ].StartsWith( START_LINK ) )
                {
                    _sentences[ i ].AddInSet( CalculateCurrent( _sentences[ i ].Tokens[ 0 ] ) );
                }
                else if ( _sentences[ i ].Tokens[ 0 ] == EMPTY_LINK )
                {
                    _stackOfEmpties.Push( _sentences[ i ].MainToken );
                    _sentences[ i ].AddInSet( CalculateEmptyCurrent( _sentences[ i ].MainToken ) );
                    _stackOfEmpties.Pop();
                }
                else
                {
                    _sentences[ i ].AddInSet( _sentences[ i ].Tokens[ 0 ] );
                }
            }
            if ( _sentences.Count != 0 )
            {
                _sentences.RemoveAt( _sentences.Count - 1 );
            }
        }

        private List<string> CalculateCurrent( string token )
        {
            var generatedSet = new List<string>();
            for ( var i = 0; i < _sentences.Count; ++i )
            {
                if ( _sentences[ i ].MainToken == token )
                {
                    if ( _sentences[ i ].ForwardSet.Count != 0 ) // has forward
                    {
                        AddInLocalSet( generatedSet, _sentences[ i ].ForwardSet );
                    }
                    else if ( _sentences[ i ].Tokens[ 0 ].StartsWith( START_LINK ) )
                    {
                        var set = CalculateCurrent( _sentences[ i ].Tokens[ 0 ] );
                        AddInLocalSet( generatedSet, set );
                        _sentences[ i ].AddInSet( set );
                    }
                    else if ( _sentences[ i ].Tokens[ 0 ] == EMPTY_LINK )
                    {
                        _stackOfEmpties.Push( _sentences[ i ].MainToken );
                        var set = CalculateEmptyCurrent( _sentences[ i ].MainToken );
                        _stackOfEmpties.Pop();
                        AddInLocalSet( generatedSet, set );
                        _sentences[ i ].AddInSet( set );
                    }
                    else
                    {
                        _sentences[ i ].AddInSet( _sentences[ i ].Tokens[ 0 ] );
                        AddInLocalSet( generatedSet, _sentences[ i ].Tokens[ 0 ] );
                    }
                }
            }

            return generatedSet;
        }

        private List<string> CalculateEmptyCurrent( string token )
        {
            var generatedSet = new List<string>();
            for ( var i = 0; i < _sentences.Count; ++i ) // search token existence
            {
                if ( token != _sentences[ i ].MainToken && !_stackOfEmpties.Contains( _sentences[ i ].MainToken ) && _sentences[ i ].Tokens.Contains( token ) ) // if contains
                {
                    var currentIndex = 0;
                    while ( currentIndex < _sentences[ i ].Tokens.Count ) // search token in tokens
                    {
                        if ( _sentences[ i ].Tokens[ currentIndex ] == token ) // token found
                        {
                            var seachedIndex = currentIndex + 1;
                            if ( seachedIndex < _sentences[ i ].Tokens.Count ) // token not last
                            {
                                if ( _sentences[ i ].Tokens[ seachedIndex ].StartsWith( START_LINK ) ) // is link
                                {
                                    AddInLocalSet( generatedSet, CalculateCurrent( _sentences[ i ].Tokens[ seachedIndex ] ) );
                                }
                                else // is determinant
                                {
                                    AddInLocalSet( generatedSet, _sentences[ i ].Tokens[ seachedIndex ] );
                                }
                            }
                            else // token is last
                            {
                                _stackOfEmpties.Push( _sentences[ i ].MainToken );
                                AddInLocalSet( generatedSet, CalculateEmptyCurrent( _sentences[ i ].MainToken ) );
                                _stackOfEmpties.Pop();
                                if ( _sentences[ i ].MainToken == _startToken )
                                {
                                    AddInLocalSet( generatedSet, END_TOKEN );
                                }
                            }
                        }

                        ++currentIndex;
                    }
                }
            }

            return generatedSet;
        }

        private void AddInLocalSet( List<string> set, List<string> items )
        {
            foreach ( var item in items )
            {
                AddInLocalSet( set, item );
            }
        }

        private void AddInLocalSet( List<string> set, string item )
        {
            if ( !set.Contains( item ) )
            {
                set.Add( item );
            }
        }
    }
}
