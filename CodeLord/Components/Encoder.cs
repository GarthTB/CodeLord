using System.Collections.Concurrent;

namespace CodeLord.Components
{
    internal static class Encoder
    {
        public static void Encode(ConcurrentDictionary<string, string> dict, string text, bool sentenceIn)
        {
            try
            {
                var tree = FindBranches(dict, text);
                var best = FindShortest(tree, sentenceIn);
                Analyze(best);
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
    }
}
