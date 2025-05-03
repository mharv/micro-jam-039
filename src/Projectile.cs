using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Entities;

public class Projectile : Entity
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
        FramesToLive = 0;
        Target = null;
        Die = true;

        CurrentSpeed = BaseSpeed;
    }

    public Projectile(float positionX, float positionY, float direction, float turnSpeed, float baseSpeed, float acceleration, int radius, int damage, int framesToLive, Entity? target = null)
    {
        PositionX = positionX;
        PositionY = positionY;
        Direction = direction;
        TurnSpeed = turnSpeed;
        BaseSpeed = baseSpeed;
        Acceleration = acceleration;
        Radius = radius;
        Damage = damage;
        FramesToLive = framesToLive;
        Target = target;
        Die = false;

        CurrentSpeed = BaseSpeed;
    }

    public float Direction;
    public float TurnSpeed;
    public float BaseSpeed;
    public float Acceleration;
    public int Radius;
    public int Damage;
    public int FramesToLive;
    public Entity? Target;

    public bool Die;
    private float CurrentSpeed;

    public void Update(float deltaTime)
    {
        BaseSpeed += Acceleration * deltaTime;
        if (Target != null)
        {
            float radians = Direction * MathF.PI / 180;
            float forwardX = MathF.Cos(radians);
            float forwardY = MathF.Sin(radians);

            float toTargetX = Target.PositionX - PositionX;
            float toTargetY = Target.PositionY - PositionY;

            float toTargetLength = MathF.Sqrt(toTargetX * toTargetX + toTargetY * toTargetY);
            if (toTargetLength < float.Epsilon)
                return;

            toTargetX /= toTargetLength;
            toTargetY /= toTargetLength;

            float forwardDotTarget = forwardX * toTargetX + forwardY * toTargetY;
            if (MathF.Abs(forwardDotTarget - 1.0f) < float.Epsilon)
                return;

            float rightX = -forwardY;
            float rightY = forwardX;
            float rightDotTarget = rightX * toTargetX + rightY * toTargetY;

            float rotationSign = -MathF.Sign(rightDotTarget);
            float maxAngle = MathF.Acos(Math.Clamp(forwardDotTarget, -1.0f, 1.0f));
            float rotationAngle = rotationSign * MathF.Min(TurnSpeed * deltaTime, maxAngle);
            Direction += rotationAngle;
        }
        else
        {
            Direction += TurnSpeed * deltaTime;
        }

        float rads = Direction * MathF.PI / 180;

        float dx = MathF.Cos(rads) * -BaseSpeed;
        float dy = MathF.Sin(rads) * -BaseSpeed;

        PositionX += dx;
        PositionY += dy;

        if (FramesToLive-- <= 0)
        {
            Die = true;
        }
    }

    public void Draw()
    {
        DrawCircle((int)PositionX, (int)PositionY, Radius, Color.Red);

        float rads = Direction * MathF.PI / 180;

        int arrowX = (int)(PositionX - 40 * Math.Cos(rads));
        int arrowY = (int)(PositionY - 40 * Math.Sin(rads));
        DrawLine((int)PositionX, (int)PositionY, arrowX, arrowY, Color.Red);
    }
}
