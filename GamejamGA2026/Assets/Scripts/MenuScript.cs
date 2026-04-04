using UnityEngine;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private GameObject LevelsPanel;
    [SerializeField] private GameObject MenuPanel;

    // Affiche le panneau des niveaux et masque le menu principal
    public void OpenLevelsPanel()
    {
        MenuPanel.SetActive(false);
        LevelsPanel.SetActive(true);
    }

    // Quitte l'application. Si on est dans l'éditeur, arrête le mode de jeu.
    public void OnApplicationQuit()
    {
        Application.Quit();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}