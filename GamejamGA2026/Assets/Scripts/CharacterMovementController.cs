using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovementController : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float tileSize;

    [SerializeField]
    private InputActionAsset inputAsset;

    private InputAction moveAction;

    private Vector3 targetPos;
    private float timer;

    [SerializeField]
    private GameObject playerSprite;

    void Awake()
    {
        if (inputAsset)
        {
            moveAction = inputAsset.FindAction("Player/Move");
        }
        else
        {
            Debug.LogError($"NO INPUT ASSET IN CHARACTER {name}");
        }
    }

    void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > .5f)
        {
            Movement();
        }

        if (playerSprite)
        {
            playerSprite.transform.rotation = cam.transform.rotation;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, Mathf.Min((transform.position - targetPos).magnitude, Time.deltaTime * 6f));
    }

    private void Movement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 mvt = new Vector3(input.y == 0 ? input.x : 0f, 0f, input.y);

        RaycastHit hit;
        if (!Physics.Raycast(targetPos + new Vector3(0,.5f,0f), mvt, out hit, tileSize+0.4f))
        {
            targetPos += Quaternion.Euler(0, cam.transform.rotation.y, 0) * mvt;
        }
        else
        {
            //verifier que le raycast hit une create et si c'est le cas, faire le mouvement sur la create et pas sur le personnage
            if (hit.collider.gameObject.CompareTag("LightCrate"))
            {
                hit.collider.gameObject.GetComponent<CrateMovement>().MoveThisDirection(input);
                targetPos += Quaternion.Euler(0, cam.transform.rotation.y, 0) * mvt;
            }
            else if (hit.collider.gameObject.CompareTag("Crate"))
            {
                hit.collider.gameObject.GetComponent<CrateMovement>().MoveThisDirection(input);
            }
        }


        if (input.magnitude != 0f)
        {
            timer = 0f;
        }
    }
}