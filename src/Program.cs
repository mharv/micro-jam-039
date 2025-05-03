using Raylib_cs;
using Globals;
using Entities;
using static Raylib_cs.Raylib;

namespace BalanceOfTime;

enum GamePhase { Menu, Round, Transition }

class Program
{
    public static void Main()
    {
        InitWindow(GlobalVariables.WindowSizeX, GlobalVariables.WindowSizeY, "Hello World");
        SetTargetFPS(60);

        Player player = new Player();
        Entities.Enemy enemy = new Entities.Enemy(GlobalVariables.WindowSizeX / 2, GlobalVariables.WindowSizeY / 2);
        GlobalState globalState = new GlobalState(player, enemy);

        player.AcquireTarget(enemy);

        // Timer variables
        GamePhase currentPhase = GamePhase.Menu;
        float currentTimer = 0f;

        while (!WindowShouldClose())
        {
            // get delta time
            float deltaTime = GetFrameTime();
            currentTimer += deltaTime;

            // Read Input and Update
            switch (currentPhase)
            {
                case GamePhase.Menu:
                    if (IsMouseButtonPressed(MouseButton.Left))
                    {
                        currentPhase = GamePhase.Round;
                        GlobalVariables.BackgroundColor = Color.White;
                        currentTimer = 0f;
                    }

                    BeginDrawing();
                    ClearBackground(GlobalVariables.BackgroundColor);
                    DrawText("Click to start", GlobalVariables.WindowSizeX / 2 - 50, GlobalVariables.WindowSizeY / 2 - 10, 20, Color.Black);
                    EndDrawing();
                    break;
                case GamePhase.Round:
                    player.ReadInputs();
                    enemy.ReadInputs();

                    //Update Player and enemies only in round
                    player.Update(deltaTime);
                    enemy.Update(deltaTime);

                    if (currentTimer >= globalState.RoundDuration)
                    {
                        currentPhase = GamePhase.Transition;
                        GlobalVariables.BackgroundColor = Color.Green;
                        currentTimer = 0f;
                    }
                    break;
                case GamePhase.Transition:
                    if (currentTimer >= globalState.TransitionDuration)
                    {
                        currentPhase = GamePhase.Round;
                        GlobalVariables.BackgroundColor = Color.White;
                        currentTimer = 0f;
                    }
                    break;
                default:
                    break;
            }

            // Draw
            if (currentPhase != GamePhase.Menu)
            {
                BeginDrawing();
                ClearBackground(GlobalVariables.BackgroundColor);

                // Text
                Color textColor = Color.Black;
                string text = $"X: {player.PositionX,-5}\n" +
                                $"Y: {player.PositionY,-5}\n" +
                                $"Left: {player.leftButtonState,-5}\n" +
                                $"Middle: {player.middleButtonState,-5}\n" +
                                $"Right: {player.rightButtonState,-5}\n" +
                                $"Direction: {player.Direction,-5}\n" +
                                $"Round: {currentPhase,-5}\n" +
                                $"Timer: {currentTimer,-5}\n" +
                                $"Score: {globalState.Score,-5}\n"; ;

                DrawText(text, 12, 12, 20, textColor);

                // Draw entities
                player.Draw();
                enemy.Draw();

                EndDrawing();
            }
        }

        CloseWindow();
    }
}
