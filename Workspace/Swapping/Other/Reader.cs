﻿using System.Runtime.CompilerServices;

namespace LilySwapper.Workspace.Swapping.Other;

public class Reader : BinaryReader
{
    public Reader(string file)
        : base(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
    {
        Name = file;
    }

    public Reader(byte[] data) : base(new MemoryStream(data))
    {
    }

    public Reader(byte[] data, long offset) : base(new MemoryStream(data))
    {
        Position = offset;
    }

    public long Length => base.BaseStream.Length;
    public string Name { get; set; }

    public long Position
    {
        get => base.BaseStream.Position;
        set => base.BaseStream.Position = value;
    }

    public new byte[] ReadBytes(int length)
    {
        var array = new byte[length];
        Read(array, 0, length);
        return array;
    }

    public string ReadStrings(int length)
    {
        var array = new byte[length];
        Read(array, 0, length);
        return Encoding.ASCII.GetString(array);
    }

    public T Read<T>()
    {
        if (typeof(T) == typeof(string) || typeof(T) == typeof(object))
        {
            var length = Read<int>();
            return (T)(object)Encoding.ASCII.GetString(ReadBytes(length));
        }

        return Unsafe.ReadUnaligned<T>(ref ReadBytes(Unsafe.SizeOf<T>())[0]);
    }

    public virtual T[] ReadArray<T>(Func<T> getter)
    {
        var length = Read<int>();
        return ReadArray(length, getter);
    }

    public T[] ReadArray<T>(int length, Func<T> getter)
    {
        var result = new T[length];

        if (length == 0) return result;

        ReadArray(result, getter);

        return result;
    }

    public void ReadArray<T>(T[] array, Func<T> getter)
    {
        // array is a reference type
        for (var i = 0; i < array.Length; i++) array[i] = getter();
    }

    public T[] ReadArray<T>()
    {
        var array = new T[Read<int>()];
        for (var i = 0; i < array.Length; i++) array[i] = Read<T>();
        return array;
    }

    public T[] ReadArray<T>(int length)
    {
        var array = new T[length];
        for (var i = 0; i < array.Length; i++) array[i] = Read<T>();
        return array;
    }

    public virtual string ReadFString()
    {
        // > 0 for ANSICHAR, < 0 for UCS2CHAR serialization
        var length = Read<int>();

        if (length == int.MinValue)
            throw new ArgumentOutOfRangeException(nameof(length), "Archive is corrupted");

        // if (length is < -512000 or > 512000)
        //     throw new ParserException($"Invalid FString length '{length}'");

        if (length == 0) return string.Empty;

        // 1 byte/char is removed because of null terminator ('\0')
        if (length < 0) // LoadUCS2Char, Unicode, 16-bit, fixed-width
            unsafe
            {
                length = -length;
                var ucs2Length = length * sizeof(ushort);
                var ucs2Bytes = ucs2Length <= 1024 ? stackalloc byte[ucs2Length] : new byte[ucs2Length];
                fixed (byte* ucs2BytesPtr = ucs2Bytes)
                {
                    Serialize(ucs2BytesPtr, ucs2Length);
#if !NO_STRING_NULL_TERMINATION_VALIDATION
                    if (ucs2Bytes[ucs2Length - 1] != 0 || ucs2Bytes[ucs2Length - 2] != 0)
                        throw new Exception("Serialized FString is not null terminated");
#endif
                    return new string((char*)ucs2BytesPtr, 0, length - 1);
                }
            }

        unsafe
        {
            var ansiBytes = length <= 1024 ? stackalloc byte[length] : new byte[length];
            fixed (byte* ansiBytesPtr = ansiBytes)
            {
                Serialize(ansiBytesPtr, length);
#if !NO_STRING_NULL_TERMINATION_VALIDATION
                if (ansiBytes[length - 1] != 0) throw new Exception("Serialized FString is not null terminated");
#endif
                return new string((sbyte*)ansiBytesPtr, 0, length - 1);
            }
        }
    }

    public static T ReadStruct<T>(byte[] buffer, int offset)
    {
        var bytes = new byte[Marshal.SizeOf<T>()];
        Buffer.BlockCopy(buffer, offset, bytes, 0, Marshal.SizeOf<T>());
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        var data = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));

        return data;
    }

    public virtual unsafe void Serialize(byte* ptr, int length)
    {
        var bytes = ReadBytes(length);
        Unsafe.CopyBlockUnaligned(ref ptr[0], ref bytes[0], (uint)length);
    }

    public Dictionary<TKey, TValue> ReadMap<TKey, TValue>()
    {
        var dictionary = new Dictionary<TKey, TValue>();
        var num = Read<int>();
        for (var i = 0; i < num; i++)
        {
            var key = Read<TKey>();
            var flag = Read<bool>();
            dictionary.Add(key, flag ? (TValue)(object)ReadArray<TValue>() : Read<TValue>());
        }

        return dictionary;
    }
}