using Raylib_cs;
using static Raylib_cs.Raylib;
using Types;
using Globals;

namespace Entities;

public class Projectile : Entity
{
    public int AnimationSpeed;
    public float Direction;
    public float TurnSpeed;
    public float BaseSpeed;
    public float Acceleration;
    public new int Radius;
    public int Damage;
    public int FramesToLive;
    public Entity? Target;
    public Texture2D Texture;
    public bool LoadedTexture;
    public int AnimationFrames;
    public bool Die;
    private float CurrentSpeed;
    private int AnimFramesCounter;
    private int CurrentAnimFrame;
    public EntityType? OriginEntityType;

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
        AnimationFrames = 1;
        AnimationSpeed = 15;
        AnimFramesCounter = 0;
        CurrentAnimFrame = 0;
        OriginEntityType = null;

        CurrentSpeed = BaseSpeed;
    }

    public Projectile(float positionX, float positionY, float direction, float turnSpeed, float baseSpeed, float acceleration, int radius, int damage, int framesToLive, EntityType? originEntityType = null, Texture2D texture = new Texture2D(), int frames = 1, Entity? target = null)
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
        Texture = texture;
        AnimationFrames = frames;
        AnimationSpeed = 15;
        AnimFramesCounter = 0;
        CurrentAnimFrame = 0;

        CurrentSpeed = BaseSpeed;
    }

    public void Update(float deltaTime, GlobalState globalState)
    {
        Entity[] nonProjectileList = globalState.NonProjectileList.ToArray();
        List<FloatingText> floatingTextList = globalState.FloatingTextList;
        // Animation Logic
        AnimFramesCounter++;
        if (AnimFramesCounter >= (60 / AnimationSpeed))
        {
            AnimFramesCounter = 0;
            CurrentAnimFrame++;
            if (CurrentAnimFrame > AnimationFrames)
            {
                CurrentAnimFrame = 0;
            }
        }

        // Gameplay Logic
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
                    entity.TakeDamage(Damage, floatingTextList);
                    Die = true;
                    break;
                }
                if (entity.EntityType == EntityType.Enemy && OriginEntityType == EntityType.PresentPlayer)
                {
                    Console.WriteLine($"HIT___________{entity.EntityType}_by {OriginEntityType}______: {PositionX}, {PositionY}");
                    // this doesnt do anything for now
                    entity.TakeDamage(Damage, floatingTextList);
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
        if (Texture.Id != 0)
        {
            int realWidth = Texture.Width / AnimationFrames;
            Rectangle sourceRect = new Rectangle(CurrentAnimFrame * realWidth, 0, Texture.Width / AnimationFrames, Texture.Height);
            Rectangle destRect = new Rectangle(PositionX, PositionY, realWidth, Texture.Height);

            System.Numerics.Vector2 origin = new System.Numerics.Vector2(realWidth / 2, Texture.Height / 2);
            DrawTexturePro(Texture, sourceRect, destRect, origin, Direction, Color.White);
        }
        else
        {
            DrawCircle((int)PositionX, (int)PositionY, Radius, Color.Red);
        }
    }
}
