using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class GazeNavigation : MonoBehaviour
{
    public XRRayInteractor gazeInteractor;
    public NavMeshAgent wheelchairAgent;
    public float gazeHoldTime = 2f;
    public float allowedAngleDifference = 0.5f; // ✅ How much the gaze can deviate (in degrees)
    
    private Coroutine gazeCoroutine;
    private Vector3 targetPosition;
    private Vector3 lastGazeDirection;
    private float gazeTimer = 0f;
    private bool movementEnabled = false;

    void Start()
    {
        if (gazeInteractor == null)
        {
            gazeInteractor = FindObjectOfType<XRRayInteractor>();
        }

        if (gazeInteractor == null)
        {
            Debug.LogError("GazeInteractor NOT found! Make sure it's in MRTK Gaze Controller.");
        }

        if (wheelchairAgent == null)
        {
            Debug.LogError("Wheelchair NavMeshAgent is NOT assigned! Assign it in the Inspector.");
        }
        else
        {
            Debug.Log("Wheelchair NavMeshAgent found, stopping movement at start.");
            wheelchairAgent.isStopped = true;
            wheelchairAgent.ResetPath();
        }
        
        lastGazeDirection = Vector3.zero;
    }

    void Update()
    {
        if (!movementEnabled) return;

        if (gazeInteractor == null) return;

        if (gazeInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Floor"))
            {
                Vector3 currentGazeDirection = (hit.point - gazeInteractor.transform.position).normalized;

                if (lastGazeDirection == Vector3.zero)
                {
                    // ✅ First time setup
                    lastGazeDirection = currentGazeDirection;
                    gazeTimer = 0f;
                }

                float angleDifference = Vector3.Angle(currentGazeDirection, lastGazeDirection);

                if (angleDifference <= allowedAngleDifference)
                {
                    // ✅ Within allowed angle range, update gaze timer
                    gazeTimer += Time.deltaTime;
                    Debug.Log($"⏳ Holding gaze at the same spot... Time: {gazeTimer} - Angle Difference: {angleDifference}°");

                    // ✅ Smoothly update the last gaze direction to allow minor adjustments
                    lastGazeDirection = Vector3.Slerp(lastGazeDirection, currentGazeDirection, Time.deltaTime * 7f);
                }
                else
                {
                    // ✅ If the gaze moves too far, reset the timer
                    gazeTimer = 0f;
                    lastGazeDirection = currentGazeDirection; // ✅ Allow user to start a new gaze focus
                    Debug.Log($"❌ Gaze moved! Timer reset. Angle Difference: {angleDifference}°");
                }

                if (gazeTimer >= gazeHoldTime && gazeCoroutine == null)
                {
                    Debug.Log("✅ Gaze held for 2 seconds - Moving wheelchair!");
                    targetPosition = hit.point;
                    gazeCoroutine = StartCoroutine(MoveToTarget());
                }
            }
            else
            {
                ResetGaze();
            }
        }
        else
        {
            ResetGaze();
        }
    }

    private IEnumerator MoveToTarget()
    {
        if (wheelchairAgent == null)
        {
            Debug.LogError("Wheelchair NavMeshAgent is NULL! Assign it in the Inspector.");
            yield break;
        }

        wheelchairAgent.isStopped = false;
        wheelchairAgent.SetDestination(targetPosition);

        while (Vector3.Distance(wheelchairAgent.transform.position, targetPosition) > wheelchairAgent.stoppingDistance + 0.2f)
        {
            yield return null;
        }

        Debug.Log("✅ Wheelchair has reached the destination.");
        wheelchairAgent.isStopped = true;
        wheelchairAgent.ResetPath();

        gazeCoroutine = null;
    }

    private void ResetGaze()
    {
        if (gazeCoroutine != null)
        {
            StopCoroutine(gazeCoroutine);
            gazeCoroutine = null;
        }

        gazeTimer = 0f;
        lastGazeDirection = Vector3.zero;
    }

    public void ToggleMovement()
    {
        movementEnabled = !movementEnabled;
        Debug.Log(movementEnabled ? "✅ Movement Enabled" : "❌ Movement Disabled");

        if (!movementEnabled)
        {
            ResetGaze();
            if (wheelchairAgent != null)
            {
                wheelchairAgent.isStopped = true;
                wheelchairAgent.ResetPath();
            }
        }
    }
}
