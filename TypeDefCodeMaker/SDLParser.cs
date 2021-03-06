using System;
using System.Collections.Generic;
using System.Text;

namespace TypeDefCodeMaker
{
	/*
	 * Grammar recognized by the parser
	 * 
	 * Productions
	 *	DL	Type definition list
	 *	DL'	Type definition list with empty production
	 *	D	Type Definition
	 *	TL	Type List
	 *	TL'	Type List with empty production
	 *	T	Type
	 *	S	Simple declaration such int a;
	 *	A	Array declaration
	 *	FA	Fixed size array declaration such int[10] a;
	 *	VA	Variable size array declaration such int[] a;
	 *	
	 *	id	Identifier
	 *	int	Integer number
	 *	ε	Empty production
	 *	
	 * Other chars like ;[]() match themselves
	 * 
	 * DL	->	D DL'
	 * DL'	->	D DL' | ε
	 * D	->	id id { TL }
	 * TL	->	T TL'
	 * TL'	->	T TL' | ε
	 * T	->	S | A
	 * A	->	FA | VA
	 * FA	->	id[ int ] id ;
	 * FA	->	id[] id ;
	 * S	->	id id ;
	 * 
	 * Since this grammar not deterministic, it is adjusted
	 *	AS	Array specifier
	 *	AL	Array Length
	 * 
	 * DL	->	D DL'
	 * DL'	->	D DL' | ε
	 * D	->	id id { TL }
	 * TL	->	T TL'
	 * TL'	->	T TL' | ε
	 * T	->	id AS id ;
	 * AS	->	[ AL | ε
	 * AL	->	] | num ]
	*/
	public class SDLParser
	{
		#region Variables

		private Token currentToken;

		private Queue<Token> tokenQ;

		private bool error;

		private string errorMessage;

		#endregion

		#region Constructors

		public SDLParser()
		{
			tokenQ = new Queue<Token>();
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		public bool Error{get { return this.error;}}

		public string ErrorMessage{get { return this.errorMessage;}}

		#endregion

		#region Methodos

		public bool Parse(Token[] tokenList)
		{
			tokenQ.Clear();
			error = false;
			errorMessage = null;

			for (int i = 0; i < tokenList.Length; ++i )
				tokenQ.Enqueue(tokenList[i]);
			currentToken = tokenQ.Dequeue();
			TypeDefinitionList();
			return !error;
		}

		protected void TypeDefinitionList()
		{
			if (error) return;

			TypeDefinition();
			TypeDefinitionListWEP();
			if (Match(TokenType.EOF))
			{
			}
		}

		protected void TypeDefinitionListWEP()
		{
			if (error) return;

			if (currentToken.Type == TokenType.Identifier)
			{
				TypeDefinition();
				TypeDefinitionListWEP();
			}
		}

		protected void TypeDefinition()
		{
			if (error) return;

			Match(TokenType.Identifier);
			Match(TokenType.Identifier);
			Match(TokenType.OpeningBrace);
			TypeList();
			Match(TokenType.ClosingBrace);
		}

		protected void TypeList()
		{
			if (error) return;

			Type();
			TypeListWEP();
		}

		protected void TypeListWEP()
		{
			if (error) return;

			if (currentToken.Type == TokenType.Identifier)
			{
				Type();
				TypeListWEP();
			}
		}

		protected void Type()
		{
			if (error) return;

			Match(TokenType.Identifier);
			ArraySpecifier();
			Match(TokenType.Identifier);
			Match(TokenType.SemiColon);
		}

		protected void ArraySpecifier()
		{
			if (error) return;

			if (currentToken.Type == TokenType.OpeningBracket)
			{
				Match(TokenType.OpeningBracket);
				ArrayLength();
			}
		}

		protected void ArrayLength()
		{
			if (error) return;

			if (currentToken.Type == TokenType.Integer)
				Match(TokenType.Integer);
			Match(TokenType.ClosingBracket);
		}

		protected bool Match(TokenType expectedTokenType)
		{
			if (currentToken.Type == expectedTokenType)
			{
				if(currentToken.Type != TokenType.EOF)
					currentToken = tokenQ.Dequeue();
				return true;
			}
			error = true;
			errorMessage = "Parser error on line " + currentToken.Line.ToString() +
				": expected " + TokenTypeToString(expectedTokenType) + " found " +
				TokenTypeToString(currentToken.Type);
			return false;
		}

		public string TokenTypeToString(TokenType tokenType)
		{
			string tokenString;
			switch (tokenType)
			{
				case TokenType.ClosingBrace: tokenString = "{"; break;
				case TokenType.ClosingBracket: tokenString = "["; break;
				case TokenType.ClosingParenthesis: tokenString = "("; break;
				case TokenType.EOF: tokenString = "end of file"; break;
				case TokenType.Identifier: tokenString = "identifier"; break;
				case TokenType.Integer: tokenString = "integer value"; break;
				case TokenType.OpeningBrace: tokenString = "{"; break;
				case TokenType.OpeningBracket: tokenString = "["; break;
				case TokenType.OpeningParenthesis: tokenString = "("; break;
				case TokenType.SemiColon: tokenString = ";"; break;
				default: tokenString = tokenType.ToString(); break;
			}
			return tokenString;
		}

		#endregion
	}
}
