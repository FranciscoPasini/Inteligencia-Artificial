using UnityEngine;

public class WinningBoxTrigger : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;

        if (other.CompareTag("Player"))
        {
            used = true;
            Time.timeScale = 0f;
            victoryPanel.SetActive(true);
        }
    }
}
