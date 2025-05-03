namespace Globals;

using Types;
class GlobalState
{
    public int Score { get; set; }
    public Player Player { get; set; } = new Player();
    public Enemy Enemy { get; set; } = new Enemy(GlobalVariables.WindowSizeX / 2, GlobalVariables.WindowSizeY / 2, 1, 40);
    public int RoundDuration { get; set; } = 3;
    public int TransitionDuration { get; set; } = 3;
}