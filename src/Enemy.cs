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
        TargetX = 0;
        TargetY = 0;
        HitboxRadius = 40;

        AttackQueue.Enqueue(EnemyAttackType.Spiral);
        AttackQueue.Enqueue(EnemyAttackType.FastBurst);
        AttackQueue.Enqueue(EnemyAttackType.LargeProjectile);
        AttackQueue.Enqueue(EnemyAttackType.Shotgun);
    }


    public void AcquireTarget(Player player)
    {
        TargetX = player.PositionX;
        TargetY = player.PositionY;
    }
    public void UpdateDirection(int currentFrame)
    {
        if (currentFrame % DifficultySettings.DirectionTrackingSpeed == 0)
        {
            if (Math.Abs(X - TargetX) > 1 || Math.Abs(Y - TargetY) > 1)
            {
                Direction = (int)(Math.Atan2(Y - TargetY, X - TargetX) * 180 / Math.PI);
            }
        }
    }
    // {
    //     Direction = (int)(Math.Atan2(Y - TargetY, X - TargetX) * 180 / Math.PI);
    // }
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
                    foreach (var i in Enumerable.Range(0, 12))
                    {
                        int adjustedDirection = Direction + (i * (360 / 12));
                        if (ProjectileSchedule.ContainsKey(currentFrame + i * 2))
                        {
                            var existingProjectiles = ProjectileSchedule[currentFrame + i * 2].ToList();
                            existingProjectiles.Add(new Projectile(X, Y, adjustedDirection, 0.0f, 10.0f, 0.0f, 5, 10));
                            ProjectileSchedule[currentFrame + i * 2] = existingProjectiles.ToArray();
                        }
                        else
                        {
                            ProjectileSchedule.Add(currentFrame + i * 2, new Projectile[]
                            {
                                new Projectile(X, Y, adjustedDirection, 0.0f, 10.0f, 0.0f, 5, 10),
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
                            existingProjectiles.Add(new Projectile(X, Y, Direction, 0.0f, 10.0f, 0.0f, 5, 10));
                            ProjectileSchedule[currentFrame + i * time_between_projectiles] = existingProjectiles.ToArray();
                        }
                        else
                        {
                            ProjectileSchedule.Add(currentFrame + i * time_between_projectiles, new Projectile[]
                            {
                                new Projectile(X, Y, Direction, 0.0f, 10.0f, 0.0f, 5, 10),
                            });
                        }
                    }
                    break;
                case EnemyAttackType.LargeProjectile:
                    // Implement large projectile attack logic
                    if (ProjectileSchedule.ContainsKey(currentFrame))
                    {
                        var existingProjectiles = ProjectileSchedule[currentFrame].ToList();
                        existingProjectiles.Add(new Projectile(X, Y, Direction, 0.0f, 10.0f, 0.0f, 60, 10));
                        ProjectileSchedule[currentFrame] = existingProjectiles.ToArray();
                    }
                    else
                    {
                        ProjectileSchedule.Add(currentFrame, new Projectile[]
                        {
                            new Projectile(X, Y, Direction, 0.0f, 10.0f, 0.0f, 60, 10),
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
                                existingProjectiles.Add(new Projectile(X, Y, adjustedDirection, 0.0f, 10.0f, 0.0f, 5, 10));
                                ProjectileSchedule[currentFrame + i * 2] = existingProjectiles.ToArray();
                            }
                            else
                            {
                                ProjectileSchedule.Add(currentFrame + i * 2, new Projectile[]
                                {
                                    new Projectile(X, Y, adjustedDirection, 0.0f, 10.0f, 0.0f, 5, 10),
                                });
                            }
                        }
                    }
                    break;
            }
        }
    }
    public int X;
    public int Y;
    public float TargetX;
    public float TargetY;
    public int HitboxRadius;
    public int Direction = 0;
    public DifficultySettings DifficultySettings;

    public void ReadInputs(int currentFrame, Player player)
    {
        AcquireTarget(player);
        UpdateDirection(currentFrame);
    }

    public Dictionary<int, Projectile[]> ProjectileSchedule = new Dictionary<int, Projectile[]>();


    public void Update(float deltaTime, int currentFrame, List<Projectile> projectileList)
    {
        foreach (var proj in ProjectileSchedule)
        {
            if (proj.Key == currentFrame)
            {
                foreach (var projectile in proj.Value)
                {
                    projectileList.Add(projectile);
                }
            }
        }
        ProjectileSchedule.Remove(currentFrame);
    }

    public void Draw()
    {
        DrawCircle(X, Y, HitboxRadius, Color.Blue);

        // draw a arrow pointing in player direction 40 pixels long
        int arrowX = (int)(X - 60 * Math.Cos(Direction * Math.PI / 180));
        int arrowY = (int)(X - 60 * Math.Sin(Direction * Math.PI / 180));
        Raylib.DrawLine((int)X, (int)X, arrowX, arrowY, Color.Red);
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
