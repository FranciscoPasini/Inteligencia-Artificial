using UnityEngine;

public static class RouletteWheel
{
    public static int Select(float[] weights)
    {
        float total = 0f;
        foreach (float w in weights)
            total += w;

        float randomPoint = Random.value * total;

        for (int i = 0; i < weights.Length; i++)
        {
            if (randomPoint < weights[i])
                return i;
            randomPoint -= weights[i];
        }

        return weights.Length - 1;
    }
}
