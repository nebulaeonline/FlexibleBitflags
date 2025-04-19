using System;
using System.Numerics;

namespace FlexibleBitflags;

public class Bitflag
{
    private ulong _bitfield = 0UL;
    private Dictionary<string, ulong> _namedBits = new();
    private Dictionary<string, ulong> _namedMasks = new();

    // [Tested]
    /// <summary>
    /// Standard parameterless constructor
    /// </summary>
    public Bitflag() { }

    // [Tested]
    /// <summary>
    /// Constructor that takes a ulong value to set the bitfield
    /// </summary>
    /// <param name="bitfield"></param>
    public Bitflag(ulong bitfield)
    {
        _bitfield = bitfield;
    }

    // [Tested]
    /// <summary>
    /// Constructs a Bitflag from a 32-bit value
    /// </summary>
    /// <param name="value">The uint value to load into the bitfield</param>
    public Bitflag(uint value)
    {
        _bitfield = value;
    }

    // [Tested]
    /// <summary>
    /// Constructs a Bitflag from a 16-bit value
    /// </summary>
    /// <param name="value">The ushort value to load into the bitfield</param>
    public Bitflag(ushort value)
    {
        _bitfield = value;
    }

    // [Tested]
    /// <summary>
    /// Constructs a Bitflag from an 8-bit value
    /// </summary>
    /// <param name="value">The byte value to load into the bitfield</param>
    public Bitflag(byte value)
    {
        _bitfield = value;
    }

    // [Tested]
    /// <summary>
    /// Creates a new instance of the Bitflag class from an enumeration of names; the order of the names will determine the bit index (starts at 0)
    /// </summary>
    /// <param name="names">Names are added in order and will start from bit 0</param>
    /// <exception cref="ArgumentException">throws if there are too many names in the enumerable list of strings passed</exception>
    public Bitflag(IEnumerable<string> names)
    {
        int i = 0;
        foreach (var name in names)
        {
            if (i > 63) throw new ArgumentException("Too many names — max 64 bits.");
            _namedBits[name] = 1UL << i++;
        }
    }

    // [Tested]
    /// <summary>
    /// Constructs a Bitflag from an enum type; the names of the enum values will be used as named bits
    /// </summary>
    /// <param name="enumType">The enum you wish to use to construct bitfields</param>
    /// <exception cref="ArgumentException">If the type passed is not an enum, or if the enum has more than 64 values</exception>
    public Bitflag(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("Type must be an enum", nameof(enumType));

        var values = Enum.GetValues(enumType);
        var names = Enum.GetNames(enumType);

        if (values.Length > 64)
            throw new ArgumentException("Enum has more than 64 values; only 64-bit fields supported.");

        for (int i = 0; i < values.Length; i++)
        {
            string name = names[i];
            object valueObj = values.GetValue(i)!;
            ulong value = Convert.ToUInt64(valueObj);

            if (value > 63)
                throw new ArgumentException($"Enum value {name} = {value} exceeds 63-bit range.");

            _namedBits[name] = 1UL << (int)value;
        }
    }


    // [Tested]
    /// <summary>
    /// Factory method to construct a Bitflag from an enum type; the names of the enum values will be used as named bits
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Bitflag FromEnum<TEnum>() where TEnum : Enum
    {
        var names = Enum.GetNames(typeof(TEnum));
        var values = Enum.GetValues(typeof(TEnum));

        if (names.Length > 64)
            throw new ArgumentException("Enum has more than 64 values; only 64-bit fields supported.");

        var bitflag = new Bitflag();

        for (int i = 0; i < names.Length; i++)
        {
            string name = names[i];
            object valueObj = values.GetValue(i)!;
            ulong value = Convert.ToUInt64(valueObj);

            if (value > 63)
                throw new ArgumentException($"Enum value {name} = {value} exceeds 63-bit range.");

            bitflag._namedBits[name] = 1UL << (int)value;
        }

        return bitflag;
    }


    // [Tested]
    /// <summary>
    /// Creates a full clone of this Bitflag instance, including named bits, named masks,
    /// and bitfield value
    /// </summary>
    /// <returns>A new Bitflag with identical values and named bit mappings</returns>
    public Bitflag Clone()
    {
        var clone = new Bitflag();
        clone._bitfield = _bitfield;
        clone._namedBits = new Dictionary<string, ulong>(_namedBits);
        clone._namedMasks = new Dictionary<string, ulong>(_namedMasks);
        return clone;
    }

    /// <summary>
    /// Creates a clone of this Bitflag instance, including named bits and bitfield value
    /// but excluding any named masks
    /// </summary>
    /// <returns>A new Bitflag with identical values and named bit mappings</returns>
    public Bitflag CloneNoMasks()
    {
        var clone = new Bitflag();
        clone._bitfield = _bitfield;
        clone._namedBits = new Dictionary<string, ulong>(_namedBits);
        return clone;
    }

    /// <summary>
    /// Clones only the bitfield value, omitting named bit mappings
    /// </summary>
    /// <returns>A new Bitflag with the same bitfield value but no named bits</returns>
    public Bitflag CloneValues()
    {
        return new Bitflag(_bitfield);
    }

    /// <summary>
    /// Creates a clone of this Bitflag with all bits cleared but all named bits retained.
    /// </summary>
    /// <returns>A new Bitflag with the same named bits, but all values unset.</returns>
    public Bitflag CloneUnset()
    {
        var clone = new Bitflag();
        clone._bitfield = 0UL;
        clone._namedBits = new Dictionary<string, ulong>(_namedBits);
        return clone;
    }

    /// <summary>
    /// Copies named bit definitions from another Bitflag instance into this one.
    /// Does not modify the current bitfield value.
    /// </summary>
    /// <param name="source">The Bitflag instance to copy names from</param>
    /// <exception cref="ArgumentException">If this instance already has named bits</exception>
    public void WithNamesFrom(Bitflag source)
    {
        if (_namedBits.Count > 0)
            throw new ArgumentException("This Bitflag instance already has named bits defined.");

        foreach (var kvp in source._namedBits)
            _namedBits[kvp.Key] = kvp.Value;
    }

    /// <summary>
    /// The bitfield value, represented as a 64-bit unsigned integer; can be read or set
    /// </summary>
    public ulong Value
    {
        get => _bitfield;
        set => _bitfield = value;
    }

    /// <summary>
    /// This will clear all bits in the bitfield, setting it to 0
    /// </summary>
    public void ClearAll() => _bitfield = 0UL;

    /// <summary>
    /// This will set all bits in the bitfield, setting it to 0xFFFFFFFFFFFFFFFF (ulong.MaxValue)
    /// </summary>
    public void SetAll() => _bitfield = ulong.MaxValue;

    /// <summary>
    /// Attempt to create a named bit; does not throw an exception if the name or bit index has already been used
    /// </summary>
    /// <param name="name">The name that should be used to refer to this bit</param>
    /// <param name="bitfieldIndex">The bit index that this name should correspond to</param>
    /// <returns>true if the named bit was created, false otherwise</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the specified bit index is out of bounds for a 64-bit integer</exception>
    public bool TryNameBit(string name, int bitfieldIndex)
    {
        if (bitfieldIndex < 0 || bitfieldIndex > 63)
            throw new ArgumentOutOfRangeException(nameof(bitfieldIndex), "Bitfield Index must be between 0 and 63.");

        if (_namedBits.ContainsKey(name) || _namedBits.ContainsValue(1UL << bitfieldIndex))
            return false;


        _namedBits.Add(name, 1UL << bitfieldIndex);
        return true;
    }

    /// <summary>
    /// Attempt to create a named bit; does not throw an exception if the name or bit index has already been used
    /// </summary>
    /// <param name="name">The name that should be used to refer to this bit</param>
    /// <param name="bitmask">The mask of the bit that this name should correspond to</param>
    /// <returns>true if the named bit was created, false otherwise</returns>
    public bool TryNameBit(string name, ulong bitmask)
    {
        if (BitOperations.PopCount(bitmask) != 1 ||
            _namedBits.ContainsKey(name) ||
            _namedBits.ContainsValue(bitmask))
            return false;

        _namedBits[name] = bitmask;
        return true;
    }

    /// <summary>
    /// Create a named bit using the index of the bit in the bitfield
    /// </summary>
    /// <param name="name">The name that should be used to refer to this bit</param>
    /// <param name="bitfieldIndex">The bit index that this name should correspond to</param>
    /// <exception cref="ArgumentOutOfRangeException">If the specified bit index is out of bounds for a 64-bit integer</exception>
    /// <exception cref="ArgumentException">If the specified name or bit index has already been used in this instance</exception>
    public void NameBit(string name, int bitfieldIndex)
    {
        if (bitfieldIndex < 0 || bitfieldIndex > 63)
            throw new ArgumentOutOfRangeException(nameof(bitfieldIndex), "Bitfield Index must be between 0 and 63.");

        if (_namedBits.ContainsKey(name) || _namedBits.ContainsValue(1UL << bitfieldIndex))
            throw new ArgumentException($"Bit '{name}' or index {bitfieldIndex} already exists.");

        _namedBits.Add(name, 1UL << bitfieldIndex);
    }

    /// <summary>
    /// Create a named bit using a bitmask
    /// </summary>
    /// <param name="name">The name that should be used to refer to this bit</param>
    /// <param name="bitmask">The mask of the bit that this name should correspond to</param>
    /// <exception cref="ArgumentException">If the bitmask does not represent a single bit or if the name or bitmask already exist for this instance</exception>
    public void NameBit(string name, ulong bitmask)
    {
        if (BitOperations.PopCount(bitmask) != 1)
            throw new ArgumentException("Bitmask must represent a single bit.");

        if (_namedBits.ContainsKey(name) || _namedBits.ContainsValue(bitmask))
            throw new ArgumentException($"Bit '{name}' or bitmask {bitmask} already exists.");

        _namedBits.Add(name, bitmask);
    }

    /// <summary>
    /// Attempt to get the bitmask of a named bit; does not throw an exception if the name does not exist
    /// </summary>
    /// <param name="name">The name of the bit to retrieve</param>
    /// <param name="mask">The out variable for the mask</param>
    /// <returns></returns>
    public bool TryGetBitmask(string name, out ulong mask)
    {
        return _namedBits.TryGetValue(name, out mask);
    }

    /// <summary>
    /// Set a bit in the bitfield using the index of the bit in the bitfield
    /// </summary>
    /// <param name="bitfieldIndex">The bit index that this name should correspond to</param>
    /// <exception cref="ArgumentOutOfRangeException">If the specified bit index is out of bounds for a 64-bit integer</exception>
    public void SetBit(int bitfieldIndex)
    {
        if (bitfieldIndex < 0 || bitfieldIndex > 63)
            throw new ArgumentOutOfRangeException(nameof(bitfieldIndex), "Bitfield Index must be between 0 and 63.");

        _bitfield |= (1UL << bitfieldIndex);
    }

    /// <summary>
    /// Sets bits according to the provided bitmask
    /// </summary>
    /// <param name="bitmask">The mask to be used for setting bits in the bitfield</param>
    public void SetBits(ulong bitmask)
    {
        _bitfield |= bitmask;
    }

    /// <summary>
    /// Sets multiple bits by index
    /// </summary>
    /// <param name="bitIndices">An array of bit indices to set (0–63)</param>
    /// <exception cref="ArgumentOutOfRangeException">If any bit index is out of range</exception>
    public void SetBits(params int[] bitIndices)
    {
        foreach (var i in bitIndices)
        {
            if (i < 0 || i > 63)
                throw new ArgumentOutOfRangeException(nameof(bitIndices), $"Bit index {i} is out of range (0–63).");

            _bitfield |= 1UL << i;
        }
    }

    /// <summary>
    /// Sets multiple bits by name
    /// </summary>
    /// <param name="names">An array of named bits to set</param>
    /// <exception cref="ArgumentException">If any name is not defined</exception>
    public void SetBits(params string[] names)
    {
        foreach (var name in names)
        {
            if (!_namedBits.TryGetValue(name, out var mask))
                throw new ArgumentException($"Bit name '{name}' is not defined.");

            _bitfield |= mask;
        }
    }

    /// <summary>
    /// Can be used to set multiple bits based on an enum array
    /// </summary>
    /// <typeparam name="TEnum">The enum type</typeparam>
    /// <param name="enumValues">An array of enum members for bits that need to be set</param>
    /// <exception cref="ArgumentException">If any name is not defined</exception>
    public void SetBits<TEnum>(params TEnum[] enumValues) where TEnum : Enum
    {
        foreach (var e in enumValues)
        {   
            if (!_namedBits.TryGetValue(e.ToString(), out var mask))
                throw new ArgumentException($"Bit name '{e}' is not defined.");

            this[e.ToString()] = true;
        }
    }

    /// <summary>
    /// Sets multiple bits using a sequence of named bit keys.
    /// </summary>
    /// <param name="names">An enumerable of bit names to set.</param>
    /// <exception cref="ArgumentException">If any name is not defined in the bitflag.</exception>
    public void SetBits(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            if (!_namedBits.TryGetValue(name, out var mask))
                throw new ArgumentException($"Bit name '{name}' is not defined.");

            _bitfield |= mask;
        }
    }

    /// <summary>
    /// Sets multiple bits using a sequence of enum values. Each enum member's name is used to resolve the bit to set.
    /// </summary>
    /// <param name="enums">An enumerable of enum values to set.</param>
    /// <exception cref="ArgumentException">If any name is not defined in the bitflag.</exception>
    public void SetBits(IEnumerable<Enum> enums)
    {
        foreach (var e in enums)
        {
            string name = e.ToString();
            if (!_namedBits.TryGetValue(name, out var mask))
                throw new ArgumentException($"Bit name '{name}' is not defined.");

            _bitfield |= mask;
        }
    }

    /// <summary>
    /// Clears a bit in the bitfield using the index of the bit in the bitfield
    /// </summary>
    /// <param name="bitfieldIndex">The bit index that this name should correspond to</param>
    /// <exception cref="ArgumentOutOfRangeException">If the specified bit index is out of bounds for a 64-bit integer</exception>
    public void ClearBit(int bitfieldIndex)
    {
        if (bitfieldIndex < 0 || bitfieldIndex > 63)
            throw new ArgumentOutOfRangeException(nameof(bitfieldIndex), "Bitfield Index must be between 0 and 63.");

        _bitfield &= ~(1UL << bitfieldIndex);
    }

    /// <summary>
    /// Clears a single bit using its name.
    /// </summary>
    /// <param name="name">The name of the bit to clear.</param>
    /// <exception cref="ArgumentException">If the name is not defined in the bitflag.</exception>
    public void ClearBit(string name)
    {
        if (!_namedBits.TryGetValue(name, out var mask))
            throw new ArgumentException($"Bit name '{name}' is not defined.");

        _bitfield &= ~mask;
    }

    /// <summary>
    /// Clears bits according to the provided bitmask
    /// </summary>
    /// <param name="bitmask">The mask to be used for clearing bits in the bitfield</param>
    public void ClearBits(ulong bitmask)
    {
        _bitfield &= ~bitmask;
    }

    /// <summary>
    /// Clears multiple bits by index
    /// </summary>
    /// <param name="bitIndices">An array of bit indices to clear (0–63)</param>
    /// <exception cref="ArgumentOutOfRangeException">If any bit index is out of range</exception>
    public void ClearBits(params int[] bitIndices)
    {
        foreach (var i in bitIndices)
        {
            if (i < 0 || i > 63)
                throw new ArgumentOutOfRangeException(nameof(bitIndices), $"Bit index {i} is out of range (0–63).");

            _bitfield &= ~(1UL << i);
        }
    }

    /// <summary>
    /// Clears multiple bits by name.
    /// </summary>
    /// <param name="names">An array of named bits to clear.</param>
    /// <exception cref="ArgumentException">If any name is not defined in the bitflag.</exception>
    public void ClearBits(params string[] names)
    {
        foreach (var name in names)
        {
            if (!_namedBits.TryGetValue(name, out var mask))
                throw new ArgumentException($"Bit name '{name}' is not defined.");

            _bitfield &= ~mask;
        }
    }

    /// <summary>
    /// Clears multiple bits using a sequence of named bit keys.
    /// </summary>
    /// <param name="names">An enumerable of bit names to clear.</param>
    /// <exception cref="ArgumentException">If any name is not defined in the bitflag.</exception>
    public void ClearBits(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            if (!_namedBits.TryGetValue(name, out var mask))
                throw new ArgumentException($"Bit name '{name}' is not defined.");

            _bitfield &= ~mask;
        }
    }

    /// <summary>
    /// Clears multiple bits using a sequence of enum values. Each enum member's name is used to resolve the bit to clear.
    /// </summary>
    /// <param name="enums">An enumerable of enum values to clear.</param>
    /// <exception cref="ArgumentException">If any name is not defined in the bitflag.</exception>
    public void ClearBits(IEnumerable<Enum> enums)
    {
        foreach (var e in enums)
        {
            string name = e.ToString();
            if (!_namedBits.TryGetValue(name, out var mask))
                throw new ArgumentException($"Bit name '{name}' is not defined.");

            _bitfield &= ~mask;
        }
    }

    /// <summary>
    /// Can be used to clear multiple bits based on an enum array
    /// </summary>
    /// <typeparam name="TEnum">The enum type</typeparam>
    /// <param name="enumValues">An array of enum members for bits that need to be cleared</param>
    /// <exception cref="ArgumentException">If any name is not defined</exception>
    public void ClearBits<TEnum>(params TEnum[] enumValues) where TEnum : Enum
    {
        foreach (var e in enumValues)
        {
            if (!_namedBits.TryGetValue(e.ToString(), out var mask))
                throw new ArgumentException($"Bit name '{e}' is not defined.");

            this[e.ToString()] = false;
        }
    }

    /// <summary>
    /// Toggles multiple bits using a sequence of named bit keys.
    /// </summary>
    /// <param name="names">An enumerable of bit names to toggle.</param>
    /// <exception cref="ArgumentException">If any name is not defined in the bitflag.</exception>
    public void ToggleBits(IEnumerable<string> names)
{
    foreach (var name in names)
    {
        if (!_namedBits.TryGetValue(name, out var mask))
            throw new ArgumentException($"Bit name '{name}' is not defined.");

        _bitfield ^= mask;
    }
}
    /// <summary>
    /// Toggles a single bit by index
    /// </summary>
    /// <param name="bitfieldIndex">The bit index to toggle</param>
    /// <exception cref="ArgumentOutOfRangeException">If index is out of range (0–63)</exception>
    public void ToggleBit(int bitfieldIndex)
    {
        if (bitfieldIndex < 0 || bitfieldIndex > 63)
            throw new ArgumentOutOfRangeException(nameof(bitfieldIndex), "Bitfield Index must be between 0 and 63.");

        _bitfield ^= 1UL << bitfieldIndex;
    }

    /// <summary>
    /// Toggles bits based on a bitmask
    /// </summary>
    /// <param name="bitmask">Bitmask of bits to toggle</param>
    public void ToggleBits(ulong bitmask)
    {
        _bitfield ^= bitmask;
    }

    /// <summary>
    /// Toggles multiple bits by index
    /// </summary>
    /// <param name="bitIndices">Array of indices (0–63)</param>
    /// <exception cref="ArgumentOutOfRangeException">If any index is out of range</exception>
    public void ToggleBits(params int[] bitIndices)
    {
        foreach (var i in bitIndices)
        {
            if (i < 0 || i > 63)
                throw new ArgumentOutOfRangeException(nameof(bitIndices), $"Bit index {i} is out of range (0–63).");

            _bitfield ^= 1UL << i;
        }
    }

    /// <summary>
    /// Toggles multiple bits by name
    /// </summary>
    /// <param name="names">Array of named bits</param>
    /// <exception cref="ArgumentException">If any name is undefined</exception>
    public void ToggleBits(params string[] names)
    {
        foreach (var name in names)
        {
            if (!_namedBits.TryGetValue(name, out var mask))
                throw new ArgumentException($"Bit name '{name}' is not defined.");

            _bitfield ^= mask;
        }
    }

    /// <summary>
    /// Toggles multiple bits using an array of enum values. Each enum member's name is used to resolve the bit to toggle.
    /// </summary>
    /// <typeparam name="TEnum">The enum type used for the bit names.</typeparam>
    /// <param name="enumValues">An array of enum members to toggle.</param>
    /// <exception cref="ArgumentException">If any name is not defined in the current instance.</exception>
    public void ToggleBits<TEnum>(params TEnum[] enumValues) where TEnum : Enum
    {
        foreach (var e in enumValues)
        {
            string name = e.ToString();
            if (!_namedBits.TryGetValue(name, out var mask))
                throw new ArgumentException($"Bit name '{name}' is not defined.");

            _bitfield ^= mask;
        }
    }

    /// <summary>
    /// Toggles multiple bits using an array of enum values. Each enum member's name is used to resolve the bit to toggle.
    /// </summary>
    /// <param name="enumValues">An array of enum members to toggle.</param>
    /// <exception cref="ArgumentException">If any name is not defined in the current instance.</exception>
    public void ToggleBits(params Enum[] enumValues)
    {
        foreach (var e in enumValues)
        {
            string name = e.ToString();
            if (!_namedBits.TryGetValue(name, out var mask))
                throw new ArgumentException($"Bit name '{name}' is not defined.");

            _bitfield ^= mask;
        }
    }

    /// <summary>
    /// Checks if a bit is set in the bitfield using the name of the bit in the bitfield
    /// </summary>
    /// <param name="name">The name set for the given bit</param>
    /// <returns>true if the bit is set, false otherwise</returns>
    /// <exception cref="ArgumentException">If the name provided has not been set for this bitfield</exception>
    public bool this[string name]
    {
        get
        {
            if (_namedBits.TryGetValue(name, out ulong bitmask))
            {
                return (_bitfield & bitmask) != 0;
            }
            else
            {
                throw new ArgumentException($"No bit named '{name}' found.");
            }
        }

        set
        {
            if (_namedBits.TryGetValue(name, out ulong bitmask))
            {
                if (value)
                {
                    _bitfield |= bitmask;
                }
                else
                {
                    _bitfield &= ~bitmask;
                }
            }
            else
            {
                throw new ArgumentException($"No bit named '{name}' found.");
            }
        }
    }

    /// <summary>
    /// Checks if a bit is set in the bitfield using the index of the bit in the bitfield
    /// </summary>
    /// <param name="bitfieldIndex">The bit index to check whether it is set or not</param>
    /// <returns>true if the bit is set, false otherwise</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the specified bit index is out of bounds for a 64-bit integer</exception>
    public bool this[int bitfieldIndex]
    {
        get
        {
            if (bitfieldIndex < 0 || bitfieldIndex > 63)
                throw new ArgumentOutOfRangeException(nameof(bitfieldIndex), "Bitfield Index must be between 0 and 63.");

            return (_bitfield & (1UL << bitfieldIndex)) != 0;
        }
    }

    /// <summary>
    /// Checks if a bit is set in the bitfield using a bitmask
    /// </summary>
    /// <param name="bitmask">The bitmask of the given bit to check whether it is set or not</param>
    /// <returns>true if the bit is set, false otherwise</returns>
    /// <exception cref="ArgumentException">If the specified mask represents more than a single bit</exception>
    public bool this[ulong bitmask]
    {
        get
        {
            if (BitOperations.PopCount(bitmask) != 1)
                throw new ArgumentException("Bitmask must represent a single bit.");

            return (_bitfield & bitmask) != 0;
        }
    }

    /// <summary>
    /// If keys correspond to an enum, you can use the enum directly to access the bitfield value
    /// </summary>
    /// <param name="key">The enum member to use as the index into the bitfield</param>
    /// <returns>true or false depending upon whether the requested bit is set or not</returns>
    /// <exception>This will throw if the name of the enum member does not exist in this bitfield</exception>
    public bool this[Enum key]
    {
        get => this[key.ToString()];
        set => this[key.ToString()] = value;
    }

    /// <summary>
    /// Generic get that allows passing an enum value to get the bitfield value. Can be used
    /// when enums might share names but are not the same type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type</typeparam>
    /// <param name="key">The enum member to be used as a key into the bitfield</param>
    /// <returns>the true/false value of the bitfield value as the bit index specified by the enum member's name</returns>
    /// <exception cref="ArgumentException">throws if the enum member name does not exist in this bitflag</exception>
    public bool Get<TEnum>(TEnum key) where TEnum : Enum
    {
        string name = key.ToString();
        if (!this._namedBits.ContainsKey(name))
            throw new ArgumentException($"No bit named '{name}' found.");
        
        return this[name];
    }

    /// <summary>
    /// Generic set that allows passing an enum value to set the bitfield value. Can be used
    /// when enums might share names but are not the same type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type</typeparam>
    /// <param name="key">The enum member to be used as a key into the bitflag</param>
    /// <param name="value">The value to which to set the bitflag</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">throws if the enum member name does not exist in this bitflag</exception>
    public void Set<TEnum>(TEnum key, bool value) where TEnum : Enum
    {
        string name = key.ToString();
        if (!this._namedBits.ContainsKey(name))
            throw new ArgumentException($"No bit named '{name}' found.");

        this[name] = value;
    }

    /// <summary>
    /// Allows to enumerate all the named bits that are set in the bitfield
    /// </summary>
    /// <returns>IEnumerable&lt;string&gt;</returns>
    public IEnumerable<string> GetSetNamedBits()
    {
        foreach (var kvp in _namedBits)
        {
            if ((_bitfield & kvp.Value) != 0)
                yield return kvp.Key;
        }
    }

    /// <summary>
    /// Allows to enumerate all the named bits that are set in the bitfield
    /// </summary>
    /// <returns>IEnumerable&lt;(string, bool)&gt;</returns>
    public IEnumerable<(string name, bool value)> EnumerateFlags()
    {
        foreach (var kv in _namedBits)
            yield return (kv.Key, (_bitfield & kv.Value) != 0);
    }

    /// <summary>
    /// Returns the number of bits that are set in the bitfield
    /// </summary>
    /// <returns>the number of bits that are set in the bitfield</returns>
    public int PopCount()
    {
        return BitOperations.PopCount(_bitfield);
    }

    /// <summary>
    /// Creates a mask for a range of bits in the bitfield
    /// </summary>
    /// <param name="startBit">the first bit to set in the mast</param>
    /// <param name="endBit">the last bit to set in the mask</param>
    /// <returns>the requested bitmask as a ulong</returns>
    /// <exception cref="ArgumentOutOfRangeException">if either of the bits are outisde the range for a 64-bit integer</exception>
    public static ulong CreateMask(int startBit, int endBit)
    {
        if (startBit < 0 || endBit > 63 || startBit > endBit)
            throw new ArgumentOutOfRangeException("Invalid bit range");

        int length = endBit - startBit + 1;
        return ((1UL << length) - 1) << startBit;
    }

    /// <summary>
    /// Allows to create a new instance from the clone of an existing instance, but with a mask applied
    /// </summary>
    /// <param name="mask">The ulong bitmask to apply</param>
    /// <returns>a new Bitflag with the mask applied to the original</returns>
    public Bitflag CloneWithMask(ulong mask)
    {
        var clone = new Bitflag();
        clone._bitfield = _bitfield & mask;
        clone._namedBits = new Dictionary<string, ulong>(_namedBits);
        return clone;
    }

    /// <summary>
    /// Allows to create a new instance from the clone of an existing instance, but with a mask applied
    /// </summary>
    /// <param name="mask">The bitmask to apply as a bitfield</param>
    /// <returns>a new Bitflag with the mask applied to the original</returns>
    public Bitflag CloneWithMask(Bitflag mask)
    {
        var clone = new Bitflag();
        clone._bitfield = _bitfield & mask.Value;
        clone._namedBits = new Dictionary<string, ulong>(_namedBits);
        return clone;
    }

    /// <summary>
    /// Returns the lower 8 bits of the bitfield as a byte
    /// </summary>
    public byte ToUInt8()
    {
        return (byte)(_bitfield & 0xFF);
    }

    /// <summary>
    /// Returns the lower 16 bits of the bitfield as a ushort
    /// </summary>
    public ushort ToUInt16()
    {
        return (ushort)(_bitfield & 0xFFFF);
    }

    /// <summary>
    /// Returns the lower 32 bits of the bitfield as a uint
    /// </summary>
    public uint ToUInt32()
    {
        return (uint)(_bitfield & 0xFFFFFFFF);
    }

    /// <summary>
    /// Loads the bitfield from a byte (8 bits)
    /// </summary>
    /// <param name="value">The 8-bit value to load</param>
    public void FromUInt8(byte value)
    {
        _bitfield = value;
    }

    /// <summary>
    /// Loads the bitfield from a ushort (16 bits)
    /// </summary>
    /// <param name="value">The 16-bit value to load</param>
    public void FromUInt16(ushort value)
    {
        _bitfield = value;
    }

    /// <summary>
    /// Loads the bitfield from a uint (32 bits)
    /// </summary>
    /// <param name="value">The 32-bit value to load</param>
    public void FromUInt32(uint value)
    {
        _bitfield = value;
    }

    /// <summary>
    /// Checks if a given Bitflag is a power of two
    /// </summary>
    /// <returns>whether or not the underlying bitfield is a power of two</returns>
    public bool IsPowerOfTwo()
    {
        return (_bitfield != 0 && (_bitfield & (_bitfield - 1)) == 0);
    }

    /// <summary>
    /// Returns the first set index in the bitfield
    /// </summary>
    /// <returns>the first set index in the bitfield</returns>
    public int FirstSetBitIndex()
    {
        return BitOperations.TrailingZeroCount(_bitfield);
    }

    /// <summary>
    /// Returns the last set index in the bitfield
    /// </summary>
    /// <returns>the last set index in the bitfield</returns>
    public int LastSetBitIndex()
    {
        return 63 - BitOperations.LeadingZeroCount(_bitfield);
    }

    /// <summary>
    /// Performs a bitwise OR operation between two <see cref="Bitflag"/> instances,
    /// returning a new instance with the combined bitfield values. Named bits and masks are not merged.
    /// </summary>
    /// <param name="a">The first <see cref="Bitflag"/> operand.</param>
    /// <param name="b">The second <see cref="Bitflag"/> operand.</param>
    /// <returns>A new <see cref="Bitflag"/> whose value is the bitwise OR of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static Bitflag operator |(Bitflag a, Bitflag b)
    {
        return new Bitflag(a._bitfield | b._bitfield);
    }

    /// <summary>
    /// Performs a bitwise AND operation between two <see cref="Bitflag"/> instances,
    /// returning a new instance containing only the bits that are set in both operands.
    /// Named bits and masks are not merged.
    /// </summary>
    /// <param name="a">The first <see cref="Bitflag"/> operand.</param>
    /// <param name="b">The second <see cref="Bitflag"/> operand.</param>
    /// <returns>A new <see cref="Bitflag"/> representing the bitwise AND of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static Bitflag operator &(Bitflag a, Bitflag b)
    {
        return new Bitflag(a._bitfield & b._bitfield);
    }

    /// <summary>
    /// Performs a bitwise exclusive OR (XOR) operation between two <see cref="Bitflag"/> instances,
    /// returning a new instance with bits set only where the corresponding bits in the operands differ.
    /// Named bits and masks are not merged.
    /// </summary>
    /// <param name="a">The first <see cref="Bitflag"/> operand.</param>
    /// <param name="b">The second <see cref="Bitflag"/> operand.</param>
    /// <returns>A new <see cref="Bitflag"/> representing the bitwise XOR of <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static Bitflag operator ^(Bitflag a, Bitflag b)
    {
        return new Bitflag(a._bitfield ^ b._bitfield);
    }

    /// <summary>
    /// Performs a bitwise NOT operation on a <see cref="Bitflag"/> instance,
    /// returning a new instance with all bits inverted.
    /// Named bits and masks are preserved but not modified or removed.
    /// </summary>
    /// <param name="a">The <see cref="Bitflag"/> operand to invert.</param>
    /// <returns>A new <see cref="Bitflag"/> with all bits flipped from the original.</returns>
    public static Bitflag operator ~(Bitflag a)
    {
        return new Bitflag(~a._bitfield);
    }

    /// <summary>
    /// Determines whether two <see cref="Bitflag"/> instances are not equal by comparing their bitfield values.
    /// Named bits and masks are not considered in the comparison.
    /// </summary>
    /// <param name="a">The first <see cref="Bitflag"/> to compare.</param>
    /// <param name="b">The second <see cref="Bitflag"/> to compare.</param>
    /// <returns><c>true</c> if the bitfield values are different; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Bitflag a, Bitflag b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a._bitfield == b._bitfield;
    }

    /// <summary>
    /// Determines whether two <see cref="Bitflag"/> instances are not equal by comparing their bitfield values.
    /// Named bits and masks are not considered in the comparison.
    /// </summary>
    /// <param name="a">The first <see cref="Bitflag"/> to compare.</param>
    /// <param name="b">The second <see cref="Bitflag"/> to compare.</param>
    /// <returns><c>true</c> if the bitfield values are different; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Bitflag a, Bitflag b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Determines whether the specified object is a <see cref="Bitflag"/> and has the same bitfield value as this instance.
    /// Named bits and masks are not considered in the comparison.
    /// </summary>
    /// <param name="obj">The object to compare with the current <see cref="Bitflag"/>.</param>
    /// <returns><c>true</c> if the object is a <see cref="Bitflag"/> with the same bitfield value; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is Bitflag other && this == other;
    }

    /// <summary>
    /// Returns a hash code for the current <see cref="Bitflag"/> instance based on its bitfield value.
    /// Named bits and masks are not included in the hash computation.
    /// </summary>
    /// <returns>An integer hash code representing the current bitfield value.</returns>
    public override int GetHashCode()
    {
        return _bitfield.GetHashCode();
    }

    public static Bitflag operator |(Bitflag a, ulong mask) => new Bitflag(a._bitfield | mask) { _namedBits = new Dictionary<string, ulong>(a._namedBits) };

    /// <summary>
    /// Defines a named bitmask that can be applied, cleared, or toggled as a group.
    /// </summary>
    /// <param name="name">The unique name for the mask</param>
    /// <param name="mask">The bitmask value</param>
    /// <exception cref="ArgumentException">If the name already exists in the mask dictionary</exception>
    public void DefineMask(string name, ulong mask)
    {
        if (_namedMasks.ContainsKey(name))
            throw new ArgumentException($"Mask '{name}' is already defined.");

        _namedMasks[name] = mask;
    }

    /// <summary>
    /// Defines a named mask using an enum member's name and its associated value as the mask.
    /// </summary>
    /// <typeparam name="TEnum">The enum type</typeparam>
    /// <param name="enumValue">The enum member whose value will be used as the mask</param>
    /// <exception cref="ArgumentException">If the mask name already exists or value is not castable to ulong</exception>
    public void DefineMask<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        string name = enumValue.ToString();
        ulong mask = Convert.ToUInt64(enumValue);

        if (_namedMasks.ContainsKey(name))
            throw new ArgumentException($"Mask '{name}' is already defined.");

        _namedMasks[name] = mask;
    }

    /// <summary>
    /// Defines named masks from every member of a given enum type, using the enum name as the mask name
    /// and the enum value as the mask.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to read</typeparam>
    /// <exception cref="ArgumentException">If any duplicate names are found</exception>
    public void DefineMasks<TEnum>() where TEnum : Enum
    {
        var names = Enum.GetNames(typeof(TEnum));
        var values = Enum.GetValues(typeof(TEnum));

        for (int i = 0; i < names.Length; i++)
        {
            string name = names[i];
            ulong mask = Convert.ToUInt64(values.GetValue(i)!);

            if (_namedMasks.ContainsKey(name))
                throw new ArgumentException($"Mask '{name}' is already defined.");

            _namedMasks[name] = mask;
        }
    }

    /// <summary>
    /// Returns a new Bitflag with the named mask applied via bitwise OR.
    /// The original Bitflag is not modified.
    /// </summary>
    /// <param name="name">The name of the mask to apply</param>
    /// <returns>A new Bitflag with the mask applied, or null if the mask name doesn't exist</returns>
    public Bitflag? ApplyMaskNew(string name)
    {
        if (!_namedMasks.TryGetValue(name, out var mask))
            return null;

        var clone = this.Clone();
        clone._bitfield |= mask;
        return clone;
    }

    /// <summary>
    /// Returns a new Bitflag with the enum-named mask applied.
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    /// <param name="enumValue">Enum member whose name matches a defined mask</param>
    /// <returns>A new Bitflag with the mask applied, or null if not found</returns>
    public Bitflag? ApplyMaskNew<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        return ApplyMaskNew(enumValue.ToString());
    }

    /// <summary>
    /// Returns a new Bitflag with a named mask applied using a non-generic enum value, matching the name of the enum member.
    /// </summary>
    /// <param name="enumValue">An enum member whose name matches a defined mask</param>
    /// <returns>A new Bitflag with the mask applied, or null if not found</returns>
    public Bitflag? ApplyMaskNew(Enum enumValue)
    {
        return ApplyMaskNew(enumValue.ToString());
    }

    /// <summary>
    /// Returns a new Bitflag with all named masks from the provided sequence applied.
    /// </summary>
    /// <param name="names">An enumerable of mask names to apply</param>
    /// <returns>A new Bitflag with all matching masks OR'd in; skips unknown masks silently</returns>
    public Bitflag ApplyMaskNew(IEnumerable<string> names)
    {
        var clone = this.Clone();

        foreach (var name in names)
        {
            if (_namedMasks.TryGetValue(name, out var mask))
                clone._bitfield |= mask;
        }

        return clone;
    }

    /// <summary>
    /// Returns a new Bitflag with all named masks corresponding to the given enum values applied.
    /// Each enum's name is used to look up a previously defined mask.
    /// </summary>
    /// <param name="enums">A sequence of enum values whose names match defined masks</param>
    /// <returns>A new Bitflag with the matching masks OR'd into a new clone</returns>
    public Bitflag ApplyMaskNew(IEnumerable<Enum> enums)
    {
        var clone = this.Clone();

        foreach (var e in enums)
        {
            string name = e.ToString();
            if (_namedMasks.TryGetValue(name, out var mask))
                clone._bitfield |= mask;
        }

        return clone;
    }

    /// <summary>
    /// Applies a named mask to the current bitfield.
    /// </summary>
    /// <param name="name">The name of the mask to apply</param>
    /// <returns>true if the mask was applied, false if the mask could not be found</returns>
    public bool ApplyMask(string name)
    {
        if (!_namedMasks.TryGetValue(name, out var mask))
            return false;

        _bitfield |= mask;
        return true;
    }

    /// <summary>
    /// Applies a named mask to the current bitfield using an enum value as the mask name
    /// </summary>
    /// <typeparam name="TEnum">The type of enum to use</typeparam>
    /// <param name="enumValue">The enum that is named the same as a mask value to apply</param>
    /// <returns>true if the mask was applied, false if the mask could not be found</returns>
    public bool ApplyMask<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        return ApplyMask(enumValue.ToString());
    }

    /// <summary>
    /// Applies a named mask to the current bitfield using a non-generic enum value as the mask name
    /// </summary>
    /// <param name="enumValue">The non-generic enum member to use as the mask name</param>
    /// <returns>true if the mask was applied, false if the mask could not be found</returns>
    public bool ApplyMask(Enum enumValue)
    {
        return ApplyMask(enumValue.ToString());
    }

    /// <summary>
    /// Applies all named masks from the provided sequence to the current bitfield.
    /// </summary>
    /// <param name="names">An IEnumerable<string> with names of the bitmasks to apply</string></param>
    public void ApplyMask(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            if (_namedMasks.TryGetValue(name, out var mask))
                _bitfield |= mask;
        }
    }

    /// <summary>
    /// Applies all named masks corresponding to the given enum values to the current bitfield.
    /// </summary>
    /// <param name="enums">An IEnumerable<Enum> with members whose names are the names of the bitmasks to apply</Enum></param>
    public void ApplyMask(IEnumerable<Enum> enums)
    {
        foreach (var e in enums)
        {
            string name = e.ToString();
            if (_namedMasks.TryGetValue(name, out var mask))
                _bitfield |= mask;
        }
    }
    
    public Bitflag? ClearMaskedBitsNew(string name)
    {
        if (!_namedMasks.TryGetValue(name, out var mask))
            return null;

        var clone = this.Clone();
        clone._bitfield &= ~mask;
        return clone;
    }

    /// <summary>
    /// Returns a new <see cref="Bitflag"/> with all bits defined by the named mask corresponding to the given enum member cleared.
    /// The original instance is not modified.
    /// </summary>
    /// <typeparam name="TEnum">The enum type used to resolve the named mask.</typeparam>
    /// <param name="enumValue">An enum member whose name corresponds to a defined mask.</param>
    /// <returns>
    /// A new <see cref="Bitflag"/> instance with the specified mask cleared,
    /// or <c>null</c> if the named mask does not exist.
    /// </returns>
    public Bitflag? ClearMaskedBitsNew<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        return ClearMaskedBitsNew(enumValue.ToString());
    }

    /// <summary>
    /// Returns a new <see cref="Bitflag"/> with all bits defined by the named mask corresponding to the given enum member cleared.
    /// The mask is resolved using the name of the enum member.
    /// The original instance is not modified.
    /// </summary>
    /// <param name="enumValue">An enum member whose name corresponds to a defined mask.</param>
    /// <returns>
    /// A new <see cref="Bitflag"/> instance with the specified mask cleared,
    /// or <c>null</c> if the named mask does not exist.
    /// </returns>
    public Bitflag? ClearMaskedBitsNew(Enum enumValue)
    {
        return ClearMaskedBitsNew(enumValue.ToString());
    }

    /// <summary>
    /// Returns a new <see cref="Bitflag"/> with all bits cleared that are defined by the named masks in the provided sequence.
    /// The original instance is not modified.
    /// </summary>
    /// <param name="names">A sequence of mask names to clear from the bitfield.</param>
    /// <returns>
    /// A new <see cref="Bitflag"/> instance with the specified masks cleared. 
    /// Any names that do not match a defined mask are silently ignored.
    /// </returns>
    public Bitflag ClearMaskedBitsNew(IEnumerable<string> names)
    {
        var clone = this.Clone();
        foreach (var name in names)
        {
            if (_namedMasks.TryGetValue(name, out var mask))
                clone._bitfield &= ~mask;
        }
        return clone;
    }

    /// <summary>
    /// Returns a new <see cref="Bitflag"/> with all bits cleared that are defined by the named masks corresponding to the provided enum members.
    /// Each enum member's name is used to resolve a defined mask. The original instance is not modified.
    /// </summary>
    /// <param name="enums">A sequence of enum members whose names correspond to defined masks.</param>
    /// <returns>
    /// A new <see cref="Bitflag"/> instance with the specified masks cleared.
    /// Any enum names that do not match a defined mask are silently ignored.
    /// </returns>
    public Bitflag ClearMaskedBitsNew(IEnumerable<Enum> enums)
    {
        var clone = this.Clone();
        foreach (var e in enums)
        {
            string name = e.ToString();
            if (_namedMasks.TryGetValue(name, out var mask))
                clone._bitfield &= ~mask;
        }
        return clone;
    }

    /// <summary>
    /// Clears all bits in the current bitfield that are defined by the named mask.
    /// </summary>
    /// <param name="name">The name of the mask to clear.</param>
    /// <returns><c>true</c> if the mask was found and applied; otherwise, <c>false</c>.</returns>
    public bool ClearMaskedBits(string name)
    {
        if (!_namedMasks.TryGetValue(name, out var mask))
            return false;

        _bitfield &= ~mask;
        return true;
    }

    /// <summary>
    /// Clears all bits in the current bitfield that are defined by the named mask corresponding to the given enum member.
    /// </summary>
    /// <typeparam name="TEnum">The enum type used to resolve the mask name.</typeparam>
    /// <param name="enumValue">The enum member whose name corresponds to a defined mask.</param>
    /// <returns><c>true</c> if the mask was found and applied; otherwise, <c>false</c>.</returns>
    public bool ClearMaskedBits<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        return ClearMaskedBits(enumValue.ToString());
    }

    /// <summary>
    /// Clears all bits in the current bitfield that are defined by the named mask corresponding to the given enum member.
    /// </summary>
    /// <param name="enumValue">An enum member whose name corresponds to a defined mask.</param>
    /// <returns><c>true</c> if the mask was found and applied; otherwise, <c>false</c>.</returns>
    public bool ClearMaskedBits(Enum enumValue)
    {
        return ClearMaskedBits(enumValue.ToString());
    }

    /// <summary>
    /// Clears all bits in the current bitfield that are defined by the named masks in the provided sequence.
    /// Any mask names that do not match a defined mask are silently ignored.
    /// </summary>
    /// <param name="names">A sequence of mask names to clear from the bitfield.</param>
    public void ClearMaskedBits(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            if (_namedMasks.TryGetValue(name, out var mask))
                _bitfield &= ~mask;
        }
    }

    /// <summary>
    /// Clears all bits in the current bitfield that are defined by the named masks corresponding to the provided enum members.
    /// Each enum member's name is used to resolve a defined mask. Unmatched enum names are silently ignored.
    /// </summary>
    /// <param name="enums">A sequence of enum members whose names correspond to defined masks.</param>
    public void ClearMaskedBits(IEnumerable<Enum> enums)
    {
        foreach (var e in enums)
        {
            string name = e.ToString();
            if (_namedMasks.TryGetValue(name, out var mask))
                _bitfield &= ~mask;
        }
    }

    /// <summary>
    /// Returns a new <see cref="Bitflag"/> with all bits toggled that are defined by the named mask.
    /// The original instance is not modified.
    /// </summary>
    /// <param name="name">The name of the mask to toggle.</param>
    /// <returns>
    /// A new <see cref="Bitflag"/> with the specified bits toggled,
    /// or <c>null</c> if the mask name does not exist.
    /// </returns>
    public Bitflag? ToggleByMaskNew(string name)
    {
        if (!_namedMasks.TryGetValue(name, out var mask))
            return null;

        var clone = this.Clone();
        clone._bitfield ^= mask;
        return clone;
    }

    /// <summary>
    /// Returns a new <see cref="Bitflag"/> with all bits toggled that are defined by the named mask corresponding to the given enum member.
    /// The original instance is not modified.
    /// </summary>
    /// <typeparam name="TEnum">The enum type used to resolve the mask name.</typeparam>
    /// <param name="enumValue">The enum member whose name corresponds to a defined mask.</param>
    /// <returns>
    /// A new <see cref="Bitflag"/> with the specified bits toggled,
    /// or <c>null</c> if the mask name does not exist.
    /// </returns>
    public Bitflag? ToggleByMaskNew<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        return ToggleByMaskNew(enumValue.ToString());
    }

    /// <summary>
    /// Returns a new <see cref="Bitflag"/> with all bits toggled that are defined by the named mask corresponding to the given enum member.
    /// The original instance is not modified.
    /// </summary>
    /// <param name="enumValue">The enum member whose name corresponds to a defined mask.</param>
    /// <returns>
    /// A new <see cref="Bitflag"/> with the specified bits toggled,
    /// or <c>null</c> if the mask name does not exist.
    /// </returns>
    public Bitflag? ToggleByMaskNew(Enum enumValue)
    {
        return ToggleByMaskNew(enumValue.ToString());
    }

    /// <summary>
    /// Returns a new <see cref="Bitflag"/> with all bits toggled that are defined by the named masks in the provided sequence.
    /// The original instance is not modified. Any names that do not match a defined mask are silently ignored.
    /// </summary>
    /// <param name="names">A sequence of mask names to toggle.</param>
    /// <returns>A new <see cref="Bitflag"/> with the specified bits toggled.</returns>
    public Bitflag ToggleByMaskNew(IEnumerable<string> names)
    {
        var clone = this.Clone();
        foreach (var name in names)
        {
            if (_namedMasks.TryGetValue(name, out var mask))
                clone._bitfield ^= mask;
        }
        return clone;
    }

    /// <summary>
    /// Returns a new <see cref="Bitflag"/> with all bits toggled that are defined by the named masks corresponding to the provided enum members.
    /// The original instance is not modified. Any enum names that do not match a defined mask are silently ignored.
    /// </summary>
    /// <param name="enums">A sequence of enum members whose names correspond to defined masks.</param>
    /// <returns>A new <see cref="Bitflag"/> with the specified bits toggled.</returns>
    public Bitflag ToggleByMaskNew(IEnumerable<Enum> enums)
    {
        var clone = this.Clone();
        foreach (var e in enums)
        {
            string name = e.ToString();
            if (_namedMasks.TryGetValue(name, out var mask))
                clone._bitfield ^= mask;
        }
        return clone;
    }


    /// <summary>
    /// Toggles all bits in the current bitfield that are defined by the named mask.
    /// </summary>
    /// <param name="name">The name of the mask to toggle.</param>
    /// <returns><c>true</c> if the mask was found and toggled; otherwise, <c>false</c>.</returns>
    public bool ToggleByMask(string name)
    {
        if (!_namedMasks.TryGetValue(name, out var mask))
            return false;

        _bitfield ^= mask;
        return true;
    }

    /// <summary>
    /// Toggles all bits in the current bitfield that are defined by the named mask corresponding to the given enum member.
    /// </summary>
    /// <typeparam name="TEnum">The enum type used to resolve the mask name.</typeparam>
    /// <param name="enumValue">The enum member whose name corresponds to a defined mask.</param>
    /// <returns><c>true</c> if the mask was found and toggled; otherwise, <c>false</c>.</returns>
    public bool ToggleByMask<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        return ToggleByMask(enumValue.ToString());
    }

    /// <summary>
    /// Toggles all bits in the current bitfield that are defined by the named mask corresponding to the given enum member.
    /// </summary>
    /// <param name="enumValue">An enum member whose name corresponds to a defined mask.</param>
    /// <returns><c>true</c> if the mask was found and toggled; otherwise, <c>false</c>.</returns>
    public bool ToggleByMask(Enum enumValue)
    {
        return ToggleByMask(enumValue.ToString());
    }

    /// <summary>
    /// Toggles all bits in the current bitfield that are defined by the named masks in the provided sequence.
    /// Any names that do not match a defined mask are silently ignored.
    /// </summary>
    /// <param name="names">A sequence of mask names to toggle.</param>
    public void ToggleByMask(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            if (_namedMasks.TryGetValue(name, out var mask))
                _bitfield ^= mask;
        }
    }

    /// <summary>
    /// Toggles all bits in the current bitfield that are defined by the named masks corresponding to the provided enum members.
    /// Each enum member's name is used to resolve a defined mask. Unmatched enum names are silently ignored.
    /// </summary>
    /// <param name="enums">A sequence of enum members whose names correspond to defined masks.</param>
    public void ToggleByMask(IEnumerable<Enum> enums)
    {
        foreach (var e in enums)
        {
            string name = e.ToString();
            if (_namedMasks.TryGetValue(name, out var mask))
                _bitfield ^= mask;
        }
    }

    /// <summary>
    /// Returns the raw masked value (from this bitfield) for the given named mask.
    /// </summary>
    /// <param name="name">The name of the defined mask</param>
    /// <returns>The result of bitfield &amp; mask</returns>
    /// <exception cref="ArgumentException">If the mask is not defined</exception>
    public ulong ExtractMask(string name)
    {
        if (!_namedMasks.TryGetValue(name, out var mask))
            throw new ArgumentException($"Mask '{name}' is not defined.");

        return _bitfield & mask;
    }

    /// <summary>
    /// Extracts a range of bits as an unsigned integer, right-aligned to bit 0.
    /// </summary>
    /// <param name="lowBit">The lowest bit index to include (0–63)</param>
    /// <param name="highBit">The highest bit index to include (0–63)</param>
    /// <returns>The extracted bitfield value, right-aligned</returns>
    /// <exception cref="ArgumentOutOfRangeException">If range is invalid</exception>
    public ulong ExtractBits(int lowBit, int highBit)
    {
        if (lowBit < 0 || highBit > 63 || lowBit > highBit)
            throw new ArgumentOutOfRangeException("Bit range must be between 0 and 63 and lowBit <= highBit");

        int width = highBit - lowBit + 1;
        ulong mask = ((1UL << width) - 1UL) << lowBit;

        return (_bitfield & mask) >> lowBit;
    }

    /// <summary>
    /// Inserts a value into a specified bitfield region by clearing that region and replacing it with the right-aligned bits from the provided value.
    /// </summary>
    /// <param name="value">The value to insert (will be masked to fit)</param>
    /// <param name="lowBit">The lowest bit index of the region to insert into</param>
    /// <param name="highBit">The highest bit index of the region to insert into</param>
    /// <exception cref="ArgumentOutOfRangeException">If bit range is invalid</exception>
    public void InsertBits(ulong value, int lowBit, int highBit)
    {
        if (lowBit < 0 || highBit > 63 || lowBit > highBit)
            throw new ArgumentOutOfRangeException("Bit range must be between 0 and 63 and lowBit <= highBit");

        int width = highBit - lowBit + 1;

        // Create a mask for the bitfield region
        ulong mask = ((1UL << width) - 1UL) << lowBit;

        // Mask the input value to prevent overflow into higher bits
        ulong maskedValue = (value & ((1UL << width) - 1UL)) << lowBit;

        // Clear the target region and insert the masked value
        _bitfield = (_bitfield & ~mask) | maskedValue;
    }

    /// <summary>
    /// Attempts to retrieve the name associated with a specific bit index (0–63).
    /// </summary>
    /// <param name="bitIndex">The bit index to look up.</param>
    /// <param name="name">The output name if the bit is defined.</param>
    /// <returns><c>true</c> if a named bit is found at the specified index; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the bit index is not between 0 and 63.</exception>
    public bool TryGetBitName(int bitIndex, out string name)
    {
        if (bitIndex < 0 || bitIndex > 63)
            throw new ArgumentOutOfRangeException(nameof(bitIndex), "Index must be between 0 and 63.");

        ulong bitmask = 1UL << bitIndex;

        foreach (var kvp in _namedBits)
        {
            if (kvp.Value == bitmask)
            {
                name = kvp.Key;
                return true;
            }
        }

        name = string.Empty;
        return false;
    }

}
