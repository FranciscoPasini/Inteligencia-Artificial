using UnityEngine;

public class TargetMover : MonoBehaviour
{
    public float moveInterval = 5f; // Cada cuántos segundos cambia de lugar
    public Vector2 minMaxX = new Vector2(-20, 20); // Límites del mapa en X
    public Vector2 minMaxZ = new Vector2(-20, 20); // Límites del mapa en Z
    public float height = 6f; // Altura de vuelo (Ground Height)

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= moveInterval)
        {
            MoveTarget();
            timer = 0;
        }
    }

    void MoveTarget()
    {
        // Genera una posición aleatoria dentro de los límites
        float randomX = Random.Range(minMaxX.x, minMaxX.y);
        float randomZ = Random.Range(minMaxZ.x, minMaxZ.y);

        // Mueve la esfera a esa nueva posición
        transform.position = new Vector3(randomX, height, randomZ);
    }
}