using System.Collections.Concurrent;

namespace CodeLord.Components
{
    internal static class Encoder
    {
        /// <summary> 找出最短编码 </summary>
        /// <param name="dict"> 词库，键值对为（词，编码） </param>
        /// <param name="text"> 要编码的文本 </param>
        /// <param name="constant"> 是否为整句输入。如果否，则词之间要加空格。 </param>
        public static void Encode(ConcurrentDictionary<string, List<string>> dict, string text, bool constant)
        {
            try
            {
                var tree = FindBranches(dict, text);
                var ways = FindShortest(tree, text, constant);
                var report = Analyze(ways, text);
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
        /// <param name="constant"> 是否为整句输入。如果否，则词之间要加空格。 </param>
        /// <returns> 长度最短的所有编码 </returns>
        private static string[] FindShortest(ConcurrentBag<(int head, int length, string code)> tree, string text, bool constant)
        {
            Console.WriteLine("正在遍历所有编码情况...");
            List<(string way, int tail)> tempWays = [];

            var starters = tree.Where(x => x.head == 0);
            foreach (var (head, length, code) in starters)
                tempWays.Add((code, head + length));

            Console.WriteLine($"共{text.Length}字：");
            for (int i = 1; i < text.Length; i++)
            {
                var prefixes = tempWays.Where(x => x.tail == i)
                                       .Select(x => x.way)
                                       .Distinct()
                                       .ToList();
                if (prefixes.Count == 0) continue;

                var mLength = prefixes.Min(x => x.Length);
                _ = prefixes.RemoveAll(x => x.Length != mLength);
                var suffixes = tree.Where(x => x.head == i);

                foreach (var way in prefixes)
                    foreach (var (head, length, code) in suffixes)
                        tempWays.Add((Connect(constant, way, code), head + length));
                
                _ = tempWays.RemoveAll(x => x.tail == i);
                Console.Write($"\r已遍历至第{i}字。");
            }

            var ways = tempWays.Select(x => x.way).Distinct().ToArray();
            Console.WriteLine($"\n遍历完成，共{ways.Length}种最短编码。");
            return ways;
        }

        private static string Connect(bool constant, string head, string tail)
        {
            return constant ? $"{head}{tail}" : $"{head} {tail}";
        }

        /// <summary> 分析每种最短编码并生成分析报告 </summary>
        /// <returns> 分析报告，每个元素为（第n种最短编码，各项评估列表） </returns>
        private static List<string>[] Analyze(string[] ways, string text)
        {
            Console.WriteLine("正在分析每种最短编码并生成分析报告...");
            var report = new List<string>[ways.Length];
            for (int i = 0; i < ways.Length; i++)
            {
                string way = ways[i];
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
