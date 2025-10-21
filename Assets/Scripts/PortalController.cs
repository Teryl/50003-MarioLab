using UnityEngine;
using UnityEngine.Events;

public class PortalController : MonoBehaviour
{
    
    public string nextSceneName;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                if (!string.IsNullOrEmpty(nextSceneName))
                {
                    gameManager.OnPortalEnter(nextSceneName);
                }
                else
                {
                    Debug.LogWarning("Next scene name is not set on PortalController.");
                }
            }
        }
    }
}
