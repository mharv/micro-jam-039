using Raylib_cs;
using Recording;
using Types;
using static Raylib_cs.Raylib;

namespace Entities;

public class Player : Entity
{
    // constructor
    public Player(float positionX = 0.0f, float positionY = 0.0f, int direction = 0, EntityType entityType = EntityType.PastPlayer)
    {
        WizardSprite = LoadTexture("assets/wizard.png");
        WeaponSprite = LoadTexture("assets/weapon.png");
        ProjectileSprite = LoadTexture("assets/fireball.png");
        BadProjectileSprite = LoadTexture("assets/badfireball.png");
        PositionX = positionX;
        PositionY = positionY;
        MouseX = 0;
        MouseY = 0;
        Target = null;
        Direction = direction;
        MoveSpeed = 5;
        Health = 100;
        EntityType = entityType;
        Radius = 20;
        ShootDistance = Radius + 10.0f;
        HitDuration = 5;
    }

    private Texture2D WizardSprite;
    private Texture2D WeaponSprite;
    private Texture2D ProjectileSprite;
    private Texture2D BadProjectileSprite;

    public int MouseX;
    public int MouseY;
    public Entity? Target;
    public int Direction;
    public float MoveSpeed;
    public int HitDuration;

    public bool leftButtonState;
    public bool leftButtonPressed;
    public bool middleButtonState;
    public bool rightButtonState;

    public float ShootDistance;
    public bool rightButtonPressed;


    public void ReadInputs()
    {
        MouseX = GetMouseX();
        MouseY = GetMouseY();

        if (Target != null)
        {
            Direction = (int)(Math.Atan2(PositionY - Target.PositionY,
                                    PositionX - Target.PositionX) * 180 / Math.PI);
        }
        else
        {
            //Just a fallback to face the mouse if we have no target (Shouldn't happen) 
            Direction = (int)(Math.Atan2(PositionY - MouseY,
                                    PositionX - MouseX) * 180 / Math.PI);
        }

        leftButtonState = Raylib.IsMouseButtonDown(MouseButton.Left);
        leftButtonPressed = Raylib.IsMouseButtonPressed(MouseButton.Left);
        middleButtonState = Raylib.IsMouseButtonDown(MouseButton.Middle);
        rightButtonState = Raylib.IsMouseButtonDown(MouseButton.Right);
        rightButtonPressed = Raylib.IsMouseButtonPressed(MouseButton.Right);
    }

    public void Update(float deltaTime, int currentFrame, List<Projectile> projectileList, List<Barrier> barrierList)
    {
        if (Hit)
        {
            HitTimer++;
            if (HitTimer >= HitDuration)
            {
                Hit = false;
            }
        }

        float dx = MouseX - PositionX;
        float dy = MouseY - PositionY;
        float length = MathF.Sqrt(dx * dx + dy * dy);
        if (length > 3.0f)
        {
            dx /= length;
            dy /= length;

            PositionX += dx * MoveSpeed;
            PositionY += dy * MoveSpeed;
        }

        if (leftButtonPressed)
        {
            Shoot(projectileList);
        }
        if (rightButtonPressed)
        {
            Place(barrierList);
        }
    }
    public void UpdatePast(int currentFrame, List<Projectile> projectileList, TimeSlice timeSlice)
    {
        PositionX = timeSlice.PlayerPositionX;
        PositionY = timeSlice.PlayerPositionY;
        Direction = timeSlice.PlayerDirection;
        if (timeSlice.PlayerShoot)
        {
            Shoot(projectileList);
        }
    }

    public void Shoot(List<Projectile> projectileList)
    {
        float radians = Direction * MathF.PI / 180;
        float forwardX = MathF.Cos(radians) * -ShootDistance;
        float forwardY = MathF.Sin(radians) * -ShootDistance;

        if (EntityType == EntityType.PresentPlayer)
        {
            projectileList.Add(new Projectile(PositionX + forwardX, PositionY + forwardY, Direction, 0.0f, 1.0f, 0.0f, 5, 10, 360, EntityType, ProjectileSprite, 3));
        }
        else
        {
            projectileList.Add(new Projectile(PositionX + forwardX, PositionY + forwardY, Direction, 0.0f, 1.0f, 0.0f, 5, 10, 360, EntityType, BadProjectileSprite, 3));
        }
    }

    public void Place(List<Barrier> barrierList)
    {
        var StartX = PositionX + (float)(Math.Cos(Direction * Math.PI / 180) * -80);
        var StartY = PositionY + (float)(Math.Sin(Direction * Math.PI / 180) * -80);
        var EndX = StartX + (float)(Math.Cos((Direction + 90) * Math.PI / 180) * -80);
        var EndY = StartY + (float)(Math.Sin((Direction + 90) * Math.PI / 180) * -80);
        var Length = (float)Math.Sqrt(Math.Pow(EndX - StartX, 2) + Math.Pow(EndY - StartY, 2));
        var Angle = (float)(Math.Atan2(EndY - StartY, EndX - StartX) * 180 / Math.PI);
        EntityType = EntityType.Barrier;

        int circleCount = 10; // Number of circles
        float stepX = (EndX - StartX) / circleCount;
        float stepY = (EndY - StartY) / circleCount;

        for (int i = 0; i <= circleCount; i++)
        {
            float circleX = StartX + stepX * i;
            float circleY = StartY + stepY * i;
            Barrier barrier = new Barrier(circleX, circleY);
            barrierList.Add(barrier);
        }
    }

    public void Draw()
    {
        //if it's you
        Color drawColor = Color.White;

        if (Hit)
        {
            drawColor = Color.Red;
        }

        //if it's the past
        if (EntityType == EntityType.PastPlayer)
        {
            drawColor = ColorAlpha(drawColor, 0.5f);
        }

        DrawTexture(WizardSprite, (int)PositionX - (WizardSprite.Width / 2), (int)PositionY - (WizardSprite.Height / 2), drawColor);
        Rectangle sourceRect = new Rectangle(0, 0, WeaponSprite.Width, WeaponSprite.Height);
        Rectangle destRect = new Rectangle(PositionX, PositionY, WeaponSprite.Width, WeaponSprite.Height);
        System.Numerics.Vector2 origin = new System.Numerics.Vector2(WeaponSprite.Width / 2, WeaponSprite.Height / 2);

        DrawTexturePro(WeaponSprite, sourceRect, destRect, origin, Direction, drawColor);
    }

    public void AcquireTarget(Entity target)
    {
        Target = target;
    }
}
