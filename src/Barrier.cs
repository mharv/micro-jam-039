using Raylib_cs;
using static Raylib_cs.Raylib;
using Types;

namespace Entities;


public class Barrier : Entity
{
    public Barrier(float locX, float locY, int roundId)
    {
        PositionX = locX;
        PositionY = locY;
        EntityType = EntityType.FutureSpell;
        RoundCast = roundId;
        PowerBarCost = 5;
    }
    public bool Status = false;
    public bool Die { get; set; } = false;
    public int RoundCast { get; set; } = 0;
    public int PowerBarCost { get; set; } = 30;

    public void Update(int Round)
    {
        if (RoundCast != Round)
        {
            Status = true;
        }
    }

    public void Draw()
    {
        var color = Status ? Color.Green : Color.LightGray;
        // Create a line of small circles
        DrawCircle((int)PositionX, (int)PositionY, 5, color); // Adjust radius and color as needed

    }
}
