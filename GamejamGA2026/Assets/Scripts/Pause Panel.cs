using UnityEngine;
using UnityEngine.InputSystem;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private InputActionAsset inputAsset;

    private InputAction pauseAction;

    bool blockPausePanel = false;

    [SerializeField]
    private AudioSource audioSrc;

    public void BlockPausePanel()
    {
        blockPausePanel = true;
        ClosePausePanel();
    }

    // Affiche le panneau de pause et met le temps à l'arrêt
    public void OpenPausePanel()
    {
        if (blockPausePanel)
        {
            return;
        }
        Time.timeScale = 0f; // Met le temps à l'arrêt
        audioSrc.PlayOneShot(audioSrc.clip);
        (pausePanel ?? gameObject).SetActive(true); // Affiche le panneau de pause (fallback)
    }

    // Masque le panneau de pause et reprend le temps
    public void ClosePausePanel()
    {
        Time.timeScale = 1f; // Reprend le temps
        if (!blockPausePanel)
        {
            audioSrc.PlayOneShot(audioSrc.clip);
        }
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
        if (blockPausePanel)
        {
            return;
        }
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

    public void ClicRestart()
    {
        audioSrc.PlayOneShot(audioSrc.clip);
        RestartLevel();
    }

    public void RestartLevel()
    {   
        Time.timeScale = 1f; // Reprend le temps
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    public void QuitGameToMenu()
    {
        audioSrc.PlayOneShot(audioSrc.clip);
        Time.timeScale = 1f; // Reprend le temps
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    void Update()
    {
        if ((pausePanel ?? gameObject).activeSelf && Time.timeScale != 0f)
        {
            Time.timeScale = 0f;
        }
    }
}
