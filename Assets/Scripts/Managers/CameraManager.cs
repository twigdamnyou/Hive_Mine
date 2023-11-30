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
    public float xOffset = 0;
    public float yOffset = 0;
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
    private float margin = 0.1f;
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
            return;
        }

        if (GameManager.minerDocked == true)
        {
            followTarget = tic.transform;
        }
        else
        {
            followTarget = miner.transform;
        }

        float targetX = followTarget.position.x + xOffset;
        float targetY = followTarget.position.y + yOffset;

        if (Mathf.Abs(transform.position.x - targetX) > margin)
            targetX = Mathf.Lerp(transform.position.x, targetX, 1 / dampTime * Time.deltaTime);

        if (Mathf.Abs(transform.position.y - targetY) > margin)
            targetY = Mathf.Lerp(transform.position.y, targetY, 1 / dampTime * Time.deltaTime);

        transform.position = new Vector3(targetX, targetY, transform.position.z);

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

    private IEnumerator ChangeOffset(float xOffset, float yOffset)
    {
        //Debug.Log("initiating panning. offset: X " + xOffset + " Y " + yOffset);
        while (this.xOffset != xOffset || this.yOffset != yOffset)
        {
            //Debug.Log("Panning");
            currentlyPanning = true;
            float targetXOffset = Mathf.MoveTowards(this.xOffset, xOffset, Time.deltaTime * zoomSpeed);
            float targetYOffset = Mathf.MoveTowards(this.yOffset, yOffset, Time.deltaTime * zoomSpeed);

            this.xOffset = targetXOffset;
            this.yOffset = targetYOffset;

            yield return new WaitForEndOfFrame();
            //Debug.Log("Zooming toward: " + zoom);
        }
        
        currentlyPanning = false;
    }

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

        Vector2 desiredOffset = Instance.GetOffsetValuesByContext(zoomContext);

        Instance.panCorutine = Instance.StartCoroutine(Instance.ChangeOffset(desiredOffset.x, desiredOffset.y));
    }

    public Vector2 GetOffsetValuesByContext(ZoomContext context)
    {
        switch (context)
        {
            case ZoomContext.Space:
                return Vector2.zero;
            case ZoomContext.PlanetSurface:
                return new Vector2(-8f, 0f);    
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
