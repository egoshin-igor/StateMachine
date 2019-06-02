using System;
using System.Collections.Generic;
using System.Linq;
using SyntacticalAnalyzerGenerator.Words;

namespace SyntacticalAnalyzerGenerator
{
    public class LLOneConverter
    {
        public static List<Expression> Convert( List<Expression> from )
        {
            var factorizedExpressions = new List<Expression>( from );
            int lengthBefore = 0;
            int lengthAfter = 0;
            do
            {
                lengthBefore = factorizedExpressions.Count;
                factorizedExpressions = GetFactorized( factorizedExpressions );
                lengthAfter = factorizedExpressions.Count;
            }
            while ( lengthBefore != lengthAfter );

            return GetLLOne( factorizedExpressions );
        }

        private static List<Expression> GetFactorized( List<Expression> from )
        {
            var result = new List<Expression>();

            var expressionsGroupByMainNoTerm = from.GroupBy( f => f.NoTerm.Name );
            foreach ( var expressionsGroup in expressionsGroupByMainNoTerm )
            {
                List<Expression> expressions = expressionsGroup.OrderBy( e => e.Words.First().Name ).ToList();
                var nonFactorizedGroupByFirstWords = expressions.GroupBy( e => e.Words.First().Name );
                foreach ( var nonFactorizedGroup in nonFactorizedGroupByFirstWords )
                {
                    int i = 10000;
                    var nonFactorizedExpressions = nonFactorizedGroup.ToList();
                    if ( nonFactorizedExpressions.Count == 1 )
                    {
                        result.Add( nonFactorizedExpressions.First() );
                        continue;
                    }
                    var word = nonFactorizedExpressions.First().Words.First();
                    foreach ( Expression nonFactorizedExpression in nonFactorizedExpressions )
                    {
                        nonFactorizedExpression.Words.RemoveAt( 0 );
                        if ( nonFactorizedExpression.Words.Count == 0 )
                        {
                            nonFactorizedExpression.Words.Add( new Word { Name = Word.Epsilant, Type = WordType.Epsilant } );
                        }
                    }
                    var newExpressionName = expressionsGroup.Key.Substring( 0, expressionsGroup.Key.Length - 1 ) + "___" + i + ">";
                    i++;

                    var expression = new Expression
                    {
                        NoTerm = nonFactorizedExpressions.First().NoTerm,
                        Words = new List<Word>
                        {
                            word,
                            new Word { Name = newExpressionName, Type = WordType.RightNoTerm }
                        }
                    };
                    result.Add( expression );
                    foreach ( Expression nonFactorizedExpression in nonFactorizedExpressions )
                    {
                        result.Add( new Expression
                        {
                            NoTerm = new Word { Name = newExpressionName, Type = WordType.LeftNoTerm },
                            Words = nonFactorizedExpression.Words
                        } );
                    }
                }
            }

            return result;
        }

        private static List<Expression> GetLLOne( List<Expression> from )
        {
            var result = new List<Expression>();

            var expressionsGroupByMainNoTerm = from.GroupBy( f => f.NoTerm.Name );
            int i = 0;
            foreach ( var expressionsGroup in expressionsGroupByMainNoTerm )
            {
                List<Expression> expressions = expressionsGroup.ToList();
                var recursiveExpression = expressions.FirstOrDefault( e => e.NoTerm.Name == e.Words.First().Name );
                if ( recursiveExpression == null )
                {
                    result.AddRange( expressions );
                    continue;
                }
                if ( recursiveExpression.Words.Count == 1 )
                    throw new Exception( "A -> A" );

                if ( expressions.Count == 1 )
                    throw new Exception( "A -> Ax - infinity cicle" );

                expressions.Remove( recursiveExpression );
                var newExpressionName = recursiveExpression.NoTerm.Name.Substring( 0, recursiveExpression.NoTerm.Name.Length - 1 ) + "___" + i + ">";
                i++;
                var newExpressionNameTwo = recursiveExpression.NoTerm.Name.Substring( 0, recursiveExpression.NoTerm.Name.Length - 1 ) + "___" + i + ">";
                i++;
                var expression = new Expression
                {
                    NoTerm = recursiveExpression.NoTerm,
                    Words = new List<Word>
                    {
                        new Word { Name = newExpressionName, Type = WordType.RightNoTerm },
                        new Word { Name = newExpressionNameTwo, Type = WordType.RightNoTerm }
                    }
                };
                result.Add( expression );
                foreach ( Expression exp in expressions )
                {
                    var noTerm = new Word { Name = newExpressionName, Type = WordType.LeftNoTerm };
                    result.Add( new Expression
                    {
                        NoTerm = noTerm,
                        Words = exp.Words
                    } );
                }

                result.Add( new Expression
                {
                    NoTerm = new Word { Name = newExpressionNameTwo, Type = WordType.LeftNoTerm },
                    Words = new List<Word> { new Word { Name = Word.Epsilant, Type = WordType.Epsilant } }
                } );
                recursiveExpression.Words.RemoveAt( 0 );
                var words = new List<Word>();
                words.AddRange( recursiveExpression.Words );
                words.Add( new Word { Name = newExpressionNameTwo, Type = WordType.RightNoTerm } );
                result.Add( new Expression
                {
                    NoTerm = new Word { Name = newExpressionNameTwo, Type = WordType.LeftNoTerm },
                    Words = words
                } );
            }

            return result;
        }
    }
}
