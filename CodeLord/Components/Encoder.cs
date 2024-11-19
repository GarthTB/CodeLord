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
                var ways = FindShortest(dict, text, codeID, limit);
                var report = Analyzer.GenerateReport(ways, text);
                Reporter.Output(report);
            }
            catch (Exception e)
            {
                Console.WriteLine($"运行出错：{e.Message}");
            }
        }

        /// <summary> 遍历所有编码以找出最短编码并输出 </summary>
        /// <param name="dict"> 词库，键值对为（词，编码） </param>
        /// <param name="text"> 要编码的文本 </param>
        /// <param name="codeID"> 词的分隔方式，0为空格，1为无分隔，2为键道 </param>
        /// <param name="limit"> 中间结果的最大数量 </param>
        /// <returns> 长度最短的所有编码 </returns>
        private static string[] FindShortest(ConcurrentDictionary<string, List<string>> dict, string text, int codeID, int limit)
        {
            Console.WriteLine($"共需遍历{text.Length}字，正在遍历...");
            Dictionary<int, HashSet<string>> tempWays = new(text.Length) { [0] = [""] }; // 键为末尾位置，值为编码集合
            var maxLen = dict.Keys.Max(x => x.Length); // 词库中的最大字数
            var slices = Enumerable.Range(0, text.Length)
                                   .Select(i => new string(text.Skip(i).Take(maxLen).ToArray()))
                                   .ToArray()
                                   .AsSpan(); // 预先切片以提升性能

            for (int i = 0; i < text.Length; i++)
            {
                var heads = tempWays[i].OrderBy(x => x.Length).Take(limit); // 最短路径
                var tails = FindBranches(dict, slices[i]);

                foreach (var head in heads)
                    foreach (var (length, codes) in tails) // code无重复项
                        foreach (var code in codes)
                            RecordWays(i + length, Concater.Join(codeID, head, code), tempWays);

                Console.Write($"\r已遍历至第{i}字。");
                tempWays[i].Clear();
            }

            var ways = tempWays[text.Length]?.ToArray() ?? [];
            Console.WriteLine($"\n遍历完成，共得到{ways.Length}种最短编码。");
            return ways;

            static (int, List<string>)[] FindBranches(ConcurrentDictionary<string, List<string>> dict, string text)
            {
                var branches = dict.Where(x => text.StartsWith(x.Key))
                                   .Select(x => (x.Key.Length, x.Value)) // Value无重复项
                                   .ToArray();
                return branches.Length == 0 ? [(1, [text[0..1]])] : branches;
            }

            static void RecordWays(int endIndex, string code, Dictionary<int, HashSet<string>> tempWays)
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
