using UnityEngine;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private GameObject LevelsPanel;
    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private GameObject creditPanel;

    [SerializeField]
    private AudioSource audioSrc;

    // Affiche le panneau des niveaux et masque le menu principal
    public void OpenLevelsPanel()
    {
        MenuPanel.SetActive(false);
        LevelsPanel.SetActive(true);
        creditPanel.SetActive(false);
        audioSrc.PlayOneShot(audioSrc.clip);

    }

    // Quitte l'application. Si on est dans l'éditeur, arrête le mode de jeu.
    public void OnApplicationQuit()
    {
        audioSrc.PlayOneShot(audioSrc.clip);

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}