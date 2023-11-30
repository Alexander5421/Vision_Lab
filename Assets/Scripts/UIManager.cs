using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject endMenu;
    [SerializeField] private Text scoreText;

    public void ShowStartMenu()
    {
        /* ... */
        throw new NotImplementedException();
    }
    public void HideStartMenu() { /* ... */ }
    public void ShowEndMenu(int score) { /* ... */ }
}