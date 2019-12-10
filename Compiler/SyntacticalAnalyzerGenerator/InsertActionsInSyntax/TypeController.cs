using System;
using Lekser;
using Lekser.Enums;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax
{
    public class TypeController
    {
        public Term LeftTerm { get; set; }
        public Term RightTerm { get; set; }

        public void CheckLeftRight( int currentRow )
        {
            if ( LeftTerm?.Type != ConvertToSimpleType( RightTerm.Type ) )
            {
                throw new ApplicationException( $"Left value:{LeftTerm.Type} and right value:{RightTerm.Type}" +
                    $" must be equal on row:{currentRow}" );
            }
        }

        public void SaveLeftTerm( Term term )
        {
            LeftTerm = term;
        }

        public void SaveRightType( Term term )
        {
            RightTerm = term;
        }

        public static TermType ConvertToSimpleType( TermType type )
        {
            switch ( type )
            {
                case TermType.DecimalWholeNumber:
                case TermType.BinaryWholeNumber:
                    return TermType.Int;
                case TermType.DecimalFixedPointNumber:
                    return TermType.Double;
                default:
                    return type;
            }
        }
    }
}
