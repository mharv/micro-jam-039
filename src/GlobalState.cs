namespace Globals;

using Entities;

class GlobalState
{
    public GlobalState(Player player, Enemy enemy)
    {
        Player = player;
        Enemy = enemy;
        Score = 0;
        RoundDuration = 15;
        TransitionDuration = 3;
    }
    public Player Player { get; set; }
    public Enemy Enemy { get; set; }
    public int Score { get; set; }
    public int RoundDuration { get; set; }
    public int TransitionDuration { get; set; }
}
