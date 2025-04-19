using System;
using FlexibleBitflags;

public static class CompBitflagDemo
{
    // Simulated flags in the e_flags field of an ELF header (for ARM)
    // Remember: you do not have to mix strings and enums; this is just a demo
    enum ElfArmFlags
    {
        EF_ARM_RELEXEC = 0,  // bit 0
        EF_ARM_HASENTRY = 1, // bit 1
        EF_ARM_INTERWORK = 2, // bit 2
        EF_ARM_APCS_26 = 3,   // bit 3
        EF_ARM_ALIGN8 = 4,    // bit 4
        EF_ARM_NEW_ABI = 5,   // bit 5
        EF_ARM_OLD_ABI = 6,   // bit 6
        EF_ARM_SOFT_FLOAT = 9,// bit 9
        EF_ARM_VFP_FLOAT = 10 // bit 10
    }

    public static void Run()
    {
        Console.WriteLine("== Comprehensive ELF Bitflag Demo ==");
        Console.WriteLine("Using FlexibleBitflags to model Linux ELF e_flags (ARM)");

        // Step 1: Define named bits manually
        var eflags = new Bitflag();
        eflags.TryNameBit("EF_ARM_RELEXEC", 0);
        eflags.TryNameBit("EF_ARM_HASENTRY", 1);
        eflags.TryNameBit("EF_ARM_INTERWORK", 2);
        eflags.TryNameBit("EF_ARM_APCS_26", 3);
        eflags.TryNameBit("EF_ARM_ALIGN8", 4);
        eflags.TryNameBit("EF_ARM_NEW_ABI", 5);
        eflags.TryNameBit("EF_ARM_OLD_ABI", 6);
        eflags.TryNameBit("EF_ARM_SOFT_FLOAT", 9);
        eflags.TryNameBit("EF_ARM_VFP_FLOAT", 10);

        // Step 2: Set initial flags
        eflags.SetBits("EF_ARM_HASENTRY", "EF_ARM_ALIGN8", "EF_ARM_SOFT_FLOAT");

        Console.WriteLine($"Initial e_flags: 0x{eflags.Value:X}");

        // Step 3: Toggle from soft float to VFP
        eflags.ClearBit("EF_ARM_SOFT_FLOAT");
        eflags.SetBits("EF_ARM_VFP_FLOAT");

        Console.WriteLine($"After float mode change: 0x{eflags.Value:X}");

        // Step 4: Insert ABI version (bits 24–27)
        eflags.InsertBits(0b_0011, 24, 27); // ABI version 3
        Console.WriteLine($"After inserting ABI version (3): 0x{eflags.Value:X}");

        // Step 5: Extract that ABI version
        ulong abi = eflags.ExtractBits(24, 27);
        Console.WriteLine($"Extracted ABI version: {abi}");

        // Step 6: Use clone to generate a stripped-down read-only view
        // Clone values only (no names)
        var stripped = eflags.CloneValues();
        stripped.WithNamesFrom(eflags);
        stripped.SetBits("EF_ARM_HASENTRY", "EF_ARM_ALIGN8");

        Console.WriteLine($"Stripped (runtime-safe) flags: 0x{stripped.Value:X}");

        // Step 7: Show all active flag names
        Console.WriteLine("Active flags:");
        foreach (var name in eflags.GetSetNamedBits())
            Console.WriteLine($" - {name}");

        // Step 8: Reverse lookup a specific bit
        if (eflags.TryGetBitName(10, out var name10))
            Console.WriteLine($"Bit 10 is named: {name10}");
    }
}
