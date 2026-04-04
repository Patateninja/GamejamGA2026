using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public abstract class GameButton : MonoBehaviour
{
    public bool active = false;

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterMovementController character))
        {
            PlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterMovementController character))
        {
            PlayerExit();
        }
    }

    public abstract void PlayerEnter();
    public abstract void PlayerExit();
}
