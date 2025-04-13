using UnityEngine;

public class FloatingMovement : MonoBehaviour
{
    public float speed = 0.5f;
    public float amplitude = 1.0f;

    private Vector2 _startingPosition = Vector2.zero;

    private void Start()
    {
        _startingPosition = transform.localPosition;
    }

    private void Update()
    {
        transform.localPosition = _startingPosition + Vector2.up * (amplitude * Mathf.Sin(Time.time * speed));
    }
}