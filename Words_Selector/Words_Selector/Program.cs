using System;
using System.Text;
using Cygni.Executors;
using Cygni.DataTypes;
using System.Linq;
using System.IO;
using JiebaNet.Segmenter;
using Cygni.Settings;
using System.Collections.Generic;
using Cygni.Errors;
namespace Words_Selector
{
	class MainClass
	{
		public static JiebaSegmenter segmenter;

		public static void Main (string[] args)
		{
			const string configPath = "config.txt";
			if (!File.Exists (configPath)) {
				Console.WriteLine ("没有找到配置文件，请将配置文件 config.txt 置于程序目录下。");
				goto End;
			}
			Console.WriteLine ("正在加载配置文件……");
			var dic = ReadConfig (configPath);
			DynValue trainPath = dic ["TrainPath"];// train data file path
			DynValue stopPath = dic ["StopWordsPath"];// stop words file path
			DynValue segmentedWordsPath = dic ["SegmentedWordsPath"];// output segmented words file path
			DynValue featureWordsPath = dic ["FeatureWordsPath"];// selected words file path
			DynValue matrixOutputPath = dic ["MatrixOutputPath"];// matrix path

			DynValue weightPath = dic ["WeightFilePath"];// weights
			DynValue testPath = dic ["TestDataPath"];// test data set 
			Console.WriteLine ("配置文件加载完毕。\r\n");

			Console.WriteLine ("正在初始化 Cygni 引擎……");
			Engine engine = Engine.CreateInstance ();
			Console.WriteLine ("Cygni 引擎初始化成功。");
			Console.WriteLine ("============================================================");
			Console.WriteLine ("1： 对训练集进行分词， 并从分词结果中选取特征词");
			Console.WriteLine ("2： 根据选取的特征词， 生成矩阵文件");
			Console.WriteLine ("3： 根据特征词所对应的权重， 对情感进行分类");

			Console.WriteLine ("============================================================");
			Console.Write ("请输入1或2或3，对任务进行选择，输入其他内容程序会出错或自动退出：");

			int choice = int.Parse (Console.ReadLine ());
			if (choice == 1) {
				Console.WriteLine ("正在加载 结巴分词 及相关函数……");
				segmenter = new JiebaSegmenter ();
				engine.SetSymbol ("jieba_cut", Cut);
				engine.SetSymbol ("sort_chi2_list", Sort_Chi2_List); 
				Console.WriteLine ("结巴分词 及相关函数 加载完毕。\r\n");
				int k = Input<int> ("请输入要选取的特征词数量： "); // select top-k words
				if (k <= 0) {
					Console.WriteLine ("特征词数量必须为正整数！");
					goto End;
				}
				DynValue K = (double)k;
				Console.WriteLine ("执行 Cygni 脚本......");
				engine.DoFile ("words_selector.cyg");
				engine.ExecuteFromEntryPoint (new DynList (new []{ trainPath, stopPath, segmentedWordsPath, featureWordsPath, K }));
			} else if (choice == 2) {
				Console.WriteLine ("执行 Cygni 脚本......");
				engine.DoFile ("gen_spmatrix.cyg");
				engine.ExecuteFromEntryPoint (new DynList (new [] { featureWordsPath, trainPath, matrixOutputPath }));
			} else if (choice == 3) {
				// To Do
				Console.WriteLine ("执行 Cygni 脚本......");
				engine.DoFile ("classify.cyg");
				engine.ExecuteFromEntryPoint (weightPath, testPath);
			}
			End:				
			Console.WriteLine ("程序已终止，按任意键退出……");
			Console.ReadKey ();
			return;
		}

		public static DynValue Cut (DynValue[] args)
		{
			RuntimeException.FuncArgsCheck (args.Length == 1,"jieba_cut");	
			string sentence = args [0].AsString ();
			var segments = segmenter.Cut (sentence, cutAll: true);
			DynList list = new DynList (segments.Select (i => DynValue.FromString (i)));
			return DynValue.FromList (list);
		}

		public static DynValue Sort_Chi2_List (DynValue[]args)
		{
			RuntimeException.FuncArgsCheck (args.Length == 1,"sort_chi2_list");	
			var ch2_values = args [0].As<DynList> ();
			var result = ch2_values.Select ((x, index) => Tuple.Create (index, x)).OrderByDescending (i => i.Item2);
			DynList list = new DynList (ch2_values.Count);
			foreach (var item in result) {
				DynList t = new DynList (new DynValue[]{ item.Item1, item.Item2 }, 2);
				list.Add (t);
			}
			return list;
		}

		public static T Input<T> (string message)
		{
			Console.Write (message);
			return (T)Convert.ChangeType (Console.ReadLine (), typeof(T));
		}

		public static Dictionary<string,string> ReadConfig (string filePath)
		{
			string[] contents = File.ReadAllLines (filePath, Encoding.UTF8);
			var dic = new Dictionary<string,string> ();
			foreach (var item in contents) {
				string[] t = item.Split ('=');
				if (t.Length == 2)
					dic.Add (t [0].Trim (), t [1].Trim ());
			}
			return dic;
		}
	}
}
