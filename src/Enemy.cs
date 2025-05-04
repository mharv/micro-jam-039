using Raylib_cs;
using static Raylib_cs.Raylib;
using Types;
using Globals;

namespace Entities;

public class DifficultySettings
{
    public int AttackInterval { get; set; }
    public int DirectionTrackingSpeed { get; set; }
    public int ProjectileSpeed { get; set; }
}

public class Enemy : Entity
{
    Queue<EnemyAttackType> AttackQueue = new Queue<EnemyAttackType>();
    Texture2D EnemySprite;
    Texture2D ProjectileSprite;
    Texture2D EffectSprite;
    Texture2D BigProjectileSprite;
    Texture2D BigEffectSprite;
    public Entity? Target;
    public int Direction = 0;
    public DifficultySettings DifficultySettings;
    public Dictionary<int, Projectile[]> ProjectileSchedule = new Dictionary<int, Projectile[]>();
    public int HitDuration;
    public List<string> Taunts;
    public int ProjectileAnimSpeed = 20;
    public int AnimFrames = 0;
    public int CurrentAnimFrame = 0;

    public Enemy(int positionX, int positionY, Difficulty difficulty)
    {
        DifficultySettings = new DifficultySettings();
        SetDifficultySettings(difficulty);

        Taunts = new List<string>();
        Taunts.Add("\"I will grind you to dust!\"");
        Taunts.Add("\"Your ancestors will feel your agony!\"");
        Taunts.Add("\"Your bones will be scattered through time!\"");
        Taunts.Add("\"You will be nothing but a forgotten whisper in history!\"");
        Taunts.Add("\"Even your descendants will tremble at the thought of me!\"");
        Taunts.Add("\"I’ll break your spirit, and your children will curse your name!\"");
        Taunts.Add("\"This moment will echo in eternity… as your end!\"");
        Taunts.Add("\"You will leave no legacy, only ashes in the wind!\"");

        PositionX = positionX;
        PositionY = positionY;
        Target = null;
        Radius = 40;
        HitDuration = 5;

        AttackQueue.Enqueue(EnemyAttackType.Spiral);
        AttackQueue.Enqueue(EnemyAttackType.FastBurst);
        AttackQueue.Enqueue(EnemyAttackType.LargeProjectile);
        AttackQueue.Enqueue(EnemyAttackType.Shotgun);

        EntityType = EntityType.Enemy;
        EnemySprite = LoadTexture("assets/badguy.png");
        ProjectileSprite = LoadTexture("assets/badfireball.png");
        BigProjectileSprite = LoadTexture("assets/bigfireball.png");
        EffectSprite = LoadTexture("assets/badfireballhit.png");
        BigEffectSprite = LoadTexture("assets/bigfireballhit.png");
        ProjectileAnimSpeed = 20;
        AnimFrames = 10;
        CurrentAnimFrame = 0;
    }

    public void AcquireTarget(Entity target)
    {
        Target = target;
    }

    public void UpdateDirection(int currentFrame)
    {
        if (currentFrame % DifficultySettings.DirectionTrackingSpeed == 0)
        {
            if (Target != null)
            {
                if (Math.Abs(PositionX - Target.PositionX) > 1 || Math.Abs(PositionY - Target.PositionY) > 1)
                {
                    Direction = (int)(Math.Atan2(PositionY - Target.PositionY, PositionX - Target.PositionX) * 180 / Math.PI);
                }
            }
        }
    }

    public void AdjustDifficulty(Difficulty difficulty)
    {
        SetDifficultySettings(difficulty);
    }

    private void SetDifficultySettings(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.VeryEasy:
                DifficultySettings.AttackInterval = 50;
                DifficultySettings.DirectionTrackingSpeed = 4;
                DifficultySettings.ProjectileSpeed = 2;
                break;
            case Difficulty.Easy:
                DifficultySettings.AttackInterval = 40;
                DifficultySettings.DirectionTrackingSpeed = 5;
                DifficultySettings.ProjectileSpeed = 3;
                break;
            case Difficulty.Medium:
                DifficultySettings.AttackInterval = 35;
                DifficultySettings.DirectionTrackingSpeed = 6;
                DifficultySettings.ProjectileSpeed = 4;
                break;
            case Difficulty.Hard:
                DifficultySettings.AttackInterval = 30;
                DifficultySettings.DirectionTrackingSpeed = 7;
                DifficultySettings.ProjectileSpeed = 5;
                break;
            case Difficulty.Chaotic:
                DifficultySettings.AttackInterval = 25;
                DifficultySettings.DirectionTrackingSpeed = 8;
                DifficultySettings.ProjectileSpeed = 8;
                break;
        }
    }

    public void Attack(int currentFrame, GlobalState globalState)
    {
        Console.WriteLine($"DifficultySettings: {DifficultySettings.AttackInterval} {DifficultySettings.DirectionTrackingSpeed} {DifficultySettings.ProjectileSpeed}");
        if (currentFrame % DifficultySettings.AttackInterval == 0)
        {
            // shuffle the attack queue
            AttackQueue = new Queue<EnemyAttackType>(AttackQueue.OrderBy(x => Guid.NewGuid()));
            EnemyAttackType attackType = AttackQueue.Dequeue();
            Console.WriteLine($"ATTACK__________________________________________{attackType}");
            AttackQueue.Enqueue(attackType);

            switch (attackType)
            {
                case EnemyAttackType.Spiral:
                    // Implement spiral attack logic
                    bool reverse = new Random().Next(0, 2) == 0; // Randomly decide whether to reverse
                    foreach (var i in Enumerable.Range(0, 12))
                    {
                        int adjustedDirection = Direction + (reverse ? -i * (360 / 12) : i * (360 / 12));
                        if (ProjectileSchedule.ContainsKey(currentFrame + i * 2))
                        {
                            var existingProjectiles = ProjectileSchedule[currentFrame + i * 2].ToList();
                            existingProjectiles.Add(new Projectile(PositionX, PositionY, adjustedDirection, 0.0f, DifficultySettings.ProjectileSpeed, 0.0f, 5, 10, 180, EntityType.Enemy, ProjectileSprite, EffectSprite, 3, 4, ProjectileAnimSpeed));
                            ProjectileSchedule[currentFrame + i * 2] = existingProjectiles.ToArray();
                        }
                        else
                        {
                            ProjectileSchedule.Add(currentFrame + i * 2, new Projectile[]
                            {
                                new Projectile(PositionX, PositionY, adjustedDirection, 0.0f, DifficultySettings.ProjectileSpeed, 0.0f, 5, 10, 180, EntityType.Enemy, ProjectileSprite, EffectSprite, 3, 4, ProjectileAnimSpeed),
                            });
                        }
                    }
                    break;
                case EnemyAttackType.FastBurst:
                    // Implement fast burst attack logic
                    var time_between_projectiles = 3;
                    foreach (var i in Enumerable.Range(0, 5))
                    {
                        if (ProjectileSchedule.ContainsKey(currentFrame + i * time_between_projectiles))
                        {
                            var existingProjectiles = ProjectileSchedule[currentFrame + i * time_between_projectiles].ToList();
                            existingProjectiles.Add(new Projectile(PositionX, PositionY, Direction, 0.0f, DifficultySettings.ProjectileSpeed, 0.0f, 5, 10, 180, EntityType.Enemy, ProjectileSprite, EffectSprite, 3, 4, ProjectileAnimSpeed));
                            ProjectileSchedule[currentFrame + i * time_between_projectiles] = existingProjectiles.ToArray();
                        }
                        else
                        {
                            ProjectileSchedule.Add(currentFrame + i * time_between_projectiles, new Projectile[]
                            {
                                new Projectile(PositionX, PositionY, Direction, 0.0f, DifficultySettings.ProjectileSpeed, 0.0f, 5, 10, 180, EntityType.Enemy, ProjectileSprite, EffectSprite, 3, 4, ProjectileAnimSpeed),
                            });
                        }
                    }
                    break;
                case EnemyAttackType.LargeProjectile:
                    globalState.FloatingTextList.Add(new FloatingText(PositionX, PositionY, Taunts[new Random().Next(0, Taunts.Count)], 16, Color.White, 180, 1.0f, 0.0f));
                    // Implement large projectile attack logic
                    if (ProjectileSchedule.ContainsKey(currentFrame))
                    {
                        var existingProjectiles = ProjectileSchedule[currentFrame].ToList();
                        existingProjectiles.Add(new Projectile(PositionX, PositionY, Direction, 0.0f, DifficultySettings.ProjectileSpeed, 0.0f, 60, 10, 180, EntityType.Enemy, BigProjectileSprite, BigEffectSprite, 4, 7, ProjectileAnimSpeed));
                        ProjectileSchedule[currentFrame] = existingProjectiles.ToArray();
                    }
                    else
                    {
                        ProjectileSchedule.Add(currentFrame, new Projectile[]
                        {
                            new Projectile(PositionX, PositionY, Direction, 0.0f, DifficultySettings.ProjectileSpeed, 0.0f, 60, 10, 180, EntityType.Enemy, BigProjectileSprite, BigEffectSprite, 4, 7, ProjectileAnimSpeed),
                        });
                    }
                    break;
                case EnemyAttackType.Shotgun:
                    // Implement shotgun attack logic
                    foreach (var i in Enumerable.Range(0, 3))
                    {
                        foreach (var j in Enumerable.Range(-1, 3)) // Adds 3 projectiles per iteration
                        {
                            int adjustedDirection = Direction + (j * 15) + new Random().Next(-5, 6); // Spread projectiles by 15 degrees with slight randomness
                            if (ProjectileSchedule.ContainsKey(currentFrame + i * 2))
                            {
                                var existingProjectiles = ProjectileSchedule[currentFrame + i * 2].ToList();
                                existingProjectiles.Add(new Projectile(PositionX, PositionY, adjustedDirection, 0.0f, DifficultySettings.ProjectileSpeed, 0.0f, 5, 10, 180, EntityType.Enemy, ProjectileSprite, EffectSprite, 3, 4, ProjectileAnimSpeed));
                                ProjectileSchedule[currentFrame + i * 2] = existingProjectiles.ToArray();
                            }
                            else
                            {
                                ProjectileSchedule.Add(currentFrame + i * 2, new Projectile[]
                                {
                                    new Projectile(PositionX, PositionY, adjustedDirection, 0.0f, DifficultySettings.ProjectileSpeed, 0.0f, 5, 10, 180, EntityType.Enemy, ProjectileSprite, EffectSprite, 3, 4, ProjectileAnimSpeed),
                                });
                            }
                        }
                    }
                    break;
            }
        }
    }

    public void ReadInputs(int currentFrame, Player player)
    {
        AcquireTarget(player);
        UpdateDirection(currentFrame);
    }

    public void Update(float deltaTime, GlobalState globalState)
    {
        float roundPercent = (float)globalState.CurrentFrame / (float)globalState.RoundDurationFrames;
        CurrentAnimFrame = (int)(AnimFrames * roundPercent);

        if (Hit)
        {
            HitTimer++;
            if (HitTimer >= HitDuration)
            {
                Hit = false;
            }
        }

        foreach (var proj in ProjectileSchedule)
        {
            if (proj.Key == globalState.CurrentFrame)
            {
                foreach (var projectile in proj.Value)
                {
                    globalState.ProjectileList.Add(projectile);
                }
            }
        }
        ProjectileSchedule.Remove(globalState.CurrentFrame);
    }

    public void Draw()
    {
        Color drawColor = Color.White;

        if (Hit)
        {
            drawColor = Color.Red;
        }

        if (EnemySprite.Id != 0)
        {
            int realWidth = EnemySprite.Width / AnimFrames;
            Rectangle sourceRect = new Rectangle(CurrentAnimFrame * realWidth, 0, EnemySprite.Width / AnimFrames, EnemySprite.Height);
            Rectangle destRect = new Rectangle(PositionX, PositionY, realWidth, EnemySprite.Height);

            System.Numerics.Vector2 origin = new System.Numerics.Vector2(realWidth / 2, EnemySprite.Height / 2);
            DrawTexturePro(EnemySprite, sourceRect, destRect, origin, 0, Color.White);
        }
        else
        {
            DrawCircle((int)PositionX, (int)PositionY, Radius, drawColor);
        }
    }
}

class Game
{
    public int RoundDuration;
}
