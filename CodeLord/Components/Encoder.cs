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
                var tree = FindBranches(dict, text);
                var bestWays = FindShortest(tree, text, constant);
                var report = Analyze(bestWays, text);
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
        private static ConcurrentBag<(int, int, string)> FindBranches(ConcurrentDictionary<string, string> dict, string text)
        {
            Console.WriteLine("正在寻找所有可能的编码分支...");
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
            Console.WriteLine($"共找到{tree.Count}个分支。");
            return tree;
        }

        /// <summary> 遍历所有编码以找出最短编码并输出 </summary>
        /// <param name="tree"> 以（起始索引，词的长度，编码）表示的所有编码分支 </param>
        /// <param name="text"> 要编码的文本 </param>
        /// <param name="constant"> 是否为整句输入。如果否，则词之间要加空格。 </param>
        /// <returns> 长度最短的所有编码 </returns>
        private static string[] FindShortest(ConcurrentBag<(int head, int length, string code)> tree, string text, bool constant)
        {
            Console.WriteLine("正在遍历所有编码情况...");
            var ways = FindAllWays(tree, text, constant);
            Console.WriteLine($"遍历完成，共{ways.Count}种编码。");
            var bestLength = ways.Min(x => x.Length);
            var bestWays = ways.Where(x => x.Length == bestLength).ToArray();
            Console.WriteLine($"已找到{bestWays.Length}种最短编码。");
            return bestWays;

            static HashSet<string> FindAllWays(ConcurrentBag<(int head, int length, string code)> tree, string text, bool constant)
            {
                List<(string way, int tail)> ways = [];

                var starters = tree.Where(x => x.head == 0);
                foreach (var (head, length, code) in starters)
                    ways.Add((code, head + length));

                for (int i = 1; i < text.Length; i++)
                {
                    var prefixes = ways.Where(x => x.tail == i).ToArray();
                    if (prefixes.Length == 0) continue;
                    var suffixes = tree.Where(x => x.head == i);
                    foreach (var (way, tail) in prefixes)
                        foreach (var (head, length, code) in suffixes)
                            ways.Add(constant
                                ? ($"{way}{code}", head + length)
                                : ($"{way} {code}", head + length));
                    _ = ways.RemoveAll(x => x.tail == i);
                }

                return ways.Select(x => x.way).ToHashSet();
            }
        }

        /// <summary> 分析每种最短编码并生成分析报告 </summary>
        /// <returns> 分析报告，每个元素为（第n种最短编码，各项评估列表） </returns>
        private static List<string>[] Analyze(string[] bestWays, string text)
        {
            Console.WriteLine("正在分析每种最短编码并生成分析报告...");
            var report = new List<string>[bestWays.Length];
            for (int i = 0; i < bestWays.Length; i++)
            {
                string way = bestWays[i];
                report[i] = [$"编码\t{way}"];
                report[i].Add($"总码长\t{way.Length}");
                report[i].Add($"字数\t{text.Length}");
                report[i].Add($"平均码长\t{(double)way.Length / text.Length}");
            }
            Console.WriteLine("分析完成。");
            return report;
        }
    }
}
