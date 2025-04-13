using UnityEngine;

public class ChangeEnemyTarget : MonoBehaviour
{
    [SerializeField] private GameObject _newTarget;
    [SerializeField] private BasicEnemyBehavior _enemy;

    private IEnemyTarget _newEnemyTarget;

    private void Awake()
    {
        _newEnemyTarget = _newTarget.GetComponent<IEnemyTarget>();

        if (_newEnemyTarget == null)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == GameplayManager.Instance.Player.gameObject)
        {
            _enemy.SetTarget(_newEnemyTarget, true);
        }
    }
}
