using System.Text;

namespace CodeLord.Components
{
    internal static class FileWriter
    {
        public static string TextPath { private get; set; } = "";

        /// <summary> 把长度最短的所有编码写入文件 </summary>
        /// <param name="bestWays"> 长度最短的所有编码 </param>
        public static void WriteWays(List<string> bestWays)
        {
            try
            {
                var fullPath = Path.GetFullPath(TextPath);
                var dir = Path.GetDirectoryName(fullPath)
                    ?? throw new NullReferenceException("路径为空。");
                var originName = Path.GetFileNameWithoutExtension(fullPath);
                var resultPath = Path.Combine(dir, $"{originName}的最短编码.txt");
                using StreamWriter sw = new(resultPath, false, Encoding.UTF8);

                for (int i = 0; i < bestWays.Count; i++)
                {
                    sw.WriteLine($"第{i + 1}种最短编码：");
                    sw.WriteLine(bestWays[i]);
                }
                Console.WriteLine("输出编码文件成功。");
            }
            catch (Exception e)
            {
                Console.WriteLine($"输出编码文件出错：{e.Message}");
            }
        }
    }
}
