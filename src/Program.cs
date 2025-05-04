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

        InitWindow(GlobalVariables.WindowSizeX, GlobalVariables.WindowSizeY, "Balance of Time");
        SetTargetFPS(60);

        GlobalState globalState = new GlobalState();
        globalState.CurrentPhase = GamePhase.Menu;

        while (!WindowShouldClose())
        {
            // get delta time
            float deltaTime = GetFrameTime();
            globalState.CurrentFrame++;

            // Read Input and Update
            switch (globalState.CurrentPhase)
            {
                case GamePhase.Menu:
                    if (IsMouseButtonPressed(MouseButton.Left))
                    {
                        globalState.CurrentPhase = GamePhase.Round;
                        GlobalVariables.BackgroundColor = Color.White;
                        globalState.CurrentFrame = 0;
                        globalState.Player.PositionX = GetMouseX();
                        globalState.Player.PositionY = GetMouseY();
                    }

                    BeginDrawing();
                    ClearBackground(GlobalVariables.BackgroundColor);
                    DrawText("Click to start", GlobalVariables.WindowSizeX / 2 - 50, GlobalVariables.WindowSizeY / 2 - 10, 20, Color.Black);
                    EndDrawing();
                    break;
                case GamePhase.Round:
                    globalState.Player.ReadInputs(globalState);
                    globalState.Enemy.ReadInputs(globalState.CurrentFrame, globalState.Player);

                    //Update Player and enemies only in round
                    globalState.Player.Update(deltaTime, globalState.CurrentFrame, globalState.ProjectileList, globalState.BarrierList, globalState.PastTrapList, globalState.NonProjectileList, globalState.CurrentRound.Id, globalState.FutureSpellTypeSelected);
                    globalState.Enemy.Attack(globalState.CurrentFrame, globalState);
                    globalState.Enemy.Update(deltaTime, globalState);
                    globalState.BarrierList.ForEach(barrier => barrier.Update(globalState.CurrentRound.Id));
                    globalState.PastTrapList.ForEach(pastTrap => pastTrap.Update(globalState.CurrentRound.Id, globalState.NonProjectileList));

                    TimeSlice[] currentFrameTimeSlices = [];
                    foreach (var round in globalState.GameHistory.Rounds)
                    {
                        foreach (var timeSlice in round.History)
                        {
                            if (timeSlice.Time == globalState.CurrentFrame)
                            {
                                currentFrameTimeSlices = currentFrameTimeSlices.Append(timeSlice).ToArray();
                            }
                        }
                    }

                    var zippedPlayersAndTimeSlices = globalState.PastPlayers.Zip(currentFrameTimeSlices, (player, timeSlice) => new { player, timeSlice });
                    foreach (var pair in zippedPlayersAndTimeSlices)
                    {
                        if (!pair.player.Die)
                        {
                            pair.player.UpdatePast(globalState.CurrentFrame, globalState.ProjectileList, pair.timeSlice);
                        }
                    }

                    // remove any projectiles on die list
                    foreach (Projectile projectile in globalState.ProjectileList)
                    {
                        if (projectile.Die)
                        {
                            globalState.KillList.Add(projectile);
                            continue;
                        }
                        projectile.Update(deltaTime, globalState);
                    }
                    foreach (HitEffect hitEffect in globalState.HitEffectList)
                    {
                        if (hitEffect.Die)
                        {
                            globalState.KillList.Add(hitEffect);
                            continue;
                        }
                        hitEffect.Update(deltaTime, globalState);
                    }

                    foreach (FloatingText damageNumber in globalState.FloatingTextList)
                    {
                        if (damageNumber.Die)
                        {
                            globalState.KillList.Add(damageNumber);
                            continue;
                        }
                        damageNumber.Update(deltaTime);
                    }

                    foreach (Entity entity in globalState.KillList)
                    {
                        if (entity is Projectile projectile)
                        {
                            globalState.ProjectileList.Remove(projectile);
                        }
                        if (entity is FloatingText floatingText)
                        {
                            globalState.FloatingTextList.Remove(floatingText);
                        }
                        if (entity is HitEffect hitEffect)
                        {
                            globalState.HitEffectList.Remove(hitEffect);
                        }
                    }
                    globalState.KillList.Clear();

                    // remove any barriers on die list
                    foreach (Entities.Barrier barrier in globalState.BarrierList)
                    {
                        if (barrier.Die)
                        {
                            globalState.KillList.Add(barrier);
                            continue;
                        }
                    }
                    foreach (Entity entity in globalState.KillList)
                    {
                        globalState.BarrierList.Remove((Entities.Barrier)entity);
                        globalState.NonProjectileList.Remove((Entities.Barrier)entity);
                    }
                    globalState.KillList.Clear();

                    // remove any barriers on die list
                    foreach (Entities.PastTrap pastTrap in globalState.PastTrapList)
                    {
                        if (pastTrap.Die)
                        {
                            globalState.KillList.Add(pastTrap);
                            continue;
                        }
                    }
                    foreach (Entity entity in globalState.KillList)
                    {
                        globalState.PastTrapList.Remove((Entities.PastTrap)entity);
                        globalState.NonProjectileList.Remove((Entities.PastTrap)entity);
                    }
                    globalState.KillList.Clear();

                    if (globalState.CurrentFrame >= globalState.RoundDurationFrames)
                    {
                        globalState.CurrentPhase = GamePhase.Transition;
                        GlobalVariables.BackgroundColor = Color.Green;
                        globalState.GameHistory.AppendToRounds(globalState.CurrentRound);
                        var startX = globalState.CurrentRound.History[0].PlayerPositionX;
                        var startY = globalState.CurrentRound.History[0].PlayerPositionY;
                        var direction = globalState.CurrentRound.History[0].PlayerDirection;
                        var tempPastPlayer = new Player(startX, startY, direction);
                        globalState.PastPlayers = globalState.PastPlayers.Append(tempPastPlayer).ToArray();
                        globalState.NonProjectileList = globalState.NonProjectileList.Append(tempPastPlayer).ToList();
                        globalState.TimeSliceCounter = 0;
                        globalState.CurrentFrame = 0;
                    }
                    // Append to history
                    globalState.CurrentRound.AppendToHistory(new TimeSlice(globalState.Player.PositionX, globalState.Player.PositionY, globalState.Player.Direction, globalState.Player.leftButtonPressed, globalState.CurrentFrame));


                    // CHECK IF DEAD
                    if (globalState.Player.Health <= 0)
                    {
                        globalState.CurrentPhase = GamePhase.GameOver;
                        GlobalVariables.BackgroundColor = Color.DarkPurple;
                    }
                    break;
                case GamePhase.Transition:
                    if (globalState.CurrentFrame >= globalState.TransitionDurationFrames)
                    {
                        globalState.CurrentPhase = GamePhase.Round;
                        GlobalVariables.BackgroundColor = Color.White;
                        globalState.CurrentRound = new Round(globalState.CurrentRound.Id);
                        globalState.CurrentRound.Id = globalState.CurrentRound.Id;

                        globalState.CurrentFrame = 0;
                    }
                    break;
                case GamePhase.GameOver:
                    if (IsMouseButtonPressed(MouseButton.Middle))
                    {
                        // reset game
                        globalState.RestartGame();
                    }

                    BeginDrawing();
                    ClearBackground(GlobalVariables.BackgroundColor);
                    DrawText($"Game over...\n Score: {globalState.Score}\nPress Middle mouse button to replay", GlobalVariables.WindowSizeX / 2 - 50, GlobalVariables.WindowSizeY / 2 - 10, 20, Color.White);
                    EndDrawing();
                    break;
                default:
                    break;
            }
            // Draw
            if (globalState.CurrentPhase != GamePhase.Menu && globalState.CurrentPhase != GamePhase.GameOver)
            {
                BeginDrawing();

                Color backgroundColor = Color.White;

                if (globalState.Player.Hit)
                {
                    backgroundColor = Color.Red;
                }

                ClearBackground(GlobalVariables.BackgroundColor);
                DrawTexture(globalState.Background, 0, 0, backgroundColor);

                // Text
                Color textColor = Color.White;
                string text = globalState.DebugString(globalState.CurrentPhase, globalState.CurrentFrame);
                DrawText(text, 12, 12, 20, textColor);

                // Draw entities
                globalState.Player.Draw();
                if (globalState.GameHistory.Rounds.Length > 0)
                {
                    foreach (var player in globalState.PastPlayers)
                    {
                        if (!player.Die)
                        {
                            player.Draw();
                        }
                    }
                    if (globalState.CurrentPhase != GamePhase.Transition)
                    {
                        globalState.TimeSliceCounter++;
                    }

                }
                globalState.Enemy.Draw();
                foreach (Projectile projectile in globalState.ProjectileList)
                {
                    projectile.Draw();
                }

                foreach (Entities.Barrier barrier in globalState.BarrierList)
                {
                    barrier.Draw();
                }

                foreach (PastTrap pastTrap in globalState.PastTrapList)
                {
                    pastTrap.Draw();
                }

                foreach (HitEffect hitEffect in globalState.HitEffectList)
                {
                    hitEffect.Draw();
                }


                DrawTexture(globalState.Foreground, 0, 0, Color.White);

                foreach (FloatingText floatingText in globalState.FloatingTextList)
                {
                    floatingText.Draw();
                }

                EndDrawing();
            }
        }

        // Save game history to file
        string filePath = "game_history.txt";
        globalState.GameHistory.SaveToFile(filePath);
        Console.WriteLine($"Game history saved to {filePath}");
        CloseWindow();
    }
}
