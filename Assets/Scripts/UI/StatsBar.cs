using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
    [SerializeField] Image fillImageBack;
    [SerializeField] Image fillImageFront;
    [SerializeField] bool delayFill = true;
    [SerializeField] float fillDelay = 0.5f;
    [SerializeField] float fillSpeed = 0.1f;
    float currentFillAmount;
    protected float targetFillAmount;
    float previousFillAmount;
    float t;

    WaitForSeconds waitForDelayFill;
    Coroutine bufferedFillingCoroutine;
    Canvas canvas;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public virtual void Initialize(float currentValue, float maxValue)
    {
        currentFillAmount = currentValue / maxValue;
        targetFillAmount = currentFillAmount;
        fillImageBack.fillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }

    public void UpdateStats(float currentValue, float maxValue)
    {
        targetFillAmount = currentValue / maxValue;

        if (bufferedFillingCoroutine != null)
        {
            StopCoroutine(bufferedFillingCoroutine);
        }

        //if stats reduce   当状态值减少时
        if (currentFillAmount > targetFillAmount)
        {
            //fill image front=target fill amount   前面图片的填充值=目标填充值
            fillImageFront.fillAmount = targetFillAmount;
            //slowly reduce fill image back's fill amount   慢慢减少后面图片的填充值
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageBack));

            return;
        }

        //if stats increase     当状态值增加时
        if (currentFillAmount < targetFillAmount)
        {
            //fill image back=target fill amount    后面图片的填充值=目标填充值
            fillImageBack.fillAmount = targetFillAmount;
            //slowly increase fill image front's fill amount    慢慢增加前面图片的填充值
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageFront));
        }
    }

    protected virtual IEnumerator BufferedFillingCoroutine(Image image)
    {
        //当延迟填充开启时，挂起等待填充延迟时间
        if (delayFill)
        {
            yield return waitForDelayFill;
        }
        previousFillAmount = currentFillAmount;
        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            currentFillAmount = Mathf.Lerp(previousFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmount;

            yield return null;
        }
    }
}
