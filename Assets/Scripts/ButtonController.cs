using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public void ButtonClick()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RestartButtonCallback();
        }
    }
}