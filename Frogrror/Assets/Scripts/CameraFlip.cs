using UnityEngine;

public class CameraFlip : MonoBehaviour
{
    public Camera renderCamera;

    void Start()
    {
        FlipCamera();
    }

    public void FlipCamera()
    {
        if (Camera.main != null)
        {
            return;
        }

        Matrix4x4 projectionMatrix = renderCamera.projectionMatrix;
        projectionMatrix *= Matrix4x4.Scale(new Vector3(-1, 1, 1));
        renderCamera.projectionMatrix = projectionMatrix;
    }
}