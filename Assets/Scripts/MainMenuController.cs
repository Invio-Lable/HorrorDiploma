using UnityEngine;
using UnityEngine.SceneManagement; // Обов'язково для керування сценами

public class MainMenuController : MonoBehaviour
{
    public void RestartGame()
    {
        // 1. Повертаємо час у нормальний стан (бо скрімер його зупинив через Time.timeScale = 0)
        Time.timeScale = 1f;

        // 2. Отримуємо назву поточної активної сцени
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 3. Завантажуємо цю ж сцену заново
        SceneManager.LoadScene(currentSceneName);
    }
}