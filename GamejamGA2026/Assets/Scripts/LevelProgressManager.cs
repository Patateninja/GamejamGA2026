using System;
using UnityEngine;

public class LevelProgressManager : MonoBehaviour
{
    [Tooltip("Liste des niveaux à gérer. Assurez-vous que les IDs correspondent à ceux utilisés dans le jeu.")]
    public string levelIds;
    public string nextLevelIds; // Optionnel : IDs des niveaux à débloquer automatiquement

    public void levelCompleted()
    {
        SetLevelCompleted(levelIds, true, nextLevelIds);
    }

    private static string CompletedKeyFor(string levelId)
    {
        if (string.IsNullOrEmpty(levelId))
            throw new ArgumentException("levelId ne peut pas être vide.", nameof(levelId));
        return $"Level_completed_{levelId}";
    }

    // Marque ou démarque un niveau comme terminé et sauvegarde dans PlayerPrefs.
    // Débloque automatiquement le niveau suivant si un nextLevelId est fourni ET que le niveau a été complété.
    public static void SetLevelCompleted(string levelId, bool completed, string nextLevelId = null)
    {
        string key = CompletedKeyFor(levelId);
        PlayerPrefs.SetInt(key, completed ? 1 : 0);

        // Débloquer automatiquement le niveau suivant si un id est fourni ET que le niveau courant est complété
        if (completed && !string.IsNullOrEmpty(nextLevelId))
        {
            string nextKey = $"Level_unlocked_{nextLevelId}";
            PlayerPrefs.SetInt(nextKey, 1);
        }

        // Sauvegarder une seule fois après toutes les modifications
        PlayerPrefs.Save();
    }

    // Vérifie si un niveau est marqué comme terminé
    public static bool IsLevelCompleted(string levelId)
    {
        string key = CompletedKeyFor(levelId);
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    // Supprime la progression d'un niveau (optionnel)
    public static void ClearLevelProgress(string levelId)
    {
        string key = CompletedKeyFor(levelId);
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
    }
}