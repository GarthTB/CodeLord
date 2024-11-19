using System.Collections.Concurrent;

namespace CodeLord.Components
{
    internal static class Launcher
    {
        public static void Initialize()
        {
            var dict = GetDict();
            var text = GetText();
            var codeID = GetCodeID();
            var limit = GetLimit();

            Encoder.Encode(dict, text, codeID, limit);
            Console.WriteLine("程序结束。如需重新计算，请再次启动。按任意键退出...");
            _ = Console.ReadKey();

            static ConcurrentDictionary<string, List<string>> GetDict()
            {
                Console.WriteLine("请输入词库路径：");
                var path = Console.ReadLine();
                ConcurrentDictionary<string, List<string>> _dict;
                while (!File.Exists(path) || !Loader.LoadDict(path, out _dict))
                {
                    Console.WriteLine("词库路径无效或载入失败。请重新输入：");
                    path = Console.ReadLine();
                }
                return _dict;
            }

            static string GetText()
            {
                Console.WriteLine("请输入文本路径：");
                var path = Console.ReadLine();
                string _text;
                while (!File.Exists(path) || !Loader.LoadText(path, out _text))
                {
                    Console.WriteLine("文本路径无效或载入失败。请重新输入：");
                    path = Console.ReadLine();
                }
                return _text;
            }

            static int GetCodeID()
            {
                Console.WriteLine("请指定编码连接方式：");
                Console.WriteLine("（1：无间隔，2：键道，其他：空格隔开）");
                return ParseCodeID(Console.ReadLine() ?? "0");
            }

            static int GetLimit()
            {
                Console.WriteLine("请指定中间路径的最高数量：");
                Console.WriteLine("（不是最终路径的数量，但不影响最终路径的码长。无效输入则默认为100。）");
                return ParseLimit(Console.ReadLine() ?? "100");
            }
        }

        public static void Launch(string dictPath, string textPath, string codeID, string limit)
        {
            if (File.Exists(dictPath) && File.Exists(textPath))
            {
                if (!Loader.LoadDict(dictPath, out var dict))
                    Console.WriteLine("词库载入失败。");
                else if (!Loader.LoadText(textPath, out var text))
                    Console.WriteLine("文本载入失败。");
                else Encoder.Encode(dict, text, ParseCodeID(codeID), ParseLimit(limit));
            }
            else Console.WriteLine("无效的词库或文本路径。");
        }

        private static int ParseCodeID(string codeID)
            => codeID switch { "1" => 1, "2" => 2, _ => 0 };

        private static int ParseLimit(string limit)
            => (int.TryParse(limit, out var _limit) && _limit > 0) ? _limit : 100;
    }
}
