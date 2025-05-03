using Types;
namespace Entities;


public class Entity
{
    public EntityType EntityType;
    public float PositionX;
    public float PositionY;

    public int Health;
    public int Radius;
    public bool Hit;
    public int HitTimer;

    public (float, float) GetPosition()
    {
        return (PositionX, PositionY);
    }

    public int TakeDamage(int damage)
    {
        HitTimer = 0;
        Hit = true;
        Health -= damage;
        return Health;
    }

}
