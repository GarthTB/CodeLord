using System.Collections.Concurrent;

namespace CodeLord.Components
{
    internal static class Encoder
    {
        /// <summary> 找出最短编码 </summary>
        /// <param name="dict"> 词库，键值对为（词，编码） </param>
        /// <param name="text"> 要编码的文本 </param>
        /// <param name="codeID"> 词的分隔方式，0为空格，1为无分隔，2为键道 </param>
        /// <param name="limit"> 中间结果的最大数量 </param>
        public static void Encode(ConcurrentDictionary<string, List<string>> dict, string text, int codeID, int limit)
        {
            try
            {
                var tree = FindBranches(dict, text);
                var ways = FindShortest(tree, text.Length, codeID, limit);
                var report = Analyzer.GenerateReport(ways, text);
                Reporter.Output(report);
            }
            catch (Exception e)
            {
                Console.WriteLine($"运行出错：{e.Message}");
            }
        }

        /// <summary> 找出所有可能的编码分支 </summary>
        /// <returns> 键为起始索引，值为（词的长度，编码），每个索引都一定会有编码 </returns>
        private static ConcurrentDictionary<int, List<(int, string)>> FindBranches(ConcurrentDictionary<string, List<string>> dict, string text)
        {
            Console.WriteLine("正在寻找所有可能的编码分支...");
            ConcurrentDictionary<int, List<(int, string)>> tree = [];
            _ = Parallel.For(0, text.Length, i =>
            {
                tree[i] = [];
                var branches = dict.Where(x => text[i..].StartsWith(x.Key))
                                   .Select(x => (i, x.Key.Length, x.Value)) // Value无重复项
                                   .ToArray();
                if (branches.Length != 0)
                    foreach (var (_, length, localList) in branches) // localList无重复项
                        foreach (var code in localList)
                            tree[i].Add((length, code));
                else tree[i].Add((1, text[i].ToString()));
            });
            Console.WriteLine($"共找到{tree.Values.Sum(list => list.Count)}个分支。");
            return tree;
        }

        /// <summary> 遍历所有编码以找出最短编码并输出 </summary>
        /// <param name="tree"> 键为起始索引，值为（词的长度，编码），每个索引都一定会有编码 </param>
        /// <param name="textLength"> 要编码的文本长度 </param>
        /// <param name="codeID"> 词的分隔方式，0为空格，1为无分隔，2为键道 </param>
        /// <param name="limit"> 中间结果的最大数量 </param>
        /// <returns> 长度最短的所有编码 </returns>
        private static string[] FindShortest(ConcurrentDictionary<int, List<(int, string)>> tree, int textLength, int codeID, int limit)
        {
            Console.WriteLine($"正在遍历所有编码情况，共需遍历{textLength}字...");
            Dictionary<int, HashSet<string>> tempWays = []; // 键为末尾位置，值为编码集合
            var starters = tree[0];
            foreach (var (length, code) in starters)
                Record(length, code, tempWays);

            for (int i = 1; i < textLength; i++)
            {
                if (!tempWays.ContainsKey(i)) continue;

                var heads = tempWays[i];
                var minLen = heads.Min(x => x.Length);
                _ = heads.RemoveWhere(x => x.Length != minLen);
                var trimHeads = heads.Take(limit);

                var tails = tree[i];
                foreach (var head in trimHeads)
                    foreach (var (length, code) in tails) // code无重复项
                        Record(i + length, Concater.Join(codeID, head, code), tempWays);

                tempWays[i] = [];

                Console.Write($"\r已遍历至第{i}字。");
            }

            var ways = tempWays[textLength]?.ToArray() ?? [];
            Console.WriteLine($"\n遍历完成，共得到{ways.Length}种最短编码。");
            return ways;

            static void Record(int endIndex, string code, Dictionary<int, HashSet<string>> tempWays)
            {
                if (!tempWays.TryGetValue(endIndex, out var hashSet))
                {
                    hashSet = [code];
                    tempWays[endIndex] = hashSet;
                }
                else _ = hashSet.Add(code);
            }
        }
    }
}
