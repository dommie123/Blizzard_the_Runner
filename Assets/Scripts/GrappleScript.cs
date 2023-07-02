using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{
    public static GrappleScript instance;
    public Camera mainCamera;
    public CameraController cameraController;
    public LineRenderer lineRenderer;
    public DistanceJoint2D distanceJoint;
    public PlayerController player;
    public float grappleCooldown;
    public bool isGrappling;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;   

        grappleCooldown = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.IsDead())
        {
            UpdateCooldown();
            UpdatePlayerInputs();
        }
        else
        {
            isGrappling = false;
            distanceJoint.enabled = false;
            lineRenderer.enabled = false;
        }
    }

    void UpdateCooldown()
    {
        if (grappleCooldown > 0)
        {
            grappleCooldown -= Time.deltaTime;
        }
    }

    void UpdatePlayerInputs()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && grappleCooldown <= 0f && !player.IsGrounded())
        {
            isGrappling = true;
            player.canCombo = true;

            Vector2 mousePos = (Vector2) mainCamera.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.SetPosition(0, mousePos);
            lineRenderer.SetPosition(1, transform.position);
            distanceJoint.connectedAnchor = mousePos;
            distanceJoint.enabled = true;
            lineRenderer.enabled = true;

            if (grappleCooldown <= 0f)
            {
                grappleCooldown = 2f;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) || player.IsGrounded())
        {
            isGrappling = false;
            distanceJoint.enabled = false;
            lineRenderer.enabled = false;
        }

        if (distanceJoint.enabled)
        {
            lineRenderer.SetPosition(1, transform.position);
        }
    }
}
