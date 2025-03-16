// Written by Jay Gunderson
// 03/13/2025
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    public Rigidbody rb;
    public Camera mainCamera;
    public GameObject crosshair; // Assign in inspector (small square object)

    public InputActionReference move;
    public InputActionReference look;

    private Vector3 moveDirection;
    [SerializeField] private float minCrosshairDistance = 1f; // Minimum distance from player
    [SerializeField] private float maxCrosshairDistance = 3f; // Maximum distance from player


    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined; // Keeps cursor inside game window but allows movement (good for top-down)

    }

    private void Update()
    {
        // Handle movement input
        Vector2 input = move.action.ReadValue<Vector2>();
        moveDirection = new Vector3(input.x, 0, input.y);

        // Handle crosshair placement
        AimCrosshair();
    }

    private void FixedUpdate()
    {
        // Move player
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
    }
    private void AimCrosshair()
    {
        Vector2 mousePosition = look.action.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        // Create ground plane (at y = 0 or player's height if needed)
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Or use transform.position for player's height

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter); // Get where mouse ray hits ground

            // Direction from player to that point
            Vector3 direction = hitPoint - transform.position;
            direction.y = 0; // Keep it flat

            // Clamp distance between min and max
            float distance = Mathf.Clamp(direction.magnitude, minCrosshairDistance, maxCrosshairDistance);
            Vector3 clampedDirection = direction.normalized * distance;

            // Final crosshair position
            Vector3 finalPosition = transform.position + clampedDirection;

            // Set crosshair position slightly above ground
            crosshair.transform.position = new Vector3(finalPosition.x, transform.position.y + 0.1f, finalPosition.z);

            // Keep crosshair upright
            crosshair.transform.rotation = Quaternion.identity;
        }
    }

}

