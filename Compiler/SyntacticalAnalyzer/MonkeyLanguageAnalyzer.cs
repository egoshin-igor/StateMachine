using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lekser;
using Lekser.Enums;

namespace SyntacticalAnalyzer
{
    public class MonkeyLanguageAnalyzer
    {
        private readonly HashSet<TermType> _directSetR1 = new HashSet<TermType> { TermType.YxTi, TermType.Xo, TermType.Ny };
        private readonly HashSet<TermType> _directSetA_1 = new HashSet<TermType> { TermType.Ay };
        private readonly HashSet<TermType> _directSetA_2 = new HashSet<TermType> { TermType.End, TermType.Kakoj };
        private readonly HashSet<TermType> _directSetR2 = new HashSet<TermType> { TermType.YxTi, TermType.Xo, TermType.Ny };
        private readonly HashSet<TermType> _directSetB_1 = new HashSet<TermType> { TermType.Ky };
        private readonly HashSet<TermType> _directSetB_2 = new HashSet<TermType> { TermType.Ay, TermType.End, TermType.Kakoj };
        private readonly HashSet<TermType> _directSetR3_1 = new HashSet<TermType> { TermType.YxTi };
        private readonly HashSet<TermType> _directSetR3_2 = new HashSet<TermType> { TermType.Xo };
        private readonly HashSet<TermType> _directSetR3_3 = new HashSet<TermType> { TermType.Ny };
        private readonly HashSet<TermType> _directSetR4_1 = new HashSet<TermType> { TermType.INy };
        private readonly HashSet<TermType> _directSetR4_2 = new HashSet<TermType> { TermType.Oj };

        private readonly List<Term> _terms = new List<Term>();
        private int _currentTermIndex = 0;

        public MonkeyLanguageAnalyzer( TextReader textReader )
        {
            GetTermsAsync( textReader ).Wait();
        }

        public bool IsMonkeyLanguage()
        {
            return IsTermInSet( _directSetR1 ) && IsR2() && IsA();
        }

        public bool IsA()
        {
            if (
                ( IsTermInSet( _directSetA_1 ) && IsValidTerm( TermType.Ay ) && IsR2() && IsA() ) ||
                ( IsTermInSet( _directSetA_2 ) )
            )
            {
                return true;
            }

            return false;
        }

        public bool IsR2()
        {
            if ( IsTermInSet( _directSetR2 ) && IsR3() && IsB() )
            {
                return true;
            }

            return false;
        }

        public bool IsB()
        {
            if ( IsTermInSet( _directSetB_1 ) && IsValidTerm( TermType.Ky ) && IsR3() && IsB() )
            {
                return true;
            }
            if ( IsTermInSet( _directSetB_2 ) )
            {
                return true;
            }

            return false;
        }

        public bool IsR3()
        {
            if ( IsTermInSet( _directSetR3_1 ) && IsValidTerm( TermType.YxTi ) )
            {
                return true;
            }

            if ( IsTermInSet( _directSetR3_2 ) && IsValidTerm( TermType.Xo ) && IsR3() )
            {
                return true;
            }

            if ( IsTermInSet( _directSetR3_3 ) && IsValidTerm( TermType.Ny ) && IsR4() && IsValidTerm( TermType.INy ) )
            {
                return true;
            }

            return false;
        }

        public bool IsR4()
        {
            if ( IsTermInSet( _directSetR4_1 ) )
            {
                return true;
            }

            if ( IsTermInSet( _directSetR4_2 ) && IsValidTerm( TermType.Oj ) && IsMonkeyLanguage() && IsValidTerm( TermType.Kakoj ) )
            {
                return true;
            }

            return false;
        }

        private bool IsTermInSet( HashSet<TermType> directSet )
        {
            if ( _currentTermIndex >= _terms.Count )
            {
                return true;
            }

            Term term = _terms[ _currentTermIndex ];
            if ( directSet.Contains( term.Type ) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsValidTerm( TermType termType )
        {
            if ( _currentTermIndex >= _terms.Count )
            {
                return termType == TermType.End;
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

