using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ExitTile : MonoBehaviour
{
    public bool PlayerInside;

    void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterMovementController character))
        {
            PlayerInside = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterMovementController character))
        {
            PlayerInside = false;
        }
    }
}