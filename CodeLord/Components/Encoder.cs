using System.Collections.Concurrent;

namespace CodeLord.Components
{
    internal static class Encoder
    {
        /// <summary> 找出最短编码 </summary>
        /// <param name="dict"> 词库，键值对为（词，编码） </param>
        /// <param name="text"> 要编码的文本 </param>
        /// <param name="constant"> 是否为整句输入。如果否，则词之间要加空格。 </param>
        public static void Encode(ConcurrentDictionary<string, string> dict, string text, bool constant)
        {
            try
            {
                Console.WriteLine("开始寻找所有可能的编码...");
                var tree = FindBranches(dict, text);
                Console.WriteLine("已全部找到。正在寻找最短编码...");
                var best = FindShortest(tree, text, constant);
                Console.WriteLine("已找到并输出最短编码。正在分析...");
                Analyze(best, dict, text);
                Console.WriteLine("已完成分析并输出结果。程序结束。");
            }
            catch (Exception e)
            {
                Console.WriteLine($"编码计算出错：{e.Message}");
            }
        }

        /// <summary> 找出所有可能的编码分支 </summary>
        /// <param name="dict"></param>
        /// <param name="text"></param>
        /// <returns> 每个元素为（起始索引，词的长度，编码） </returns>
        private static ConcurrentBag<(int, int, string)> FindBranches(ConcurrentDictionary<string, string> dict, string text)
        {
            ConcurrentBag<(int, int, string)> tree = [];
            _ = Parallel.For(0, text.Length, i =>
            {
                var branches = dict.Where(x => text[i..].StartsWith(x.Key))
                                   .Select(x => (i, x.Key.Length, x.Value));
                if (branches.Any())
                    foreach (var branch in branches)
                        tree.Add(branch);
                else tree.Add((i, 1, text[i].ToString()));
            });
            return tree;
        }

        private static string FindShortest(ConcurrentBag<(int, int, string)> tree, string text, bool constant)
        {

        }

        private static void Analyze(string best, ConcurrentDictionary<string, string> dict, string text)
        {

        }
    }
}
