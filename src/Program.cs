using Raylib_cs;
using Globals;
using static Raylib_cs.Raylib;
using Entities;
using Recording;
using Types;


namespace BalanceOfTime;


class Program
{

    public static void Main()
    {
        InitWindow(GlobalVariables.WindowSizeX, GlobalVariables.WindowSizeY, "Hello World");
        SetTargetFPS(60);

        GlobalState globalState = new GlobalState();

        // Timer variables
        globalState.CurrentPhase = GamePhase.Menu;

        GameHistory gameHistory = new GameHistory();
        Round currentRound = new Round();
        int roundId = currentRound.Id;
        int timeSliceCounter = 0;

        int currentFrame = 0;

        while (!WindowShouldClose())
        {
            // get delta time
            float deltaTime = GetFrameTime();
            currentFrame++;

            // Read Input and Update
            switch (globalState.CurrentPhase)
            {
                case GamePhase.Menu:
                    if (IsMouseButtonPressed(MouseButton.Left))
                    {
                        globalState.CurrentPhase = GamePhase.Round;
                        GlobalVariables.BackgroundColor = Color.White;
                        currentFrame = 0;
                    }

                    BeginDrawing();
                    ClearBackground(GlobalVariables.BackgroundColor);
                    DrawText("Click to start", GlobalVariables.WindowSizeX / 2 - 50, GlobalVariables.WindowSizeY / 2 - 10, 20, Color.Black);
                    EndDrawing();
                    break;
                case GamePhase.Round:
                    globalState.Player.ReadInputs();
                    globalState.Enemy.ReadInputs(currentFrame, globalState.Player);

                    //Update Player and enemies only in round
                    globalState.Player.Update(deltaTime, globalState.ProjectileList);

                    globalState.Enemy.Attack(currentFrame);
                    globalState.Enemy.Update(deltaTime, currentFrame, globalState.ProjectileList);
                    Console.WriteLine(globalState.ProjectileList.Count);
                    foreach (Projectile projectile in globalState.ProjectileList)
                    {
                        projectile.Update(deltaTime);
                    }

                    if (currentFrame >= globalState.RoundDurationFrames)
                    {
                        globalState.CurrentPhase = GamePhase.Transition;
                        GlobalVariables.BackgroundColor = Color.Green;
                        gameHistory.AppendToRounds(currentRound);
                        timeSliceCounter = 0;
                        currentFrame = 0;
                    }

                    break;
                case GamePhase.Transition:
                    if (currentFrame >= globalState.TransitionDurationFrames)
                    {
                        globalState.CurrentPhase = GamePhase.Round;
                        GlobalVariables.BackgroundColor = Color.White;
                        currentRound = new Round(roundId);
                        roundId = currentRound.Id;

                        currentFrame = 0;
                    }
                    break;
                default:
                    break;
            }
            // get past player if exists
            if (gameHistory.Rounds.Length > 0 && globalState.CurrentPhase == GamePhase.Round)
            {
                Round lastRound = gameHistory.Rounds[gameHistory.Rounds.Length - 1];
                globalState.PastPlayer.PositionX = lastRound.History[timeSliceCounter].PlayerPositionX;
                globalState.PastPlayer.PositionY = lastRound.History[timeSliceCounter].PlayerPositionY;
                globalState.PastPlayer.Direction = lastRound.History[timeSliceCounter].PlayerDirection;


            }
            if (globalState.CurrentPhase == GamePhase.Round)
            {
                currentRound.AppendToHistory(new TimeSlice(globalState.Player.PositionX, globalState.Player.PositionY, globalState.Player.Direction, currentFrame));
            }
            // Draw
            if (globalState.CurrentPhase != GamePhase.Menu)
            {
                BeginDrawing();

                ClearBackground(GlobalVariables.BackgroundColor);

                // Text
                Color textColor = Color.Black;
                string text = globalState.DebugString(globalState.CurrentPhase, currentFrame);
                DrawText(text, 12, 12, 20, textColor);

                // Draw entities
                globalState.Player.Draw();
                if (gameHistory.Rounds.Length > 0 && globalState.CurrentPhase == GamePhase.Round)
                {
                    globalState.PastPlayer.Draw();

                    timeSliceCounter++;
                }
                globalState.Enemy.Draw();
                foreach (Projectile projectile in globalState.ProjectileList)
                {
                    projectile.Draw();
                }

                EndDrawing();
            }
        }

        // Save game history to file
        string filePath = "game_history.txt";
        gameHistory.SaveToFile(filePath);
        Console.WriteLine($"Game history saved to {filePath}");
        CloseWindow();
    }
}
