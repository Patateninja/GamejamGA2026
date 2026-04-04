using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [Tooltip("Identifiant unique utilisé pour la sauvegarde (ex: lvl_01). Si vide, le nom de l'objet sera utilisé.")]
    public string levelId;

    [Tooltip("Nom de la scène associée à ce slot (utilisé par la méthode loadThis).")]
    public string sceneName;

    [Tooltip("Identifiant du niveau à débloquer automatiquement lorsque ce niveau est terminé (optionnel).")]
    public string nextLevelId;

    [Header("État")]
    public bool isUnlocked = false;
    public bool isCompleted = false;

    private Button _button;
    private Image _image;

    private string Id => string.IsNullOrEmpty(levelId) ? gameObject.name : levelId;
    private string UnlockedKey => $"Level_unlocked_{levelId}";
    private string CompletedKey => $"Level_completed_{levelId}";

    private void Awake()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        LoadState();
        ApplyVisualState();
    }

    // Charge l'état depuis PlayerPrefs
    private void LoadState()
    {
        isUnlocked = PlayerPrefs.GetInt(UnlockedKey, isUnlocked ? 1 : 0) == 1;
        isCompleted = PlayerPrefs.GetInt(CompletedKey, isCompleted ? 1 : 0) == 1;
    }

    // Sauvegarde l'état "unlock"
    private void SaveUnlocked(bool unlocked)
    {
        PlayerPrefs.SetInt(UnlockedKey, unlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Sauvegarde l'état "completed"
    private void SaveCompleted(bool completed)
    {
        PlayerPrefs.SetInt(CompletedKey, completed ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Met à jour l'apparence en fonction des états
    public void ApplyVisualState()
    {
        if (_button != null)
            _button.interactable = isUnlocked;

        if (_image != null)
        {
            if (!isUnlocked)
            {
                _image.color = Color.gray;
            }
            else if (isCompleted)
            {
                // Niveau débloqué et terminé -> teinte verte claire
                _image.color = new Color(0.8f, 1f, 0.8f);
            }
            else
            {
                // Débloqué mais non terminé -> couleur normale
                _image.color = Color.white;
            }
        }
    }

    // Débloque ce slot et sauvegarde
    public void UnlockSlot()
    {
        isUnlocked = true;
        SaveUnlocked(true);
        ApplyVisualState();
    }

    // Verrouille ce slot et sauvegarde
    public void LockSlot()
    {
        isUnlocked = false;
        SaveUnlocked(false);
        ApplyVisualState();
    }

    // Marque le niveau comme terminé et sauvegarde ; débloque nextLevelId si fourni
    public void MarkCompleted()
    {
        isCompleted = true;
        SaveCompleted(true);
        ApplyVisualState();

        // Débloquer automatiquement le niveau suivant si un id est fourni
        if (!string.IsNullOrEmpty(nextLevelId))
        {
            string nextKey = $"Level_unlocked_{nextLevelId}";
            PlayerPrefs.SetInt(nextKey, 1);
            PlayerPrefs.Save();
        }
    }

    // Réinitialise la progression pour ce niveau
    public void ResetProgress()
    {
        // si la clé est string firsthey = $"Level_unlocked_Level01"; alors on doit la laisser accessible pour débloquer le niveau 1 et pouvoir jouer au jeu, mais on doit réinitialiser les autres niveaux
        string nextKey = $"Level_unlocked_Level01";
        if (UnlockedKey != nextKey)
        {
            PlayerPrefs.DeleteKey(UnlockedKey);
        }
        else
        {
            PlayerPrefs.SetInt(UnlockedKey, 1);
        }
        PlayerPrefs.DeleteKey(CompletedKey);
        PlayerPrefs.Save();

        isUnlocked = PlayerPrefs.HasKey(UnlockedKey);
        isCompleted = false;
        ApplyVisualState();
    }

    // Nouvelle méthode : efface toutes les PlayerPrefs et réinitialise l'état local
    public void clearPlayerPref()
    {
        // Supprime toutes les clés sauvegardées
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Réinitialise l'état local et met à jour l'affichage
        isUnlocked = false;
        isCompleted = false;
        ApplyVisualState();

        // Debug utile pendant le développement
        Debug.Log("PlayerPrefs cleared (DeleteAll) and slot state reset.");
    }

    // Méthode publique existante pour charger une scène
    public void loadThis(string levelName)
    {
        if (isUnlocked)
        {
            if (!string.IsNullOrEmpty(levelName))
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



