using UnityEngine;
using UnityEngine.InputSystem;

public class CrateMovement : MonoBehaviour
{
    [SerializeField]
    Camera cam;

    [SerializeField]
    private float tileSize;

    private Vector3 targetPos;
    void Start()
    {
        targetPos = transform.position;
    }
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Mathf.Min((transform.position - targetPos).magnitude, Time.deltaTime * 5f));
    }

    public void MoveThisDirection(Vector2 input)
    {
        Vector3 mvt = new Vector3(input.y == 0 ? input.x : 0f, 0f, input.y);

        if (!Physics.Raycast(targetPos, mvt, tileSize))
        {
            targetPos += Quaternion.Euler(0, cam.transform.rotation.y, 0) * mvt;
        }
    }
}
