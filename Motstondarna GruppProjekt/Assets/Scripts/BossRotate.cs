using UnityEngine;

public class BossRotate : MonoBehaviour
{
    public float rotateSpeed = 1; // hastigheten på rotation - Anton
    void Update()
    {
        transform.rotation *= Quaternion.Euler(new Vector3(0, 0, rotateSpeed)); // roterar bosskaraktären - Anton
    }
}
