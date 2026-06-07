using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Ќалаштуванн€ ѕеремоги")]
    public int totalLetters = 5;
    private int collectedLetters = 0;

    [Header("UI ≈лементи")]
    public GameObject letterReadingPanel;  // ѕанель з фоном записки
    public Text letterUIText;              // “екст на ц≥й панел≥
    public GameObject victoryCanvas;       // ≈кран "¬» ¬»√–јЋ»"

    [Header("√равцев≥ —крипти (щоб зупин€ти)")]
    public MonoBehaviour playerMoveScript;
    public MonoBehaviour playerLookScript;

    // «м≥нна, €ка показуЇ, чи читаЇмо ми зараз лист
    public bool isReading = false;
    private bool justOpened = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (isReading)
        {
            // якщо ми т≥льки-но в≥дкрили лист у цьому кадр≥ Ч пропускаЇмо перев≥рку ≥ чекаЇмо
            if (justOpened)
            {
                justOpened = false;
                return;
            }

            // якщо ми натиснули E вже ѕ≤—Ћя того, €к лист був в≥дкритий Ч закриваЇмо
            if (Input.GetKeyDown(KeyCode.E))
            {
                CloseLetter();
            }
        }
    }

    // ÷ей метод викликаЇтьс€, коли ми п≥дн≥маЇмо лист ≥з земл≥
    public void OpenLetter(LetterData data)
    {
        collectedLetters++;
        isReading = true;
        justOpened = true; // ¬микаЇмо наш захист при в≥дкритт≥!

        letterUIText.text = data.letterText;
        letterReadingPanel.SetActive(true);

        Time.timeScale = 0f;
        if (playerMoveScript != null) playerMoveScript.enabled = false;
        if (playerLookScript != null) playerLookScript.enabled = false;
    }

    private void CloseLetter()
    {
        isReading = false;
        letterReadingPanel.SetActive(false);

        // «н≥маЇмо з паузи
        Time.timeScale = 1f;
        if (playerMoveScript != null) playerMoveScript.enabled = true;
        if (playerLookScript != null) playerLookScript.enabled = true;

        // ѕерев≥р€Їмо, чи з≥брали ми вс≥ 5 лист≥в
        if (collectedLetters >= totalLetters)
        {
            TriggerVictory();
        }
    }

    private void TriggerVictory()
    {
        if (victoryCanvas != null) victoryCanvas.SetActive(true);
        Time.timeScale = 0f; // «нову зупин€Їмо час
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}