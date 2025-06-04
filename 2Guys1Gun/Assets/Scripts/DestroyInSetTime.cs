using UnityEngine;

public class DestroyInSetTime : MonoBehaviour
{
    [SerializeField] private float destroyAfterSeconds = 2f; // How long before destruction

    void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }
}
