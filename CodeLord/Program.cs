namespace CodeLord
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 3)
                Components.Launcher.Launch(args[0], args[1], args[2] == "1");
            else Components.Launcher.Initialize();
        }
    }
}
