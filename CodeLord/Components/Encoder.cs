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
                var slices = SliceText(dict, text); // 预先切片以提升性能
                var routes = FindShortest(dict, slices, codeID, limit);
                var report = Analyzer.GenerateReport(routes, text);
                Reporter.Output(report);
            }
            catch (Exception e)
            {
                Console.WriteLine($"运行出错：{e.Message}");
            }
        }

        /// <summary> 预先切片文本以提升性能 </summary>
        /// <returns> 文本逐字切片 </returns>
        private static Span<string> SliceText(ConcurrentDictionary<string, List<string>> dict, string text)
        {
            Console.WriteLine("正在逐字切片文本...");
            var maxLen = dict.Keys.Max(x => x.Length); // 词库中的最大字数
            var slices = Enumerable.Range(0, text.Length)
                                   .Select(i => new string(text.Skip(i).Take(maxLen).ToArray()))
                                   .ToArray()
                                   .AsSpan();
            Console.WriteLine("切片完成。");
            return slices;
        }

        /// <summary> 遍历所有编码以找出最短编码并输出 </summary>
        /// <param name="dict"> 词库，键值对为（词，编码） </param>
        /// <param name="slices"> 文本逐字切片 </param>
        /// <param name="codeID"> 词的分隔方式，0为空格，1为无分隔，2为键道 </param>
        /// <param name="limit"> 中间结果的最大数量 </param>
        /// <returns> 长度最短的所有编码 </returns>
        private static string[] FindShortest(ConcurrentDictionary<string, List<string>> dict, Span<string> slices, int codeID, int limit)
        {
            Console.WriteLine($"共需遍历{slices.Length}字，正在遍历...");
            var tempRoutes = new HashSet<string>[slices.Length + 1]; // 索引为词的起始位置，元素为编码
            _ = Parallel.For(0, slices.Length + 1, i => tempRoutes[i] = []); // 初始化
            _ = tempRoutes[0].Add(""); // 启动子

            for (int i = 0; i < slices.Length; i++)
            {
                var heads = tempRoutes[i].OrderBy(x => x.Length).Take(limit); // 最短路径
                var tails = GetTails(dict, slices[i]);

                foreach (var head in heads)
                    foreach (var (len, codes) in tails) // code无重复项
                        foreach (var code in codes)
                            _ = tempRoutes[i + len].Add(Concater.Join(codeID, head, code));

                Console.Write($"\r已遍历至第{i + 1}字。");
                tempRoutes[i].Clear();
            }

            var minLen = tempRoutes[slices.Length].Min(x => x.Length);
            var routes = tempRoutes[slices.Length].Where(x => x.Length == minLen).ToArray();
            Console.WriteLine($"\n遍历完成，共得到{routes.Length}种最短编码。");
            return routes;

            static (int, List<string>)[] GetTails(ConcurrentDictionary<string, List<string>> dict, string text)
            {
                var branches = dict.Where(x => text.StartsWith(x.Key))
                                   .Select(x => (x.Key.Length, x.Value)) // Value无重复项
                                   .ToArray();
                return branches.Length == 0 ? [(1, [text[0..1]])] : branches;
            }
        }
    }
}
