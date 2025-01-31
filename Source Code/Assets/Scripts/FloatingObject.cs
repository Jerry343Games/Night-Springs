using UnityEngine;
using DG.Tweening;

public class FloatingObject : MonoBehaviour
{
    // 浮动高度
    public float floatHeight = 0.5f;
    // 浮动时间
    public float floatDuration = 2f;
    // 左右晃动的角度
    public float swayAngle = 15f;
    // 左右晃动的时间
    public float swayDuration = 1f;

    void Start()
    {
        // 上下浮动的动画
        transform.DOMoveY(transform.position.y + floatHeight, floatDuration)
            .SetLoops(-1, LoopType.Yoyo) // 无限循环，Yoyo模式（来回移动）
            .SetEase(Ease.InOutSine);    // 使用平滑的缓动函数

        // 左右晃动的动画
        transform.DORotate(new Vector3(0, 0, swayAngle), swayDuration, RotateMode.LocalAxisAdd)
            .SetLoops(-1, LoopType.Yoyo) // 无限循环，Yoyo模式（来回晃动）
            .SetEase(Ease.InOutSine);    // 使用平滑的缓动函数
    }
}