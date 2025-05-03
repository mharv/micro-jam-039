using Raylib_cs;
using static Raylib_cs.Raylib;
using Types;


namespace Entities;


public class DifficultySettings
{
    public int AttackInterval { get; set; }
    public int DirectionTrackingSpeed { get; set; }
    public int ProjectileSpeed { get; set; }
}


public class Enemy
{
    Queue<EnemyAttackType> AttackQueue = new Queue<EnemyAttackType>();

    public Enemy(int x, int y, Difficulty difficulty)
    {
        DifficultySettings = new DifficultySettings();

        switch (difficulty)
        {
            case Difficulty.Easy:
                DifficultySettings = new DifficultySettings
                {
                    AttackInterval = 40,
                    DirectionTrackingSpeed = 5,
                    ProjectileSpeed = 3
                };
                break;
            case Difficulty.Medium:
                DifficultySettings = new DifficultySettings
                {
                    AttackInterval = 24,
                    DirectionTrackingSpeed = 8,
                    ProjectileSpeed = 7
                };
                break;
            case Difficulty.Hard:
                DifficultySettings = new DifficultySettings
                {
                    AttackInterval = 14,
                    DirectionTrackingSpeed = 12,
                    ProjectileSpeed = 10
                };
                break;
        }

        X = x;
        Y = y;
        HitboxRadius = 40;

        AttackQueue.Enqueue(EnemyAttackType.Spiral);
        AttackQueue.Enqueue(EnemyAttackType.FastBurst);
        AttackQueue.Enqueue(EnemyAttackType.LargeProjectile);
        AttackQueue.Enqueue(EnemyAttackType.Shotgun);
    }

    public void Attack(int currentFrame)
    {
        if (currentFrame % DifficultySettings.AttackInterval == 0)
        {
            EnemyAttackType attackType = AttackQueue.Dequeue();
            Console.WriteLine($"ATTACK__________________________________________{attackType}");
            AttackQueue.Enqueue(attackType);

            switch (attackType)
            {
                case EnemyAttackType.Spiral:
                    // Implement spiral attack logic
                    break;
                case EnemyAttackType.FastBurst:
                    // Implement fast burst attack logic
                    break;
                case EnemyAttackType.LargeProjectile:
                    // Implement large projectile attack logic
                    break;
                case EnemyAttackType.Shotgun:
                    // Implement shotgun attack logic
                    break;
            }
        }
    }
    public int X;
    public int Y;
    public int HitboxRadius;
    public int Direction = 0;
    public DifficultySettings DifficultySettings;

    public void ReadInputs()
    {
    }

    public void Update(float deltaTime)
    {

    }

    public void Draw()
    {
        DrawCircle(X, Y, HitboxRadius, Color.Blue);
    }

    public (int, int) GetPosition()
    {
        return (X, Y);
    }
}

class Game
{
    public int RoundDuration;
}
