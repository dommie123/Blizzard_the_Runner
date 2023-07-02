using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShakeRate { IMPULSE, CONTINUOUS }
public enum ShakeMode { VIBRATE_X, VIBRATE_Y, RUMBLE }

public class CameraShakeSystem : MonoBehaviour
{
    [SerializeField] private ShakeRate shakeRate;
    [SerializeField] private ShakeMode shakeMode;
    [SerializeField] private float amplitude;
    [SerializeField] private float frequency;

    private float x;            // x as in y=Asin(Bx) for camera
    private float shakeTimer;
    private bool isShaking;

    // Start is called before the first frame update
    void Start()
    {
        x = 0f;
        shakeTimer = -1f;
        isShaking = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isShaking && (x < shakeTimer || shakeRate == ShakeRate.CONTINUOUS))
        {
            ShakeCamera();
        }
        else if (x >= shakeTimer && shakeTimer >= 0f)
        {
            StopShaking();
        }
    }

    public void StartShaking()
    {
        isShaking = true;

        switch(shakeRate)
        {
            case ShakeRate.IMPULSE:
                shakeTimer = (2 * Mathf.PI) / frequency;
                break;
            case ShakeRate.CONTINUOUS:
                shakeTimer = -1f;
                break;
        }

        
    }

    public void StopShaking()
    {
        isShaking = false;
        x = 0f;
    }

    public void ChangeShakeMode(string newMode)
    {
        switch(newMode)
        {
            case "vibrate x":
                shakeMode = ShakeMode.VIBRATE_X;
                break;
            case "vibrate y":
                shakeMode = ShakeMode.VIBRATE_Y;
                break;
            case "rumble":
                shakeMode = ShakeMode.RUMBLE;
                break;
        }
    }

    public void SetAmplitude(float amplitude)
    {
        this.amplitude = amplitude;
    }

    public void SetFrequency(float frequency)
    {
        this.frequency = frequency;
    }

    public float GetCameraShakeOffset()
    {
        float cameraOffset = amplitude * Mathf.Sin(frequency * x);
        return cameraOffset;
    }

    public string GetCameraShakeMode()
    {
        switch(shakeMode)
        {
            case ShakeMode.VIBRATE_X:
                return "vibrate x";
            case ShakeMode.VIBRATE_Y:
                return "vibrate y";
            case ShakeMode.RUMBLE:
                return "rumble";
            default: 
                return "unknown";
        }
    }

    private void ShakeCamera()
    {
        x += Time.deltaTime;

        float cameraOffset = amplitude * Mathf.Sin(frequency * x);
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 newCameraPosition;
        
        switch(shakeMode)
        {
            case ShakeMode.VIBRATE_X:
                newCameraPosition = new Vector3(cameraPosition.x + cameraOffset, cameraPosition.y, cameraPosition.z);
                Camera.main.gameObject.transform.position = newCameraPosition;
                break;
            case ShakeMode.VIBRATE_Y:
                newCameraPosition = new Vector3(cameraPosition.x, cameraPosition.y + cameraOffset, cameraPosition.z);
                Camera.main.gameObject.transform.position = newCameraPosition;
                break;
            case ShakeMode.RUMBLE:
                Camera.main.orthographicSize += cameraOffset;
                break;
        }
    }
}
