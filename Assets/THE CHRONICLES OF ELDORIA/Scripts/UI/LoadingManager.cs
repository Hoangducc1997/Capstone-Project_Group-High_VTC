using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject canvasLoading;
    [SerializeField] private Image processBar;
    [SerializeField] private TextMeshProUGUI textPercent;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float fixedLoadingTime = 3f;
    [SerializeField] private List<string> hintMessages;
    [SerializeField] private float hintChangeInterval = 1f;
    private void Start()
    {
        StartFakeLoading();
    }

    public void StartFakeLoading()
    {
        StartCoroutine(FakeLoading());
    }

    private IEnumerator FakeLoading()
    {
        canvasLoading.SetActive(true);
        canvasGroup.alpha = 0f;

        // Fade in
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * 2f;
            yield return null;
        }

        float elapsedTime = 0f;
        float nextHintTime = 0f;
        int hintIndex = 0;

        while (elapsedTime < fixedLoadingTime)
        {
            float progress = Mathf.Clamp01(elapsedTime / fixedLoadingTime);
            processBar.fillAmount = progress;
            textPercent.text = (progress * 100).ToString("0") + "%";

            if (elapsedTime >= nextHintTime && hintMessages.Count > 0)
            {
                hintText.text = hintMessages[hintIndex % hintMessages.Count];
                hintIndex++;
                nextHintTime += hintChangeInterval;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Fade out
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * 2f;
            yield return null;
        }

        canvasLoading.SetActive(false);
    }
}
