using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class LevelClock : MonoBehaviour
{
    private float timer = 0f;
    private float levelDuration = 60f;
    private bool hasStartedVignette = false;

    private Volume postProcessVolume; // 在 Inspector 中拖入
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private Image mask;

    void Start()
    {
        postProcessVolume = GameObject.FindWithTag("PostProcess").GetComponent<Volume>();
        mask = GameObject.FindWithTag("AutoMask").GetComponentInChildren<Image>();
        // 获取 Vignette 效果
        if (postProcessVolume != null)
        {
            if (postProcessVolume.profile.TryGet(out vignette) == false)
            {
                Debug.LogError("Vignette 未在 PostProcessVolume 中添加。");
            }
            if (postProcessVolume.profile.TryGet(out colorAdjustments) == false)
            {
                Debug.LogError("ColorAdjust 未在 PostProcessVolume 中添加。");
            }
        }
        else
        {
            Debug.LogError("PostProcessVolume 未设置。");
        }
    }

    void Update()
    {
        if (!MyEventManager.Instance.isDisplayDialogue)
        {
            timer += Time.deltaTime;

            if (timer >= 50f && !hasStartedVignette)
            {
                hasStartedVignette = true;
                IncreaseVignette();
                DecreaseColor();
            }

            if (timer >= levelDuration)
            {
                LoadNextLevel();
            }
        }
    }

    void IncreaseVignette()
    {
        // 目标强度为 0.5，从当前时间到关卡结束的剩余时间内平滑过渡
        float duration = 1;
        float initialIntensity = vignette.intensity.value;
        float targetIntensity = 0.65f;

        DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, targetIntensity, duration);
    }
    
    void DecreaseColor()
    {
        // 目标强度为 0，从当前时间到关卡结束的剩余时间内平滑过渡
        float duration = 1;
        float initialIntensity = colorAdjustments.saturation.value;
        float targetIntensity = -100;

        DOTween.To(() => colorAdjustments.saturation.value, x => colorAdjustments.saturation.value = x, targetIntensity, duration);
    }

    void LoadNextLevel()
    {
        mask.DOFade(1, 1f).OnComplete(()=>SceneManager.LoadScene("StoryWriting"));
        // 在这里可以添加关卡结束前的其他处理逻辑
    }
}
