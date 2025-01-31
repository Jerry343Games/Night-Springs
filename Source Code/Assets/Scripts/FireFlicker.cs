using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlicker : MonoBehaviour
{
    private Light fireLight;

    // 定义强度范围
    public float minIntensity = 6f;
    public float maxIntensity = 8f;

    // 变化速度
    public float flickerSpeed = 5f;

    // 用于Perlin噪声的时间变量
    private float noiseOffset;

    void Start()
    {
        // 获取灯光组件
        fireLight = GetComponent<Light>();
        // 随机初始化噪声偏移
        noiseOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // 使用Perlin噪声生成平滑过渡的随机数，模拟火苗闪烁
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, noiseOffset);
        
        // 计算当前灯光的强度
        fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}
