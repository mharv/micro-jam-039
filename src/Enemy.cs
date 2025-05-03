using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Entities;

public class Enemy
{
    public Enemy(int x, int y, int attackSpeed = 1, int hitboxRadius = 40)
    {
        X = x;
        Y = y;
        AttackSpeed = attackSpeed;
        HitboxRadius = hitboxRadius;
    }
    public int X;
    public int Y;
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
        DrawCircle(X, Y, HitboxRadius, Color.Blue);
    }

    public (int, int) GetPosition()
    {
        return (X, Y);
    }
}

class Game
{
    public int RoundDuration;
}
