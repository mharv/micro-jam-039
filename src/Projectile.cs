using Raylib_cs;
using static Raylib_cs.Raylib;
using Types;
using Globals;

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
        EntityType = EntityType.Projectile;
        OriginEntityType = null;

        CurrentSpeed = BaseSpeed;
    }

    public Projectile(float positionX, float positionY, float direction, float turnSpeed, float baseSpeed, float acceleration, int radius, int damage, int framesToLive, EntityType? originEntityType = null, Entity? target = null)
    {
        OriginEntityType = originEntityType;
        EntityType = EntityType.Projectile;

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
    public new int Radius;
    public int Damage;
    public int FramesToLive;
    public Entity? Target;

    public bool Die;
    private float CurrentSpeed;
    public EntityType? OriginEntityType;

    public void Update(float deltaTime, Entity[] nonProjectileList, GlobalState globalState)
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

        // check if projectile collides with anyhting in globalState.NonProjectileList
        foreach (Entity entity in nonProjectileList)
        {
            float deltax = MathF.Abs(entity.PositionX - PositionX);
            float deltay = MathF.Abs(entity.PositionY - PositionY);
            float distance = MathF.Sqrt(MathF.Pow(deltax, 2) + MathF.Pow(deltay, 2));

            if (distance < Radius + entity.Radius)
            {
                if (entity.EntityType == EntityType.PresentPlayer)
                {
                    Console.WriteLine($"HIT___________{entity.EntityType}_by {OriginEntityType}______: {PositionX}, {PositionY}");
                    entity.Health -= Damage;
                    Die = true;
                    break;
                }
                if (entity.EntityType == EntityType.Enemy && OriginEntityType == EntityType.PresentPlayer)
                {
                    Console.WriteLine($"HIT___________{entity.EntityType}_by {OriginEntityType}______: {PositionX}, {PositionY}");
                    // this doesnt do anything for now
                    entity.Health -= Damage;
                    globalState.IncreaseScore(Damage);
                    Die = true;
                    break;
                }
                if (entity.EntityType == EntityType.Enemy && OriginEntityType == EntityType.PastPlayer)
                {
                    // Console.WriteLine($"HIT___________{entity.EntityType}_by {OriginEntityType}______: {PositionX}, {PositionY}");
                    // this doesnt do anything for now
                    // entity.Health -= Damage;
                    // globalState.IncreaseScore(Damage);
                    Die = true;
                    break;
                }
            }
        }

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
