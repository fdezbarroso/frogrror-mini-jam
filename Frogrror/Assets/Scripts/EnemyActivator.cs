using System.Collections.Generic;
using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    [SerializeField] private List<BasicEnemyBehavior> _enemies;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == GameplayManager.Instance.Player.gameObject)
        {
            foreach (var enemy in _enemies)
            {
                enemy.enabled = true;
            }
        }
    }
}
