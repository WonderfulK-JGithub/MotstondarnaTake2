using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    Transform platform; // plattformen - Anton
    [Range(0, 5)] // hastigheten ska inte vara f�r h�g - Anton
    public float speed; // hastigheten - Anton
    public bool backAndForth; // om plattformen ska r�ra sig fram och tillbaka - Anton
    public bool enableMove; // om plattformen ska r�ra sig - Anton
    bool reverse; // anv�nds endast om backAndForth �r satt p� true och plattformen �ker tillbaka - Anton
    Transform[] children; // f�r tag p� alla children f�r att den senare ska extrahera ut punkterna - Anton
    public bool autoAssignPoints = true;
    [SerializeField]
    Transform[] points; // alla punkter (de ber�ttar f�r plattformen hur den ska �ka) - Anton
    int targetPoint = 0; // vilken punkt plattan r�r sig mot - Anton
    List<Transform> objectsOnPlatform = new List<Transform>(); // vilka objekt som nuddar plattformen - Anton

    private void Start()
    {
        children = GetComponentsInChildren<Transform>(); // f�r tag p� alla children - Anton
        platform = children[1]; // plattformen �r f�rsta childen - Anton
        if (autoAssignPoints)
        {
            points = new Transform[children.Length - 2]; // antalet punkter �r points l�ngd utan parenten och plattformen - Anton
            for (int i = 2; i < children.Length; i++)
            {
                points[i - 2] = children[i]; // l�gger in punkterna i points-arrayen - Anton
            }
        }
    }
    private void FixedUpdate()
    {
        Vector3 difference = Vector3.zero;
        if (enableMove)
        {
            difference = platform.transform.position - Vector3.MoveTowards(platform.transform.position, points[targetPoint].position, speed * Time.fixedDeltaTime); // den ska alltid r�ra sig mot n�sta punkt - Anton
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
            objectsOnPlatform[i].position -= difference; // detta flyttar alla objekt som �r p� plattan - Anton
        }

        if (platform.transform.position == points[targetPoint].position) // n�r plattan n�tt en punkt - Anton
        {
            if (backAndForth) // om backAndForth �r p� - Anton
            {

                if (targetPoint >= points.Length - 1) { reverse = true; } // om sista punkten �r n�dd ska den b�rja g� bakl�nges - Anton
                else if (targetPoint <= 0) { reverse = false; } // om f�rsta punkten �r n�dd ska den b�rja g� framl�nges - Anton

                if (!reverse) // om den r�r sig framl�nges ska plattan vilja r�ra sig mot n�sta punkt - Anton
                {
                    targetPoint++;
                }
                else // annars ska den vilja r�ra sig mot f�rra punkten - Anton
                { 
                    targetPoint--;
                }
            }
            else // om backAndForth inte �r p� - Anton
            {
                targetPoint++; // d� ska den alltid r�ra sig mot n�sta punkt - Anton
                if (targetPoint >= points.Length) { targetPoint = 0; } // �terst�ller om targetPoint �r h�gre �n antalet punkter - Anton
            }
        }
    }
    private void OnCollisionEnter(Collision collision) // n�r n�got nuddar plattan - Anton
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null) // och har en rigidbody - Anton
        {
            objectsOnPlatform.Add(collision.transform); // d� ska den f�lja med plattan - Anton
        }
    }
    private void OnCollisionExit(Collision collision) // n�r n�got sl�pper plattan - Anton
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null) // och har en rigidbody - Anton
        {
            objectsOnPlatform.Remove(collision.transform); // d� ska den sluta f�lja med plattan - Anton
        }
    }
}
