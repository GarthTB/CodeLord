using System.Collections.Concurrent;
using System.Text;

namespace CodeLord.Components
{
    internal class Loader
    {
        /// <summary> 载入词库 </summary>
        /// <param name="path"> 提供的词库路径 </param>
        /// <param name="dict"> 载入的词库，键值对为（词，编码） </param>
        /// <returns> 是否载入成功 </returns>
        public static bool LoadDict(string path, out ConcurrentDictionary<string, string> dict)
        {
            try
            {
                dict = ProcessEntries(ReadEntries(path));
                Console.WriteLine($"词库载入成功，共{dict.Count}个词。");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"词库载入出错：{e.Message}");
                dict = [];
                return false;
            }

            static HashSet<(string, string, int)> ReadEntries(string path)
            {
                using StreamReader sr = new(path, Encoding.UTF8);
                HashSet<(string, string, int)> entries = [];
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    var slices = RemoveComment(line).Split('\t');
                    if (slices.Length == 2)
                        _ = entries.Add((slices[0], slices[1], 0));
                    else if (slices.Length == 3)
                        if (int.TryParse(slices[2], out int priority))
                            _ = entries.Add((slices[0], slices[1], priority));
                        else Console.WriteLine($"词库格式错误，无法识别优先级：{line}");
                }
                return entries.Count == 0 ? throw new Exception("词库为空。") : entries;

                static string RemoveComment(string line)
                {
                    int hashIndex = line.IndexOf('#');
                    return hashIndex != -1 ? line[..hashIndex] : line;
                }
            }

            static ConcurrentDictionary<string, string> ProcessEntries(HashSet<(string word, string code, int priority)> entries)
            {
                var orderedEntries = entries.OrderByDescending(x => x.priority);
                ConcurrentDictionary<string, string> dict = [];
                foreach (var (word, code, priority) in orderedEntries)
                {
                    var realCode = code;
                    for (int position = 2; dict.Any(x => x.Value == realCode); position++)
                        realCode = $"{code}{position}"; // 选重直接加数字
                    _ = dict.TryAdd(word, realCode);
                }
                return dict.IsEmpty ? throw new Exception("词库为空。") : dict;
            }
        }

        /// <summary> 载入文本 </summary>
        /// <param name="path"> 提供的文本路径 </param>
        /// <param name="text"> 载入的文本 </param>
        /// <returns> 是否载入成功 </returns>
        public static bool LoadText(string path, out string text)
        {
            try
            {
                using StreamReader sr = new(path, Encoding.UTF8);
                text = sr.ReadToEnd();
                Console.WriteLine($"文本载入成功，共{text.Length}个字。");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"文本载入出错：{e.Message}");
                text = "";
                return false;
            }
        }
    }
}
