using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    Transform platform; // plattformen - Anton
    [Range(0, 5)] // hastigheten ska inte vara för hög - Anton
    public float speed; // hastigheten - Anton
    public bool backAndForth; // om plattformen ska röra sig fram och tillbaka - Anton
    public bool enableMove; // om plattformen ska röra sig - Anton
    bool reverse; // används endast om backAndForth är satt på true och plattformen åker tillbaka - Anton
    Transform[] children; // får tag på alla children för att den senare ska extrahera ut punkterna - Anton
    public bool autoAssignPoints = true;
    [SerializeField]
    Transform[] points; // alla punkter (de berättar för plattformen hur den ska åka) - Anton
    int targetPoint = 0; // vilken punkt plattan rör sig mot - Anton
    List<Transform> objectsOnPlatform = new List<Transform>(); // vilka objekt som nuddar plattformen - Anton

    private void Start()
    {
        children = GetComponentsInChildren<Transform>(); // får tag på alla children - Anton
        platform = children[1]; // plattformen är första childen - Anton
        if (autoAssignPoints)
        {
            points = new Transform[children.Length - 2]; // antalet punkter är points längd utan parenten och plattformen - Anton
            for (int i = 2; i < children.Length; i++)
            {
                points[i - 2] = children[i]; // lägger in punkterna i points-arrayen - Anton
            }
        }
    }
    private void FixedUpdate()
    {
        Vector3 difference = Vector3.zero;
        if (enableMove)
        {
            difference = platform.transform.position - Vector3.MoveTowards(platform.transform.position, points[targetPoint].position, speed * Time.fixedDeltaTime); // den ska alltid röra sig mot nästa punkt - Anton
        }
        platform.transform.position -= difference; // detta flyttar plattan - Anton

        for (int i = 0; i < objectsOnPlatform.Count; i++)
        {
            if (objectsOnPlatform[i] == null)
            {
                print("When the imposter is sus, whoaoh, Yeah Yeah");
                objectsOnPlatform.RemoveAt(i);
                i--;
                continue;
            }
            objectsOnPlatform[i].position -= difference; // detta flyttar alla objekt som är på plattan - Anton
        }

        if (platform.transform.position == points[targetPoint].position) // när plattan nått en punkt - Anton
        {
            if (backAndForth) // om backAndForth är på - Anton
            {

                if (targetPoint >= points.Length - 1) { reverse = true; } // om sista punkten är nådd ska den börja gå baklänges - Anton
                else if (targetPoint <= 0) { reverse = false; } // om första punkten är nådd ska den börja gå framlänges - Anton

                if (!reverse) // om den rör sig framlänges ska plattan vilja röra sig mot nästa punkt - Anton
                {
                    targetPoint++;
                }
                else // annars ska den vilja röra sig mot förra punkten - Anton
                { 
                    targetPoint--;
                }
            }
            else // om backAndForth inte är på - Anton
            {
                targetPoint++; // då ska den alltid röra sig mot nästa punkt - Anton
                if (targetPoint >= points.Length) { targetPoint = 0; } // återställer om targetPoint är högre än antalet punkter - Anton
            }
        }
    }
    private void OnCollisionEnter(Collision collision) // när något nuddar plattan - Anton
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null) // och har en rigidbody - Anton
        {
            objectsOnPlatform.Add(collision.transform); // då ska den följa med plattan - Anton
        }
    }
    private void OnCollisionExit(Collision collision) // när något släpper plattan - Anton
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null) // och har en rigidbody - Anton
        {
            objectsOnPlatform.Remove(collision.transform); // då ska den sluta följa med plattan - Anton
        }
    }
}
