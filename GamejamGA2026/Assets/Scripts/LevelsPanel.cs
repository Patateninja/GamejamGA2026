using UnityEngine;

public class LevelsPanel : MonoBehaviour
{
    [SerializeField] private GameObject levelsPanel;
    [SerializeField] private GameObject menuPanel;


    // Affiche le panneau des niveaux et masque le menu principal
    public void GoBack()
    {
        levelsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    // Supprime toutes les données de progression et débloque uniquement le premier niveau
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        string nextKey = $"Level_unlocked_Level01";
        PlayerPrefs.SetInt(nextKey, 1);
        PlayerPrefs.Save();
    }
}
