using System;
using FlexibleBitflags;

public static class StringsBitflagDemo
{
    // Not necessary if using strings; included here for clarity
    // and to contrast with the enum-based demo
    enum RenderOptions
    {
        IsVisible,
        IsPaused,
        EnableVSync,
        RenderShadows,
        DebugOverlay
    }

    public static void Run()
    {
        Console.WriteLine("== String-Based Bitflag Demo ==");
        Console.WriteLine("Using FlexibleBitflags with string keys (and enum for access)");

        // Create using raw string keys (config-style)
        var flags = new Bitflag(new[]
        {
            "IsVisible",
            "IsPaused",
            "EnableVSync",
            "RenderShadows",
            "DebugOverlay"
        });

        // Set some bits using strings
        flags.SetBits("IsVisible", "RenderShadows");
        flags.ToggleBits("EnableVSync");

        Console.WriteLine($"Bitfield value: 0x{flags.Value:X}");

        // Access using enum members (names resolve to strings)
        if (flags[RenderOptions.EnableVSync])
            Console.WriteLine("VSync is ON (accessed via enum)");

        // Access using strings
        if (flags["EnableVSync"])
            Console.WriteLine("VSync is ON (accessed via string)");

        // Define a reusable mask for debug settings
        flags.DefineMask("DebugOptions", 0b00011000); // VSync + DebugOverlay
        flags.ApplyMask("DebugOptions");

        Console.WriteLine($"After applying DebugOptions: 0x{flags.Value:X}");

        // Extract just one bit
        ulong debugBit = flags.ExtractBits(4, 4);
        Console.WriteLine($"DebugOverlay bit: {(debugBit == 1 ? "ON" : "OFF")}");

        // Clone and strip debug flags
        var cleanFlags = flags.ClearMaskedBitsNew("DebugOptions");
        Console.WriteLine($"After clearing debug: 0x{cleanFlags!.Value:X}");

        // List all remaining flags
        Console.WriteLine("Set flags:");
        foreach (var name in cleanFlags.GetSetNamedBits())
            Console.WriteLine($" - {name}");
    }
}
