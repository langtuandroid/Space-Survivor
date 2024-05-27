using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;

    private Vector3 cameraShakeOffset;


    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(playerPosition.position.x, playerPosition.position.y, transform.position.z) + cameraShakeOffset;
    }

    public void SetCameraShakeOffset(Vector3 offset)
    {
        cameraShakeOffset = offset;
    }
}
