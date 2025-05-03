using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Entities;

public class Enemy : Entity
{
    public Enemy(float positionX, float positionY, int attackSpeed = 1, int hitboxRadius = 40)
    {
        PositionX = positionX;
        PositionY = positionY;
        AttackSpeed = attackSpeed;
        HitboxRadius = hitboxRadius;
    }

    public int AttackSpeed;
    public int HitboxRadius;

    public void ReadInputs()
    {
    }

    public void Update(float deltaTime)
    {

    }

    public void Draw()
    {
        DrawCircle((int)PositionX, (int)PositionY, HitboxRadius, Color.Blue);
    }
}

class Game
{
    public int RoundDuration;
}
