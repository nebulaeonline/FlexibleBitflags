using System;
using FlexibleBitflags;

public static class EnumsBitflagDemo
{
    // Define your enum — no [Flags] or values required
    enum PlayerAbilities
    {
        CanJump,
        CanShoot,
        CanDoubleJump,
        HasJetpack,
        IsDead
    }

    public static void Run()
    {
        Console.WriteLine("== Enum-Based Bitflag Demo ==");
        Console.WriteLine("Using FlexibleBitflags with C# enums");
        
        // Create a Bitflag using the enum
        var flags = Bitflag.FromEnum<PlayerAbilities>();

        // Set some flags using enum values directly; this can be 1 or many
        flags.SetBits(PlayerAbilities.CanJump, PlayerAbilities.HasJetpack);
        Console.WriteLine($"Set CanJump and HasJetpack: 0x{flags.Value:X}");

        // Clear some flags using enum values directly; this can be 1 or many
        flags.ClearBits(PlayerAbilities.CanJump);
        Console.WriteLine($"Cleared CanJump: 0x{flags.Value:X}");

        // Check a flag
        if (flags[PlayerAbilities.HasJetpack])
            Console.WriteLine("Jetpack enabled!");

        // Toggle a flag
        flags.ToggleBits(PlayerAbilities.CanJump);
        Console.WriteLine($"Toggled CanJump: 0x{flags.Value:X}");

        // Clone it for another player
        var player2 = flags.Clone();
        player2.SetBits(PlayerAbilities.IsDead);

        Console.WriteLine($"Player 1: 0x{flags.Value:X}");
        Console.WriteLine($"Player 2: 0x{player2.Value:X}");

        // Enumerate all set bits by name
        Console.WriteLine("Active player 2 flags:");
        foreach (var name in player2.GetSetNamedBits())
            Console.WriteLine($" - {name}");
    }
}
