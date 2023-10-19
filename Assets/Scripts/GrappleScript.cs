using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{
    public static GrappleScript instance;
    public Camera mainCamera;
    public LineRenderer lineRenderer;
    public DistanceJoint2D distanceJoint;
    public PlayerController player;
    public float grappleCooldown;
    public bool isGrappling;

    private AudioSource grappleSFX;
    private ClipSwapper grappleSwapper;

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;   

        grappleCooldown = 0f;

        grappleSFX = GameObject.Find("Grapple").GetComponent<AudioSource>();
        grappleSwapper = GameObject.Find("Grapple").GetComponent<ClipSwapper>();
    }

    // Update is called once per frame
    private void Update()
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

    private void UpdateCooldown()
    {
        if (grappleCooldown > 0)
        {
            grappleCooldown -= Time.deltaTime;
        }
    }

    private void UpdatePlayerInputs()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && grappleCooldown <= 0f && !player.IsGrounded() && !player.GetPlayerPausedGame())
        {
            grappleSwapper.SwitchToClip(0);
            grappleSFX.Play();

            isGrappling = true;
            player.SetCanCombo(true);

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
            if (isGrappling)
            {
                grappleSwapper.SwitchToClip(1);
                grappleSFX.Play();
            }

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
