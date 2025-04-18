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

## License

MIT