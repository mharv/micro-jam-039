namespace Types;
public enum GamePhase
{
    Menu,
    RoundStart,
    Round,
    TransitionStart,
    Transition,
    GameOver
};
public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    Chaotic,
}


public enum EnemyAttackType
{
    Spiral,
    FastBurst,
    LargeProjectile,
    Shotgun
}
public enum FutureSpellType
{
    Barrier,
    PastTrap
}
public enum EntityType
{
    PresentPlayer,
    Enemy,
    Projectile,
    PastPlayer,
    FutureSpell,
    Text,
}
