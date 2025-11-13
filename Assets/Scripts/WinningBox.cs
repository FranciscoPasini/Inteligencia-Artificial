using UnityEngine;

public class WinningBoxTrigger : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == gameManager.gameObject) return; // seguridad

        //if (other.gameObject == gameManager.Player)
        {
            gameManager.TriggerWin();
        }
    }
}
