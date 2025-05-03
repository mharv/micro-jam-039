using Types;
namespace Entities;


public class Entity
{
    EntityType EntityType;
    public float PositionX;
    public float PositionY;

    public (float, float) GetPosition()
    {
        return (PositionX, PositionY);
    }
}
