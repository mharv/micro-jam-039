namespace Types;

class Player
{
    // constructor
    public Player()
    {
        PositionX = 0;
        PositionY = 0;
        Direction = 0;
    }
    public int PositionX;
    public int PositionY;
    public int Direction;
}

class Enemy
{
    public Enemy(int x, int y, int attackSpeed = 1, int hitboxRadius = 20)
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
}

class Game
{
    public int RoundDuration;
}