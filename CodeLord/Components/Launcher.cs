using System.Collections.Concurrent;

namespace CodeLord.Components
{
    internal static class Launcher
    {
        public static void Initialize()
        {
            var (limit, dict) = GetDict();
            var text = GetText();
            var codeID = GetCodeID();

            Encoder.Encode(limit, dict, text, codeID);
            Console.WriteLine("程序结束。如需重新计算，请再次启动。按任意键退出...");
            _ = Console.ReadKey();

            static (int, ConcurrentDictionary<string, List<string>>) GetDict()
            {
                Console.WriteLine("请输入词库路径：");
                var path = Console.ReadLine();
                int limit;
                ConcurrentDictionary<string, List<string>> _dict;
                while (!File.Exists(path) || !Loader.LoadDict(path, out limit, out _dict))
                {
                    Console.WriteLine("词库路径无效或载入失败。请重新输入：");
                    path = Console.ReadLine();
                }
                return (limit, _dict);
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
        }

        public static void Launch(string dictPath, string textPath, string codeID)
        {
            if (File.Exists(dictPath) && File.Exists(textPath))
            {
                if (!Loader.LoadDict(dictPath, out var limit, out var dict))
                    Console.WriteLine("词库载入失败。");
                else if (!Loader.LoadText(textPath, out var text))
                    Console.WriteLine("文本载入失败。");
                else Encoder.Encode(limit, dict, text, ParseCodeID(codeID));
            }
            else Console.WriteLine("无效的词库或文本路径。");
        }

        private static int ParseCodeID(string codeID)
            => codeID switch { "1" => 1, "2" => 2, _ => 0 };
    }
}
