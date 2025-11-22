using UnityEngine;
using TMPro;
using System.Collections;

public class TextGuideManager : MonoBehaviour
{
    [SerializeField] private TMP_Text guideText;
    [SerializeField] private string[] guideSteps;

    private int currentStep = 0;
    private bool isFinalMessage = false;

    void Start()
    {
        ShowCurrentStep();
    }

    private void ShowCurrentStep()
    {
        if (guideText == null)
        {
            Debug.LogError("Error: guideText is not assigned!");
            return;
        }

        guideText.text = guideSteps[currentStep];
        Debug.Log("Guide step: " + currentStep);

        // 👉 如果是最后一句，开始倒计时
        if (currentStep == guideSteps.Length - 1)
        {
            if (!isFinalMessage)
            {
                isFinalMessage = true;
                StartCoroutine(HideAfterDelay());
            }
        }
    }

    public void NextStep()
    {
        if (currentStep < guideSteps.Length - 1)
        {
            currentStep++;
            ShowCurrentStep();
        }
    }

    // 👉 最后一段文本 5 秒后自动隐藏
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(5f);

        // 确保不在中途切换文本
        if (currentStep == guideSteps.Length - 1)
        {
            guideText.text = "";
        }
    }
}
