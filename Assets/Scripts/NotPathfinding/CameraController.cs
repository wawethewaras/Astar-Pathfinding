using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    private bool followPlayer;

    [SerializeField]
    private float xOffset;
    [SerializeField]
    private float yOffset;
    [SerializeField]
    private float smoothing = 10f;
    [SerializeField]
    private Transform target;

    private Camera theCamera;


    void Start()
    {
        if (followPlayer)
        {
            target = FindObjectOfType<PlayerController>().transform;
        }

        if (target != null)
        {
            transform.position = target.transform.position;
        }

    }

    void Update()
    {
        CameraFollowTarget(target);

    }

    public void CameraFollowTarget(Transform target)
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x + xOffset, target.position.y + yOffset, /*target.position.z */-10f), smoothing * Time.deltaTime);
        }
    }

    public void TransformCameraInstantlyToPosiotion(Transform target)
    {
        if (target != null)
        {
            transform.position = target.transform.position;
        }
    }

}

