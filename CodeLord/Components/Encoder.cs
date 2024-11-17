using System.Collections.Concurrent;

namespace CodeLord.Components
{
    internal static class Encoder
    {
        /// <summary> 找出最短编码 </summary>
        /// <param name="dict"> 词库，键值对为（词，编码） </param>
        /// <param name="text"> 要编码的文本 </param>
        /// <param name="codeID"> 词的分隔方式，0为空格，1为无分隔，2为键道 </param>
        public static void Encode(ConcurrentDictionary<string, List<string>> dict, string text, int codeID)
        {
            try
            {
                var tree = FindBranches(dict, text);
                var ways = FindShortest(tree, text, codeID);
                var report = Analyzer.GenerateReport(ways, text);
                Reporter.Output(report);
            }
            catch (Exception e)
            {
                Console.WriteLine($"运行出错：{e.Message}");
            }
        }

        /// <summary> 找出所有可能的编码分支 </summary>
        /// <param name="dict"></param>
        /// <param name="text"></param>
        /// <returns> 每个元素为（起始索引，词的长度，编码），每个索引都一定会有编码 </returns>
        private static ConcurrentBag<(int, int, string)> FindBranches(ConcurrentDictionary<string, List<string>> dict, string text)
        {
            Console.WriteLine("正在寻找所有可能的编码分支...");
            ConcurrentBag<(int, int, string)> tree = [];
            _ = Parallel.For(0, text.Length, i =>
            {
                var branches = dict.Where(x => text[i..].StartsWith(x.Key))
                                   .Select(x => (i, x.Key.Length, x.Value))
                                   .ToArray();
                if (branches.Length != 0)
                    foreach (var (j, length, localList) in branches)
                        foreach (var code in localList)
                            tree.Add((j, length, code));
                else tree.Add((i, 1, text[i].ToString()));
            });
            Console.WriteLine($"共找到{tree.Count}个分支。");
            return tree;
        }

        /// <summary> 遍历所有编码以找出最短编码并输出 </summary>
        /// <param name="tree"> 以（起始索引，词的长度，编码）表示的所有编码分支 </param>
        /// <param name="text"> 要编码的文本 </param>
        /// <param name="codeID"> 词的分隔方式，0为空格，1为无分隔，2为键道 </param>
        /// <returns> 长度最短的所有编码 </returns>
        private static string[] FindShortest(ConcurrentBag<(int head, int length, string code)> tree, string text, int codeID)
        {
            Console.WriteLine("正在遍历所有编码情况...");
            Dictionary<string, int> tempWays = [];

            var starters = tree.Where(x => x.head == 0);
            foreach (var (_, length, code) in starters)
                tempWays[code] = length;

            Console.WriteLine($"共需遍历{text.Length}字：");
            for (int i = 1; i < text.Length; i++)
            {
                var prefixes = tempWays.Where(x => x.Value == i)
                                       .Select(x => x.Key)
                                       .ToList();
                if (prefixes.Count == 0) continue;

                foreach (var way in prefixes)
                    _ = tempWays.Remove(way);

                var mLength = prefixes.Min(x => x.Length);
                _ = prefixes.RemoveAll(x => x.Length != mLength);
                var suffixes = tree.Where(x => x.head == i);

                foreach (var way in prefixes)
                    foreach (var (_, length, code) in suffixes)
                        tempWays[Concater.Join(codeID, way, code)] = i + length;

                Console.Write($"\r已遍历至第{i}字。");
            }

            var ways = tempWays.Keys.ToArray();
            Console.WriteLine($"\n遍历完成，共{ways.Length}种最短编码。");
            return ways;
        }
    }
}
