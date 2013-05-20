using System;
using System.Collections.Generic;
using System.Text;

namespace TypeDefCodeMaker
{
	public class CodeMaker
	{
		#region Variables

		protected SDLScanner scanner;
		protected SDLParser parser;

		#endregion

		#region Constructors

		public CodeMaker()
		{
			scanner = new SDLScanner();
			parser = new SDLParser();
		}

		#endregion

		#region Events
		#endregion

		#region Properties
		#endregion

		#region Methodos

		public void Make(string[] sourceFiles, SupportedLanguages[] languages, string defaultNamespace)
		{
			Token[] tokens;

			foreach(string filePath in sourceFiles)
			{
				if (scanner.ScanFile(filePath, out tokens))
				{
					if (!parser.Parse(tokens))
					{
						Console.WriteLine("File " + filePath + ". " + parser.ErrorMessage);
						continue;
					}
				}
				Console.WriteLine("File " + filePath + " parsed successfully");
			}
		}

		#endregion
	}
}
