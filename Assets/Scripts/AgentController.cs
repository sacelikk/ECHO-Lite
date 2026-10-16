
using UnityEngine;
using UnityEngine.InputSystem;

public class AgentController : MonoBehaviour
{
    private GridManager grid;
    private Vector2Int currentPosition;
    private bool hasWon;

    public void Initialize(GridManager gridManager, Vector2Int startPosition)
    {
        grid = gridManager;
        currentPosition = startPosition;
        hasWon = false;

        transform.position = grid.GetWorldPosition(currentPosition);

        Debug.Log("ECHO: Agent baþlatýldý. Konum: " + currentPosition);
    }

    private void Update()
    {
        if (grid == null || hasWon)
            return;

        if (Keyboard.current == null)
        {
            Debug.LogWarning("ECHO: Klavye algýlanmýyor!");
            return;
        }

        Vector2Int direction = Vector2Int.zero;

        // Yukarý
        if (Keyboard.current.wKey.wasPressedThisFrame ||
            Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            direction = Vector2Int.up;
        }
        // Aþaðý
        else if (Keyboard.current.sKey.wasPressedThisFrame ||
                 Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            direction = Vector2Int.down;
        }
        // Sol
        else if (Keyboard.current.aKey.wasPressedThisFrame ||
                 Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            direction = Vector2Int.left;
        }
        // Sað
        else if (Keyboard.current.dKey.wasPressedThisFrame ||
                 Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            direction = Vector2Int.right;
        }

        // Tuþa basýlmadýysa çýk
        if (direction == Vector2Int.zero)
            return;

        Vector2Int nextPosition = currentPosition + direction;

        Debug.Log("ECHO: Hareket denendi. Yeni konum: " + nextPosition);

        // Duvar veya harita dýþý kontrolü
        if (!grid.IsWalkable(nextPosition))
        {
            Debug.Log("ECHO: Bu kareye hareket edilemiyor!");
            return;
        }

        // Konumu güncelle
        currentPosition = nextPosition;
        transform.position = grid.GetWorldPosition(currentPosition);

        Debug.Log("ECHO: Hareket baþarýlý! Konum: " + currentPosition);

        // Hedef kontrolü
        if (grid.IsGoal(currentPosition))
        {
            hasWon = true;
            Debug.Log("ECHO: Hedefe ulaþýldý!");
        }
    }
}
