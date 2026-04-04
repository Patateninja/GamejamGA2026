using UnityEngine;

public class MainMenuRotation : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0, 5 * Time.deltaTime, 0);
    }
}
