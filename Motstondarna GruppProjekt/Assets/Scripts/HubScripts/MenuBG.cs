using System.Collections;
using UnityEngine;

// Detta skript hanterar vad som händer i bakgrunden av startmenyn, dvs. kloten som slår in i käglorna på varje bana, och sedan hur käglorna reagerar på det. - Anton

public class MenuBG : MonoBehaviour
{
    [SerializeField] GameObject[] pins; // käglor som tillhör banan - Anton
    [SerializeField] GameObject pinprefab; // kägelprefabe, så man kan spawna tillbaka käglor efter de försvunnit - Anton
    [SerializeField] GameObject ball; // bollen som tillhör banan - Anton

    Vector3 ballpos; // bollens ursprungsposition - Anton
    Vector3[] pinpos; // alla käglornas ursprungspositioner - Anton
    int run; // första eller andra rundan? - Anton
    bool allPinsDown; // har klotet haft ner alla käglor? - Anton

    void Start()
    {
        ballpos = ball.transform.position; // sätter värdet på ballpos - Anton
        pinpos = new Vector3[pins.Length]; // pinpos längd måste vara lika stor som pins[] - Anton
        for (int i = 0; i < pins.Length; i++) { pinpos[i] = pins[i].transform.position; } // ger pinpos alla käglors positioner - Anton

        StartCoroutine(Play()); // kallar på Play() som sköter allt när det kommer till skjutandet av klotet och käglorna - Anton
    }

    IEnumerator Play()
    {
        foreach (var item in pins) // ser till att av-frysa alla käglor (de fryser efter varje runda) - Anton
        {
            if (item != null) // ser till att inte välja borttagna käglor för då blir det errors - Anton
            {
                item.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        run++; // ökar vilken runda de är på - Anton
        yield return new WaitForSeconds(Random.Range(0.4f, 2.4f)); // slumpar väntetider (du kommer märka mycket Random.Range() i detta skript, det är för att jag helst inte vill att banorna synkar alls med varandra) - Anton

        float offset = Random.Range(-2, 2); // klotet ska inte alltid skjuta rakt, så detta ser till att det är lite snett - Anton
        float speed = Random.Range(15, 30); // klotet ska inte alltid skjuta med samma kraft och hastighet, så detta ser till att det växlar lite - Anton
        float waitTime = Random.Range(28, 48); // slumpar väntetider, värderna i Random.Range() här divideras med 4 - Anton

        for (int i = 0; i < waitTime; i++)
        {
            ball.GetComponent<Rigidbody>().velocity = new Vector3(speed, 0, offset); // ser till att klotet håller sig på banan när den skjuts - Anton
            yield return new WaitForSeconds(0.25f); // väntar lite så käglorna hinner falla omkull innan de återställs - Anton
        }

        allPinsDown = true;  foreach (var item in pins) // kollar om alla käglor är nere - Anton
        {
            if (item != null) { allPinsDown = false; } // om någon står upp så sätts värdet till false igen - Anton
        }

        if (!allPinsDown) // detta körs bara när det finns käglor kvar på banan - Anton
        {
            for (int i = 0; i < 40; i++) 
            {
                ball.GetComponent<Rigidbody>().AddForce(new Vector3(100, 0, 0)); // vill se till att bollen inte syns när den teleporterar, så jag ser till att den knuffas fram så mycket som möjligt - Anton
                foreach (var item in pins)
                {
                    if (item != null)
                    {
                        item.GetComponent<Rigidbody>().isKinematic = true; // fryser käglorna - Anton
                        item.transform.position += new Vector3(0, 0.1f, 0); // här lyfts käglorna - Anton
                    }

                }
                yield return new WaitForSeconds(0.05f);
            }
        }

        ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); // får bollen att stanna - Anton
        ball.transform.position = ballpos; // återställer bollens position - Anton

        if (run == 2) // endast på andra rundan ska käglorna helt återställas - Anton
        {
            foreach (var item in pins)
            {
                if (item != null)
                {
                    Destroy(item); // tar bort alla käglor som finns kvar - Anton
                }

            }
            yield return new WaitForSeconds(Random.Range(2, 7)); // slumpade väntetider igen - Anton

            for (int i = 0; i < pins.Length; i++)
            {
                pins[i] = Instantiate(pinprefab, pinpos[i], Quaternion.identity); // skapa en ny kägla vid gamla käglans position - Anton
                pins[i].GetComponent<Rigidbody>().isKinematic = true; // frys käglan - Anton
                pins[i].transform.position += new Vector3(0, 4, 0); // flytta upp käglan - Anton
            }

            run = 0; // börja om från början med rundräkningen - Anton
        }

        for (int i = 0; i < 40; i++)
        {
            foreach (var item in pins)
            {
                if (item != null)
                {
                    item.transform.position += new Vector3(0, -0.1f, 0); // här säkns käglorna igen efter varje runda - Anton
                }

            }
            yield return new WaitForSeconds(0.05f);
        }
        StartCoroutine(Play()); // här kallas funktionen igen, så att den håller på för alltid - Anton
    }
}
