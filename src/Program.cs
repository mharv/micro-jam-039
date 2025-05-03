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
                    globalState.Player.Update(deltaTime, currentFrame, globalState.ProjectileList);
                    globalState.Enemy.Attack(currentFrame);
                    globalState.Enemy.Update(deltaTime, currentFrame, globalState.ProjectileList);

                    TimeSlice[] currentFrameTimeSlices = [];
                    foreach (var round in gameHistory.Rounds)
                    {
                        foreach (var timeSlice in round.History)
                        {
                            if (timeSlice.Time == currentFrame)
                            {
                                currentFrameTimeSlices = currentFrameTimeSlices.Append(timeSlice).ToArray();
                            }
                        }
                    }

                    var zippedPlayersAndTimeSlices = globalState.PastPlayers.Zip(currentFrameTimeSlices, (player, timeSlice) => new { player, timeSlice });
                    foreach (var pair in zippedPlayersAndTimeSlices)
                    {
                        pair.player.UpdatePast(currentFrame, globalState.ProjectileList, pair.timeSlice);
                    }


                    foreach (Projectile projectile in globalState.ProjectileList)
                    {
                        if (projectile.Die)
                        {
                            globalState.KillList.Add(projectile);
                            continue;
                        }
                        projectile.Update(deltaTime, globalState.NonProjectileList.ToArray(), globalState);
                    }

                    foreach (Entity entity in globalState.KillList)
                    {
                        globalState.ProjectileList.Remove((Projectile)entity);
                    }

                    globalState.KillList.Clear();

                    if (currentFrame >= globalState.RoundDurationFrames)
                    {
                        globalState.CurrentPhase = GamePhase.Transition;
                        GlobalVariables.BackgroundColor = Color.Green;
                        gameHistory.AppendToRounds(currentRound);
                        var startX = currentRound.History[0].PlayerPositionX;
                        var startY = currentRound.History[0].PlayerPositionY;
                        var direction = currentRound.History[0].PlayerDirection;
                        var tempPastPlayer = new Player(startX, startY, direction);
                        globalState.PastPlayers = globalState.PastPlayers.Append(tempPastPlayer).ToArray();
                        globalState.NonProjectileList = globalState.NonProjectileList.Append(tempPastPlayer).ToList();
                        timeSliceCounter = 0;
                        currentFrame = 0;
                    }
                    // Append to history
                    currentRound.AppendToHistory(new TimeSlice(globalState.Player.PositionX, globalState.Player.PositionY, globalState.Player.Direction, globalState.Player.leftButtonPressed, currentFrame));
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
            // Draw
            if (globalState.CurrentPhase != GamePhase.Menu)
            {
                BeginDrawing();

                ClearBackground(GlobalVariables.BackgroundColor);
                DrawTexture(globalState.Background, 0, 0, Color.White);

                // Text
                Color textColor = Color.White;
                string text = globalState.DebugString(globalState.CurrentPhase, currentFrame);
                DrawText(text, 12, 12, 20, textColor);

                // Draw entities
                globalState.Player.Draw();
                if (gameHistory.Rounds.Length > 0 && globalState.CurrentPhase == GamePhase.Round)
                {
                    foreach (var player in globalState.PastPlayers)
                    {
                        player.Draw();
                    }
                    timeSliceCounter++;
                }
                globalState.Enemy.Draw();
                foreach (Projectile projectile in globalState.ProjectileList)
                {
                    projectile.Draw();
                }

                DrawTexture(globalState.Foreground, 0, 0, Color.White);
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
