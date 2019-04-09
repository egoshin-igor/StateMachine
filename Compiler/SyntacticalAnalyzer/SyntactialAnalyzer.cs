using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Lekser;
using Lekser.Enums;

namespace SyntacticalAnalyzer
{
    public class SyntactialAnalyzer
    {
        private readonly List<Term> _terms = new List<Term>();
        private int _currentTermIndex = 0;

        public SyntactialAnalyzer( TextReader textReader )
        {
            GetTermsAsync( textReader ).Wait();
        }

        public bool IsVar()
        {
            return IsValidTerm( TermType.Var )
                && IsType()
                && IsValidTerm( TermType.Colon )
                && IsIdList()
                && IsValidTerm( TermType.InstructionEnd );
        }

        private bool IsType()
        {
            return IsValidTerm( TermType.Int ) || IsValidTerm( TermType.Float ) || IsValidTerm( TermType.Char );
        }

        private bool IsIdList()
        {
            return IsValidTerm( TermType.Identifier ) && IsIdListRightPart();
        }

        private bool IsIdListRightPart()
        {
            if ( IsValidTerm( TermType.Comma ) )
            {
                if ( IsValidTerm( TermType.Identifier ) )
                {
                    return IsIdListRightPart();
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        private bool IsValidTerm( TermType termType )
        {
            if ( _currentTermIndex >= _terms.Count )
            {
                return false;
            }

            Term term = _terms[ _currentTermIndex ];
            if ( term.Type == termType )
            {
                ++_currentTermIndex;
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task GetTermsAsync( TextReader textReader )
        {
            var lekser = new ProgramLekser( textReader );
            Term term = null;
            do
            {
                term = await lekser.GetTermAsync();
                if ( term != null )
                {
                    _terms.Add( term );
                }
            } while ( term != null );
        }
    }
}
