using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private List<ExitTile> exits;

    [SerializeField] private VictoryPanel VPanel;

    void Start()
    {
        if (exits == null || exits.Count == 0)
        {
            Debug.LogWarning("NO EXIT IN LEVELMANAGER");
        }
    }

    void Update()
    {
        if (exits.Where(e => e.PlayerInside == false).ToArray().Length == 0)
        {
            Debug.Log("Level Complete");
            if (VPanel == null)
            {
                Debug.LogWarning("NO VICTORY PANEL IN LEVELMANAGER");
                return;
            }
            VPanel.OpenVictoryPanel();
        }
    }
}
