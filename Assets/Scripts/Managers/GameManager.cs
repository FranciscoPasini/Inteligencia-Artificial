using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject losePanel;

    [Header("Player")]
    [SerializeField] private GameObject player;

    [Header("Victory Block")]
    [SerializeField] private GameObject victoryBlock;  // El cuadrado que aparece al ganar

    private bool triggeredLose = false;
    private bool victoryActivated = false;

    private int totalNPCs;

    void Start()
    {
        // Contar NPCs iniciales por tag
        totalNPCs = GameObject.FindGameObjectsWithTag("NPC").Length;

        if (victoryBlock != null)
            victoryBlock.SetActive(false); // Ocultarlo al inicio
    }

    void Update()
    {
        if (triggeredLose) return;

        // Si el player muere  Lose
        if (player == null)
        {
            triggeredLose = true;
            OpenLosePanel();
            return;
        }

        // Verificar NPCs restantes
        int npcsLeft = GameObject.FindGameObjectsWithTag("NPC").Length;

        // Si no queda ninguno activar bloque de victoria
        if (!victoryActivated && npcsLeft == 0 && totalNPCs > 0)
        {
            ActivateVictoryBlock();
        }
    }

    private void ActivateVictoryBlock()
    {
        victoryActivated = true;

        if (victoryBlock != null)
        {
            victoryBlock.SetActive(true);
            Debug.Log("? Victory block activado");
        }
    }

    private void OpenLosePanel()
    {
        Time.timeScale = 0f;

        if (losePanel != null)
            losePanel.SetActive(true);
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
