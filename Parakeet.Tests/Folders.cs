using Ara3D.Utils;

namespace Parakeet.Tests;

public class Folders
{
    public static DirectoryPath SourceFolder
        = PathUtil.GetCallerSourceFolder();

    public static DirectoryPath BaseInputFolder 
        = SourceFolder.RelativeFolder("..", "input");

    public static DirectoryPath BaseOutputFolder
        = SourceFolder.RelativeFolder("..", "output");

    public static DirectoryPath CstOutputFolder
        = SourceFolder.RelativeFolder("..", "Parakeet.Cst.Demo");

    public static DirectoryPath InputFolder(string name) 
        => BaseInputFolder.RelativeFolder(name);
}