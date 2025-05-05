using Globals;
using System.Collections.Generic;
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
        if (entityType == EntityType.PastPlayer)
        {
            BadWizardSprite = LoadTexture("assets/badwizard.png");
            BadWeaponSprite = LoadTexture("assets/badweapon.png");
            BadProjectileSprite = LoadTexture("assets/badfireball.png");
            BadEffectSprite = LoadTexture("assets/badfireballhit.png");
        }

        if (entityType == EntityType.PresentPlayer)
        {
            WizardSprite = LoadTexture("assets/wizard.png");
            WeaponSprite = LoadTexture("assets/weapon.png");
            ProjectileSprite = LoadTexture("assets/fireball.png");
            EffectSprite = LoadTexture("assets/fireballhit.png");
            FireballIcon = LoadTexture("assets/fireballicon.png");
            TrapIcon = LoadTexture("assets/trapicon.png");
            WallIcon = LoadTexture("assets/wallicon.png");
        }

        PositionX = positionX;
        PositionY = positionY;
        MouseX = 0;
        MouseY = 0;
        Target = null;
        Direction = direction;
        MoveSpeed = 5;
        MaxHealth = 100;
        Health = MaxHealth;
        FuturePowerBar = 0;
        FuturePowerMax = 100;
        EntityType = entityType;
        Radius = 20;
        ShootDistance = Radius + 10.0f;
        HitDuration = 5;
        Die = false;
    }

    private Texture2D WizardSprite;
    private Texture2D WeaponSprite;
    private Texture2D BadWizardSprite;
    private Texture2D BadWeaponSprite;
    private Texture2D ProjectileSprite;
    private Texture2D BadProjectileSprite;
    private Texture2D EffectSprite;
    private Texture2D BadEffectSprite;
    private Texture2D FireballIcon;
    private Texture2D TrapIcon;
    private Texture2D WallIcon;

    public int MaxHealth = 100;
    public int MouseX;
    public int MouseY;
    public Entity? Target;
    public int Direction;
    public float MoveSpeed;
    public int HitDuration;
    public int FuturePowerBar = 0;
    public int FuturePowerMax = 100;

    public bool leftButtonState;
    public bool leftButtonPressed;
    public bool middleButtonState;
    public bool rightButtonState;

    public float ShootDistance;
    public bool rightButtonPressed;
    public bool Die = false;

    public void ReadInputs(GlobalState globalState)
    {
        MouseX = GetMouseX();
        MouseY = GetMouseY();

        var ScrollWheelMove = GetMouseWheelMove();
        // scroll through FutureSpellSelected enums
        if (ScrollWheelMove > 0)
        {
            globalState.FutureSpellTypeSelected++;
            if (globalState.FutureSpellTypeSelected > FutureSpellType.PastTrap) // Assuming MaxSpell is the highest enum value
            {
                globalState.FutureSpellTypeSelected = FutureSpellType.Barrier; // Assuming MinSpell is the lowest enum value
            }
        }
        else if (ScrollWheelMove < 0)
        {
            globalState.FutureSpellTypeSelected--;
            if (globalState.FutureSpellTypeSelected < FutureSpellType.Barrier) // Assuming MinSpell is the lowest enum value
            {
                globalState.FutureSpellTypeSelected = FutureSpellType.PastTrap; // Assuming MaxSpell is the highest enum value
            }
        }

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

    public void IncreasePowerBar(int damage)
    {
        if (FuturePowerBar < FuturePowerMax)
        {
            FuturePowerBar += damage / 5;
        }
        if (FuturePowerBar + (damage / 5) > FuturePowerMax)
        {
            FuturePowerBar = FuturePowerMax;
        }
    }

    public void Update(float deltaTime, int currentFrame, List<Projectile> projectileList, List<Barrier> barrierList, List<PastTrap> pastTrapList, List<Entity> nonProjectileList, int roundId, FutureSpellType selectedSpell)
    {
        // Update the hit timer
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
            Place(barrierList, pastTrapList, nonProjectileList, roundId, selectedSpell);
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
            projectileList.Add(new Projectile(PositionX + forwardX, PositionY + forwardY, Direction, 0.0f, 1.0f, 0.0f, 5, 10, 360, EntityType, ProjectileSprite, EffectSprite, 3, 4, 20));
        }
        else
        {
            projectileList.Add(new Projectile(PositionX + forwardX, PositionY + forwardY, Direction, 0.0f, 1.0f, 0.0f, 5, 10, 360, EntityType, BadProjectileSprite, BadEffectSprite, 3, 4, 20));
        }
    }

    public void Place(List<Barrier> barrierList, List<PastTrap> pastTrapList, List<Entity> nonProjectileList, int roundId, FutureSpellType selectedSpell)
    {
        var StartX = PositionX + (float)(Math.Cos(Direction * Math.PI / 180) * -80);
        var StartY = PositionY + (float)(Math.Sin(Direction * Math.PI / 180) * -80);
        var EndX = StartX + (float)(Math.Cos((Direction + 90) * Math.PI / 180) * -80);
        var EndY = StartY + (float)(Math.Sin((Direction + 90) * Math.PI / 180) * -80);
        var Length = (float)Math.Sqrt(Math.Pow(EndX - StartX, 2) + Math.Pow(EndY - StartY, 2));
        var Angle = (float)(Math.Atan2(EndY - StartY, EndX - StartX) * 180 / Math.PI);

        int circleCount = 10; // Number of circles
        float stepX = (EndX - StartX) / circleCount;
        float stepY = (EndY - StartY) / circleCount;
        if (selectedSpell == FutureSpellType.PastTrap)
        {
            // PlacePastTrap(barrierList, nonProjectileList, roundId);
            Console.WriteLine("Placing Past Trap");
            PastTrap pastTrap = new PastTrap(StartX, StartY, roundId);
            if (pastTrap.PowerBarCost <= FuturePowerBar)
            {
                FuturePowerBar -= pastTrap.PowerBarCost;
                pastTrapList.Add(pastTrap);
                nonProjectileList.Add(pastTrap);
                Console.WriteLine($"Placed Past Trap at {StartX}, {StartY} with radius {pastTrap.Radius}");
            }
            else
            {
                return;
            }

        }
        else if (selectedSpell == FutureSpellType.Barrier)
        {
            for (int i = 0; i <= circleCount; i++)
            {
                float circleX = StartX + stepX * i;
                float circleY = StartY + stepY * i;
                Barrier barrier = new Barrier(circleX, circleY, roundId);
                if (barrier.PowerBarCost <= FuturePowerBar)
                {
                    FuturePowerBar -= barrier.PowerBarCost;
                    barrierList.Add(barrier);
                    nonProjectileList.Add(barrier);
                }
                else
                {
                    return;
                }
            }
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
            DrawTexture(BadWizardSprite, (int)PositionX - (WizardSprite.Width / 2), (int)PositionY - (WizardSprite.Height / 2), drawColor);

            Rectangle sourceRect = new Rectangle(0, 0, WeaponSprite.Width, WeaponSprite.Height);
            Rectangle destRect = new Rectangle(PositionX, PositionY, WeaponSprite.Width, WeaponSprite.Height);
            System.Numerics.Vector2 origin = new System.Numerics.Vector2(WeaponSprite.Width / 2, WeaponSprite.Height / 2);

            DrawTexturePro(BadWeaponSprite, sourceRect, destRect, origin, Direction, drawColor);
        }
        else
        {

            DrawTexture(WizardSprite, (int)PositionX - (WizardSprite.Width / 2), (int)PositionY - (WizardSprite.Height / 2), drawColor);

            Rectangle sourceRect = new Rectangle(0, 0, WeaponSprite.Width, WeaponSprite.Height);
            Rectangle destRect = new Rectangle(PositionX, PositionY, WeaponSprite.Width, WeaponSprite.Height);
            System.Numerics.Vector2 origin = new System.Numerics.Vector2(WeaponSprite.Width / 2, WeaponSprite.Height / 2);

            DrawTexturePro(WeaponSprite, sourceRect, destRect, origin, Direction, drawColor);
        }
    }

    public void DrawUI(FutureSpellType selectedSpell)
    {
        //Magic Number Heaven
        int healthWidth = (int)((float)238 * (float)Health / (float)MaxHealth);
        int powerWidth = (int)((float)238 * (float)FuturePowerBar / (float)FuturePowerMax);
        DrawRectangle(65, 753, healthWidth, 30, ColorFromHSV(345, 0.85f, 0.85f));
        DrawRectangle(497, 753, powerWidth, 30, ColorFromHSV(150, 0.89f, 0.60f));

        DrawTexture(FireballIcon, 336, 720, Color.White);
        if (selectedSpell == FutureSpellType.Barrier)
        {
            DrawTexture(WallIcon, 400, 720, Color.White);
        }
        if (selectedSpell == FutureSpellType.PastTrap)
        {
            DrawTexture(TrapIcon, 400, 720, Color.White);
        }
    }

    public void AcquireTarget(Entity target)
    {
        Target = target;
    }
}
