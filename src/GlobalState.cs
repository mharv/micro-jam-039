namespace Globals;

using Raylib_cs;
using Entities;
using Types;

class GlobalState
{
    public GlobalState()
    {
        Player player = new Player();
        Entities.Enemy enemy = new Entities.Enemy(GlobalVariables.WindowSizeX / 2, GlobalVariables.WindowSizeY / 2, Types.Difficulty.Easy);

        player.EntityType = EntityType.PresentPlayer;
        enemy.EntityType = EntityType.Enemy;
        player.AcquireTarget(enemy);
        enemy.AcquireTarget(player);

        Player = player;
        PastPlayers = [];
        Enemy = enemy;
        Score = 0;
        RoundDurationFrames = 10 * 60; // 3 seconds
        TransitionDurationFrames = 3 * 60; // 3 seconds

        Background = Raylib.LoadTexture("assets/background.png");
        Foreground = Raylib.LoadTexture("assets/foreground.png");
    }

    public Texture2D Background;
    public Texture2D Foreground;
    public GamePhase CurrentPhase { get; set; } = GamePhase.Menu;
    public List<Projectile> ProjectileList = new List<Projectile>();
    public List<Entity> KillList = new List<Entity>();
    public Player Player { get; set; }
    public Player[] PastPlayers { get; set; } = [];
    public Enemy Enemy { get; set; }
    public int Score { get; set; }
    public int RoundDurationFrames { get; set; } = 0;
    public int TransitionDurationFrames { get; set; } = 0;

    public string DebugString(GamePhase currentPhase, int currentFrame)
    {
        return $"X: {Player.PositionX,-5}\n" +
                $"Y: {Player.PositionY,-5}\n" +
                $"Left: {Player.leftButtonState,-5}\n" +
                $"Middle: {Player.middleButtonState,-5}\n" +
                $"Right: {Player.rightButtonState,-5}\n" +
                $"Direction: {Player.Direction,-5}\n" +
                $"Round: {currentPhase,-5}\n" +
                $"Timer: {currentFrame,-5}\n" +
                $"Score: {Score,-5}\n"; ; ;
    }
}
