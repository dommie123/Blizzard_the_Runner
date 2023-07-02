using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private Vector2 offset;
    private Vector3 initialPosition;
    private CameraShakeSystem cameraShake;
    private float cameraShakeOffset;

    [Tooltip("How far left/right the camera is set from the player")]
    public float offsetX;
    [Tooltip("How far up/down the camera is to the player")]
    public float offsetY;

    [Tooltip("Lock the camera to its X axis")]
    public bool lockX;
    [Tooltip("Lock the camera to its Y axis")]
    public bool lockY;

    [Tooltip("Enable minimum and maximum X position for camera")]
    public bool limitX;
    [Tooltip("Enable minimum and maximum Y position for camera")]
    public bool limitY;

    [Tooltip("Minimum and maximum values for cameras X distance")]
    public Vector2 xMinMax;
    [Tooltip("Minimum and maximum values for cameras Y distance")]
    public Vector2 yMinMax;

    // Start is called before the first frame update
    private void Awake()
    {
        // offset = new Vector2(offsetX, offsetY);
        cameraShake = GameObject.Find("Camera Shake System").GetComponent<CameraShakeSystem>();
        initialPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        float cameraShakeOffset = cameraShake.GetCameraShakeOffset();
        string cameraShakeMode = cameraShake.GetCameraShakeMode();

        switch(cameraShakeMode)
        {
            case "vibrate x":
                offset = new Vector2(offsetX + cameraShakeOffset, offsetY);
                break;
            case "vibrate y":
                offset = new Vector2(offsetX, offsetY + cameraShakeOffset);
                break;
        }

        // offset = new Vector2(offsetX, offsetY);
        
        if (lockX || lockY)
        {
            if (!lockX && lockY)
            {
                transform.position = new Vector3(player.transform.position.x + offset.x, initialPosition.y + offset.y, -10);
            }

            else if (!lockY && lockX)
            {
                transform.position = new Vector3(initialPosition.x + offset.x, player.transform.position.y + offset.y, -10);
            }
        }

        /*
        if (!lockX && !lockY)
        {
            transform.position = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, -10);
        }
        */

        if (limitX || limitY)
        {
            if (limitX && !limitY)
            {
                transform.position = new Vector3(Mathf.Clamp(player.transform.position.x, xMinMax.x, xMinMax.y) + offset.x, player.transform.position.y + offset.y, -10);
            }

            else if (!limitX && limitY)
            {
                transform.position = new Vector3(player.transform.position.x + offset.x, Mathf.Clamp(player.transform.position.y, yMinMax.x, yMinMax.y) + offset.y, -10);
            }

            else
            {
                transform.position = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, -10);
            }
        }
    }
}
