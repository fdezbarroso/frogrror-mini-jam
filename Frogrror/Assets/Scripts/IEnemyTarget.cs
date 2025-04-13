using UnityEngine;

public interface IEnemyTarget
{
    GameObject GameObject { get; }
    
    Transform Transform { get; }
    
    bool IsDead { get; }
    
    bool IsHiding { get; }

    void Kill(BasicEnemyBehavior enemy);
}
