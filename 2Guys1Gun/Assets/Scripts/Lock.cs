using UnityEngine;

public class Lock : MonoBehaviour
{
    private LockManager lockManager;

    private void Start()
    {
        lockManager = FindObjectOfType<LockManager>();
    }

    private void OnDestroy()
    {
        if (lockManager != null)
        {
            lockManager.MarkLockDestroyed(gameObject);
        }
    }
}
