namespace Globals;

using Raylib_cs;
using Entities;
using Types;
using Recording;

public class GlobalState
{
    public GlobalState()
    {
        Player player = new Player();
        Entities.Enemy enemy = new Entities.Enemy(GlobalVariables.WindowSizeX / 2, GlobalVariables.WindowSizeY / 2, Types.Difficulty.Easy);


        player.EntityType = EntityType.PresentPlayer;
        enemy.EntityType = EntityType.Enemy;

        NonProjectileList.Add(player);
        NonProjectileList.Add(enemy);

        player.AcquireTarget(enemy);
        enemy.AcquireTarget(player);

        Player = player;
        PastPlayers = [];
        Enemy = enemy;
        Score = 0;
        RoundDurationFrames = 10 * 60; // 3 seconds
        TransitionDurationFrames = 3 * 60; // 3 seconds
        GameHistory = new GameHistory();
        CurrentRound = new Round();
        TimeSliceCounter = 0;
        CurrentFrame = 0;
        ProjectileList = new List<Projectile>();
        BarrierList = new List<Barrier>();
        KillList = new List<Entity>();


        Background = Raylib.LoadTexture("assets/background.png");
        Foreground = Raylib.LoadTexture("assets/foreground.png");
    }

    public GameHistory GameHistory { get; set; } = new GameHistory();
    public Round CurrentRound { get; set; } = new Round();
    public int TimeSliceCounter { get; set; } = 0;
    public int CurrentFrame { get; set; } = 0;
    public Texture2D Background;
    public Texture2D Foreground;
    public GamePhase CurrentPhase { get; set; } = GamePhase.Menu;
    public List<Projectile> ProjectileList = new List<Projectile>();
    public List<Barrier> BarrierList = new List<Barrier>();
    public List<Entity> NonProjectileList = new List<Entity>();
    public List<FloatingText> FloatingTextList = new List<FloatingText>();
    public List<Entity> KillList = new List<Entity>();
    public Player Player { get; set; }
    public Player[] PastPlayers { get; set; } = [];
    public Enemy Enemy { get; set; }
    public int Score { get; set; }
    public int RoundDurationFrames { get; set; } = 0;
    public int TransitionDurationFrames { get; set; } = 0;
    public FutureSpellType FutureSpellTypeSelected = FutureSpellType.Barrier;

    public void IncreaseScore(int amount)
    {
        Score += amount;
    }

    public void RestartGame()
    {
        Player = new Player();
        Enemy = new Entities.Enemy(GlobalVariables.WindowSizeX / 2, GlobalVariables.WindowSizeY / 2, Types.Difficulty.Easy);
        Player.EntityType = EntityType.PresentPlayer;
        Enemy.EntityType = EntityType.Enemy;

        NonProjectileList.Clear();
        NonProjectileList.Add(Player);
        NonProjectileList.Add(Enemy);

        Player.AcquireTarget(Enemy);
        Enemy.AcquireTarget(Player);

        Score = 0;
        CurrentRound = new Round();
        TimeSliceCounter = 0;
        CurrentFrame = 0;
        ProjectileList.Clear();
        BarrierList.Clear();
        GameHistory = new GameHistory();
        KillList.Clear();
        PastPlayers = [];
        CurrentPhase = GamePhase.Menu;

        GlobalVariables.BackgroundColor = Color.Red;
    }

    public string DebugString(GamePhase currentPhase, int currentFrame)
    {
        // return $"\tX: {Player.PositionX,-5}\n" +
        //         $"\tY: {Player.PositionY,-5}\n" +
        //         $"\tLeft: {Player.leftButtonState,-5}\n" +
        //         $"\tMiddle: {Player.middleButtonState,-5}\n" +
        //         $"\tRight: {Player.rightButtonState,-5}\n" +
        //         $"\tDirection: {Player.Direction,-5}\n" +
        //         $"\tRound: {currentPhase,-5}\n" +
        //         $"\tTimer: {currentFrame,-5}\n" +
        //         $"\tScore: {Score,-5}\n" +
        //         $"\tHealth: {Player.Health,-5}\n" +
        //         $"\tFuture power: {Player.FuturePowerBar,-5}\n";
        return $"\n\n\n\tSpell: {FutureSpellTypeSelected,-5}\n";
    }
}
