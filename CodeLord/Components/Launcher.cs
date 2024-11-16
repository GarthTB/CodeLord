using System.Collections.Concurrent;

namespace CodeLord.Components
{
    internal static class Launcher
    {
        public static void Initialize()
        {
            var dict = GetDict();
            var text = GetText();
            var sentenceIn = GetSentenceIn();

            Encoder.Encode(dict, text, sentenceIn);

            static ConcurrentDictionary<string, string> GetDict()
            {
                Console.WriteLine("请输入词库路径：");
                var path = Console.ReadLine();
                ConcurrentDictionary<string, string> _dict;
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

            static bool GetSentenceIn()
            {
                Console.WriteLine("请指定是否为整句输入（1代表是，0代表否）：");
                return Console.ReadLine() == "1";
            }
        }

        public static void Launch(string dictPath, string textPath, bool sentenceIn)
        {
            if (File.Exists(dictPath) && File.Exists(textPath))
            {
                if (!Loader.LoadDict(dictPath, out var dict))
                    Console.WriteLine("词库载入失败。");
                else if (!Loader.LoadText(textPath, out var text))
                    Console.WriteLine("文本载入失败。");
                else Encoder.Encode(dict, text, sentenceIn);
            }
            else Console.WriteLine("无效的词库或文本路径。");
        }
    }
}
