using System.Text;

namespace CodeLord.Components
{
    internal static class Reporter
    {
        public static string TextPath { private get; set; } = "";

        /// <summary> 输出分析报告到文件或控制台 </summary>
        /// <param name="report"> 分析报告，每个元素为最终的一行 </param>
        public static void Output(List<string> report)
        {
            if (!WriteFile(report))
                OutputToConsole(report);
        }

        private static bool WriteFile(List<string> report)
        {
            try
            {
                Console.WriteLine("正在将分析报告写入文件...");
                var fullPath = Path.GetFullPath(TextPath);
                var dir = Path.GetDirectoryName(fullPath)
                    ?? throw new NullReferenceException("路径为空。");
                var originName = Path.GetFileNameWithoutExtension(fullPath);
                var resultPath = Path.Combine(dir, $"{originName}的分析报告.txt");
                using StreamWriter sw = new(resultPath, false, Encoding.UTF8); // 不续写，直接覆盖
                foreach (var line in report)
                    sw.WriteLine(line);
                Console.WriteLine("分析报告已成功写入文件。");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"写入文件出错：{e.Message}");
                Console.WriteLine("分析报告将输出到控制台。");
                return false;
            }
        }

        private static void OutputToConsole(List<string> report)
        {
            foreach (var line in report)
                Console.WriteLine(line);
            Console.WriteLine("分析报告输出完毕。");
        }
    }
}
