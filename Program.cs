using Raylib_cs;
using Globals;

namespace BalanceOfTime;

class Program
{
    public static void Main()
    {
        Raylib.InitWindow(GlobalVariables.WindowSizeX, GlobalVariables.WindowSizeY, "Hello World");
        GlobalState globalState = new GlobalState();
        globalState.Score = 0;
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            Raylib.DrawText("Hello, world!", 12, 12, 20, Color.Black);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}