namespace iSystem.IO;

// ReSharper disable once UnusedMember.Global
public static class PathEx
{
	// ReSharper disable once UnusedMember.Global
	public static string GetRelativePathToWorkingDirectory(string filePath)
    {
        return string.IsNullOrEmpty(filePath) ? string.Empty : Path.GetRelativePath(Path.GetFullPath("."), filePath);
    }
}