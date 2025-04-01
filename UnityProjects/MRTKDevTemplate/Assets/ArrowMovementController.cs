using UnityEngine;

public class MoveObjectController : MonoBehaviour
{
    public enum MovementMode
    {
        SimpleSpeedSet, // Option 1
        AcceleratedHold // Option 2
    }

    public MovementMode mode = MovementMode.SimpleSpeedSet;

    public GameObject objectToMove;
    public float slowSpeed = 1f;
    public float fastSpeed = 2f;
    public float accelerationRate = 1f;
    public float maxSpeed = 5f;
    public float slowTurnSpeed = 30f;
    public float fastTurnSpeed = 60f;

    private Rigidbody rb;
    private Vector3 movementDirection = Vector3.zero;
    private float currentSpeed = 0f;
    private float currentTurnSpeed = 0f;
    private bool isMoving = false;

    private bool acceleratingForward = false;
    private bool acceleratingBackward = false;

    private void Start()
    {
        rb = objectToMove.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on objectToMove!");
        }
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        if (mode == MovementMode.SimpleSpeedSet)
        {
            // Option 1 movement
            if (isMoving && currentSpeed > 0f)
            {
                Vector3 newPosition = rb.position + movementDirection * currentSpeed * Time.fixedDeltaTime;
                rb.MovePosition(newPosition);
            }
        }
        else if (mode == MovementMode.AcceleratedHold)
        {
            // Option 2 movement
            if (acceleratingForward)
            {
                currentSpeed += accelerationRate * Time.fixedDeltaTime;
            }
            else if (acceleratingBackward)
            {
                currentSpeed += accelerationRate * Time.fixedDeltaTime;
            }

            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

            if (currentSpeed > 0f)
            {
                Vector3 direction = acceleratingBackward ? -objectToMove.transform.forward : objectToMove.transform.forward;
                Vector3 newPosition = rb.position + direction * currentSpeed * Time.fixedDeltaTime;
                rb.MovePosition(newPosition);
            }
        }

        // Shared turning
        if (currentTurnSpeed != 0f)
        {
            Quaternion newRotation = Quaternion.Euler(0, currentTurnSpeed * Time.fixedDeltaTime, 0);
            rb.MoveRotation(rb.rotation * newRotation);
        }
    }

    // ==== Option 1 methods ====
    public void MoveForwardSlow() { if (mode == MovementMode.SimpleSpeedSet) SetMovement(objectToMove.transform.forward, slowSpeed); }
    public void MoveForwardFast() { if (mode == MovementMode.SimpleSpeedSet) SetMovement(objectToMove.transform.forward, fastSpeed); }
    public void MoveBackwardSlow() { if (mode == MovementMode.SimpleSpeedSet) SetMovement(-objectToMove.transform.forward, slowSpeed); }
    public void MoveBackwardFast() { if (mode == MovementMode.SimpleSpeedSet) SetMovement(-objectToMove.transform.forward, fastSpeed); }

    private void SetMovement(Vector3 direction, float speed)
    {
        movementDirection = direction;
        currentSpeed = speed;
        isMoving = true;
    }

    // ==== Option 2 methods ====
    public void StartAccelerateForward()
    {
        if (mode == MovementMode.AcceleratedHold)
        {
            acceleratingForward = true;
            acceleratingBackward = false;
        }
    }

    public void StopAccelerateForward()
    {
        if (mode == MovementMode.AcceleratedHold)
            acceleratingForward = false;
    }

    public void StartAccelerateBackward()
    {
        if (mode == MovementMode.AcceleratedHold)
        {
            acceleratingBackward = true;
            acceleratingForward = false;
        }
    }

    public void StopAccelerateBackward()
    {
        if (mode == MovementMode.AcceleratedHold)
            acceleratingBackward = false;
    }

    // ==== Shared turning ====
    public void TurnLeftSlow() => SetTurn(-slowTurnSpeed);
    public void TurnLeftFast() => SetTurn(-fastTurnSpeed);
    public void TurnRightSlow() => SetTurn(slowTurnSpeed);
    public void TurnRightFast() => SetTurn(fastTurnSpeed);
    public void StopTurn() => currentTurnSpeed = 0f;

    private void SetTurn(float turnSpeed) => currentTurnSpeed = turnSpeed;

    public void Stop()
    {
        currentSpeed = 0f;
        currentTurnSpeed = 0f;
        isMoving = false;
        acceleratingForward = false;
        acceleratingBackward = false;
    }
}
