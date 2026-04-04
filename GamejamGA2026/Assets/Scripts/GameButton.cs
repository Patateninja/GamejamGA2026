using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public abstract class GameButton : MonoBehaviour
{
    [HideInInspector]
    public bool active = false;

    [SerializeField]
    protected GameObject OffModel;
    [SerializeField]
    protected GameObject OnModel;

    void Start()
    {
        OffModel.SetActive(true);
        OnModel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterMovementController character) || other.gameObject.CompareTag("LightCrate") || other.gameObject.CompareTag("Crate"))
        {
            Entered();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterMovementController character) || other.gameObject.CompareTag("LightCrate") || other.gameObject.CompareTag("Crate"))
        {
            Exited();
        }
    }
        
    public abstract void Entered();
    public abstract void Exited();
}
