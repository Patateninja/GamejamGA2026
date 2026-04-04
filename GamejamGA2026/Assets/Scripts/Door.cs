using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Door : MonoBehaviour
{
    [SerializeField]
    private List<GameButton> button;

    [SerializeField]
    private GameObject door;

    private bool open = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (button.Where(b => !b.active).ToArray().Length == 0)
        {
            open = true;
        }
        else
        {
            open = false;
        }

        door.transform.position = Vector3.Lerp(door.transform.position, new Vector3(door.transform.position.x, open ? -.75f : .5f, door.transform.position.z), Time.deltaTime * 3f);

    }
}
