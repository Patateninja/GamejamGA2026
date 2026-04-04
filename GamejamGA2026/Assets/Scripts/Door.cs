using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Door : MonoBehaviour
{
    [SerializeField]
    List<GameButton> button;

    void Start()
    {
        
    }

    void Update()
    {
        if (button.Where(b => !b.active).ToArray().Length == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
