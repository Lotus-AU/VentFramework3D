using System;
using System.Linq;
using VentLib.Networking;
using RlAssembly = System.Reflection.Assembly;

namespace VentLib.Version.Git;

public class GitVersion: Version
{
    public readonly string MajorVersion;
    public readonly string MinorVersion;
    public readonly string PatchNumber;

    public readonly string CommitNumber;
    public readonly string Branch;

    public readonly string Sha;
    public readonly string Tag;

    internal string? CommitDate;
    internal string? RepositoryUrl;
    private static Type? _thisAssembly;

    internal GitVersion()
    {
        MajorVersion = null!;
        MinorVersion = null!;
        PatchNumber = null!;
        CommitNumber = null!;
        Branch = null!;
        Sha = null!;
        Tag = null!;
        RepositoryUrl = null!;
    }
    
    public GitVersion(RlAssembly? targetAssembly = null)
    {
        targetAssembly ??= Vents.RootAssemby;
        _thisAssembly ??= AppDomain.CurrentDomain.GetAssemblies()
           .Where(assembly => assembly.FullName == targetAssembly.FullName)
           .SelectMany(assembly =>
           {
               try { return assembly.GetTypes(); }
               catch (Exception) { return Array.Empty<Type>(); }
           })
           .FirstOrDefault(type => type.Name == "ThisAssembly");

        if (_thisAssembly == null)
            throw new Exception("Assemblies relying on GitVersion must include GitInfo as a package dependency.");

        Type git = _thisAssembly.GetNestedType("Git")!;
        Type baseVersion = git.GetNestedType("BaseVersion")!;

        MajorVersion = StaticValue(baseVersion, "Major");
        MinorVersion = StaticValue(baseVersion, "Minor");
        PatchNumber = StaticValue(baseVersion, "Patch");

        CommitNumber = StaticValue(git, "Commit");
        Branch = StaticValue(git, nameof(Branch));

        Sha = StaticValue(git, nameof(Sha));
        Tag = StaticValue(git, nameof(Tag));

        CommitDate = StaticValue(git, nameof(CommitDate));
        RepositoryUrl = StaticValue(git, nameof(RepositoryUrl));
    }
    
    private GitVersion(MessageReader reader)
    {
        MajorVersion = reader.Read<string>();
        MinorVersion = reader.Read<string>();
        PatchNumber = reader.Read<string>();

        CommitNumber = reader.Read<string>();
        Branch = reader.Read<string>();

        Sha = reader.Read<string>();
        Tag = reader.Read<string>();
    }

    public GitVersion Clone() => (GitVersion)this.MemberwiseClone();

    public override Version Read(MessageReader reader) => new GitVersion(reader);

    protected override void WriteInfo(MessageWriter writer)
    {
        writer.Write(MajorVersion);
        writer.Write(MinorVersion);
        writer.Write(PatchNumber);
        writer.Write(CommitNumber);
        writer.Write(Branch);
        writer.Write(Sha);
        writer.Write(Tag);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not GitVersion other) return false;
        return MajorVersion == other.MajorVersion && MinorVersion == other.MinorVersion && PatchNumber == other.PatchNumber && CommitNumber == other.CommitNumber && Branch == other.Branch && Sha == other.Sha && Tag == other.Tag;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MajorVersion, MinorVersion, PatchNumber, CommitNumber, Branch, Sha, Tag);
    }

    public override string ToSimpleName() => $"{MajorVersion}.{MinorVersion}.{PatchNumber}";
    
    public override string ToString()
    {
        return $"GitVersion({MajorVersion}.{MinorVersion}.{PatchNumber} Branch: {Branch} Commit: {CommitNumber})";
    }

    private static string StaticValue(Type type, string fieldName) => (string)type.GetField(fieldName)!.GetValue(null)!;
}