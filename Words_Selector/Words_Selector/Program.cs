using System;
using Cygni.Executors;
using Cygni.DataTypes;
using System.Linq;
using System.IO;
using JiebaNet.Segmenter;

namespace Words_Selector
{
	class MainClass
	{
		public static readonly JiebaSegmenter segmenter = new JiebaSegmenter ();

		public static void Main (string[] args)
		{
			Engine engine = Engine.CreateInstance ();
			engine.SetSymbol ("jieba_cut", Cut);
			engine.SetSymbol ("sort_chi2_list", Sort_Chi2_List);
			string trainPath = "/home/jasonhe/Data/auto sentiment训练集.csv";// train data file path
			string stopPath =  "/home/jasonhe/Data/stopwords.txt";// stop words file path
			string wordsPath = "/home/jasonhe/Data/seg_words_20161103.txt";// output segmented words file path
			string selectedWordsPath = "/home/jasonhe/Data/selected_words_20161103.txt";// selected words file path
			int k = 500;// select top-k words
			var cyg_args = DynValue.FromList (
				               new DynList (
					               new DynValue[]{ trainPath, stopPath, wordsPath, selectedWordsPath,k }));
			engine.SetSymbol ("cyg_args", cyg_args);
			engine.DoFile ("words_selector.cyg");
			engine.Evaluate ("__MAIN__(cyg_args)");
			//engine.ExecuteInConsole ();
		}

		public static DynValue Cut (DynValue[] args)
		{
			string sentence = args [0].AsString ();
			var segments = segmenter.Cut (sentence, cutAll: true);
			var list = new DynList (segments.Select (i => DynValue.FromString (i)));
			return DynValue.FromList (list);
		}

		public static DynValue Sort_Chi2_List (DynValue[]args)
		{
			var ch2_values = args [0].As<DynList> ();
			var list = ch2_values.Select ((x, index) => Tuple.Create (index, x)).OrderByDescending (i => i.Item2);
			var result = new DynList (
				             list.Select (
					             i => DynValue.FromList (new DynList (new DynValue[]{ i.Item1, i.Item2 }))));
			return result;

		}
	}
}
