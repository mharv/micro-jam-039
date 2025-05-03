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
    
            globalState.Player.PositionX = GetMouseX();
            globalState.Player.PositionY = GetMouseY();
            // direction is a rotation in degrees
            // 0 degrees is right, 90 degrees is up, 180 degrees is left, 270 degrees is down
            globalState.Player.Direction = (int)(Math.Atan2(globalState.Player.PositionY - globalState.Enemy.Y, 
                                   globalState.Player.PositionX - globalState.Enemy.X) * 180 / Math.PI);

            bool leftButtonState = Raylib.IsMouseButtonDown(MouseButton.Left);
            bool middleButtonState = Raylib.IsMouseButtonDown(MouseButton.Middle);
            bool rightButtonState = Raylib.IsMouseButtonDown(MouseButton.Right);
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);


            Color textColor = Color.Black;
            string text = $"X: {globalState.Player.PositionX,-5}\n" +
                          $"Y: {globalState.Player.PositionY,-5}\n" +
                          $"Left: {leftButtonState,-5}\n" +
                          $"Middle: {middleButtonState,-5}\n" +
                          $"Right: {rightButtonState,-5}";
        
            Raylib.DrawCircle(globalState.Player.PositionX, globalState.Player.PositionY, 20, Color.Red);
            // draw a arrow pointing in player direction 40 pixels long
            int arrowX = (int)(globalState.Player.PositionX - 40 * Math.Cos(globalState.Player.Direction * Math.PI / 180));
            int arrowY = (int)(globalState.Player.PositionY - 40 * Math.Sin(globalState.Player.Direction * Math.PI / 180));
            Raylib.DrawLine(globalState.Player.PositionX, globalState.Player.PositionY, arrowX, arrowY, Color.Red);
            Raylib.DrawCircle(globalState.Enemy.X, globalState.Enemy.Y, globalState.Enemy.HitboxRadius, Color.Blue);

            Raylib.DrawText(text, 12, 12, 20, textColor);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}