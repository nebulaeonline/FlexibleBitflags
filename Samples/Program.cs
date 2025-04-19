using System;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return;
        }

        string command = args[0].ToLowerInvariant();

        switch (command)
        {
            case "basic":
                BasicBitflagDemo.Run();
                break;
            case "enums":
                EnumsBitflagDemo.Run();
                break;
            case "strings":
                StringsBitflagDemo.Run();
                break;
            case "comp":
                CompBitflagDemo.Run();
                break;
            default:
                Console.WriteLine($"Unknown sample: {command}");
                PrintUsage();
                break;
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("FlexibleBitflags Sample Runner");
        Console.WriteLine("Usage: dotnet run -- [sample-name]");
        Console.WriteLine("Available samples:");
        Console.WriteLine("  basic   - Using FlexibleBitflags as a bit-twiddler");
        Console.WriteLine("  enums   - Using FlexibleBitflags with C# enums");
        Console.WriteLine("  strings - Using FlexibleBitflags with string keys");
        Console.WriteLine("  comp    - Using FlexibleBitflags in a more comprehensive example");
    }
}
