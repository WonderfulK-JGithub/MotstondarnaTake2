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
        pinpos = new Vector3[pins.Length];
        for (int i = 0; i < pins.Length; i++)
        {
            pinpos[i] = pins[i].transform.position;
        }

        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        foreach (var item in pins)
        {
            if (item != null)
            {
                item.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        run++;
        yield return new WaitForSeconds(Random.Range(0.4f, 2.4f));
        ball.GetComponent<Rigidbody>().AddForce(new Vector3(1000, Random.Range(-5, 5), 0));


        yield return new WaitForSeconds(Random.Range(5, 10));

        for (int i = 0; i < 40; i++)
        {
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

        ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }
}
