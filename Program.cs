using Raylib_cs;
using Globals;
using static Raylib_cs.Raylib;
using Recording;

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

        GameHistory gameHistory = new GameHistory();
        Round currentRound = new Round();
        int roundId = currentRound.Id;
        int timeSliceCounter = 0;

        int currentFrame = 0;


        while (!Raylib.WindowShouldClose())
        {
            // get delta time
            float deltaTime = Raylib.GetFrameTime();
            currentFrame++;


            if (currentPhase == GamePhase.Menu)
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    currentPhase = GamePhase.Round;
                    GlobalVariables.BackgroundColor = Color.White;
                    currentFrame = 0;
                }
                else
                {
                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(GlobalVariables.BackgroundColor);
                    Raylib.DrawText("Click to start", GlobalVariables.WindowSizeX / 2 - 50, GlobalVariables.WindowSizeY / 2 - 10, 20, Color.Black);
                    Raylib.EndDrawing();
                    continue;
                }
            }
            else if (currentPhase == GamePhase.Round && currentFrame >= globalState.RoundDurationFrames)
            {
                currentPhase = GamePhase.Transition;
                GlobalVariables.BackgroundColor = Color.Green;
                gameHistory.AppendToRounds(currentRound);
                timeSliceCounter = 0;
                currentFrame = 0;
            }
            else if (currentPhase == GamePhase.Transition && currentFrame >= globalState.TransitionDurationFrames)
            {
                currentPhase = GamePhase.Round;
                GlobalVariables.BackgroundColor = Color.White;
                currentRound = new Round(roundId);
                roundId = currentRound.Id;

                currentFrame = 0;
            }


            if (currentPhase == GamePhase.Transition)
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(GlobalVariables.BackgroundColor);
                Raylib.DrawText("Transition... \nnext boss", GlobalVariables.WindowSizeX / 2 - 50, GlobalVariables.WindowSizeY / 2 - 10, 20, Color.Black);
                Raylib.EndDrawing();
                continue;
            }

            // get past player if exists
            if (gameHistory.Rounds.Length > 0)
            {
                // get the last round
                Round lastRound = gameHistory.Rounds[gameHistory.Rounds.Length - 1];
                globalState.PastPlayer.PositionX = lastRound.History[timeSliceCounter].PlayerPositionX;
                globalState.PastPlayer.PositionY = lastRound.History[timeSliceCounter].PlayerPositionY;
            }



            // inputs 

            globalState.Player.PositionX = GetMouseX();
            globalState.Player.PositionY = GetMouseY();
            // direction is a rotation in degrees
            // 0 degrees is right, 90 degrees is up, 180 degrees is left, 270 degrees is down
            globalState.Player.Direction = (int)(Math.Atan2(globalState.Player.PositionY - globalState.Enemy.Y,
                                   globalState.Player.PositionX - globalState.Enemy.X) * 180 / Math.PI);
            globalState.PastPlayer.Direction = (int)(Math.Atan2(globalState.PastPlayer.PositionY - globalState.Enemy.Y,
                                   globalState.PastPlayer.PositionX - globalState.Enemy.X) * 180 / Math.PI);

            bool leftButtonState = Raylib.IsMouseButtonDown(MouseButton.Left);
            bool middleButtonState = Raylib.IsMouseButtonDown(MouseButton.Middle);
            bool rightButtonState = Raylib.IsMouseButtonDown(MouseButton.Right);

            // update the round history
            currentRound.AppendToHistory(new TimeSlice(globalState.Player.PositionX, globalState.Player.PositionY, currentFrame));

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
                            $"Timer: {currentFrame,-5}\n" +
                            $"Score: {globalState.Score,-5}\n"; ;

            Raylib.DrawCircle(globalState.Player.PositionX, globalState.Player.PositionY, 20, Color.Red);
            // draw a arrow pointing in player direction 40 pixels long
            int arrowX = (int)(globalState.Player.PositionX - 40 * Math.Cos(globalState.Player.Direction * Math.PI / 180));
            int arrowY = (int)(globalState.Player.PositionY - 40 * Math.Sin(globalState.Player.Direction * Math.PI / 180));
            Raylib.DrawLine(globalState.Player.PositionX, globalState.Player.PositionY, arrowX, arrowY, Color.Red);

            if (gameHistory.Rounds.Length > 0)
            {
                Raylib.DrawCircle(globalState.PastPlayer.PositionX, globalState.PastPlayer.PositionY, 20, Color.Green);
                // draw a arrow pointing in player direction 40 pixels long
                int pastArrowX = (int)(globalState.PastPlayer.PositionX - 40 * Math.Cos(globalState.PastPlayer.Direction * Math.PI / 180));
                int pastArrowY = (int)(globalState.PastPlayer.PositionY - 40 * Math.Sin(globalState.PastPlayer.Direction * Math.PI / 180));
                Raylib.DrawLine(globalState.PastPlayer.PositionX, globalState.PastPlayer.PositionY, pastArrowX, pastArrowY, Color.Green);
            }


            Raylib.DrawCircle(globalState.Enemy.X, globalState.Enemy.Y, globalState.Enemy.HitboxRadius, Color.Blue);

            Raylib.DrawText(text, 12, 12, 20, textColor);

            Raylib.EndDrawing();
            timeSliceCounter++;
        }

        // Save the game history to a file DEBUGGING
        string filePath = "game_history.txt";
        gameHistory.SaveToFile(filePath);
        Console.WriteLine($"Game history saved to {filePath}");

        Raylib.CloseWindow();
    }
}