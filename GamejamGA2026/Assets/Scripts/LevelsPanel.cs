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
        //applique les changements visuels à tous les slots de niveaux (optionnel, dépend de votre implémentation)
        Slot[] allSlots = FindObjectsByType<Slot>(FindObjectsSortMode.None);
        foreach (Slot slot in allSlots)
        {
            slot.ResetProgress();
        }

    }
}

