# FlexibleBitflags

**FlexibleBitflags** is a lightweight C# library that makes bitfield manipulation safe, intuitive, and expressive.  
It replaces manual bit-twiddling and `[Flags]` boilerplate with clean, name-based access and modern tooling.

> Designed for developer ergonomics. Built for performance.  
> Great for game dev, systems programming, tooling, and interop.

---

## Features

- Create flags from enums or string names — no magic numbers
- Define reusable named masks (e.g., `"DebugOverlay"`, `"CombatState"`)
- Insert/extract bit regions cleanly (`InsertBits`, `ExtractBits`)
- Pure and mutable workflows: `ApplyMaskNew()` vs `ApplyMask()`
- Supports cloning, partial resets, and precise control over bit state
- Works great for packed formats, PE headers, hardware registers, config flags

---

## Quick Start

```csharp
enum PlayerFlags
{
    CanJump,
    CanShoot,
    HasPowerup,
    IsDead
}

var flags = Bitflag.FromEnum<PlayerFlags>();

flags.SetBits(PlayerFlags.CanJump, PlayerFlags.HasPowerup);

if (flags[PlayerFlags.IsDead])
    Console.WriteLine("Game Over!");

---

enum Powerups
{
    Star,
    FireFlower,
    Cape
}

var flags = Bitflag.FromEnum<Powerups>();
flags.DefineMask("AllPowerups", Bitflag.CreateMask(0, 2));

flags.ApplyMask("AllPowerups");
flags.ClearMaskedBits("AllPowerups");

---

// Get bits 8–15 as a numeric value
ulong field = flags.ExtractBits(8, 15);

// Insert 0xAB into bits 16–23
flags.InsertBits(0xAB, 16, 23);
```

---

## Unit Tested

FlexibleBitflags includes a comprehensive xUnit test suite covering:

 - Bitfield construction and manipulation
 - Mask application and extraction
 - Insert/extract logic
 - Equality and operator overloads
 - Edge cases and invalid inputs

---

 ## Installation

 git clone https://github.com/nebulaeonline/FlexibleBitflags.git

---

 ## Why Use This Instead of [Flags]?

  - [Flags] enums are verbose, unsafe, and hard to evolve
  - They require hardcoded values and offer no grouping
  - FlexibleBitflags gives you clear syntax, zero guesswork, and real control

  ---

  ## Philosophy

  - FlexibleBitflags is built around developer ergonomics:
  - Everything is discoverable and readable
  - You always know what your bits mean
  - Performance is never sacrificed for clarity

---

## Full Example

```csharp

using FlexibleBitflags;

// This enum is used for gameplay state flags
enum PlayerFlags
{
    IsAlive,
    CanJump,
    HasDoubleJump,
    IsInvisible
}

// These flags come from a dynamic scripting system or editor config
string[] systemFlags = [
    "DevMode",
    "ShowFPS",
    "ShowHitboxes",
    "RenderWireframe"
];

// This enum defines actual bitmasks for named flag groups
[Flags]
enum DebugMasks : ulong
{
    None = 0,
    Visuals = 0b0000000000001110, // ShowFPS, ShowHitboxes, RenderWireframe
    DevTools = 0b0000000000001001  // DevMode + ShowHitboxes
}

// Initialize from both enum and string list
var player = Bitflag.FromEnum<PlayerFlags>();
var system = new Bitflag(systemFlags);

// Set some initial player flags
player.SetBits(PlayerFlags.IsAlive, PlayerFlags.CanJump);
Console.WriteLine($"Player flags: 0x{player.Value:X}");

// Enable ShowFPS and Hitboxes
system.SetBits("ShowFPS", "ShowHitboxes");
Console.WriteLine($"System flags: 0x{system.Value:X}");

// Define masks based on the enum with real bitmask values
system.DefineMasks<DebugMasks>();

// Apply a mask by enum member
system.ApplyMask(DebugMasks.DevTools);
Console.WriteLine($"After DevTools: 0x{system.Value:X}");

// Toggle a mask and check the result
system.ToggleByMask(DebugMasks.Visuals);
Console.WriteLine($"After toggling Visuals: 0x{system.Value:X}");

// Extract the ShowFPS bit (bit 1)
ulong fpsOn = system.ExtractBits(1, 1);
Console.WriteLine($"FPS bit: {fpsOn}");

// Insert a value into a defined region (e.g. setting difficulty level 3 in bits 4–7)
system.InsertBits(3, 4, 7);
Console.WriteLine($"After inserting difficulty (bits 4–7): 0x{system.Value:X}");

// Clear DevTools using a clone
var productionFlags = system.ClearMaskedBitsNew(DebugMasks.DevTools);
Console.WriteLine($"Production-safe flags: 0x{productionFlags!.Value:X}");

// Enumerate active system flags
Console.WriteLine("Active system flags:");
foreach (var name in system.GetSetNamedBits())
    Console.WriteLine($" - {name}");

```

---

## License

MIT