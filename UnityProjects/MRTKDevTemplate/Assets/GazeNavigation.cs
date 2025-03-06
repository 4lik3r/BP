using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class GazeNavigation : MonoBehaviour
{
    public XRRayInteractor gazeInteractor;
    public NavMeshAgent wheelchairAgent;
    public float gazeHoldTime = 2f;

    private Coroutine gazeCoroutine;
    private Vector3 targetPosition;
    private Vector3 lastGazePosition;
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
    }

    void Update()
    {
        if (!movementEnabled) return;

        if (gazeInteractor == null) return;

        if (gazeInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            Debug.Log("Eye Gaze hit: " + hit.collider.name);

            if (hit.collider.CompareTag("Floor"))
            {
                if (Vector3.Distance(hit.point, lastGazePosition) < 0.05f)
                {
                    gazeTimer += Time.deltaTime;
                    Debug.Log("Holding gaze at the same spot... Time: " + gazeTimer);
                }
                else
                {
                    gazeTimer = 0f;
                    Debug.Log("Gaze moved! Timer reset.");
                }

                lastGazePosition = hit.point;

                if (gazeTimer >= gazeHoldTime && gazeCoroutine == null)
                {
                    Debug.Log("Gaze held for 2 seconds - Moving wheelchair!");
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

        Debug.Log("Wheelchair has reached the destination.");
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
    }

    public void ToggleMovement()
    {
        movementEnabled = !movementEnabled;
        Debug.Log(movementEnabled ? "Movement Enabled" : "Movement Disabled");

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