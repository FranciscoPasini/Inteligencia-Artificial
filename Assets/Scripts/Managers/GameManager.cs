using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject victoryPanel;

    [SerializeField] private GameObject winningBox;

    private bool panelOpened = false;

    void Start()
    {
        // Aseguramos que el trigger de victoria tenga isTrigger = true
        if (winningBox != null)
        {
            Collider col = winningBox.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }
    }

    void Update()
    {
        if (panelOpened) return;

        // Busca si todavía existe el Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            OpenPanel(losePanel);
            panelOpened = true;
        }
    }

    // Dentro de GameManager (si está en el GameObject con collider trigger)
    private void OnTriggerEnter(Collider other)
    {
        if (panelOpened) return;
        if (other.CompareTag("Player"))
        {
            Win();
        }
    }


    public void Win()
    {
        OpenPanel(victoryPanel);
        panelOpened = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene("Level"); // asegúrate que "Level" exista en Build Settings
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenPanel(GameObject panel)
    {
        losePanel.SetActive(false);
        victoryPanel.SetActive(false);

        panel.SetActive(true);
    }
}
