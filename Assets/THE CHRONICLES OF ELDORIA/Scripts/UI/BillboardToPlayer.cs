using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCam == null) return;

        // Chỉ xoay theo trục Y 
        Vector3 lookPos = mainCam.transform.position - transform.position;
        lookPos.y = 0; // giữ nguyên chiều đứng
        if (lookPos.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(-lookPos);
        }
    }
}
