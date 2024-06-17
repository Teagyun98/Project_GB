using TMPro;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static Transform target; // 추적할 플레이어
    public Vector3 offset; // 카메라의 위치 오프셋

    // 플레이어의 현재 크기를 보여주는 UI Text
    [SerializeField] private TextMeshProUGUI meter;

    private void FixedUpdate()
    {
        if (target != null)
        {
            // 카메라 위치 최신화
            transform.position = target.position + offset;
            // 플레이어의 크기에 따라 카메라의 멀어짐 정도 변경
            Camera.main.orthographicSize = 5 * target.localScale.x;
            // 카메라의 현재 크기 최신화
            meter.text = $"{target.localScale.x : 0.0}M";
        }
    }
}