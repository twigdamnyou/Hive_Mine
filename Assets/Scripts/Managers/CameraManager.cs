using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum ZoomContext
    {
        Space,
        PlanetSurface,
        Miner,
    }

    [Header("Camera Variables")]
    public Transform followTarget;
    public float dampTime = 10f;
    public float yOffsetPercentage = 0;
    public float rotationSpeed;
    private Coroutine zoomCorutine;
    private Coroutine panCorutine;
    public bool currentlyZooming;
    public bool currentlyPanning;

    [Header("Camera Zoom")]
    public float zoomSpeed = 20f;
    public float spaceZoom = 25f;
    public float planetZoom = 15f;
    public float minerZoom = 8f;

    [Header("Camera Targets")]
    public Player tic;
    public Miner miner;

    public static CameraManager Instance { get; private set; }


    private bool orientToPlayer;
    [SerializeField]
    //private float margin = 0.1f;
    private Camera myCam;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        myCam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (followTarget == null)
        {
            Debug.LogWarning("Camera has no follow target");
            return;
        }

        if (GameManager.minerDocked == true)
        {
            //Debug.Log("Miner docked camera locked to TIC");
            followTarget = tic.transform;

            Vector2 localUp = Vector2.zero;

            if (GameManager.currentPlanet != null)
            {
                Vector2 localReletive = GameManager.GetTICPosition() - (Vector2)GameManager.currentPlanet.transform.position;
                localUp = localReletive.normalized * yOffsetPercentage;

                //Debug.Log("rotating camera relative to planet: " + localUp + " and adjusting for offset");
            }

            HandleOffset(localUp);
        }
        else
        {
            Debug.Log("Miner undocked camera locked to Miner");
            followTarget = miner.transform;

            Vector2 localUp = Vector2.zero;
            HandleOffset(localUp);
        }

        HandleRotation();
    }

    private void HandleRotation()
    {
        if (orientToPlayer == true)
        {
            RotateTo(followTarget.transform.rotation);
        }
        else
        {
            RotateTo(Quaternion.identity);
        }
    }

    /// <summary>
    /// Handles camera rotation in relation to the current plannet and offsets the camera when on planet
    /// </summary>
    private void HandleOffset(Vector2 offset)
    {
        float targetX = followTarget.position.x;
        float targetY = followTarget.position.y;

        targetX = Mathf.Lerp(transform.position.x, targetX + offset.x, 1 / dampTime * Time.deltaTime);
        targetY = Mathf.Lerp(transform.position.y, targetY + offset.y, 1 / dampTime * Time.deltaTime);

        //Debug.Log(localUp.normalized);
        transform.position = new Vector3(targetX, targetY, transform.position.z);
    }

    private void RotateTo(Quaternion desiredRotation, float modifier = 1f)
    {
        Quaternion rot = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed * modifier);
        transform.rotation = rot;
    }



    private IEnumerator ChangeZoom(float zoom)
    {
        //Debug.Log("Camera Current Zoom: " + myCam.orthographicSize);
        //Debug.Log("Incoming Zoom: " + zoom);

        while (myCam.orthographicSize != zoom)
        {
            currentlyZooming = true;
            float targetZoom = Mathf.MoveTowards(myCam.orthographicSize, zoom, Time.deltaTime * zoomSpeed);
            myCam.orthographicSize = targetZoom;
            yield return new WaitForEndOfFrame();
            //Debug.Log("Zooming toward: " + zoom);
        }
        currentlyZooming = false;
    }

    //private IEnumerator ChangeOffset(float xOffset, float yOffset)
    //{
    //    //Debug.Log("initiating panning. offset: X " + xOffset + " Y " + yOffset);
    //    while (this.xOffset != xOffset || this.yOffset != yOffset)
    //    {
    //        //Debug.Log("Panning");
    //        currentlyPanning = true;
    //        float targetXOffset = Mathf.MoveTowards(this.xOffset, xOffset, Time.deltaTime * zoomSpeed);
    //        float targetYOffset = Mathf.MoveTowards(this.yOffset, yOffset, Time.deltaTime * zoomSpeed);

    //        this.xOffset = targetXOffset;
    //        this.yOffset = targetYOffset;

    //        yield return new WaitForEndOfFrame();
    //        //Debug.Log("Zooming toward: " + zoom);
    //    }

    //    currentlyPanning = false;
    //}

    public static void ActivateRotation(float zoom, ZoomContext zoomContext)
    {
        if (Instance.zoomCorutine != null)
        {
            Instance.StopCoroutine(Instance.zoomCorutine);
        }

        Instance.orientToPlayer = true;
        Instance.zoomCorutine = Instance.StartCoroutine(Instance.ChangeZoom(zoom));
        ActivatePanning(zoomContext);
    }

    public static void ActivatePanning(ZoomContext zoomContext)
    {
        if (Instance.panCorutine != null)
        {
            Instance.StopCoroutine(Instance.panCorutine);
        }

        //Vector2 desiredOffset = Instance.GetOffsetValuesByContext(zoomContext);

        //Instance.panCorutine = Instance.StartCoroutine(Instance.ChangeOffset(desiredOffset.x, desiredOffset.y));
    }

    public Vector2 GetOffsetValuesByContext(ZoomContext context)
    {
        switch (context)
        {
            case ZoomContext.Space:
                return Vector2.zero;
            case ZoomContext.PlanetSurface:
                return Vector2.zero;
            case ZoomContext.Miner:
                return Vector2.zero;
            default:
                return Vector2.zero;
        }
    }

    public static void ResetRoation(float zoom)
    {
        if (Instance.zoomCorutine != null)
        {
            Instance.StopCoroutine(Instance.zoomCorutine);
        }
        Instance.orientToPlayer = false;
        Instance.zoomCorutine = Instance.StartCoroutine(Instance.ChangeZoom(zoom));
        ActivatePanning(ZoomContext.Space);
    }
}
