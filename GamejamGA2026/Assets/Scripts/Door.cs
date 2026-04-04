using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Door : MonoBehaviour
{
    [SerializeField]
    List<bool> button;

    void Start()
    {
        
    }

    void Update()
    {
        if (button.Where(b => !b).ToArray().Length == 0)
        {
            Destroy(gameObject);
        }
    }
}
