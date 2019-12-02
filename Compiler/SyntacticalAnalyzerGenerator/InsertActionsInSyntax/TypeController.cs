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
            if ( LeftTerm?.Type != RightTermConvertToType() )
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

        private TermType RightTermConvertToType()
        {
            switch ( RightTerm.Type )
            {
                case TermType.DecimalWholeNumber:
                case TermType.BinaryWholeNumber:
                    return TermType.Int;
                default:
                    return RightTerm.Type;
            }
        }
    }
}
