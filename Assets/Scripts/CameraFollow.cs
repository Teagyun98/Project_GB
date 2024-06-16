using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static Transform target; // 추적할 플레이어
    public Vector3 offset; // 카메라의 위치 오프셋

    private void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}