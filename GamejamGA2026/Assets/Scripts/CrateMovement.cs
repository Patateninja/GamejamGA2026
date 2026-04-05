using UnityEngine;
using System.Collections;

public class CrateMovement : MonoBehaviour
{
    [SerializeField]
    private float tileSize;

    private bool falling = false;

    [SerializeField]
    private GameObject Splash;
    [SerializeField]
    private AudioSource splashAudioSrc;
    [SerializeField]
    private AudioSource pushAudioSrc;

    private Vector3 targetPos;
    void Start()
    {
        gameObject.SetActive(true);
        targetPos = transform.position;
    }
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Mathf.Min((transform.position - targetPos).magnitude, Time.deltaTime * 5f));
    }

    public bool MoveThisDirection(Vector2 input, Camera cam)
    {
        Vector3 mvt = new Vector3(input.y == 0 ? input.x : 0f, 0f, input.y);

        if (!Physics.Raycast(targetPos + new Vector3(0, .5f, 0f), mvt, tileSize))
        {
            targetPos += Quaternion.Euler(0, cam.transform.rotation.y, 0) * mvt;
            pushAudioSrc.PlayOneShot(pushAudioSrc.clip);
            if (!Physics.Raycast(targetPos + new Vector3(0f, .5f, 0f), Vector3.down, 1f) && !falling)
            {
                falling = true;
                StartCoroutine(Fall());
            }
            return true;
        }
        return false;
    }

    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(0.6f);

        targetPos += new Vector3(0f, 1f, 0f);

        yield return new WaitForSeconds(0.5f);

        targetPos += new Vector3(0f, -5f, 0f);

        Splash.transform.position = targetPos;
        Splash.transform.position = new Vector3(Splash.transform.position.x, -1.5f, Splash.transform.position.z);
        splashAudioSrc.PlayOneShot(splashAudioSrc.clip);
        Splash.SetActive(true);

        yield return new WaitForSeconds(0.9f);

        Splash.SetActive(false);
        gameObject.SetActive(false);
    }

}
