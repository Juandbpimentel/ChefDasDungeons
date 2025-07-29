using UnityEngine;

public class UtilFunctions : MonoBehaviour
{
    public static void DrawBox(Vector2 origin, Vector2 size, Vector2 direction, float angle, float distance, Color color)
    {
        // Calcula os vértices da caixa
        Vector2 right = new Vector2(direction.y, -direction.x) * size.x / 2;
        Vector2 up = direction * size.y / 2;

        Vector2 topLeft = origin - right + up;
        Vector2 topRight = origin + right + up;
        Vector2 bottomLeft = origin - right - up;
        Vector2 bottomRight = origin + right - up;

        // Calcula os vértices deslocados pela distância
        Vector2 topLeftEnd = topLeft + direction * distance;
        Vector2 topRightEnd = topRight + direction * distance;
        Vector2 bottomLeftEnd = bottomLeft + direction * distance;
        Vector2 bottomRightEnd = bottomRight + direction * distance;

        // Desenha as linhas da caixa inicial
        Debug.DrawLine(topLeft, topRight, color);
        Debug.DrawLine(topRight, bottomRight, color);
        Debug.DrawLine(bottomRight, bottomLeft, color);
        Debug.DrawLine(bottomLeft, topLeft, color);

        // Desenha as linhas da caixa final (após o deslocamento)
        Debug.DrawLine(topLeftEnd, topRightEnd, color);
        Debug.DrawLine(topRightEnd, bottomRightEnd, color);
        Debug.DrawLine(bottomRightEnd, bottomLeftEnd, color);
        Debug.DrawLine(bottomLeftEnd, topLeftEnd, color);

        // Desenha as linhas conectando os lados da caixa inicial e final
        Debug.DrawLine(topLeft, topLeftEnd, color);
        Debug.DrawLine(topRight, topRightEnd, color);
        Debug.DrawLine(bottomLeft, bottomLeftEnd, color);
        Debug.DrawLine(bottomRight, bottomRightEnd, color);

        // Desenha a linha central da caixa
        Vector2 centerStart = origin;
        Vector2 centerEnd = origin + direction * distance;
        Debug.DrawLine(centerStart, centerEnd, color);
    }
}
