using System;
using Lekser;
using Lekser.Enums;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax
{
    public class TypeController
    {
        public Term LeftTerm { get; set; }
        public Term RightTerm { get; set; }

        public Term LastTerm { get; set; }

        public void CheckLeftRight( int currentRow )
        {
            if ( LeftTerm?.Type != RightTermConvertToType() )
            {
                throw new ApplicationException( $"Left value:{LeftTerm.Type} and right value:{RightTerm.Type}" +
                    $" must be equal on row:{currentRow}" );
            }
        }

        public void SaveLastTerm( Term term )
        {
            LastTerm = term;
        }

        public void SaveLeftTerm( Term term )
        {
            LeftTerm = term;
        }

        public void SaveRightTerm( Term term )
        {
            RightTerm = term;
        }

        public void DefineArrElemType( Term type )
        {
            switch ( type.Type )
            {
                case TermType.IntArray:
                    LastTerm = type.Copy();
                    LastTerm.Type = TermType.Int;
                    break;
                default:
                    break;
            }
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
