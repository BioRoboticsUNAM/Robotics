using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TypeDefCodeMaker
{
	public partial class SDLScanner
	{
		#region Variables



		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SDLParser
		/// </summary>
		public SDLScanner()
		{
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		#endregion

		#region Methodos

		public bool ScanFile(string fileName, out Token[] tokenList)
		{
			string[] fileContent;

			tokenList = null;
			if (!File.Exists(fileName))
				return false;

			try
			{
				fileContent = File.ReadAllLines(fileName);
			}
			catch { return false; }

			tokenList = Scan(fileContent);
			return true;
		}

		public Token[] Scan(string[] input)
		{
			int i;
			int lastColumn = 0;
			List<Token> tokens = new List<Token>();

			for (i = 0; i < input.Length; ++i)
			{
				ScanLine(input[i], tokens, i, out lastColumn);
			}
			tokens.Add(new Token(TokenType.EOF, (char)0xFFFF, i -1, lastColumn)); 
			return tokens.ToArray();
		}

		public Token[] Scan(string input)
		{
			string[] lines;

			lines = Regex.Split(input, @"[\r\n]+", RegexOptions.Multiline);
			return Scan(lines);
		}

		protected void ScanLine(string input, List<Token> tokens, int lineNumber, out int lastColumn)
		{
			int cc = 0;

			while (cc < input.Length)
			{
				if (Char.IsLetter(input[cc]) || (input[cc] == '_'))
				{
					ParseIdentifier(input, ref cc, tokens, lineNumber);
					continue;
				}
				else if (Char.IsNumber(input[cc]))
				{
					ParseNumber(input, ref cc, tokens, lineNumber);
					continue;
				}
				else if (Char.IsWhiteSpace(input[cc]))
				{
					++cc;
					continue;
				}
				else
				{
					switch (input[cc])
					{
						case ';':
							tokens.Add(new Token(TokenType.SemiColon, input[cc], lineNumber, cc));
							break;

						case '{':
							tokens.Add(new Token(TokenType.OpeningBrace, input[cc], lineNumber, cc));
							break;

						case '}':
							tokens.Add(new Token(TokenType.ClosingBrace, input[cc], lineNumber, cc));
							break;

						case '[':
							tokens.Add(new Token(TokenType.OpeningBracket, input[cc], lineNumber, cc));
							break;

						case ']':
							tokens.Add(new Token(TokenType.ClosingBracket, input[cc], lineNumber, cc));
							break;

						default:
							tokens.Add(new Token(TokenType.Invalid, input[cc], lineNumber, cc));
							break;
					}
				}
				++cc;
			}
			lastColumn = cc - 1;
		}

		protected void ParseIdentifier(string input, ref int cc, List<Token> tokens, int lineNumber)
		{
			int bcc;
			Token<string> token;
			string lexeme;

			if (!Char.IsLetter(input[cc]) && (input[cc] != '_'))
				return;
			bcc = cc;
			++cc;
			while ((cc < input.Length) && (Char.IsLetterOrDigit(input[cc]) || (input[cc] == '_')))
				++cc;
			lexeme = input.Substring(bcc, cc - bcc);
			token = new Token<string>(TokenType.Identifier, lexeme, lexeme, lineNumber, bcc);

			tokens.Add(token);
		}

		protected void ParseNumber(string input, ref int cc, List<Token> tokens, int lineNumber)
		{
			int bcc;
			Token<int> token;
			string lexeme;

			if (!Char.IsDigit(input[cc]))
				return;
			bcc = cc;
			++cc;
			while ((cc < input.Length) && Char.IsDigit(input[cc]))
				++cc;
			lexeme = input.Substring(bcc, cc - bcc);
			token = new Token<int>(TokenType.Identifier, lexeme, Int32.Parse(lexeme), lineNumber, bcc);

			tokens.Add(token);
		}

		#endregion
	}
}
