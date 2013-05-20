using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TypeDefCodeMaker
{
	public enum SupportedLanguages
	{
		CPP, 
		CS,
		Java
	}

	class Program
	{
		public static void Main(string[] args)
		{
			string[] sourceFiles;
			SupportedLanguages[] languages;
			string defaultNamespace;
			CodeMaker codeMaker;

			if (!ParseArgs(args, out sourceFiles, out languages, out defaultNamespace))
				return;

			codeMaker = new CodeMaker();
			codeMaker.Make(sourceFiles, languages, defaultNamespace);
		}

		public static void Help()
		{
			Console.WriteLine("Generates data type structures for use to exchange information with blackboard in multiple languages");
			Console.WriteLine();
			Console.WriteLine("TDCM -lang <language> [-ns <namespace>] <source files>");
			Console.WriteLine();
			Console.WriteLine("-help\tShows help.");
			Console.WriteLine("-lang\tSpecifies the language of the genrated code files. Use:");
			Console.WriteLine("\t  C++\tC++");
			Console.WriteLine("\t  cpp\tC++");
			Console.WriteLine("\t  C#\tC#");
			Console.WriteLine("\t  cs\tC#");
			Console.WriteLine("\t  java\tjava");
			Console.WriteLine("\tNote. Multiple output files can be specified using a ',' (comma)");
			Console.WriteLine("\tas separator: TDCM -lang cpp,cs,java var.sdl");
			Console.WriteLine("-ns\tSpecifies the namespace/package for the genrated code files. Default:");
			Console.WriteLine("\t  C++\tNone");
			Console.WriteLine("\t  C#\tRobotics.Types");
			Console.WriteLine("\t  Java\trobotics.types");
		}

		public static bool ParseArgs(string[] args, out string[] files, out SupportedLanguages[] langs, out string defaultNamespace)
		{
			int i;
			Regex rxNameSpaceValidator = new Regex(@"[A-Za-z_][0-9A-Za-z_]*(\.[A-Za-z_][0-9A-Za-z_]*)*");
			List<SupportedLanguages> languages = new List<SupportedLanguages>();
			List<string> sourceFiles =  new List<string>();

			files = null;
			langs = null;
			defaultNamespace = null;

			for (i = 0; i < args.Length; ++i)
			{
				switch (args[i])
				{
					case "/?":
					case "/h":
					case "/help":
					case "-?":
					case "-h":
					case "-help":
					case "--h":
					case "--help":
						Help();
						return false;

					case "/l":
					case "/lang":
					case "/language":
					case "-l":
					case "-lang":
					case "-language":
					case "--l":
					case "--lang":
					case "--language":
						if (args.Length <= ++i)
						{
							Console.WriteLine("\t" + args[i - 1] + " error. No output language specified.");
							return false;
						}
						ParseLanguages(languages, args[i]);
						break;

					case "/ns":
					case "-ns":
					case "--ns":
					case "/package":
					case "-package":
					case "--package":
						if (args.Length <= ++i)
						{
							Console.WriteLine("\t" + args[i - 1] + " error. No namespace/package specified.");
							return false;
						}
						if (String.IsNullOrEmpty(defaultNamespace) && !rxNameSpaceValidator.IsMatch(args[i]))
						{
							Console.WriteLine("\t" + args[i - 1] + " " + args[i] + " error. Invalid namespace/package specified.");
							return false;
						}
						if (!String.IsNullOrEmpty(defaultNamespace))
							Console.WriteLine("\tWarning. namespace/package already defined will be overwrited");
						defaultNamespace = args[i];
						break;

					default:
						if (File.Exists(args[i]))
							sourceFiles.Add(args[i]);
						else if (args[i].StartsWith("-") || args[i].StartsWith("/"))
							Console.WriteLine("\tInvalid flag: " + args[i]);
						else
							Console.WriteLine("\tInvalid file: '" + args[i] + "'. File does not exist.");
						break;
				}
			}

			if ((languages.Count < 1) || (sourceFiles.Count < 1))
			{
				Console.WriteLine("Invalid input.");
				Console.WriteLine("Usage: tdcm -lang <language> <source files>");
				Console.WriteLine("use -help for a list of possible options");
				Console.WriteLine();
				return false;
			}
			langs = languages.ToArray();
			files = sourceFiles.ToArray();
			return true;
		}

		private static void ParseLanguages(List<SupportedLanguages> languages, string languageString)
		{
			languageString = languageString.ToUpper();
			string[] sa = languageString.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string s in sa)
			{
				switch (s)
				{
					case "CPP":
					case "C++":
						if (!languages.Contains(SupportedLanguages.CPP))
							languages.Add(SupportedLanguages.CPP);
						break;

					case "CS":
					case "C#":
						if (!languages.Contains(SupportedLanguages.CS))
							languages.Add(SupportedLanguages.CS);
						break;

					case "JAVA":
						if (!languages.Contains(SupportedLanguages.Java))
							languages.Add(SupportedLanguages.Java);
						break;
				}
			}
			
		}
	}
}
