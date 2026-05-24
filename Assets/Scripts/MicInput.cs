using UnityEngine;

public class MicInput : MonoBehaviour
{
    public static float MicLoudness;
    private string _device;
    private AudioClip _clipRecord;
    private int _sampleWindow = 128;

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            _device = Microphone.devices[0];
            _clipRecord = Microphone.Start(_device, true, 10, 44100);
        }
        else
        {
            Debug.LogError("Мікрофон не знайдено!");
        }
    }

    void Update()
    {
        MicLoudness = LevelMax();
    }

    float LevelMax()
    {
        float levelMax = 0;
        float[] waveData = new float[_sampleWindow];

        // Отримуємо позицію запису (щоб читати свіжі дані)
        int micPosition = Microphone.GetPosition(_device) - (_sampleWindow + 1);
        if (micPosition < 0) return 0;

        // Зчитуємо дані з кліпу в масив waveData
        _clipRecord.GetData(waveData, micPosition);

        // Шукаємо найгучніший пік у вибірці
        for (int i = 0; i < _sampleWindow; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }
        return levelMax;
    }
}