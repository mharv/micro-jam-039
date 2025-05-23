﻿using System.Runtime.InteropServices.JavaScript;
using Raylib_cs;
using Globals;
using static Raylib_cs.Raylib;
using Entities;
using Recording;
using Types;


namespace BalanceOfTime
{


    public partial class Program
    {
        public static GlobalState globalState;

        public static void Main()
        {
            InitWindow(GlobalVariables.WindowSizeX, GlobalVariables.WindowSizeY, "Balance of Time");
            SetTargetFPS(60);
            globalState = new GlobalState();

            globalState.CurrentPhase = GamePhase.Menu;


            //while (!WindowShouldClose())
            //{
            //    UpdateFrame();
            //}
            //
            //// Save game history to file
            //string filePath = "game_history.txt";
            //globalState.GameHistory.SaveToFile(filePath);
            //Console.WriteLine($"Game history saved to {filePath}");
            //UnloadMusicStream(globalState.Bgm1);
            //UnloadMusicStream(globalState.Bgm2);
            //UnloadMusicStream(globalState.Bgm3);
            //UnloadMusicStream(globalState.Bgm4);
            //UnloadMusicStream(globalState.Bgm5);
            //UnloadMusicStream(globalState.BgmTransition);
            //CloseWindow();
        }

        [JSExport]
        public static void UpdateFrame()
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
                        globalState.CurrentPhase = GamePhase.RoundStart;
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
                case GamePhase.RoundStart:
                    globalState.CurrentPhase = GamePhase.Round;
                    globalState.Player.Health = globalState.Player.MaxHealth;
                    foreach (Projectile projectile in globalState.ProjectileList)
                    {
                        globalState.KillList.Add(projectile);
                    }
                    foreach (Entity entity in globalState.KillList)
                    {
                        if (entity is Projectile projectile)
                        {
                            globalState.ProjectileList.Remove(projectile);
                        }
                    }
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
                        globalState.CurrentPhase = GamePhase.TransitionStart;
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
                case GamePhase.TransitionStart:
                    globalState.CurrentPhase = GamePhase.Transition;
                    globalState.HitEffectList.Add(globalState.TransitionEffect.Clone());
                    break;
                case GamePhase.Transition:
                    foreach (HitEffect hitEffect in globalState.HitEffectList)
                    {
                        if (hitEffect.Die)
                        {
                            globalState.KillList.Add(hitEffect);
                            continue;
                        }
                        hitEffect.Update(deltaTime, globalState);
                    }
                    foreach (Entity entity in globalState.KillList)
                    {
                        if (entity is HitEffect hitEffect)
                        {
                            globalState.HitEffectList.Remove(hitEffect);
                        }
                    }
                    if (globalState.CurrentFrame >= globalState.TransitionDurationFrames)
                    {
                        globalState.CurrentPhase = GamePhase.RoundStart;
                        GlobalVariables.BackgroundColor = Color.White;
                        globalState.CurrentRound = new Round(globalState.CurrentRound.Id);
                        globalState.CurrentRound.Id = globalState.CurrentRound.Id;

                        globalState.CurrentFrame = 0;
                        globalState.Update();
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

                string roundText = "Round: " + globalState.CurrentRound.Id.ToString();
                string scoreText = "Score: " + globalState.Score.ToString();
                //Some cool magic numbers here
                DrawText(roundText, 68, 20, 30, Color.White);
                DrawText(scoreText, 500, 20, 30, Color.White);
                globalState.Player.DrawUI(globalState.FutureSpellTypeSelected);

                DrawTexture(globalState.ForegroundIcons, 0, 0, Color.White);

                foreach (FloatingText floatingText in globalState.FloatingTextList)
                {
                    floatingText.Draw();
                }

                EndDrawing();
            }
        }
    }
}
