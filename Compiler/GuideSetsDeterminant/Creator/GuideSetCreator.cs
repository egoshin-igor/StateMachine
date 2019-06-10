using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GuideSetsDeterminant.Creator
{
    public sealed class GuideSetCreator
    {
        const string END_TOKEN = "[END]";
        const string START_LINK = "<";
        const string EMPTY_LINK = "[EPS]";

        public List<Sentence> Sentences { get; private set; } = new List<Sentence>();
        private Stack<int> _stackOfEmpties = new Stack<int>();
        private string _startToken = "";

        private string _TempToken = "";

        public GuideSetCreator( List<Sentence> sentenses )
        {
            if ( !IsWithoutCircles( sentenses ) )
                throw new ApplicationException( "Not valid lang. Circles exist" );

            Sentences = sentenses;
            Create();
            if ( !IsWithoutIntersections() )
                throw new ApplicationException( "Not valid lang. Intersections exist" );
        }

        public void WriteResultToStream( TextWriter writer )
        {
            foreach ( var s in Sentences )
            {
                writer.WriteLine( $"{ s.MainToken } -> { TokensToString( s.Tokens, ' ' ) } / { TokensToString( s.ForwardSet, ',' ) }" );
            }
        }

        private bool IsWithoutCircles( List<Sentence> sentenses )
        {
            bool result = true;
            Dictionary<string, List<Sentence>> sentensesByName = sentenses.GroupBy( s => s.MainToken ).ToDictionary( g => g.Key, g => g.ToList() );
            foreach ( Sentence sentence in sentenses )
            {
                if ( !sentence.Tokens.First().StartsWith( START_LINK ) )
                    continue;
                if ( sentence.Tokens.Count == 0 )
                    continue;

                if ( !IsWithoutCircles( sentensesByName, sentence.MainToken, sentence.Tokens.First() ) )
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        private bool IsWithoutCircles( Dictionary<string, List<Sentence>> sentensesByName, string mainToken, string currentToken )
        {
            if ( mainToken == currentToken )
                return false;

            var sentences = sentensesByName[ currentToken ];
            foreach ( Sentence sentence in sentences )
            {
                if ( sentence.Tokens.Count == 0 )
                    continue;
                if ( !sentence.Tokens.First().StartsWith( START_LINK ) )
                    continue;
                if ( !IsWithoutCircles( sentensesByName, mainToken, sentence.Tokens.First() ) )
                    return false;
            }

            return true;
        }

        private bool IsWithoutIntersections()
        {
            var sentensesGroupsByName = Sentences.GroupBy( s => s.MainToken );
            foreach ( var sentensesGroupByName in sentensesGroupsByName )
            {
                List<Sentence> sentenses = sentensesGroupByName.ToList();
                foreach ( Sentence sentense in sentenses )
                {
                    if ( sentenses.Any( s => s != sentense && s.ForwardSet.Intersect( sentense.ForwardSet ).Count() != 0 ) )
                        return false;
                }
            }

            return true;
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
            if ( Sentences.Count != 0 )
            {
                _startToken = Sentences[ 0 ].MainToken;
                var listOfTokens = new List<string>();
                listOfTokens.Add( END_TOKEN );
                Sentences.Add( new Sentence( _startToken, listOfTokens ) );
            }

            for ( var i = 0; i < Sentences.Count; ++i )
            {
                _TempToken = Sentences[ i ].MainToken;

                if ( Sentences[ i ].Tokens[ 0 ].StartsWith( START_LINK ) )
                {
                    Sentences[ i ].AddInSet( CalculateCurrent( Sentences[ i ].Tokens[ 0 ] ) );
                }
                else if ( Sentences[ i ].Tokens[ 0 ] == EMPTY_LINK )
                {
                    _stackOfEmpties.Push( i );
                    Sentences[ i ].AddInSet( CalculateEmptyCurrent( Sentences[ i ].MainToken ) );
                    _stackOfEmpties.Pop();
                }
                else
                {
                    Sentences[ i ].AddInSet( Sentences[ i ].Tokens[ 0 ] );
                }
            }
            if ( Sentences.Count != 0 )
            {
                Sentences.RemoveAt( Sentences.Count - 1 );
            }
        }

        private List<string> CalculateCurrent( string token )
        {
            var generatedSet = new List<string>();
            for ( var i = 0; i < Sentences.Count; ++i )
            {
                if ( Sentences[ i ].MainToken == token )
                {
                    if ( Sentences[ i ].ForwardSet.Count != 0 ) // has forward
                    {
                        AddInLocalSet( generatedSet, Sentences[ i ].ForwardSet );
                    }
                    else if ( Sentences[ i ].Tokens[ 0 ].StartsWith( START_LINK ) )
                    {
                        var set = CalculateCurrent( Sentences[ i ].Tokens[ 0 ] );
                        AddInLocalSet( generatedSet, set );
                        Sentences[ i ].AddInSet( set );
                    }
                    else if ( Sentences[ i ].Tokens[ 0 ] == EMPTY_LINK )
                    {
                        _stackOfEmpties.Push( i );
                        var set = CalculateEmptyCurrent( Sentences[ i ].MainToken );
                        _stackOfEmpties.Pop();
                        AddInLocalSet( generatedSet, set );
                        Sentences[ i ].AddInSet( set );
                    }
                    else
                    {
                        Sentences[ i ].AddInSet( Sentences[ i ].Tokens[ 0 ] );
                        AddInLocalSet( generatedSet, Sentences[ i ].Tokens[ 0 ] );
                    }
                }
            }

            return generatedSet;
        }

        private List<string> CalculateEmptyCurrent( string token )
        {
            var generatedSet = new List<string>();
            for ( var i = 0; i < Sentences.Count; ++i ) // search token existence
            {
                if ( !_stackOfEmpties.Contains( i ) && Sentences[ i ].Tokens.Contains( token ) ) // if contains
                {
                    var currentIndex = 0;
                    while ( currentIndex < Sentences[ i ].Tokens.Count ) // search token in tokens
                    {
                        if ( Sentences[ i ].Tokens[ currentIndex ] == token ) // token found
                        {
                            var seachedIndex = currentIndex + 1;
                            if ( seachedIndex < Sentences[ i ].Tokens.Count ) // token not last
                            {
                                if ( Sentences[ i ].Tokens[ seachedIndex ].StartsWith( START_LINK ) ) // is link
                                {
                                    AddInLocalSet( generatedSet, CalculateCurrent( Sentences[ i ].Tokens[ seachedIndex ] ) );
                                }
                                else // is determinant
                                {
                                    AddInLocalSet( generatedSet, Sentences[ i ].Tokens[ seachedIndex ] );
                                }
                            }
                            else // token is last
                            {
                                _stackOfEmpties.Push( i );
                                AddInLocalSet( generatedSet, CalculateEmptyCurrent( Sentences[ i ].MainToken ) );
                                _stackOfEmpties.Pop();
                                if ( Sentences[ i ].MainToken == _startToken )
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
