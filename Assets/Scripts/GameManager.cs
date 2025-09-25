using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TMP_Text messageText; // asignar en inspector
    bool gameOver = false;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
    }

    public void PlayerKilled()
    {
        if (gameOver) return;
        gameOver = true;
        if (messageText != null)
            messageText.text = "GAME OVER\nHas sido eliminado por un enemigo.";
        Time.timeScale = 0f; // pausar la simulación para demo
        Debug.Log("Player killed - Game Over");
    }

    // Podés agregar Restart() que recarge la escena, etc.
}

