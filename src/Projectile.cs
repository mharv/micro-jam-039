using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Entities;

public class Projectile
{
    // constructor
    public Projectile()
    {
        PositionX = 0.0f;
        PositionY = 0.0f;
        Direction = 0.0f;
        TurnSpeed = 0.0f;
        BaseSpeed = 0;
        Acceleration = 0.0f;
        Radius = 0;
        Damage = 0;

        CurrentSpeed = BaseSpeed;
    }

    public Projectile(float positionX, float positionY, float direction, float turnSpeed, float baseSpeed, float acceleration, int radius, int damage)
    {
        PositionX = positionX;
        PositionY = positionY;
        Direction = direction;
        TurnSpeed = turnSpeed;
        BaseSpeed = baseSpeed;
        Acceleration = acceleration;
        Radius = radius;
        Damage = damage;

        CurrentSpeed = BaseSpeed;
    }

    public float PositionX;
    public float PositionY;
    public float Direction;
    public float TurnSpeed;
    public float BaseSpeed;
    public float Acceleration;
    public int Radius;
    public int Damage;

    private float CurrentSpeed;

    public void Update(float deltaTime)
    {
        BaseSpeed += Acceleration * deltaTime;
        Direction += TurnSpeed * deltaTime;

        float rads = Direction * MathF.PI / 180;

        float dx = MathF.Cos(rads) * -BaseSpeed;
        float dy = MathF.Sin(rads) * -BaseSpeed;

        PositionX += dx;
        PositionY += dy;
    }

    public void Draw()
    {
        DrawCircle((int)PositionX, (int)PositionY, Radius, Color.Red);

        float rads = Direction * MathF.PI / 180;

        int arrowX = (int)(PositionX - 40 * Math.Cos(Direction * Math.PI / 180));
        int arrowY = (int)(PositionY - 40 * Math.Sin(Direction * Math.PI / 180));
        DrawLine((int)PositionX, (int)PositionY, arrowX, arrowY, Color.Red);
    }
}
