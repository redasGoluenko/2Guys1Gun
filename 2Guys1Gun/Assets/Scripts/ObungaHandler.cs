using UnityEngine;

public class ObungaHandler : MonoBehaviour
{
    public ObungaPiece[] obungaPieces;
    private int currentIndex = 0;

    public bool CanBeDamaged(int index)
    {
        return index == currentIndex;
    }

    public void DestroyObunga(int index)
    {
        if (index == currentIndex)
        {
            currentIndex++;
            Debug.Log($"Obunga {index + 1} destroyed. Next is {currentIndex + 1}");
        }
    }
}
