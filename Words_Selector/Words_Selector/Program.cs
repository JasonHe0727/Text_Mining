using System;
using Cygni.Executors;
using Cygni.DataTypes;
using System.Linq;
using System.IO;
namespace Words_Selector
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Engine engine = Engine.CreateInstance ();
			engine.Evaluate ("import 'MySQL'");
			engine.Evaluate ("import 'io'");
			engine.SetSymbol ("pos_path", "/home/jasonhe/文档/公司数据/pos.txt");
			engine.SetSymbol ("words_path", "/home/jasonhe/Data/words.txt");
			engine.SetSymbol ("sen_path", "/home/jasonhe/Data/auto sentiment训练集.csv");
			engine.DoFile ("words_selector.cyg");
			engine.Evaluate ("ch2_values = loadDataFromCsvFile(sen_path,words_path)");
			//engine.ExecuteInConsole ();

			DynList ch2_values = engine.GetSymbol ("ch2_values").As<DynList>();
			var list = ch2_values.Select ((x, index) => Tuple.Create (index, x)).OrderByDescending (i => i.Item2).ToList ();
			foreach (var item in list.Take(100)) {
				Console.WriteLine (item);
			}
			File.WriteAllText ("result.txt", string.Join ("\n", list));
		}
	}
}
