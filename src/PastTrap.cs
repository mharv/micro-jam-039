using Raylib_cs;
using static Raylib_cs.Raylib;
using Types;

namespace Entities;


public class PastTrap : Entity
{
    public PastTrap(float locX, float locY, int roundId)
    {
        PositionX = locX;
        PositionY = locY;
        EntityType = EntityType.FutureSpell;
        RoundCast = roundId;
        PowerBarCost = 40;
        Radius = 40;
    }
    public bool Status = false;
    public bool Die { get; set; } = false;
    public int RoundCast { get; set; } = 0;
    public int PowerBarCost { get; set; } = 40;

    public void Update(int Round, List<Entity> nonProjectileList)
    {
        if (RoundCast != Round)
        {
            Status = true;
        }

        if (Status)
        {

            foreach (Entity entity in nonProjectileList)
            {
                float deltax = MathF.Abs(entity.PositionX - PositionX);
                float deltay = MathF.Abs(entity.PositionY - PositionY);
                float distance = MathF.Sqrt(MathF.Pow(deltax, 2) + MathF.Pow(deltay, 2));

                if (distance < (Radius * .25) + entity.Radius)
                {
                    if (entity.EntityType == EntityType.PastPlayer && entity is Player pastPlayer)
                    {
                        Console.WriteLine($"TRAPPED___________{entity.EntityType}_by {EntityType}______: {PositionX}, {PositionY}");
                        pastPlayer.Die = true;
                        Die = true;
                        break;
                    }
                }
            }
        }
    }

    public void Draw()
    {
        var color = Status ? Color.Blue : Color.LightGray;
        // Create a line of small circles
        DrawCircle((int)PositionX, (int)PositionY, Radius, color); // Adjust radius and color as needed

    }
}
