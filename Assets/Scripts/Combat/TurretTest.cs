using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TurretTest : MonoBehaviour
{
    public Vector3 pos;
    Transform player;

    public int maxAngle;
    public int minAngle;

    public int Angle { get; private set; }

    void Start()
    {
        player = transform.parent;

        Angle = 0;
    }

    public void Update()
    {
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 dir = pos - transform.position;
        dir.Normalize();
        Angle = Mathf.RoundToInt(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);


        //Used to Clamp the Angle, not working properly :(
        Angle = Mathf.Clamp(Angle, minAngle, maxAngle);

        if (player.localScale.x == 1)
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, -Angle - 180f);
        }
    }

    //public void TestMethod1()
    //{
    //    angle += Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;
    //    angle += Input.GetKey(KeyCode.RightArrow) ? -1 : 0;
    //    angle = Mathf.Clamp(angle, min, max);
    //    head.transform.eulerAngles = new Vector3(0, 0, angle);
    //}

    //public void RotateShip(Vector3 targetPos)
    //{
    //    float polarity = (targetPos.x < halfWidth) ? 1f : -1f;
    //    transform.Rotate(Vector3.forward, polarity * Time.deltaTime * speed);
    //    float angle = transform.eulerAngles.z;
    //    transform.eulerAngles = new Vector3(0f, 0f, ClampAngle(angle, min, max));
    //}

    //private float ClampAngle(float angle, float min, float max)
    //{
    //    if (angle < 90f || angle > 270f)
    //    {
    //        if (angle > 180)
    //        {
    //            angle -= 360f;
    //        }
    //        if (max > 180)
    //        {
    //            max -= 360f;
    //        }
    //        if (min > 180)
    //        {
    //            min -= 360f;
    //        }
    //    }
    //    angle = Mathf.Clamp(angle, min, max);
    //    if (angle < 0)
    //    {
    //        angle += 360f;
    //    }
    //    return angle;
    //}
}
