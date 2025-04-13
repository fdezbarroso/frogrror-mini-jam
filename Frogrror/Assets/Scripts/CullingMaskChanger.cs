using UnityEngine;

public class CullingMaskChanger : MonoBehaviour
{
    [SerializeField] private Camera _targetCamera;
    [SerializeField] private LayerMask _targetMask;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == GameplayManager.Instance.Player.gameObject)
        {
           _targetCamera.cullingMask = _targetMask; 
        }
    }
}
