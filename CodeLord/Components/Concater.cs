namespace CodeLord.Components
{
    internal static class Concater
    {
        public static string Join(bool constant, string head, string tail)
        {
            return constant ? $"{head}{tail}" : $"{head} {tail}";
        }
    }
}
