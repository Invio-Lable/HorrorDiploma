using UnityEngine;

public class LetterItem : MonoBehaviour
{
    [Header("який це лист?")]
    public LetterData myData; // —юди ми перет€гнемо файлик Letter_1, Letter_2 тощо

    public void PickUp()
    {
        //  ажемо менеджеру в≥дкрити UI з нашими даними
        GameManager.Instance.OpenLetter(myData);

        // «нищуЇмо саму модельку з≥ столу
        Destroy(gameObject);
    }
}