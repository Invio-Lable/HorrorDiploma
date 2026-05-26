/*using UnityEngine;
using UnityEngine.UI;

public class NoiseMeterUI : MonoBehaviour
{
    [Header("UI Елементи")]
    public Image noiseBarFill;

    [Header("Налаштування чутливості")]
    [Tooltip("Максимальне значення гучності для заповнення смужки")]
    public float maxVisualLoudness = 0.2f;

    [Tooltip("Поріг, при якому ворог нас чує (має співпадати з налаштуванням ворога)")]
    public float dangerThreshold = 0.05f;

    [Header("Кольори")]
    public Color safeColor = Color.white;
    public Color dangerColor = Color.red;

    private float currentVisualFill = 0f;

    void Update()
    {
        // 1. Отримуємо поточну гучність з нашого мікрофона
        float currentLoudness = MicInput.MicLoudness;

        // 2. Обчислюємо, наскільки має бути заповнена смужка (від 0 до 1)
        float targetFill = currentLoudness / maxVisualLoudness;
        targetFill = Mathf.Clamp01(targetFill);

        // 3. Робимо рух смужки плавним (щоб вона не "дьоргалась" надто різко)
        currentVisualFill = Mathf.Lerp(currentVisualFill, targetFill, Time.deltaTime * 15f);
        noiseBarFill.fillAmount = currentVisualFill;

        // 4. Змінюємо колір, якщо ми надто гучні
        if (currentLoudness >= dangerThreshold)
        {
            noiseBarFill.color = dangerColor;
        }
        else
        {
            noiseBarFill.color = safeColor;
        }
    }
}*/