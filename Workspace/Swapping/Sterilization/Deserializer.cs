using LilySwapper.Workspace.Hashes;

namespace LilySwapper.Workspace.Swapping.Sterilization;

public class Deserializer
{
    public byte[] Asset;
    public FBulkDataMapEntry[] BulkDataMap;
    public FNameEntrySerialized[] Entires;
    public FExportMapEntry[] ExportMap;
    public ulong[] Hashes;
    public ulong HashVersion;
    public byte[] Pad;
    public byte[] RestOfData;
    public FZenPackageSummary Summary;

    public Deserializer(byte[] buffer)
    {
        Asset = buffer;
    }

    public void Read()
    {
        var reader = new Reader(Asset);

        Summary = new FZenPackageSummary
        {
            bHasVersioningInfo = reader.Read<uint>(),
            HeaderSize = reader.Read<uint>(),
            Name = reader.Read<FMappedName>(),
            PackageFlags = reader.Read<EPackageFlags>(),
            CookedHeaderSize = reader.Read<uint>(),
            ImportedPublicExportHashesOffset = reader.Read<int>(),
            ImportMapOffset = reader.Read<int>(),
            ExportMapOffset = reader.Read<int>(),
            ExportBundleEntriesOffset = reader.Read<int>(),
            DependencyBundleHeadersOffset = reader.Read<int>(),
            DependencyBundleEntriesOffset = reader.Read<int>(),
            ImportedPackageNamesOffset = reader.Read<int>()
        };

        LoadNameMaps(reader);
        LoadPad(reader);
        LoadBulkDataMaps(reader);
        LoadExportMap(reader);

        reader.Position = Summary.ImportedPublicExportHashesOffset;
        RestOfData = reader.ReadBytes((int)(reader.Length - reader.Position));
    }

    private void LoadNameMaps(Reader reader)
    {
        var numStrings = reader.Read<int>();
        var numBuffer = reader.Read<uint>();

        HashVersion = reader.Read<ulong>();
        Hashes = reader.ReadArray<ulong>(numStrings);

        var headers = reader.ReadArray<FSerializedNameHeader>(numStrings);
        Entires = new FNameEntrySerialized[numStrings];
        for (var i = 0; i < numStrings; i++)
        {
            var header = headers[i];
            var length = (int)header.Length;
            var s = header.IsUtf16
                ? new string(reader.ReadArray<char>(length))
                : Encoding.UTF8.GetString(reader.ReadBytes(length));
            Entires[i] = new FNameEntrySerialized(s);
        }
    }

    private void LoadExportMap(Reader reader)
    {
        ExportMap = new FExportMapEntry[(Summary.ExportBundleEntriesOffset - Summary.ExportMapOffset) / 72];
        reader.Position = Summary.ExportMapOffset;
        for (var i = 0; i < ExportMap.Length; i++)
        {
            var position = reader.Position;
            ExportMap[i].CookedSerialOffset = reader.Read<ulong>();
            ExportMap[i].CookedSerialSize = reader.Read<ulong>();
            ExportMap[i].ObjectName = reader.Read<FMappedName>();
            ExportMap[i].OuterIndex = reader.Read<FPackageObjectIndex>();
            ExportMap[i].ClassIndex = reader.Read<FPackageObjectIndex>();
            ExportMap[i].SuperIndex = reader.Read<FPackageObjectIndex>();
            ExportMap[i].TemplateIndex = reader.Read<FPackageObjectIndex>();
            ExportMap[i].PublicExportHash = reader.Read<ulong>();
            ExportMap[i].ObjectFlags = reader.Read<EObjectFlags>();
            ExportMap[i].FilterFlags = reader.Read<byte>();
            reader.Position = position + 72;
        }
    }

    private void LoadPad(Reader reader)
    {
        var padSize = reader.Read<ulong>();
        Pad = reader.ReadBytes((int)padSize);
    }

    private void LoadBulkDataMaps(Reader reader)
    {
        var bulkDataMapSize = reader.Read<ulong>();

        BulkDataMap = new FBulkDataMapEntry[bulkDataMapSize / FBulkDataMapEntry.Size];

        for (var i = 0; i < BulkDataMap.Length; i++)
        {
            BulkDataMap[i].SerialOffset = reader.Read<ulong>();
            BulkDataMap[i].DuplicateSerialOffset = reader.Read<ulong>();
            BulkDataMap[i].SerialSize = reader.Read<ulong>();
            BulkDataMap[i].Flags = reader.Read<uint>();
            BulkDataMap[i].Pad = reader.Read<uint>();
        }
    }

    public void ReplaceEntry(string search, string replace)
    {
        var index = Entires.ToList().FindIndex(entry => entry.Name.Equals(search, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
        {
            Log.Error($"Failed to find index for: {search}");
            return;
        }

        if (replace.Length > 255)
        {
            Log.Error($"{replace} is longer then 255 chars");
            return;
        }

        Entires[index].Name = replace;
        Hashes[index] = CityHash.Hash(Encoding.ASCII.GetBytes(replace.ToLower()));

        Log.Information($"Replaced: {search} to {replace}");

        Adjust(replace.Length - search.Length);
    }

    public void ReplaceEntry(Deserializer deserializer, string search, string replace)
    {
        var index = Entires.ToList().FindIndex(entry => entry.Name.Equals(search, StringComparison.OrdinalIgnoreCase));
        var overrideindex = deserializer.Entires.ToList()
            .FindIndex(entry => entry.Name.Equals(replace, StringComparison.OrdinalIgnoreCase));

        if (index < 0)
        {
            Log.Warning($"Failed to find index for: {search} attaching _C and trying again");
            search = $"{search}_C";
            index = Entires.ToList().FindIndex(entry => entry.Name.Equals(search, StringComparison.OrdinalIgnoreCase));

            if (index < 0)
            {
                Log.Error($"Failed to find index for: {search}");
                throw new Exception($"Failed to find index for: {search}");
            }
        }

        if (overrideindex < 0)
        {
            Log.Warning($"Failed to find index for: {replace} attaching _C and trying again");
            replace = $"{replace}_C";
            overrideindex = deserializer.Entires.ToList()
                .FindIndex(entry => entry.Name.Equals(replace, StringComparison.OrdinalIgnoreCase));

            if (overrideindex < 0)
            {
                Log.Error($"Failed to find index for: {replace}");
                throw new Exception($"Failed to find index for: {replace}");
            }
        }

        Entires[index].Name = deserializer.Entires[overrideindex].Name;
        Hashes[index] = deserializer.Hashes[overrideindex];

        Log.Information($"Replaced: {search} to {replace} with hashes");

        Adjust(replace.Length - search.Length);
    }

    public void ReplaceMaterialOverrideArray(MaterialData materialData)
    {
        var arrayPos = (int)materialData.Offset;

        if (materialData.SearchBuffer?.Length > 0)
        {
            arrayPos = RestOfData.IndexOfSequence(materialData.SearchBuffer, 0);
            if (arrayPos < 0)
            {
                Log.Error("Failed to find 'MaterialOverrides' array");
                throw new CustomException("Failed to find 'MaterialOverrides' array");
            }
        }

        var reader = new Reader(RestOfData, arrayPos);

        var materialOverridesCount = reader.Read<int>();
        var materialOverridesSize = sizeof(int) + 3 + 20 + 26 * (materialOverridesCount - 1);

        var writer = new Writer(new byte[sizeof(int) + 26 * materialData.Materials.Count]);

        writer.Write(materialData.Materials.Count);

        foreach (var material in materialData.Materials)
        {
            var entryPath = material.Material.SubstringBefore('.');
            var entryName = material.Material.SubstringAfter('.');

            writer.WriteByte(0);
            writer.WriteByte(5);
            writer.Write(material.MaterialOverrideIndex);
            writer.Write(Entires.Count());
            writer.Write(0);
            writer.Write(Entires.Count() + 1);
            writer.Write(0);
            writer.Write(0);

            AddEntry(entryPath, entryName);
        }

        var MaterialOverridesBuffer = writer.ToByteArray(writer.Position);
        var restOfDataArray = new List<byte>(RestOfData);

        //Insert our new 'MaterialOverrides' array
        restOfDataArray.RemoveRange(arrayPos, materialOverridesSize);
        restOfDataArray.InsertRange(arrayPos, MaterialOverridesBuffer);

        var materialOverrideFlagsPos = (int)materialData.MaterialOverrideFlags.Offset;

        if (materialData.MaterialOverrideFlags.SearchBuffer?.Length > 0)
        {
            materialOverrideFlagsPos = restOfDataArray.ToArray()
                .IndexOfSequenceReverse(materialData.MaterialOverrideFlags.SearchBuffer);

            if (materialOverrideFlagsPos < 0)
            {
                Log.Error("Failed to find MaterialOverrideFlags array");
                throw new CustomException("Failed to find MaterialOverrideFlags array");
            }
        }

        restOfDataArray.RemoveRange(materialOverrideFlagsPos, sizeof(int));
        restOfDataArray.InsertRange(materialOverrideFlagsPos,
            BitConverter.GetBytes(materialData.MaterialOverrideFlags.MaterialOverrideFlags));

        RestOfData = restOfDataArray.ToArray();
        ExportMap[^1].CookedSerialSize += (ulong)(MaterialOverridesBuffer.Length - materialOverridesSize);
    }

    public void ReplaceTextureParametersArray(TextureData textureData)
    {
        var arrayPos = (int)textureData.Offset;

        if (textureData.SearchBuffer?.Length > 0)
        {
            arrayPos = RestOfData.IndexOfSequence(textureData.SearchBuffer, 0);
            if (arrayPos < 0)
            {
                Log.Error("Failed to find 'TextureParameters' array");
                throw new CustomException("Failed to find 'TextureParameters' array");
            }
        }

        var reader = new Reader(RestOfData, arrayPos);

        //Orignal 'TextureParameters' data
        var textureParametersCount = reader.Read<int>();
        var textureParametersSize = sizeof(int) + 3 + 28;

        if (textureParametersCount > 1) textureParametersSize += 31 * (textureParametersCount - 1);

        var writer = new Writer(new byte[sizeof(int) + 34 * textureData.TextureParameters.Count]);

        writer.Write(textureData.TextureParameters.Count);

        foreach (var textureParameter in textureData.TextureParameters)
        {
            var entryPath = textureParameter.TextureOverride.SubstringBefore('.');
            var entryName = textureParameter.TextureOverride.SubstringAfter('.');

            writer.WriteByte(0);
            writer.WriteByte(0x07);
            writer.Write(textureParameter.MaterialIndexForTextureParameter);
            writer.Write(Entires.Count());
            writer.Write(0);
            writer.Write(Entires.Count() + 1);
            writer.Write(0);
            writer.Write(Entires.Count() + 2);
            writer.Write(0);
            writer.Write(0);

            AddEntry(textureParameter.TextureParameterNameForMaterial, entryPath, entryName);
        }

        var newtextureParametersBuffer = writer.ToByteArray(writer.Position);
        var restOfDataArray = new List<byte>(RestOfData);

        //Insert our new 'TextureParameters' array
        restOfDataArray.RemoveRange(arrayPos, textureParametersSize);
        restOfDataArray.InsertRange(arrayPos, newtextureParametersBuffer);

        RestOfData = restOfDataArray.ToArray();
        ExportMap[^1].CookedSerialSize += (ulong)(newtextureParametersBuffer.Length - textureParametersSize);
    }

    public void AddEntry(params string[] nameMaps)
    {
        Entires = Entires.Concat(nameMaps.Select(nameMap => new FNameEntrySerialized(nameMap))).ToArray();
        Hashes = Hashes.Concat(nameMaps.Select(nameMap => CityHash.Hash(Encoding.ASCII.GetBytes(nameMap.ToLower()))))
            .ToArray();

        Adjust(nameMaps.Sum(nameMap => nameMap.Length) + 10 * nameMaps.Length);
    }

    public void Adjust(int diff)
    {
        Summary.ImportMapOffset += diff;
        Summary.ExportMapOffset += diff;
        Summary.ExportBundleEntriesOffset += diff;
        Summary.DependencyBundleHeadersOffset += diff;
        Summary.DependencyBundleEntriesOffset += diff;
        Summary.ImportedPackageNamesOffset += diff;
        Summary.ImportedPublicExportHashesOffset += diff;
        Summary.HeaderSize += (uint)diff;
        Summary.CookedHeaderSize += (uint)diff;

        Log.Information($"Adjusting asset with for: {diff} difference");
    }
}