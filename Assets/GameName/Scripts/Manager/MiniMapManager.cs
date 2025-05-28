using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 50, 0);

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPos = target.position + offset;
            transform.position = newPos;
            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        }
    }
}