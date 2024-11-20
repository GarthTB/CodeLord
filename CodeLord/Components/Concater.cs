namespace CodeLord.Components
{
    internal static class Concater
    {
        /// <summary> 将新的编码与已有编码拼接 </summary>
        /// <param name="codeID"> 词的分隔方式，0为空格，1为无分隔，2为键道 </param>
        /// <param name="head"> 已有的编码 </param>
        /// <param name="tail"> 新的编码 </param>
        /// <returns> 拼接后的编码 </returns>
        public static string Join(int codeID, string head, string tail)
        => codeID switch
        {
            1 => $"{head}{tail}",
            2 => JD(head, tail),
            _ => $"{head} {tail}"
        };

        /// <summary> 按键道6的规则连接编码 </summary>
        /// <param name="head"> 已有的整篇编码 </param>
        /// <param name="tail"> 新的词组的编码 </param>
        /// <returns> 拼接后的编码 </returns>
        private static string JD(string head, string tail)
        {
            var x = "aiouv"; // 形码码元
            var y = "bcdefghjklmnpqrstwxyz"; // 音码码元
            var a = "abcdefghijklmnopqrstuvwxyz"; // 所有码元
            var oldEnd = head[^1];
            var newStart = tail[0];
            if (!a.Contains(newStart) && oldEnd == ' ') // 标点开头且前为空格，直接替换空格
                return $"{head[..^1]}{tail}";
            if (tail.Length < 4 && y.Contains(tail[^1])) // 不足4码且以音码结尾，后补空格
                tail = $"{tail} ";
            if (x.Contains(newStart) && a.Contains(oldEnd)) // 形码开头且无标点断开，前加空格
                tail = $" {tail}";
            return $"{head}{tail}";
        }
    }
}
