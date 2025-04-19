using System;
using FlexibleBitflags;

public static class BasicBitflagDemo
{
    public static void Run()
    {
        Console.WriteLine("== Basic Bitflag Demo ==");
        Console.WriteLine("Using FlexibleBitflags as a bit-twiddler");

        var flags = new Bitflag();

        // Set a few bits directly by index
        flags.SetBit(0); // Bit 0 (LSB)
        flags.SetBit(3); // Bit 3
        flags.SetBit(55); // Bit 55

        Console.WriteLine($"Value after setting bits 0, 3, and 55: 0x{flags.Value:X}");

        // Clear a bit
        flags.ClearBit(55); // Clear bit 55
        Console.WriteLine("Cleared bit 55:");
        Console.WriteLine($"Value: 0x{flags.Value:X}");

        // Toggle a bit
        flags.ToggleBit(3);
        Console.WriteLine("Toggled bit 3:");
        Console.WriteLine($"Value: 0x{flags.Value:X}");

        // Insert a value into bits 8–15
        flags.InsertBits(0xAB, 8, 15);
        Console.WriteLine("Inserted 0xAB into bits 8–15:");
        Console.WriteLine($"Value: 0x{flags.Value:X}");

        // Extract the same region
        ulong extracted = flags.ExtractBits(8, 15);
        Console.WriteLine($"Extracted bits 8–15: 0x{extracted:X2}");

        // PopCount and index checks
        Console.WriteLine($"PopCount: {flags.PopCount()}");
        Console.WriteLine($"First set bit: {flags.FirstSetBitIndex()}");
        Console.WriteLine($"Last set bit: {flags.LastSetBitIndex()}");
    }
}
