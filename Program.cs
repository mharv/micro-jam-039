using Raylib_cs;
using Globals;
using static Raylib_cs.Raylib;

namespace BalanceOfTime;


enum GamePhase { Menu, Round, Transition }

class Program
{
    public static void Main()
    {
        Raylib.InitWindow(GlobalVariables.WindowSizeX, GlobalVariables.WindowSizeY, "Hello World");
        GlobalState globalState = new GlobalState();
        globalState.Score = 0;
        Raylib.SetTargetFPS(60);

        // Timer variables
        GamePhase currentPhase = GamePhase.Menu;
        float currentTimer = 0f;

        while (!Raylib.WindowShouldClose())
        {
            // get delta time
            float deltaTime = Raylib.GetFrameTime();
            currentTimer += deltaTime;

            if (currentPhase == GamePhase.Menu)
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    currentPhase = GamePhase.Round;
                    GlobalVariables.BackgroundColor = Color.White;
                    currentTimer = 0f;

                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(GlobalVariables.BackgroundColor);
                Raylib.DrawText("Click to start", GlobalVariables.WindowSizeX / 2 - 50, GlobalVariables.WindowSizeY / 2 - 10, 20, Color.Black);
                Raylib.EndDrawing();
                continue;
            }
            else if (currentPhase == GamePhase.Round && currentTimer >= globalState.RoundDuration)
            {
                currentPhase = GamePhase.Transition;
                GlobalVariables.BackgroundColor = Color.Green;
                currentTimer = 0f;
            }
            else if (currentPhase == GamePhase.Transition && currentTimer >= globalState.TransitionDuration)
            {
                currentPhase = GamePhase.Round;
                GlobalVariables.BackgroundColor = Color.White;
                currentTimer = 0f;
            }



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
            Raylib.ClearBackground(GlobalVariables.BackgroundColor);


            Color textColor = Color.Black;
            string text = $"X: {globalState.Player.PositionX,-5}\n" +
                            $"Y: {globalState.Player.PositionY,-5}\n" +
                            $"Left: {leftButtonState,-5}\n" +
                            $"Middle: {middleButtonState,-5}\n" +
                            $"Right: {rightButtonState,-5}\n" +
                            $"Direction: {globalState.Player.Direction,-5}\n" +
                            $"Round: {currentPhase,-5}\n" +
                            $"Timer: {currentTimer,-5}\n" +
                            $"Score: {globalState.Score,-5}\n"; ;

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