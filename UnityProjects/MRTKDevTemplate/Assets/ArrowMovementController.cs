using UnityEngine;

public class MoveObjectController : MonoBehaviour
{
    public GameObject objectToMove; // The object you want to move
    public float slowSpeed = 1f; // Speed for forward/backward movement for 1-arrow buttons
    public float fastSpeed = 2f; // Speed for forward/backward movement for 2-arrow buttons
    public float slowTurnSpeed = 30f; // Speed for turning left/right for 1-arrow buttons
    public float fastTurnSpeed = 60f; // Speed for turning left/right for 2-arrow buttons

    private Rigidbody rb; // Reference to the Rigidbody component
    private Vector3 movementDirection = Vector3.zero; // Direction the object should move
    private float currentSpeed = 0f; // Current movement speed of the object
    private float currentTurnSpeed = 0f; // Current rotation speed of the object
    private bool isMoving = false; // Flag to control movement based on gaze or input

    private void Start()
    {
        // Get the Rigidbody component
        rb = objectToMove.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on the object to move!");
        }
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        // Move the object forward or backward
        if (isMoving && currentSpeed > 0)
        {
            Vector3 newPosition = rb.position + movementDirection * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }

        // Rotate the object left or right
        if (isMoving && currentTurnSpeed != 0)
        {
            Quaternion newRotation = Quaternion.Euler(0, currentTurnSpeed * Time.fixedDeltaTime, 0);
            rb.MoveRotation(rb.rotation * newRotation);
        }
    }

    // Functions to be called by buttons for forward/backward movement
    public void MoveForwardSlow() { SetMovement(objectToMove.transform.forward, slowSpeed); }
    public void MoveForwardFast() { SetMovement(objectToMove.transform.forward, fastSpeed); }

    public void MoveBackwardSlow() { SetMovement(-objectToMove.transform.forward, slowSpeed); }
    public void MoveBackwardFast() { SetMovement(-objectToMove.transform.forward, fastSpeed); }

    // Functions to be called by buttons for turning left/right
    public void TurnLeftSlow() { SetTurn(-slowTurnSpeed); }
    public void TurnLeftFast() { SetTurn(-fastTurnSpeed); }

    public void TurnRightSlow() { SetTurn(slowTurnSpeed); }
    public void TurnRightFast() { SetTurn(fastTurnSpeed); }

    public void StopMovement() { SetMovement(Vector3.zero, 0f); StopTurn(); }

    // Helper function to set movement direction and speed
    private void SetMovement(Vector3 direction, float speed)
    {
        movementDirection = direction;
        currentSpeed = speed;
        isMoving = true; // Start moving
    }

    // Helper function to set turning speed
    private void SetTurn(float turnSpeed)
    {
        currentTurnSpeed = turnSpeed;
        isMoving = true; // Start turning
    }

    // Stop turning
    private void StopTurn()
    {
        currentTurnSpeed = 0f;
    }

    // Stop all movement
    public void Stop()
    {
        isMoving = false;
        currentSpeed = 0f;
        currentTurnSpeed = 0f;
    }
}
