using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrace : MonoBehaviour
{
    public Transform playerTransform;
    public float smoothTime = 0.3f;  // SmoothDamp 所需的时间
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;  // SmoothDamp 用来记录速度

    private void Start()
    {
        offset = transform.position - playerTransform.position;
    }

    void LateUpdate()
    {
        Vector3 targetPosition = playerTransform.position + offset;

        // 使用 SmoothDamp 平滑移动相机到目标位置
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        transform.position = smoothedPosition;
    }
}
