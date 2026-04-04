using UnityEngine;
using UnityEngine.InputSystem;

public class VictoryPanel : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private LevelProgressManager levelProgressManager;

    // Affiche le panneau de victory et met le temps à l'arrêt
    public void OpenVictoryPanel()
    {
        Time.timeScale = 0f; // Met le temps à l'arrêt
        (victoryPanel ?? gameObject).SetActive(true); // Affiche le panneau de victory (fallback)
        //appelle la fonction de level completed du level progress manager pour marquer le niveau comme terminé
        levelProgressManager.levelCompleted();
    }

    // Masque le panneau de victory et reprend le temps
    public void CloseVictoryPanel()
    {
        Time.timeScale = 1f; // Reprend le temps
        (victoryPanel ?? gameObject).SetActive(false); // Masque le panneau de victory (fallback)
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Reprend le temps
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGameToMenu()
    {
        Time.timeScale = 1f; // Reprend le temps
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void NextLevel()
    {
        Time.timeScale = 1f; // Reprend le temps
                             //regarde les players prefs pour savoir si le niveau suivant est débloqué et si oui, charge le niveau suivant avec le system de unlock fait dans le levelProgressManager
        string nextLevelId = levelProgressManager.nextLevelIds;
        if (!string.IsNullOrEmpty(nextLevelId))
        {
            string nextKey = $"Level_unlocked_{nextLevelId}";
            if (PlayerPrefs.GetInt(nextKey, 0) == 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelId);
            }
            else
            {
                Debug.LogWarning($"Niveau suivant '{nextLevelId}' non débloqué. Vérifiez les PlayerPrefs.");
                // retourne au menu principal si le niveau suivant n'est pas débloqué (optionnel)
                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
            }
        }
        else
        {
            Debug.LogWarning("Aucun ID de niveau suivant spécifié dans LevelProgressManager.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }

    void Update()
    {
        if ((victoryPanel ?? gameObject).activeSelf && Time.timeScale != 0f)
        {
            Time.timeScale = 0f;
        }
    }
}

