using Raylib_cs;
using Globals;

using static Raylib_cs.Raylib;

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
            // inputs 
    
            globalState.PlayerPosition.X = GetMouseX();
            globalState.PlayerPosition.Y = GetMouseY();

            bool leftButtonState = IsMouseButtonDown(MouseButton.Left);
            bool middleButtonState = IsMouseButtonDown(MouseButton.Middle);
            bool rightButtonState = IsMouseButtonDown(MouseButton.Right);

            if (IsMouseButtonDown(MouseButton.Left))
            {
                leftButtonState = true;
            } else {
                leftButtonState = false;
            }
            if (IsMouseButtonDown(MouseButton.Middle))
            {
                middleButtonState = true;
            }
            else {
                middleButtonState = false;
            }
            if (IsMouseButtonDown(MouseButton.Right))
            {
                rightButtonState = true;
            }
            else {
                rightButtonState = false;
            }

            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);


            Color textColor = Color.Black;
            string text = $"X: {globalState.PlayerPosition.X,-5}\n" +
                          $"Y: {globalState.PlayerPosition.Y,-5}\n" +
                          $"Left: {leftButtonState,-5}\n" +
                          $"Middle: {middleButtonState,-5}\n" +
                          $"Right: {rightButtonState,-5}";
        


            Raylib.DrawText(text, 12, 12, 8, textColor);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}