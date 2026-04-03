using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool isUnlocked = false;

    public void UnlockSlot()
    {
        isUnlocked = true;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void LockSlot()
    {
        isUnlocked = false;
        GetComponent<SpriteRenderer>().color = Color.gray;
    }

    private void Start()
    {
        if (isUnlocked)
        {
            UnlockSlot();
        }
        else
        {
            LockSlot();
        }
    }

    public void loadThis(string levelName)
    {
        if (isUnlocked)
        {
            if (levelName != null)
            {
                LoadLevelByName(levelName);
            }
        }
    }

    private void LoadLevelByName(string levelName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }
}
