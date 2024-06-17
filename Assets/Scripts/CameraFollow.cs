using TMPro;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static Transform target; // 추적할 플레이어
    public Vector3 offset; // 카메라의 위치 오프셋

    [SerializeField] private TextMeshProUGUI meter;

    private void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            Camera.main.orthographicSize = 5 * target.localScale.x;
            meter.text = target.localScale.x + "M";
        }
    }
}