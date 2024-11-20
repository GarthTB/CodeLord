namespace CodeLord.Components
{
    internal static class Analyzer
    {
        /// <summary> 分析每种最短编码并生成分析报告 </summary>
        /// <returns> 分析报告，每个元素为（第n种最短编码，各项评估列表） </returns>
        public static List<string>[] GenerateReport(string[] routes, string text)
        {
            Console.WriteLine("正在分析每种最短编码并生成分析报告...");
            var report = new List<string>[routes.Length];
            for (int i = 0; i < routes.Length; i++)
            {
                string way = routes[i];
                report[i] = [$"编码\t{way}"];
                report[i].Add($"总码长\t{way.Length}");
                report[i].Add($"字数\t{text.Length}");
                report[i].Add($"字均码长\t{(double)way.Length / text.Length}");
            }
            Console.WriteLine("分析完成。");
            return report;
        }
    }
}
