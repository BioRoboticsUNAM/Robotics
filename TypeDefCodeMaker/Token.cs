using System;
using System.Collections.Generic;
using System.Text;

namespace TypeDefCodeMaker
{
	public enum TokenType
	{
		/// <summary>
		/// Represents a [
		/// </summary>
		OpeningBracket,
		/// <summary>
		/// Represents a ]
		/// </summary>
		ClosingBracket,
		/// <summary>
		/// Represents a {
		/// </summary>
		OpeningBrace,
		/// <summary>
		/// Represents a }
		/// </summary>
		ClosingBrace,
		/// <summary>
		/// Represents a (
		/// </summary>
		OpeningParenthesis,
		/// <summary>
		/// Represents a )
		/// </summary>
		ClosingParenthesis,
		/// <summary>
		/// Represents an identifier
		/// </summary>
		Identifier,
		/// <summary>
		/// Represents an integer number
		/// </summary>
		Integer,
		/// <summary>
		/// Invalid token
		/// </summary>
		Invalid,
		/// <summary>
		/// Represents a ;
		/// </summary>
		SemiColon,
		/// <summary>
		/// Final Token
		/// </summary>
		EOF
	}

	public class Token
	{
		#region Variables

		private TokenType type;
		private string lexeme;
		private int line;
		private int column;

		#endregion

		#region Constructors

		public Token(TokenType type, char lexeme, int line, int column)
			: this(type, lexeme.ToString(), line, column)
		{
			
		}

		public Token(TokenType type, string lexeme, int line, int column)
		{
			this.type = type;
			this.lexeme = lexeme;
			this.line = line;
			this.column = column;
		}

		#endregion

		#region Properties

		public TokenType Type { get { return this.type; } }
		public string Lexeme { get { return this.lexeme; } }
		public int Line { get { return this.line; } }
		public int Column { get { return this.column; } }

		#endregion

		#region Methods

		public override string ToString()
		{
			return type.ToString() + ": '" + lexeme + "'";
		}

		#endregion

	}

	public class Token<TValue> : Token
	{
		#region Variables

		TValue value;

		#endregion

		#region Constructors

		public Token(TokenType type, char lexeme, TValue value, int line, int column)
			:base(type, lexeme, line, column)
		{
			this.value = value;
		}

		public Token(TokenType type, string lexeme, TValue value, int line, int column)
			: base(type, lexeme, line, column)
		{
			this.value = value;
		}

		#endregion

		#region Properties

		public TValue Value { get { return this.value; } }

		#endregion
	}
}
