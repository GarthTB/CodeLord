using System.Collections.Concurrent;

namespace CodeLord.Components
{
    internal static class Encoder
    {
        /// <summary> 找出最短编码 </summary>
        /// <param name="dict"> 词库，键值对为（词，编码） </param>
        /// <param name="text"> 要编码的文本 </param>
        /// <param name="codeID"> 词的分隔方式，0为空格及标点，1为无分隔，2为键道 </param>
        /// <param name="limit"> 中间结果的最大数量 </param>
        public static void Encode(ConcurrentDictionary<string, List<string>> dict, string text, int codeID, int limit)
        {
            try
            {
                var routes = FindShortest(dict, text, codeID, limit);
                var report = Analyzer.GenerateReport(routes, text);
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
        /// <param name="codeID"> 词的分隔方式，0为空格及标点，1为无分隔，2为键道 </param>
        /// <param name="limit"> 中间结果的最大数量 </param>
        /// <returns> 长度最短的所有编码 </returns>
        private static string[] FindShortest(ConcurrentDictionary<string, List<string>> dict, string text, int codeID, int limit)
        {
            Console.WriteLine($"共需遍历{text.Length}字，正在遍历...");
            var tempRoutes = new HashSet<string>[text.Length + 1]; // 索引为词的起始位置，元素为编码
            _ = Parallel.For(0, text.Length + 1, i => tempRoutes[i] = []); // 初始化
            _ = tempRoutes[0].Add(""); // 启动子

            var maxLen = dict.Keys.Max(x => x.Length); // 词库中的最大字数

            for (int i = 0; i < text.Length; i++)
            {
                var heads = tempRoutes[i].OrderBy(x => x.Length).Take(limit); // 最短路径
                var tails = GetTails(dict, GetSlice(text, maxLen, i));

                foreach (var head in heads)
                    foreach (var (len, codes) in tails) // code无重复项
                        foreach (var code in codes)
                            _ = tempRoutes[i + len].Add(Concater.Join(codeID, head, code));

                Console.Write($"\r已遍历至第{i + 1}字。");
                tempRoutes[i].Clear();
            }

            var minLen = tempRoutes[text.Length].Min(x => x.Length); // 最短路径
            var routes = tempRoutes[text.Length].Where(x => x.Length == minLen).ToArray();
            Console.WriteLine($"\n遍历完成，共得到{routes.Length}种最短编码。");
            return routes;

            static string GetSlice(string text, int maxLen, int i)
                => text.Substring(i, Math.Min(maxLen, text.Length - i));

            static (int, List<string>)[] GetTails(ConcurrentDictionary<string, List<string>> dict, string text)
            {
                var tails = dict.AsParallel()
                                .Where(x => text.StartsWith(x.Key))
                                .Select(static x => (x.Key.Length, x.Value)) // Value无重复项
                                .ToArray();
                return tails.Length == 0 ? [(1, [text[0..1]])] : tails;
            }
        }
    }
}
