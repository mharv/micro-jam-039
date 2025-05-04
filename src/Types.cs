namespace Types;
public enum GamePhase
{
    Menu,
    Round,
    Transition,
    GameOver
};
public enum Difficulty
{
    Easy,
    Medium,
    Hard
}


public enum EnemyAttackType
{
    Spiral,
    FastBurst,
    LargeProjectile,
    Shotgun
}

public enum EntityType
{
    PresentPlayer,
    Enemy,
    Projectile,
    PastPlayer,
    Barrier,
    Text,
}
