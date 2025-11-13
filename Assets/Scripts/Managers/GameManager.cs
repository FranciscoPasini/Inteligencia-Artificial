using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject victoryPanel;

    [Header("Player Reference")]
    [SerializeField] private GameObject player;  // referencia directa

    private bool panelOpened = false;

    void Update()
    {
        // Si ya abrió panel, no seguimos
        if (panelOpened) return;

        // Si el player fue destruido -> pierde
        if (player == null)
        {
            OpenPanel(losePanel);
            panelOpened = true;
        }
    }

    public void TriggerWin()
    {
        if (panelOpened) return;

        OpenPanel(victoryPanel);
        panelOpened = true;
    }

    private void OpenPanel(GameObject panel)
    {
        Time.timeScale = 0f;

        if (losePanel != null) losePanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        panel.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
