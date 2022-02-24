using System.Collections;
using UnityEngine;

public class MenuBG : MonoBehaviour
{
    [SerializeField] GameObject[] pins;
    [SerializeField] GameObject pinprefab;
    [SerializeField] GameObject ball;

    Vector3 ballpos;
    Vector3[] pinpos;
    int run;
    bool allPinsDown;

    void Start()
    {
        ballpos = ball.transform.position;
        pinpos = new Vector3[pins.Length];
        for (int i = 0; i < pins.Length; i++)
        {
            pinpos[i] = pins[i].transform.position;
        }

        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        allPinsDown = false;
        foreach (var item in pins)
        {
            if (item != null)
            {
                item.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        run++;
        yield return new WaitForSeconds(Random.Range(0.4f, 2.4f));

        float offset = Random.Range(-2, 2);
        float speed = Random.Range(15, 30);
        float waitTime = Random.Range(28, 48);

        for (int i = 0; i < waitTime; i++)
        {
            ball.GetComponent<Rigidbody>().velocity = new Vector3(speed, 0, offset);
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(Random.Range(7, 12));

        allPinsDown = true;  foreach (var item in pins)
        {
            if (item != null) { allPinsDown = false; }
        }

        if (!allPinsDown)
        {
            for (int i = 0; i < 40; i++)
            {
                ball.GetComponent<Rigidbody>().AddForce(new Vector3(100, 0, 0));
                foreach (var item in pins)
                {
                    if (item != null)
                    {
                        item.GetComponent<Rigidbody>().isKinematic = true;
                        item.transform.position += new Vector3(0, 0.1f, 0);
                    }

                }
                yield return new WaitForSeconds(0.05f);
            }
        }

        ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        ball.transform.position = ballpos;

        if (run == 2)
        {
            foreach (var item in pins)
            {
                if (item != null)
                {
                    Destroy(item);
                }

            }
            yield return new WaitForSeconds(Random.Range(2, 7));

            for (int i = 0; i < pins.Length; i++)
            {
                pins[i] = Instantiate(pinprefab, pinpos[i], Quaternion.identity);
                pins[i].GetComponent<Rigidbody>().isKinematic = true;
                pins[i].transform.position += new Vector3(0, 4, 0);
            }

            run = 0;
        }

        for (int i = 0; i < 40; i++)
        {
            foreach (var item in pins)
            {
                if (item != null)
                {
                    item.transform.position += new Vector3(0, -0.1f, 0);
                }

            }
            yield return new WaitForSeconds(0.05f);
        }
        StartCoroutine(Play());
    }
}
