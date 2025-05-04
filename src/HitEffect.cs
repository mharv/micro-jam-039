using Raylib_cs;
using static Raylib_cs.Raylib;
using Globals;

namespace Entities;

public class HitEffect : Entity
{
    public Texture2D EffectSprite;
    public bool Die;
    public float Direction;
    public int AnimationFrames;
    public int AnimationSpeed;
    private int AnimFramesCounter;
    private int CurrentAnimFrame;

    public HitEffect(float positionX, float positionY, float direction, Texture2D texture = new Texture2D(), int animFrames = 1, int animSpeed = 1)
    {
        Direction = direction;
        PositionX = positionX;
        PositionY = positionY;
        Die = false;
        EffectSprite = texture;
        AnimationFrames = animFrames;
        AnimationSpeed = animSpeed;
        AnimFramesCounter = 0;
        CurrentAnimFrame = 0;
    }

    public HitEffect Clone()
    {
        return new HitEffect(PositionX, PositionY, Direction, EffectSprite, AnimationFrames, AnimationSpeed);
    }

    public void Update(float deltaTime, GlobalState globalState)
    {
        if (!Die)
        {
            AnimFramesCounter++;
            if (AnimFramesCounter >= (60 / AnimationSpeed))
            {
                CurrentAnimFrame++;
                AnimFramesCounter = 0;
                if (CurrentAnimFrame >= AnimationFrames)
                {
                    Die = true;
                }
            }
        }
    }

    public void Draw()
    {
        if (EffectSprite.Id != 0 && !Die)
        {
            int realWidth = EffectSprite.Width / AnimationFrames;
            Rectangle sourceRect = new Rectangle(CurrentAnimFrame * realWidth, 0, EffectSprite.Width / AnimationFrames, EffectSprite.Height);
            Rectangle destRect = new Rectangle(PositionX, PositionY, realWidth, EffectSprite.Height);

            System.Numerics.Vector2 origin = new System.Numerics.Vector2(realWidth / 2, EffectSprite.Height / 2);
            DrawTexturePro(EffectSprite, sourceRect, destRect, origin, 0, Color.White);
        }
    }
}
