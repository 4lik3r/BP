using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit; // ✅ XR Toolkit for Gaze Interactor

public class GazeNavigation : MonoBehaviour
{
    public XRRayInteractor gazeInteractor;  // ✅ Assign GazeInteractor from MRTK Gaze Controller
    public NavMeshAgent wheelchairAgent;   // ✅ NavMesh Agent for movement
    public float gazeHoldTime = 2f;        // ✅ Time to hold gaze before moving

    private Coroutine gazeCoroutine;
    private Vector3 targetPosition;
    private Vector3 lastGazePosition;
    private float gazeTimer = 0f; // ✅ Keeps track of how long the gaze stays in one spot

    void Start()
    {
        if (gazeInteractor == null)
        {
            gazeInteractor = FindObjectOfType<XRRayInteractor>();
        }

        if (gazeInteractor == null)
        {
            Debug.LogError("🚨 GazeInteractor NOT found! Make sure it's in MRTK Gaze Controller.");
        }

        if (wheelchairAgent == null)
        {
            Debug.LogError("🚨 Wheelchair NavMeshAgent is NOT assigned! Assign it in the Inspector.");
        }
        else
        {
            Debug.Log("✅ Wheelchair NavMeshAgent found, stopping movement at start.");
            wheelchairAgent.isStopped = true;  // ✅ Stop movement at start
            wheelchairAgent.ResetPath();       // ✅ Clear any existing movement command
        }
    }

    void Update()
    {
        if (gazeInteractor == null) return;

        // ✅ Get gaze target using MRTK3’s GazeInteractor
        if (gazeInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            Debug.Log("🎯 Eye Gaze hit: " + hit.collider.name);

            if (hit.collider.CompareTag("Floor")) // ✅ Ensure the floor is tagged as "Floor"
            {
                if (Vector3.Distance(hit.point, lastGazePosition) < 0.05f) // ✅ If gaze stays in the same spot
                {
                    gazeTimer += Time.deltaTime;
                    Debug.Log("⏳ Holding gaze at the same spot... Time: " + gazeTimer);
                }
                else
                {
                    gazeTimer = 0f; // ✅ Reset timer if gaze moves
                    Debug.Log("❌ Gaze moved! Timer reset.");
                }

                lastGazePosition = hit.point;

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
            Debug.LogError("🚨 Wheelchair NavMeshAgent is NULL! Assign it in the Inspector.");
            yield break;
        }

        wheelchairAgent.isStopped = false;  // ✅ Enable movement
        wheelchairAgent.SetDestination(targetPosition);

        // ✅ Wait for the wheelchair to reach the destination
        while (Vector3.Distance(wheelchairAgent.transform.position, targetPosition) > wheelchairAgent.stoppingDistance + 0.2f)
        {
            yield return null; // Wait for the agent to reach the target
        }

        Debug.Log("✅ Wheelchair has reached the destination.");
        wheelchairAgent.isStopped = true; // ✅ Stop movement
        wheelchairAgent.ResetPath();

        gazeCoroutine = null; // ✅ Allow a new gaze command
    }

    private void ResetGaze()
    {
        if (gazeCoroutine != null)
        {
            StopCoroutine(gazeCoroutine);
            gazeCoroutine = null;
        }

        gazeTimer = 0f; // ✅ Reset gaze hold timer when looking away
    }
}
