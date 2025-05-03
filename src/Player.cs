using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Entities;

public class Player
{
    // constructor
    public Player()
    {
        PositionX = 0.0f;
        PositionY = 0.0f;
        MouseX = 0;
        MouseY = 0;
        TargetX = 0;
        TargetY = 0;
        Direction = 0;
        MoveSpeed = 5;
    }
    public float PositionX;
    public float PositionY;
    public int MouseX;
    public int MouseY;
    public int TargetX;
    public int TargetY;
    public int Direction;
    public float MoveSpeed;

    public bool leftButtonState;
    public bool leftButtonPressed;
    public bool middleButtonState;
    public bool rightButtonState;

    public void ReadInputs()
    {
        MouseX = GetMouseX();
        MouseY = GetMouseY();

        Direction = (int)(Math.Atan2(PositionY - TargetY,
                               PositionX - TargetX) * 180 / Math.PI);

        leftButtonState = Raylib.IsMouseButtonDown(MouseButton.Left);
        leftButtonPressed = Raylib.IsMouseButtonPressed(MouseButton.Left);
        middleButtonState = Raylib.IsMouseButtonDown(MouseButton.Middle);
        rightButtonState = Raylib.IsMouseButtonDown(MouseButton.Right);
    }

    public void Update(float deltaTime, List<Projectile> projectileList)
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
            projectileList.Add(new Projectile(PositionX, PositionY, Direction, 0.0f, 10.0f, 0.0f, 5, 10));
        }
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

    public void AcquireTarget(Enemy enemy)
    {
        TargetX = enemy.X;
        TargetY = enemy.Y;
    }
}
