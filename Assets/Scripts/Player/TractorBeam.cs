using UnityEngine;

public class TractorBeam : MonoBehaviour
{
    [Header("Laser Components")]
    public LineRenderer lineRenderer;
    public Transform firePoint;

    public Stats MyStats { get; private set; }
    [Header("Stat Definitions")]
    public StatDataGroup startingStats;

    [Header("Masks")]
    public LayerMask diggingMask;
    public LayerMask vacumMask;
    public LayerMask minerBackpackMask;

    [Header("Empty Object Container On Hit Mask")]
    public LayerMask containerMask;
    public Timer emptyContainerTimer;
    public float emptyContainerDelay = 0.5f;
    private bool minerInBeam;

    [Header("Owner")]
    public GameObject owner;

    public Collider2D minerOreCollector;

    private void Awake()
    {
        MyStats = new Stats(startingStats);
    }

    private void Start()
    {
        lineRenderer.enabled = false;
        emptyContainerTimer = new Timer(emptyContainerDelay, OnBackpackTimerComplete, true);
        //Debug.Log("Starting tractor beam for " + owner.gameObject.name);

        if (minerOreCollector != null)
        {
            minerOreCollector.enabled = false;
        }
    }

    private void Update()
    {
        if (owner.tag == "Miner")
        {
            if (Input.GetMouseButton(1) && GameManager.minerDocked == false)
            {
                minerOreCollector.enabled = true;
                ShootVacum();
            }
            if (Input.GetMouseButtonUp(1) && GameManager.minerDocked == false)
            {
                lineRenderer.enabled = false;
                minerOreCollector.enabled = false;
            }
        }

        if (owner.tag == "Player" && GameManager.ticDocked == true)
        {
            ShootVacum();
            UpdateEmptyBackpackTimer();
        }


    }

    private void ShootVacum()
    {
        float maxLaserDistance = MyStats.GetStat(Stat.MaxLaserDistance);
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.up, maxLaserDistance, diggingMask);
        if (hit.collider != null)
        {
            DrawLine(firePoint.position, hit.point);
            float dis = Vector2.Distance(firePoint.position, hit.point);

            //Debug.Log(hit.collider.gameObject.name + " is being hit with the vacum");
            ShootVacumRaycast(dis);
            CheckForMinerInBeam(dis);
        }
        else
        {
            DrawLine(firePoint.position, firePoint.position + firePoint.up * maxLaserDistance);
            ShootVacumRaycast(maxLaserDistance);
            CheckForMinerInBeam(maxLaserDistance);
        }
    }

    private void ShootVacumRaycast(float distance)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(firePoint.position, 0.2f, firePoint.up, distance, vacumMask);
        //Debug.Log(hits.Length + " number of vacum hits");

        for (int i = 0; i < hits.Length; i++)
        {
            PerformVacum(hits[i].collider);
        }
    }

    private void DrawLine(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    private void PerformVacum(Collider2D hit)
    {
        Vector2 direction = firePoint.position - hit.transform.position;
        Rigidbody2D lootBody = hit.GetComponent<Rigidbody2D>();
        if (lootBody != null)
        {
            lootBody.AddForce(direction * Time.fixedDeltaTime * MyStats.GetStat(Stat.VacumForce));
        }
    }

    private void CheckForMinerInBeam(float distance)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(firePoint.position, 0.2f, firePoint.up, distance, minerBackpackMask);

        if (hits == null || hits.Length == 0)
        {
            minerInBeam = false;
            //Debug.Log("NO Miner detected in Beam");
            return;
        }
        //Debug.Log("Miner detected in Beam");
        minerInBeam = true;
    }

    private void UpdateEmptyBackpackTimer()
    {
        if (minerInBeam == false || emptyContainerTimer == null)
        {
            //Debug.Log((emptyContainerTimer == null) + " is the null state of timer " + owner.tag);
            //Debug.Log(minerInBeam + " is the state of miner in beam " + owner.tag);
            return;
        }


        //Debug.Log("updating the backpack empty clock");
        emptyContainerTimer.UpdateClock();
    }

    private void OnBackpackTimerComplete()
    {
        Debug.Log("Attempting Pulling 1 Ore from Miner Backpack");
        InventoryManager.instance.EmptyMinerBackpackSlowly();
    }

    #region Depricated


    //private void FixedUpdate()
    //{
    //    for (int i = currentVacumedLoot.Count - 1; i >= 0; i--)
    //    {
    //        if (currentVacumedLoot[i] == null)
    //            continue;

    //        PerformVacum(currentVacumedLoot[i]);
    //    }
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (LayerMask.LayerToName(collision.gameObject.layer) != "Ore")
    //        return;
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (LayerMask.LayerToName(collision.gameObject.layer) != "Ore")
    //        return;
    //}

    #endregion
}
