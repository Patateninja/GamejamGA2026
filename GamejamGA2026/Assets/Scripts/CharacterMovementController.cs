using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    private bool falling = false;

    [SerializeField]
    private Canvas Canvas;

    [SerializeField]
    private GameObject Splash;

    void Awake()
    {
        Splash.SetActive(false);
        if (inputAsset)
        {
            moveAction = inputAsset.FindAction("Player/Move");
        }
        else
        {
            Debug.LogError($"NO INPUT ASSET IN CHARACTER {name}");
        }
        Canvas.GetComponent<ShadeManager>().FadeOut(1f);
    }

    void Start()
    {
        targetPos = transform.position;

        playerSprite.GetComponent<Animator>().SetFloat("Magnitude", 0f);
        playerSprite.GetComponent<Animator>().SetFloat("X", 0f);
        playerSprite.GetComponent<Animator>().SetFloat("Y", 0f);
        playerSprite.GetComponent<Animator>().SetFloat("MemX", -1f);
        playerSprite.GetComponent<Animator>().SetFloat("MemY", 0f);
        playerSprite.GetComponent<Animator>().SetBool("falling", falling);
    }

    void Update()
    {

        if (!falling)
        {
            timer += Time.deltaTime;

            if (timer > .5f)
            {
                Movement();
            }
        }
        else
        {
            Vector3 mvt = new Vector3(playerSprite.GetComponent<Animator>().GetFloat("MemX"), 0f, playerSprite.GetComponent<Animator>().GetFloat("MemY"));
            if (mvt.sqrMagnitude > 1f)
            {
                mvt.Normalize();
            }
            if (mvt.magnitude > 0.1f)
            {
                playerSprite.GetComponent<Animator>().SetFloat("Magnitude", mvt.magnitude);
            }
            else
            {
                playerSprite.GetComponent<Animator>().SetFloat("Magnitude", 0f);
            }
        }

        // Lerp pour un mouvement plus fluide
        transform.position = Vector3.Lerp(transform.position, targetPos, Mathf.Min((transform.position - targetPos).magnitude, Time.deltaTime * 6f));
        //quand le lerp est presque terminé, snap à la position cible pour éviter les petits écarts
        if ((transform.position - targetPos).magnitude < 0.1f)
        {
            transform.position = targetPos;
        }

    }

    private void Movement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 mvt = new Vector3(input.y == 0 ? input.x : 0f, 0f, input.y);

        if (mvt.sqrMagnitude > 1f)
        {
            mvt.Normalize();
        }

        if (mvt.magnitude > 0.1f)
        {
            playerSprite.GetComponent<Animator>().SetFloat("Magnitude", mvt.magnitude);
        }
        else
        {
            playerSprite.GetComponent<Animator>().SetFloat("Magnitude", 0f);
        }

        if (mvt.magnitude == 0f)
        {
            // Si le personnage ne bouge pas, on garde la dernière direction pour l'animation
            playerSprite.GetComponent<Animator>().SetFloat("X", playerSprite.GetComponent<Animator>().GetFloat("MemX"));
            playerSprite.GetComponent<Animator>().SetFloat("Y", playerSprite.GetComponent<Animator>().GetFloat("MemY"));
        }
        else
        {
            // Si le personnage bouge, on met à jour la direction et la mémorise pour l'animation
            playerSprite.GetComponent<Animator>().SetFloat("X", mvt.x > 0f ? 1f : mvt.x < 0f ? -1f : 0f);
            playerSprite.GetComponent<Animator>().SetFloat("Y", mvt.z > 0f ? 1f : mvt.z < 0f ? -1f : 0f);
            // Mémorisation de la direction pour l'animation
            playerSprite.GetComponent<Animator>().SetFloat("MemX", mvt.x > 0f ? 1f : mvt.x < 0f ? -1f : 0f);
            playerSprite.GetComponent<Animator>().SetFloat("MemY", mvt.z > 0f ? 1f : mvt.z < 0f ? -1f : 0f);
        }


        RaycastHit hit;
        if (!Physics.Raycast(targetPos + new Vector3(0, .5f, 0f), mvt, out hit, tileSize + 0.4f))
        {
            targetPos += Quaternion.Euler(0, cam.transform.rotation.y, 0) * mvt;
        }
        else
        {
            if (hit.collider.gameObject.CompareTag("LightCrate"))
            {
                if (hit.collider.gameObject.GetComponent<CrateMovement>().MoveThisDirection(input))
                {
                    targetPos += Quaternion.Euler(0, cam.transform.rotation.y, 0) * mvt;
                }
            }
            else if (hit.collider.gameObject.CompareTag("Crate"))
            {
                hit.collider.gameObject.GetComponent<CrateMovement>().MoveThisDirection(input);
            }
        }

        if (!Physics.Raycast(targetPos + new Vector3(0f, .5f, 0f), Vector3.down, 1f) && !falling)
        {
            falling = true;
            StartCoroutine(Fall());
        }

        if (input.magnitude != 0f)
        {
            timer = 0f;
        }
    }

    private IEnumerator Fall()
    {
        Canvas.GetComponent<VictoryPanel>().BlockVictoryPanel();
        Canvas.GetComponent<PausePanel>().BlockPausePanel();

        yield return new WaitForSeconds(0.6f);

        playerSprite.GetComponent<Animator>().SetBool("falling", falling);

        targetPos += new Vector3(0f, 1f, 0f);

        yield return new WaitForSeconds(0.5f);

        targetPos += new Vector3(0f, -5f, 0f);
        Splash.transform.position = targetPos;
        Splash.transform.position = new Vector3(Splash.transform.position.x, -1.5f, Splash.transform.position.z);
        Splash.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        Canvas.GetComponent<ShadeManager>().FadeIn(1f);

        yield return new WaitForSeconds(0.4f);

        Splash.SetActive(false);

        yield return new WaitForSeconds(0.7f);

        Canvas.GetComponent<PausePanel>().RestartLevel();
    }
}