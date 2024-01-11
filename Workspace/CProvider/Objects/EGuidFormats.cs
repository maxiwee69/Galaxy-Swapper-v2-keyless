namespace LilySwapper.Workspace.CProvider.Objects;

public enum EGuidFormats
{
    Digits, // "00000000000000000000000000000000"
    DigitsWithHyphens, // 00000000-0000-0000-0000-000000000000
    DigitsWithHyphensInBraces, // {00000000-0000-0000-0000-000000000000}
    DigitsWithHyphensInParentheses, // (00000000-0000-0000-0000-000000000000)
    HexValuesInBraces, // {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}
    UniqueObjectGuid, // 00000000-00000000-00000000-00000000
    Short, // AQsMCQ0PAAUKCgQEBAgADQ
    Base36Encoded // 1DPF6ARFCM4XH5RMWPU8TGR0J
}