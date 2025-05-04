using Raylib_cs;
using Types;
namespace Globals;

class GlobalVariables
{
    public static int WindowSizeX { get; } = 800;
    public static int WindowSizeY { get; } = 800;
    public static Color BackgroundColor = Color.Red;
    public static Difficulty GameDifficulty = Difficulty.VeryEasy;
}
