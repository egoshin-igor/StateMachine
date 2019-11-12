using System;
using Lekser;
using Lekser.Enums;

namespace SyntacticalAnalyzerGenerator.InsertActionsInSyntax
{
    public class TypeController
    {
        private Term _leftTerm { get; set; }
        private Term _rightTerm { get; set; }

        public Term LastTerm { get; set; }

        public void CheckLeftRight( int currentRow )
        {
            if ( _leftTerm?.Type != _rightTerm?.Type )
            {
                throw new ApplicationException( $"Left value:{_leftTerm.Type} and right value:{_rightTerm.Type}" +
                    $" must be equal on row:{currentRow}" );
            }
        }

        public void SaveLastTerm( Term term )
        {
            LastTerm = term;
        }

        public void SaveLeftTerm( Term term )
        {
            _leftTerm = term;
        }

        public void SaveRightTerm( Term term )
        {
            _rightTerm = term;
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
    }
}
