using Ara3D.Utils;

namespace Ara3D.Parakeet.Tests;

public class Folders
{
    public static DirectoryPath SourceFolder
        = PathUtil.GetCallerSourceFolder();

    public static DirectoryPath BaseInputFolder 
        = SourceFolder.RelativeFolder("..", "input");

    public static DirectoryPath BaseOutputFolder
        = SourceFolder.RelativeFolder("..", "output");

    public static DirectoryPath CstOutputFolder
        = SourceFolder.RelativeFolder("..", "Parakeet.Cst");

    public static DirectoryPath InputFolder(string name) 
        => BaseInputFolder.RelativeFolder(name);
}