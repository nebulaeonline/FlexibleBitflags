using Xunit;
using FlexibleBitflags;
using System;

namespace FlexibleBitflags.Tests;

public class BitflagTests
{
    enum TestEnum { X, Y, Z }
    enum EnumWithMoreThan64Values
    {
        V0, V1, V2, V3, V4, V5, V6, V7, V8, V9,
        V10, V11, V12, V13, V14, V15, V16, V17, V18, V19,
        V20, V21, V22, V23, V24, V25, V26, V27, V28, V29,
        V30, V31, V32, V33, V34, V35, V36, V37, V38, V39,
        V40, V41, V42, V43, V44, V45, V46, V47, V48, V49,
        V50, V51, V52, V53, V54, V55, V56, V57, V58, V59,
        V60, V61, V62, V63, V64
    }

    enum TestMask
    {
        A = 0b0001,
        B = 0b0010,
        C = 0b0100
    }

    private static readonly string[] TestNames = { "A", "B", "C", "D", "E", "F", "G", "H" };

    [Fact]
    public void BaseConstructor_Functions()
    {
        var f = new Bitflag();

        f.SetBit(8);
        f.SetBit(42);

        Assert.True(f[8]);
        Assert.True(f[42]);
        Assert.False(f[39]);
    }

    [Fact]
    public void Constructor_FromUlong_Functions()
    {
        var f = new Bitflag(0xDEADBEEFFEEDDEEDUL);

        Assert.True(f[63]);
        Assert.True(f[62]);
        Assert.False(f[1]);
    }

    [Fact]
    public void Constructor_FromUint_Functions()
    {
        var f = new Bitflag(0xDEADBEEF);

        Assert.True(f[31]);
        Assert.True(f[30]);
        Assert.False(f[4]);
    }

    [Fact]
    public void Constructor_FromUshort_Functions()
    {
        var f = new Bitflag((ushort)0xBEEF);

        Assert.True(f[15]);
        Assert.True(f[13]);
        Assert.False(f[4]);
    }

    [Fact]
    public void Constructor_FromByte_Functions()
    {
        var f = new Bitflag((byte)0xEF);

        Assert.True(f[7]);
        Assert.True(f[6]);
        Assert.False(f[4]);
    }

    [Fact]
    public void Constructor_FromNames_AssignsCorrectBitIndices()
    {
        var f = new Bitflag(TestNames);
        f["A"] = true;
        f["B"] = true;

        Assert.True(f[0]); // A
        Assert.True(f[1]); // B
        Assert.False(f[2]); // C
        Assert.Equal(0b0011UL, f.Value); // A and B set
    }

    [Fact]
    public void Constructor_FromEnum_AssignsCorrectBitIndices()
    {
        var f = new Bitflag(typeof(TestEnum));
        f["X"] = true;
        f["Y"] = true;

        Assert.True(f[0]); // X
        Assert.True(f[1]); // Y
        Assert.False(f[2]); // Z
        Assert.Equal(0b0011UL, f.Value); // X and Y set
    }

    [Fact]
    public void Constructor_FromEnum_AssignsEnumNames()
    {
        var f = Bitflag.FromEnum<TestEnum>();
        f.SetBits(TestEnum.X, TestEnum.Z);
        Assert.True(f[TestEnum.X]);
        Assert.False(f[TestEnum.Y]);
        Assert.True(f[TestEnum.Z]);
    }

    [Fact]
    public void Clone_ProducesIdenticalNamedBitCopy()
    {
        var f = new Bitflag(["Flag"]);
        f["Flag"] = true;
        var clone = f.Clone();
        Assert.True(clone["Flag"]);
        Assert.Equal(f.Value, clone.Value);
    }

    [Fact]
    public void Constructor_FromTypeEnum_SetsNamedBitsCorrectly()
    {
        var flags = new Bitflag(typeof(ConsoleColor));
        flags["Red"] = true;
        Assert.True(flags["Red"]);
        Assert.False(flags["Green"]);
    }

    [Fact]
    public void Constructor_TooManyEnumValues_Throws()
    {
        var enumType = typeof(EnumWithMoreThan64Values);
        Assert.Throws<ArgumentException>(() => new Bitflag(enumType));
    }

    [Fact]
    public void Clone_CreatesDeepCopy()
    {
        var f1 = new Bitflag(["A", "B"]);
        f1["A"] = true;
        var f2 = f1.Clone();

        Assert.True(f2["A"]);
        Assert.False(f2["B"]);
        Assert.NotSame(f1, f2);
        Assert.Equal(f1.Value, f2.Value);
    }

    [Fact]
    public void CloneValues_OnlyCopiesBitfield()
    {
        var f = new Bitflag(["X", "Y"]);
        f["X"] = true;
        var clone = f.CloneValues();

        Assert.Equal(f.Value, clone.Value);
        Assert.Throws<ArgumentException>(() => clone["X"]);
    }

    [Fact]
    public void CloneUnset_CopiesNames_ClearsBits()
    {
        var f = new Bitflag(["Debug", "Trace"]);
        f.SetBits("Debug", "Trace");

        var clean = f.CloneUnset();
        Assert.Equal(0UL, clean.Value);
        Assert.True(clean.TryGetBitmask("Debug", out _));
    }

    [Fact]
    public void SetAndClearBit_ByIndex_WorksCorrectly()
    {
        var f = new Bitflag();
        f.SetBit(4);
        Assert.True(f[4]);
        f.ClearBit(4);
        Assert.False(f[4]);
    }

    [Fact]
    public void ToggleBit_ByIndex_Works()
    {
        var f = new Bitflag();
        f.ToggleBit(1);
        Assert.True(f[1]);
        f.ToggleBit(1);
        Assert.False(f[1]);
    }

    [Fact]
    public void SetBit_ByIndex_SetsCorrectBit()
    {
        var f = new Bitflag();
        f.SetBit(3);
        Assert.True(f[3]);
    }

    [Fact]
    public void SetBits_ByIndices_SetsAll()
    {
        var f = new Bitflag();
        f.SetBits(1, 3, 5);
        Assert.True(f[1]);
        Assert.True(f[3]);
        Assert.True(f[5]);
    }

    [Fact]
    public void ClearBit_ByIndex_ClearsCorrectBit()
    {
        var f = new Bitflag(0b1000UL);
        f.ClearBit(3);
        Assert.False(f[3]);
    }

    [Fact]
    public void ClearBits_ByIndices_ClearsAll()
    {
        var f = new Bitflag();
        f.SetAll();
        f.ClearBits(1, 3, 5);
        Assert.False(f[1]);
        Assert.False(f[3]);
        Assert.False(f[5]);
    }

    [Fact]
    public void ClearBits_ByName_ClearsCorrectBits()
    {
        var f = new Bitflag(["X", "Y"]);
        f.SetBits("X", "Y");
        f.ClearBits("X");
        Assert.False(f["X"]);
        Assert.True(f["Y"]);
    }

    [Fact]
    public void NamedBits_AreResolvedCorrectly()
    {
        var f = new Bitflag(["X", "Y"]);
        f["X"] = true;
        Assert.True(f["X"]);
        Assert.False(f["Y"]);
    }

    [Fact]
    public void SetBits_ByName_SetsCorrectBits()
    {
        var f = new Bitflag(["Jump", "Shoot"]);
        f.SetBits("Jump", "Shoot");
        Assert.True(f["Jump"]);
        Assert.True(f["Shoot"]);
    }

    [Fact]
    public void ToUintConversions_AreCorrect()
    {
        var f = new Bitflag(0xDEADBEEFCAFEBABEUL);
        Assert.Equal(0xBE, f.ToUInt8());
        Assert.Equal(0xBABE, f.ToUInt16());
        Assert.Equal(0xCAFEBABE, f.ToUInt32());
    }

    [Fact]
    public void DefineMask_AddsNamedMaskSuccessfully()
    {
        var f = new Bitflag(["X", "Y", "Z"]);
        f.DefineMask("Combo", 0b0110UL);
        Assert.True(f.ApplyMask("Combo"));
        Assert.True(f[1]); // Y
        Assert.True(f[2]); // Z
    }

    [Fact]
    public void DefineMask_FromEnum_UsesEnumNameAndValue()
    {
        var f = new Bitflag();
        f.DefineMask(TestMask.B);
        Assert.True(f.ApplyMask(TestMask.B));
        Assert.Equal(0b0010UL, f.Value);
    }

    [Fact]
    public void DefineMasks_FromEnum_CreatesMultipleNamedMasks()
    {
        var f = new Bitflag();
        f.DefineMasks<TestMask>();
        f.ApplyMask(new Enum[] { TestMask.A, TestMask.C });
        Assert.Equal(0b0101UL, f.Value);
    }

    [Fact]
    public void ApplyMaskNew_CreatesCloneWithMaskApplied()
    {
        var f = new Bitflag(["A", "B", "C"]);
        f.DefineMask("Two", 0b0110UL);
        var clone = f.ApplyMaskNew("Two");
        Assert.NotNull(clone);
        Assert.True(clone!["B"]);
        Assert.True(clone["C"]);
        Assert.False(f["B"]); // original unchanged
    }

    [Fact]
    public void ClearMaskedBitsNew_RemovesBitsFromClone()
    {
        var f = new Bitflag(["D", "E", "F"]);
        f.SetAll();
        f.DefineMask("DropE", 0b0010UL); // Clear E
        var reduced = f.ClearMaskedBitsNew("DropE");
        Assert.True(f["E"]);
        Assert.False(reduced!["E"]);
    }

    [Fact]
    public void ToggleByMask_FlipsBitsInPlace()
    {
        var f = new Bitflag(["A", "B", "C"]);
        f.DefineMask("FlipBC", 0b0110UL);
        f.ToggleByMask("FlipBC");
        Assert.True(f["B"]);
        Assert.True(f["C"]);
        f.ToggleByMask("FlipBC");
        Assert.False(f["B"]);
        Assert.False(f["C"]);
    }

    [Fact]
    public void ToggleByMaskNew_ReturnsCloneWithToggledBits()
    {
        var f = new Bitflag(["X", "Y"]);
        f.DefineMask("SwapXY", 0b0011UL);
        var toggled = f.ToggleByMaskNew("SwapXY");
        Assert.True(toggled!["X"]);
        Assert.True(toggled["Y"]);
        Assert.False(f["X"]);
        Assert.False(f["Y"]);
    }

    [Fact]
    public void CloneValues_ExcludesNamedBits()
    {
        var f = new Bitflag(["Flag"]);
        f["Flag"] = true;
        var clone = f.CloneValues();
        Assert.Equal(f.Value, clone.Value);
        Assert.Throws<ArgumentException>(() => clone["Flag"]);
    }

    [Fact]
    public void CloneUnset_ReturnsEmptyBitfieldWithNames()
    {
        // Arrange
        var flags = new Bitflag(["A", "B", "C"]);
        flags.SetBits("A", "C");

        // Act
        var clone = flags.CloneUnset();

        // Assert
        Assert.Equal(0UL, clone.Value);                     // All bits should be cleared
        Assert.False(clone["A"]);                           // A was set in original, but not in clone
        Assert.False(clone["B"]);
        Assert.False(clone["C"]);

        // The names should still be valid
        clone["B"] = true;
        Assert.True(clone["B"]);
    }

    [Fact]
    public void WithNamesFrom_InjectsNamesIntoUnnamedInstance()
    {
        var f1 = new Bitflag(["A", "B"]);
        f1.SetBits("A");

        var raw = f1.CloneValues();
        raw.WithNamesFrom(f1);

        Assert.True(raw["A"]);
        Assert.False(raw["B"]);
    }

    [Fact]
    public void CreateMask_ReturnsCorrectBitmask()
    {
        ulong mask = Bitflag.CreateMask(2, 5);
        Assert.Equal(0b00111100UL, mask);
    }

    [Fact]
    public void PopCount_Works()
    {
        var f = new Bitflag(0b1011UL);
        Assert.Equal(3, f.PopCount());
    }

    [Fact]
    public void FirstSetBitIndex_Works()
    {
        var f = new Bitflag(0b1000UL);
        Assert.Equal(3, f.FirstSetBitIndex());
    }

    [Fact]
    public void LastSetBitIndex_Works()
    {
        var f = new Bitflag(0x8000000000000000);
        Assert.Equal(63, f.LastSetBitIndex());
    }

    [Fact]
    public void ToggleBit_ByIndex_TogglesCorrectBit()
    {
        var f = new Bitflag();
        f.ToggleBit(2);
        Assert.True(f[2]);
        f.ToggleBit(2);
        Assert.False(f[2]);
    }

    [Fact]
    public void ToggleBits_ByNames_Works()
    {
        var f = new Bitflag(["X", "Y"]);
        f.ToggleBits("X");
        Assert.True(f["X"]);
        f.ToggleBits("X", "Y");
        Assert.False(f["X"]);
        Assert.True(f["Y"]);
    }

    [Fact]
    public void ExtractMask_ReturnsOnlyMaskedBits()
    {
        var f = new Bitflag(["A", "B", "C"]);
        f.DefineMask("GroupBC", 0b0110UL);
        f.SetBits("B", "C");

        ulong result = f.ExtractMask("GroupBC");
        Assert.Equal(0b0110UL, result);
    }

    [Fact]
    public void ExtractBits_ReturnsRightAlignedValue()
    {
        var f = new Bitflag();
        f.InsertBits(0b1011, 4, 7); // Insert into bits 4-7

        ulong field = f.ExtractBits(4, 7);
        Assert.Equal(0b1011UL, field);
    }

    [Fact]
    public void ExtractBits_InvalidRange_Throws()
    {
        var f = new Bitflag();
        Assert.Throws<ArgumentOutOfRangeException>(() => f.ExtractBits(8, 4));
        Assert.Throws<ArgumentOutOfRangeException>(() => f.ExtractBits(-1, 4));
        Assert.Throws<ArgumentOutOfRangeException>(() => f.ExtractBits(0, 64));
    }

    [Fact]
    public void InsertBits_ReplacesRegionSafely()
    {
        var f = new Bitflag(0xFFFFUL);
        f.InsertBits(0x0, 8, 15); // Clear byte 1

        ulong expected = 0xFFFFUL & ~Bitflag.CreateMask(8, 15); // preserve bits 0–7
        Assert.Equal(expected, f.Value);
    }

    [Fact]
    public void InsertBits_TruncatesInputValue()
    {
        var f = new Bitflag();
        f.InsertBits(0xFFUL, 0, 3); // only 4 bits allowed
        Assert.Equal(0b1111UL, f.Value); // 0xF
    }

    [Fact]
    public void ExtractMask_ThrowsIfMaskNotDefined()
    {
        var f = new Bitflag();
        Assert.Throws<ArgumentException>(() => f.ExtractMask("Nope"));
    }

    [Fact]
    public void DefineMask_DuplicateName_Throws()
    {
        var f = new Bitflag();
        f.DefineMask("Dup", 0xFF);
        Assert.Throws<ArgumentException>(() => f.DefineMask("Dup", 0xF0));
    }

    [Fact]
    public void ToggleBit_ByIndex_TogglesCorrectly()
    {
        var f = new Bitflag();
        f.ToggleBit(3);
        Assert.True(f[3]);
        f.ToggleBit(3);
        Assert.False(f[3]);
    }

    [Fact]
    public void SetBits_ByName_SetsAll()
    {
        var f = new Bitflag(["A", "B", "C"]);
        f.SetBits("A", "C");
        Assert.True(f["A"]);
        Assert.True(f["C"]);
        Assert.False(f["B"]);
    }

    [Fact]
    public void ClearBits_ByName_ClearsAll()
    {
        var f = new Bitflag(["X", "Y", "Z"]);
        f.SetBits("X", "Y", "Z");
        f.ClearBits("X", "Z");
        Assert.False(f["X"]);
        Assert.False(f["Z"]);
        Assert.True(f["Y"]);
    }

    [Fact]
    public void ToggleBits_ByName_TogglesCorrectly()
    {
        var f = new Bitflag(["D", "E"]);
        f.ToggleBits("D", "E");
        Assert.True(f["D"]);
        Assert.True(f["E"]);
        f.ToggleBits("D");
        Assert.False(f["D"]);
        Assert.True(f["E"]);
    }

    [Fact]
    public void GetSetNamedBits_EnumeratesSetFlags()
    {
        var f = new Bitflag(["A", "B", "C"]);
        f.SetBits("A", "C");

        var names = f.GetSetNamedBits().ToArray();
        Assert.Contains("A", names);
        Assert.Contains("C", names);
        Assert.DoesNotContain("B", names);
    }

    [Fact]
    public void EnumerateFlags_ReturnsAllNamesAndStates()
    {
        var f = new Bitflag(["A", "B"]);
        f["A"] = true;

        var list = f.EnumerateFlags().ToList();
        Assert.Contains(list, kv => kv.name == "A" && kv.value);
        Assert.Contains(list, kv => kv.name == "B" && !kv.value);
    }

    [Fact]
    public void BitwiseOperators_WorkAsExpected()
    {
        var a = new Bitflag(0b1010UL);
        var b = new Bitflag(0b1100UL);

        Assert.Equal(0b1110UL, (a | b).Value);
        Assert.Equal(0b1000UL, (a & b).Value);
        Assert.Equal(0b0110UL, (a ^ b).Value);
        Assert.Equal(~a.Value, (~a).Value);
    }

    [Fact]
    public void EqualityOperators_CompareByValueOnly()
    {
        var f1 = new Bitflag(["A", "B"]);
        var f2 = new Bitflag(["X", "Y"]);

        f1.SetBits("A");
        f2.SetBit(0); // same bit

        Assert.True(f1 == f2);
        Assert.False(f1 != f2);
        Assert.True(f1.Equals(f2));
    }
}
