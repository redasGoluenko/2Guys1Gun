using UnityEngine;

public class LockManager : MonoBehaviour
{
    public GameObject lock1;
    public GameObject lock2;
    public GameObject lock3;
    public GameObject lock4;

    [Header("Destroyed Status (Read Only)")]
     public bool lock1Destroyed;
     public bool lock2Destroyed;
     public bool lock3Destroyed;
     public bool lock4Destroyed;

    private void Start()
    {
        // Update local flags from PlayerPrefs
        lock1Destroyed = PlayerPrefs.GetInt("Lock1Destroyed", 0) == 1;
        lock2Destroyed = PlayerPrefs.GetInt("Lock2Destroyed", 0) == 1;
        lock3Destroyed = PlayerPrefs.GetInt("Lock3Destroyed", 0) == 1;
        lock4Destroyed = PlayerPrefs.GetInt("Lock4Destroyed", 0) == 1;

        // Destroy based on flags
        if (lock1Destroyed && lock1 != null) Destroy(lock1);
        if (lock2Destroyed && lock2 != null) Destroy(lock2);
        if (lock3Destroyed && lock3 != null) Destroy(lock3);
        if (lock4Destroyed && lock4 != null) Destroy(lock4);
    }

    public void MarkLockDestroyed(GameObject destroyedLock)
    {
        if (destroyedLock == lock1)
        {
            PlayerPrefs.SetInt("Lock1Destroyed", 1);
            lock1Destroyed = true;
        }
        else if (destroyedLock == lock2)
        {
            PlayerPrefs.SetInt("Lock2Destroyed", 1);
            lock2Destroyed = true;
        }
        else if (destroyedLock == lock3)
        {
            PlayerPrefs.SetInt("Lock3Destroyed", 1);
            lock3Destroyed = true;
        }
        else if (destroyedLock == lock4)
        {
            PlayerPrefs.SetInt("Lock4Destroyed", 1);
            lock4Destroyed = true;
        }

        PlayerPrefs.Save();
    }
}
