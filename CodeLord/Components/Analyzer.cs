namespace CodeLord.Components
{
    internal static class Analyzer
    {
        /// <summary> 分析每种最短编码并生成分析报告 </summary>
        /// <returns> 分析报告，每个元素为最终的一行 </returns>
        public static List<string> GenerateReport(string[] routes, string text)
        {
            Console.WriteLine("正在分析每种最短编码并生成分析报告...");
            List<string> report = [];
            for (int i = 0; i < routes.Length; i++)
            {
                string route = routes[i];
                report.Add($"第{i + 1}种最短编码：");
                report.Add(route);
                ReportCodeLength(text, route, report);
                ReportFingerRates(route, report);
                ReportRowRates(route, report);
                ReportSpaceRate(route, report);
                report.Add(""); // 用来分隔的空行
            }
            Console.WriteLine("分析完成。");
            return report;
        }

        /// <summary> 记录码长 </summary>
        /// <param name="text"> 一篇文章 </param>
        /// <param name="route"> 一篇文章的完整编码 </param>
        /// <param name="report"> 分析报告 </param>
        private static void ReportCodeLength(string text, string route, List<string> report)
        {
            report.Add($"码数\t{route.Length}");
            report.Add($"字数\t{text.Length}");
            report.Add($"字均码长\t{(double)route.Length / text.Length}");
        }

        /// <summary> 记录（左小指、无名指、中指、食指，右食指、中指、无名指、小指，其他）的使用率 </summary>
        /// <param name="route"> 一篇文章的完整编码 </param>
        /// <param name="report"> 分析报告 </param>
        private static void ReportFingerRates(string route, List<string> report)
        {
            if (!Loader.LoadConfig(out var config))
            {
                Console.WriteLine("将不会分析各手指的使用率。");
                return;
            }

            List<int> counts = [];
            foreach (var keys in config.Take(8))
                counts.Add(route.Count(keys.Contains));
            counts.Add(route.Length - counts.Sum()); // 其他

            var leftSum = counts[0..4].Sum();
            var rightSum = counts[4..8].Sum();
            report.Add($"左手\t{(double)leftSum / route.Length}");
            report.Add($"左小指\t{(double)counts[0] / route.Length}");
            report.Add($"左无名\t{(double)counts[1] / route.Length}");
            report.Add($"左中指\t{(double)counts[2] / route.Length}");
            report.Add($"左食指\t{(double)counts[3] / route.Length}");
            report.Add($"右手\t{(double)rightSum / route.Length}");
            report.Add($"右食指\t{(double)counts[4] / route.Length}");
            report.Add($"右中指\t{(double)counts[5] / route.Length}");
            report.Add($"右无名\t{(double)counts[6] / route.Length}");
            report.Add($"右小指\t{(double)counts[7] / route.Length}");
            report.Add($"其他指\t{(double)counts[8] / route.Length}");
            if (leftSum != 0 && rightSum != 0)
                report.Add($"偏倚程度\t{(double)Math.Abs(leftSum - rightSum) / (leftSum + rightSum)}");
        }

        /// <summary> 记录（数字排、上排、中排、下排、其他排）的使用率 </summary>
        /// <param name="route"> 一篇文章的完整编码 </param>
        /// <param name="report"> 分析报告 </param>
        private static void ReportRowRates(string route, List<string> report)
        {
            if (!Loader.LoadConfig(out var config))
            {
                Console.WriteLine("将不会分析各排键的使用率。");
                return;
            }

            List<int> counts = [];
            foreach (var keys in config.Skip(8).Take(4))
                counts.Add(route.Count(keys.Contains));
            counts.Add(route.Length - counts.Sum()); // 其他排

            report.Add($"数字键\t{(double)counts[0] / route.Length}");
            report.Add($"上排键\t{(double)counts[1] / route.Length}");
            report.Add($"中排键\t{(double)counts[2] / route.Length}");
            report.Add($"下排键\t{(double)counts[3] / route.Length}");
            report.Add($"其他排\t{(double)counts[4] / route.Length}");
        }

        /// <summary> 记录空格键使用率 </summary>
        /// <param name="route"> 一篇文章的完整编码 </param>
        /// <param name="report"> 分析报告 </param>
        private static void ReportSpaceRate(string route, List<string> report)
            => report.Add($"空格率\t{(double)route.Count(c => c == ' ') / route.Length}");
    }
}
