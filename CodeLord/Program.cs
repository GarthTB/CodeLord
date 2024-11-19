namespace CodeLord
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 4)
                Components.Launcher.Launch(args[0], args[1], args[2], args[3]);
            else Components.Launcher.Initialize();
        }
    }
}
