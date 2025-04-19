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

## Motivation

Anyone who has tried to use C# enums in the same way that you use bitfields in C/C++ has hit their head against a wall more times than once, because everything that *would* be ergonomic or straighforward to do is a *challenge*. It's like Microsoft stopped one feature short in every direction to hinder using Enums for their Deity-given purpose. So I got tired of fiddling with it, and wondered if I could build a library that did all the little things that make working with bitfields in C# so unergonomic. And voila, we now have a new library I'm releasing to the world in FlexibleBitflags.
The idea was to make creating & manipulating bitfields easy and painless. You can create what an enum *should* have been *out of an enum*! Just feed this library an enum, and even use your enums directly in the manipulation functions. But enums aren't required! You can use the library plain-Jane and just enjoy the ergonomic bitfields, or you can make bitfields out of any IEnumerable<string>, meaning you can just pass an array of names, and they will become bit 0...bit n's flags. It really is that simple. Then you can read & write them with string keys (if you didn't choose the enum route). And yes, if you chose neither route, you can still name all of your bitflags. But that won't allow me to re-use them! Aha! There's a few Clone() methods that will allow you to have multiple instances of the bitfields you build. Woo-hoo!
I literally went with the kitchen sink approach here, but it's a nice 3-compartment industrial stainless-steel kitchen sink! Every little nicety that I could think of, every roadblock I had ever hit just melts away. I know it's not terribly hard to bit-twiddle. This wasn't about the level of difficulty, it was about simplicity and flexibility. And I threw in bit-by-bit injection and extraction too, for thoes of you who work low-level in C# (yes, we do exist). What's not to love? Please give it a shot; I am working on real documentation now, but in the interim, at least every function has docxml, so Intellisense can help out.
I felt this was a good milestone for a 1.0 release given that it has unit tests for nearly everything and these functions aren't super complicated, it's just that no one has sat down to spit out the 1k lines or so to do it this way. Truth be told, it is written the way *I* wanted it done. And yes, I know the saying (if you want it done your way, do it yourself). So I did. I hope you all enjoy and can make use of this tidbit of code. If you find problems or just can't quite do that thing you think you should be able to, please reach out and I'll get it in the library.
Remember folks, this is supposed to be fun (at least sometimes).

N


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

---

enum PlayerFlags
{
    CanMove,
    CanShoot,
    CanFly,
    HasShield
}

// Create a shared template for all players
var baseFlags = Bitflag.FromEnum<PlayerFlags>();
baseFlags.SetBits(PlayerFlags.CanMove, PlayerFlags.CanShoot);

// Clone for two players
var player1 = baseFlags.Clone();
var player2 = baseFlags.Clone();

// Customize player 2
player2.SetBits(PlayerFlags.CanFly, PlayerFlags.HasShield);

// player1 remains unchanged
Console.WriteLine($"Player1: 0x{player1.Value:X}");
Console.WriteLine($"Player2: 0x{player2.Value:X}");

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