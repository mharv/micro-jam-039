namespace Entities;

public class Entity
{
    public float PositionX;
    public float PositionY;

    public (float, float) GetPosition()
    {
        return (PositionX, PositionY);
    }
}
