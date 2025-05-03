using Raylib_cs;
using static Raylib_cs.Raylib;
using Types;

namespace Entities;


public class Barrier : Entity
{
    public Barrier(float locX, float locY)
    {
        PositionX = locX;
        PositionY = locY;
        EntityType = EntityType.Barrier;

    }
    public bool Status = false;

    public void Draw()
    {
        var color = Status ? Color.Green : Color.LightGray;
        // Create a line of small circles
        DrawCircle((int)PositionX, (int)PositionY, 5, color); // Adjust radius and color as needed

    }
}