using Types;
namespace Entities;


public class Entity
{
    public EntityType EntityType;
    public float PositionX;
    public float PositionY;

    public int Health;
    public int Radius;

    public (float, float) GetPosition()
    {
        return (PositionX, PositionY);
    }
}
