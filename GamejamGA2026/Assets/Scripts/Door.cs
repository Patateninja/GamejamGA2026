using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Door : MonoBehaviour
{
    [SerializeField]
    private List<GameButton> button;

    [SerializeField]
    private GameObject door;

    [SerializeField]
    private AudioSource audioSrc;

    private bool open = false;

    private float initY;

    void Start()
    {
        initY = door.transform.position.y;
    }

    void Update()
    {
        if (open == false && button.Where(b => !b.active).ToArray().Length == 0)
        {
            open = true;
            audioSrc.PlayOneShot(audioSrc.clip);
        }
        else if (open == true && button.Where(b => !b.active).ToArray().Length != 0)
        {
            open = false;
            audioSrc.PlayOneShot(audioSrc.clip);
        }

        door.transform.position = Vector3.Lerp(door.transform.position, new Vector3(door.transform.position.x, open ? initY -.75f : initY, door.transform.position.z), Time.deltaTime * 3f);

    }
}
