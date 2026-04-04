using UnityEngine;
using UnityEngine.InputSystem;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private InputActionAsset inputAsset;

    private InputAction pauseAction;

    // Affiche le panneau de pause et met le temps à l'arrêt
    public void OpenPausePanel()
    {
        Time.timeScale = 0f; // Met le temps à l'arrêt
        (pausePanel ?? gameObject).SetActive(true); // Affiche le panneau de pause (fallback)
    }

    // Masque le panneau de pause et reprend le temps
    public void ClosePausePanel()
    {
        Time.timeScale = 1f; // Reprend le temps
        (pausePanel ?? gameObject).SetActive(false); // Masque le panneau de pause (fallback)
    }

    private void Awake()
    {
        if (inputAsset == null)
        {
            Debug.LogError($"NO INPUT ASSET IN PAUSE PANEL {name}");
            return;
        }

        pauseAction = inputAsset.FindAction("Player/Pause");
        if (pauseAction == null)
        {
            Debug.LogError($"Pause action not found in input asset for {name}. Vérifiez le chemin 'Player/Pause'.");
        }
    }

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.performed += TogglePause;
            pauseAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.performed -= TogglePause;
            pauseAction.Disable();
        }
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        //si le panneau de victory est actif, ne pas permettre d'ouvrir le panneau de pause
        if ((victoryPanel ?? gameObject).activeSelf)
        {
            return;
        }

        var panel = pausePanel ?? gameObject;
        if (panel.activeSelf)
        {
            ClosePausePanel();
        }
        else
        {
            OpenPausePanel();
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Reprend le temps
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    public void QuitGameToMenu()
    {
        Time.timeScale = 1f; // Reprend le temps
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu 1");
    }

    void Update()
    {
        if ((pausePanel ?? gameObject).activeSelf && Time.timeScale != 0f)
        {
            Time.timeScale = 0f;
        }
    }
}
