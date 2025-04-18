namespace FlexibleBitflags
{
    public static class Bits
    {
        public const ulong Bit0 = 1UL;
        public const ulong Bit1 = 1UL << 1;
        public const ulong Bit2 = 1UL << 2;
        public const ulong Bit3 = 1UL << 3;
        public const ulong Bit4 = 1UL << 4;
        public const ulong Bit5 = 1UL << 5;
        public const ulong Bit6 = 1UL << 6;
        public const ulong Bit7 = 1UL << 7;
        public const ulong Bit8 = 1UL << 8;
        public const ulong Bit9 = 1UL << 9;
        public const ulong Bit10 = 1UL << 10;
        public const ulong Bit11 = 1UL << 11;
        public const ulong Bit12 = 1UL << 12;
        public const ulong Bit13 = 1UL << 13;
        public const ulong Bit14 = 1UL << 14;
        public const ulong Bit15 = 1UL << 15;
        public const ulong Bit16 = 1UL << 16;
        public const ulong Bit17 = 1UL << 17;
        public const ulong Bit18 = 1UL << 18;
        public const ulong Bit19 = 1UL << 19;
        public const ulong Bit20 = 1UL << 20;
        public const ulong Bit21 = 1UL << 21;
        public const ulong Bit22 = 1UL << 22;
        public const ulong Bit23 = 1UL << 23;
        public const ulong Bit24 = 1UL << 24;
        public const ulong Bit25 = 1UL << 25;
        public const ulong Bit26 = 1UL << 26;
        public const ulong Bit27 = 1UL << 27;
        public const ulong Bit28 = 1UL << 28;
        public const ulong Bit29 = 1UL << 29;
        public const ulong Bit30 = 1UL << 30;
        public const ulong Bit31 = 1UL << 31;
        public const ulong Bit32 = 1UL << 32;
        public const ulong Bit33 = 1UL << 33;
        public const ulong Bit34 = 1UL << 34;
        public const ulong Bit35 = 1UL << 35;
        public const ulong Bit36 = 1UL << 36;
        public const ulong Bit37 = 1UL << 37;
        public const ulong Bit38 = 1UL << 38;
        public const ulong Bit39 = 1UL << 39;
        public const ulong Bit40 = 1UL << 40;
        public const ulong Bit41 = 1UL << 41;
        public const ulong Bit42 = 1UL << 42;
        public const ulong Bit43 = 1UL << 43;
        public const ulong Bit44 = 1UL << 44;
        public const ulong Bit45 = 1UL << 45;
        public const ulong Bit46 = 1UL << 46;
        public const ulong Bit47 = 1UL << 47;
        public const ulong Bit48 = 1UL << 48;
        public const ulong Bit49 = 1UL << 49;
        public const ulong Bit50 = 1UL << 50;
        public const ulong Bit51 = 1UL << 51;
        public const ulong Bit52 = 1UL << 52;
        public const ulong Bit53 = 1UL << 53;
        public const ulong Bit54 = 1UL << 54;
        public const ulong Bit55 = 1UL << 55;
        public const ulong Bit56 = 1UL << 56;
        public const ulong Bit57 = 1UL << 57;
        public const ulong Bit58 = 1UL << 58;
        public const ulong Bit59 = 1UL << 59;
        public const ulong Bit60 = 1UL << 60;
        public const ulong Bit61 = 1UL << 61;
        public const ulong Bit62 = 1UL << 62;
        public const ulong Bit63 = 1UL << 63;

        public const ulong AllBits = ulong.MaxValue;
        public const ulong Upper32Bits = 0xFFFFFFFF00000000;
        public const ulong Lower32Bits = 0x00000000FFFFFFFF;
        public const ulong Upper16BitsHigh = 0xFFFF000000000000;
        public const ulong Lower16BitsHigh = 0x0000FFFF00000000;
        public const ulong Upper16BitsLow = 0x00000000FFFF0000;
        public const ulong Lower16BitsLow = 0x000000000000FFFF;
        public const ulong Upper8BitsHigh = 0xFF00000000000000;
        public const ulong Lower8BitsHigh = 0x00FF000000000000;
        public const ulong Upper8BitsMidHigh = 0x0000FF0000000000;
        public const ulong Lower8BitsMidHigh = 0x000000FF00000000;
        public const ulong Upper8BitsMidLow = 0x00000000FF000000;
        public const ulong Lower8BitsMidLow = 0x0000000000FF0000;
        public const ulong Upper8BitsLow = 0x000000000000FF00;
        public const ulong Lower8BitsLow = 0x00000000000000FF;

        public static readonly ulong[] ByIndex =
        [
            Bit0, Bit1 , Bit2, Bit3, Bit4, Bit5, Bit6, Bit7,
            Bit8, Bit9, Bit10, Bit11, Bit12, Bit13, Bit14, Bit15,
            Bit16, Bit17, Bit18, Bit19, Bit20, Bit21, Bit22, Bit23,
            Bit24, Bit25, Bit26, Bit27, Bit28, Bit29, Bit30, Bit31,
            Bit32, Bit33, Bit34, Bit35, Bit36, Bit37, Bit38, Bit39,
            Bit40, Bit41, Bit42, Bit43, Bit44, Bit45, Bit46, Bit47,
            Bit48, Bit49, Bit50, Bit51, Bit52, Bit53, Bit54, Bit55,
            Bit56, Bit57, Bit58, Bit59, Bit60, Bit61, Bit62, Bit63,
        ];

        /// <summary>
        /// Get the bit value at the specified index
        /// </summary>
        /// <param name="index">the index of the bit to retrieve (0-63)</param>
        /// <returns>The bitmask of the specified bit</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the specified bit index is out of bounds for a 64-bit integer</exception>
        public static ulong GetBit(int index)
        {
            if (index < 0 || index > 63)
                throw new ArgumentOutOfRangeException(nameof(index), "Bitfield Index must be between 0 and 63.");

            return ByIndex[index];
        }
    }
}
