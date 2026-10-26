namespace DGRV3TS;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Operations(args.FirstOrDefault()));
    }
}
