﻿namespace LilySwapper.Workspace.CProvider.Objects;

[StructLayout(LayoutKind.Sequential)]
#pragma warning disable 660,661
public struct FGuid
#pragma warning restore 660,661
{
    public readonly uint A;
    public readonly uint B;
    public readonly uint C;
    public readonly uint D;

    public FGuid(uint v)
    {
        A = B = C = D = v;
    }

    public FGuid(uint a, uint b, uint c, uint d)
    {
        A = a;
        B = b;
        C = c;
        D = d;
    }

    public FGuid(string hexString)
    {
        A = Convert.ToUInt32(hexString.Substring(0, 8), 16);
        B = Convert.ToUInt32(hexString.Substring(8, 8), 16);
        C = Convert.ToUInt32(hexString.Substring(16, 8), 16);
        D = Convert.ToUInt32(hexString.Substring(24, 8), 16);
    }

    public bool IsValid()
    {
        return (A | B | C | D) != 0;
    }

    public readonly string ToString(EGuidFormats guidFormat)
    {
        switch (guidFormat)
        {
            case EGuidFormats.DigitsWithHyphens:
                return
                    $"{A:X8}-{B >> 16:X4}-{B & 0xFFFF:X4}-{C >> 16:X4}-{C & 0xFFFF:X4}{D:X8}";
            case EGuidFormats.DigitsWithHyphensInBraces:
                return
                    $"{{{A:X8}-{B >> 16:X4}-{B & 0xFFFF:X4}-{C >> 16:X4}-{C & 0xFFFF:X4}{D:X8}}}";
            case EGuidFormats.DigitsWithHyphensInParentheses:
                return
                    $"({A:X8}-{B >> 16:X4}-{B & 0xFFFF:X4}-{C >> 16:X4}-{C & 0xFFFF:X4}{D:X8})";
            case EGuidFormats.HexValuesInBraces:
                return
                    $"{{0x{A:X8},0x{B >> 16:X4},0x{B & 0xFFFF:X4},{{0x{C >> 24:X2},0x{(C >> 16) & 0xFF:X2},0x{(C >> 8) & 0xFF:X2},0x{C & 0XFF:X2},0x{D >> 24:X2},0x{(D >> 16) & 0XFF:X2},0x{(D >> 8) & 0XFF:X2},0x{D & 0XFF:X2}}}}}";
            case EGuidFormats.UniqueObjectGuid: return $"{A:X8}-{B:X8}-{C:X8}-{D:X8}";
            case EGuidFormats.Short:
            {
                var data = BitConverter.GetBytes(A).Concat(BitConverter.GetBytes(B)).Concat(BitConverter.GetBytes(C))
                    .Concat(BitConverter.GetBytes(D));
                var result = Convert.ToBase64String(data.ToArray()).Replace('+', '-').Replace('/', '_');
                if (result.Length == 24) // Remove trailing '=' base64 padding
                    result = result.Substring(0, result.Length - 2);

                return result;
            }
            default: return $"{A:X8}{B:X8}{C:X8}{D:X8}";
        }
    }

    public override string ToString()
    {
        return ToString(EGuidFormats.Digits);
    }

    public static bool operator ==(FGuid one, FGuid two)
    {
        return one.A == two.A && one.B == two.B && one.C == two.C && one.D == two.D;
    }

    public static bool operator !=(FGuid one, FGuid two)
    {
        return one.A != two.A || one.B != two.B || one.C != two.C || one.D != two.D;
    }

    public static implicit operator FGuid(Guid g)
    {
        return new FGuid(g.ToString().Replace("-", ""));
    }
}