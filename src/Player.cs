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
        PositionX = positionX;
        PositionY = positionY;
        MouseX = 0;
        MouseY = 0;
        Target = null;
        Direction = direction;
        MoveSpeed = 5;
        Health = 100;
        EntityType = entityType;
    }

    public int MouseX;
    public int MouseY;
    public Entity? Target;
    public int Direction;
    public float MoveSpeed;

    public bool leftButtonState;
    public bool leftButtonPressed;
    public bool middleButtonState;
    public bool rightButtonState;
    public int Health;
    public EntityType EntityType;



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
    }

    public void Update(float deltaTime, int currentFrame, List<Projectile> projectileList)
    {
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
        projectileList.Add(new Projectile(PositionX, PositionY, Direction, 0.0f, 10.0f, 0.0f, 5, 10, 180));
    }

    public void Draw()
    {
        // draw player
        DrawCircle((int)PositionX, (int)PositionY, 20, Color.Red);

        // draw a arrow pointing in player direction 40 pixels long
        int arrowX = (int)(PositionX - 40 * Math.Cos(Direction * Math.PI / 180));
        int arrowY = (int)(PositionY - 40 * Math.Sin(Direction * Math.PI / 180));
        Raylib.DrawLine((int)PositionX, (int)PositionY, arrowX, arrowY, Color.Red);
    }

    public void AcquireTarget(Entity target)
    {
        Target = target;
    }
}
