using Raylib_cs;
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

    public int TakeDamage(int damage, List<FloatingText> floatingText)
    {
        string damageText = "-" + damage.ToString();
        floatingText.Add(new FloatingText(PositionX, PositionY, damageText, 20, Color.Red, 60, 1.0f, -1.0f));
        HitTimer = 0;
        Hit = true;
        Health -= damage;
        return Health;
    }
}
