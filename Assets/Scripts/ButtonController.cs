using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public void ButtonClick()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.RestartButtonCallback();
        }
    }
}