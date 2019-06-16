using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lekser;
using Lekser.Enums;

namespace SLR.Table
{
    public sealed class Token : IEquatable<Token>
    {
        private static readonly Dictionary<string, TermType> _reservedTermByName = new Dictionary<string, TermType>
        {
            { ReservedTerm.Identifier, TermType.Identifier },
            { ReservedTerm.DecimalWholeNumber, TermType.DecimalWholeNumber }
        };

        const string END_TOKEN = "[END]";
        const string START_TOKEN = "[START]";
        const char START_LINK = '<';
        const char RULE_LINK = '[';

        /// <summary>
        /// token string value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// token position in sentence 
        /// </summary>
        public int ColIndex { get; set; }

        /// <summary>
        /// token position in row of sentences
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// token type : rule [], non-terminal <>, terminal without braces
        /// </summary>
        public TokenType Type { get; set; }

        public TermType? LekserTermType { get; private set; } = null;

        public Token( string value )
        {
            Value = value;
            DefineType( value );
            DefineTermType( value );
        }

        public Token( string value, int colIndex, int rowIndex )
            : this( value )
        {
            ColIndex = colIndex;
            RowIndex = rowIndex;
        }

        private void DefineType( string value )
        {
            if ( value.StartsWith( START_LINK ) )
            {
                Type = TokenType.NonTerminal;
            }
            else if ( value.StartsWith( RULE_LINK ) )
            {
                if ( value == END_TOKEN )
                {
                    Type = TokenType.End;
                }
                else if ( value == START_TOKEN )
                {
                    Type = TokenType.Start;
                }
                else
                {
                    Type = TokenType.Rule;
                }
            }
            else
            {
                Type = TokenType.Terminal;
            }
        }

        private void DefineTermType( string value )
        {
            if ( Type != TokenType.Terminal || Type != TokenType.End )
                return;

            if ( Type == TokenType.End )
            {
                LekserTermType = TermType.End;
                return;
            }

            if ( _reservedTermByName.ContainsKey( value ) )
            {
                LekserTermType = _reservedTermByName[ value ];
            }
            else
            {
                LekserTermType = TermRecognizer.GetTypeByTermString( value );
            }
            if ( !_reservedTermByName.ContainsKey( value ) && ( LekserTermType == TermType.Error || _reservedTermByName.Values.Contains( LekserTermType.Value ) ) )
                throw new Exception( $"Unrecognized term {value}" );
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as Token );
        }

        public bool Equals( Token other )
        {
            //Check whether the compared object is null. 
            if ( other == null )
                return false;

            //Check whether the compared object references the same data. 
            if ( ReferenceEquals( this, other ) )
                return true;

            return Value == other.Value &&
                ColIndex == other.ColIndex &&
                RowIndex == other.RowIndex &&
                Type == other.Type &&
                EqualityComparer<TermType?>.Default.Equals( LekserTermType, other.LekserTermType );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( Value, ColIndex, RowIndex, Type, LekserTermType );
        }
    }
}
