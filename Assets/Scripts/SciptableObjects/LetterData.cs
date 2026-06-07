using UnityEngine;

[CreateAssetMenu(fileName = "NewLetter", menuName = "Horror/Letter")]
public class LetterData : ScriptableObject
{
    [TextArea(10, 15)]
    public string letterText; // Текст, який буде в цій конкретній записці
}