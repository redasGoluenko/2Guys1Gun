using UnityEngine;

public class ObungaHandler : MonoBehaviour
{
    public ObungaPiece[] obungaPieces;
    private int currentIndex = 0;

    private void Start()
    {
        UpdatePieceColors();
    }

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
            UpdatePieceColors();
        }
    }

    private void UpdatePieceColors()
    {
        for (int i = 0; i < obungaPieces.Length; i++)
        {
            SpriteRenderer sr = obungaPieces[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = (i == currentIndex) ? Color.white : Color.black;
            }
        }
    }
}
