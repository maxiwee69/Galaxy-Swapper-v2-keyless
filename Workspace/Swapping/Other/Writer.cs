using System.Runtime.CompilerServices;

namespace LilySwapper.Workspace.Swapping.Other;

public class Writer : BinaryWriter
{
    public Writer(string file)
        : base(File.OpenWrite(file))
    {
    }

    public Writer(byte[] data)
        : base(new MemoryStream(data))
    {
    }

    public long Position
    {
        get => base.BaseStream.Position;
        set => base.BaseStream.Position = value;
    }

    public byte[] ToByteArray(long length)
    {
        if (length == -1) length = base.BaseStream.Length;
        var array = new byte[length];
        Position = 0L;
        base.BaseStream.Read(array, 0, array.Length);
        return array;
    }

    public void WriteBytes(byte[] buf)
    {
        Write(buf, 0, buf.Length);
    }

    public void WriteByte(byte b)
    {
        Write(new byte[1] { b }, 0, 1);
    }

    public void Write<T>(T value, bool writeLengthForString = true, bool swap = false)
    {
        if (typeof(T) == typeof(string) || typeof(T) == typeof(object))
        {
            var bytes = Encoding.ASCII.GetBytes((string)(object)value);
            if (writeLengthForString) Write(bytes.Length);
            WriteBytes(bytes);
            return;
        }

        var array = new byte[Unsafe.SizeOf<T>()];
        Unsafe.WriteUnaligned(ref array[0], value);
        if (swap) array = array.Reverse().ToArray();
        WriteBytes(array);
    }

    public void WriteArray<T>(T[] array, bool writeLength = true)
    {
        if (writeLength) Write(array.Length);
        foreach (var value in array) Write(value);
    }

    public void WriteArray<T>(Array array)
    {
        var array2 = new T[array.Length];
        for (var i = 0; i < array2.Length; i++) array2[i] = (T)array.GetValue(i);
        WriteArray(array2);
    }

    public void WriteMap<TKey, TValue>(Dictionary<TKey, TValue> map)
    {
        Write(map.Count);
        foreach (var item in map)
        {
            Write(item.Key);
            if ((object)item.Value is Array array)
            {
                Write(true);
                WriteArray<object>(array);
            }
            else
            {
                Write(false);
                Write(item.Value);
            }
        }
    }
}