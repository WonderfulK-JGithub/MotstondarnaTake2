using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    [SerializeField] float horizontalSpeed;
    [SerializeField] float verticlaSpeed;
    [SerializeField] float verticalTarget;
    [SerializeField] float shockWaveTime;

    bool goDown;

    float ogYPos;

    private void Awake()
    {
        ogYPos = transform.position.y;
    }

    void FixedUpdate()
    {
        Vector3 newPos = transform.position;

        newPos.x += horizontalSpeed * Time.fixedDeltaTime;

        shockWaveTime -= Time.fixedDeltaTime;

        if (goDown)
        {
            if(shockWaveTime > 0f) newPos.y = Mathf.MoveTowards(newPos.y, ogYPos, verticlaSpeed * Time.fixedDeltaTime * 0.1f);
            else newPos.y = Mathf.MoveTowards(newPos.y, ogYPos, verticlaSpeed * Time.fixedDeltaTime);
        }
        else
        {
            newPos.y = Mathf.Lerp(newPos.y, verticalTarget, 0.125f);
        }
        

        transform.position = newPos;

        if(Mathf.Abs(transform.position.y - verticalTarget) < 0.1f)
        {
            goDown = true;
        }



        if (transform.position.y == ogYPos) Destroy(gameObject);
    }
}
