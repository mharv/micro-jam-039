using Raylib_cs;
using static Raylib_cs.Raylib;
using Types;
namespace Entities;

public class FloatingText : Entity
{
    public float Direction;
    public float TurnSpeed;
    public float BaseSpeed;
    public float Acceleration;
    public string Text = "";
    public int FramesToLive;
    public int TotalLife;
    public int Size;
    public bool Die;
    public Color BaseColor;
    public Color DrawColor;

    public FloatingText(float positionX, float positionY, string text, int size, Color color, int lifeTime, float speed, float acceleration)
    {
        PositionX = positionX;
        PositionY = positionY;
        Direction = new Random().Next(0, 360);
        TurnSpeed = new Random().Next(-90, 90);
        BaseSpeed = speed;
        Acceleration = acceleration;
        Text = text;
        FramesToLive = lifeTime;
        TotalLife = lifeTime;
        Size = 20;
        EntityType = EntityType.Text;
        BaseColor = color;
        DrawColor = BaseColor;
        PositionX -= MeasureText(Text, Size) / 2;
    }

    public void Update(float deltaTime)
    {
        BaseSpeed += Acceleration * deltaTime;
        Direction += TurnSpeed * deltaTime;

        float rads = Direction * MathF.PI / 180;

        float dx = MathF.Cos(rads) * -BaseSpeed;
        float dy = MathF.Sin(rads) * -BaseSpeed;

        PositionX += dx;
        PositionY += dy;

        DrawColor = ColorAlpha(BaseColor, (float)FramesToLive / TotalLife);

        if (FramesToLive-- <= 0)
        {
            Die = true;
        }
    }

    public void Draw()
    {
        DrawText(Text, (int)PositionX, (int)PositionY, Size, DrawColor);
    }
}
