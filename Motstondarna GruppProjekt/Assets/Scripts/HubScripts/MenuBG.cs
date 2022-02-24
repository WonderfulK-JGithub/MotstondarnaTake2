using System.Collections;
using UnityEngine;

// Detta skript hanterar vad som h�nder i bakgrunden av startmenyn, dvs. kloten som sl�r in i k�glorna p� varje bana, och sedan hur k�glorna reagerar p� det. - Anton

public class MenuBG : MonoBehaviour
{
    [SerializeField] GameObject[] pins; // k�glor som tillh�r banan - Anton
    [SerializeField] GameObject pinprefab; // k�gelprefabe, s� man kan spawna tillbaka k�glor efter de f�rsvunnit - Anton
    [SerializeField] GameObject ball; // bollen som tillh�r banan - Anton

    Vector3 ballpos; // bollens ursprungsposition - Anton
    Vector3[] pinpos; // alla k�glornas ursprungspositioner - Anton
    int run; // f�rsta eller andra rundan? - Anton
    bool allPinsDown; // har klotet haft ner alla k�glor? - Anton

    void Start()
    {
        ballpos = ball.transform.position; // s�tter v�rdet p� ballpos - Anton
        pinpos = new Vector3[pins.Length]; // pinpos l�ngd m�ste vara lika stor som pins[] - Anton
        for (int i = 0; i < pins.Length; i++) { pinpos[i] = pins[i].transform.position; } // ger pinpos alla k�glors positioner - Anton

        StartCoroutine(Play()); // kallar p� Play() som sk�ter allt n�r det kommer till skjutandet av klotet och k�glorna - Anton
    }

    IEnumerator Play()
    {
        foreach (var item in pins) // ser till att av-frysa alla k�glor (de fryser efter varje runda) - Anton
        {
            if (item != null) // ser till att inte v�lja borttagna k�glor f�r d� blir det errors - Anton
            {
                item.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        run++; // �kar vilken runda de �r p� - Anton
        yield return new WaitForSeconds(Random.Range(0.4f, 2.4f)); // slumpar v�ntetider (du kommer m�rka mycket Random.Range() i detta skript, det �r f�r att jag helst inte vill att banorna synkar alls med varandra) - Anton

        float offset = Random.Range(-2, 2); // klotet ska inte alltid skjuta rakt, s� detta ser till att det �r lite snett - Anton
        float speed = Random.Range(15, 30); // klotet ska inte alltid skjuta med samma kraft och hastighet, s� detta ser till att det v�xlar lite - Anton
        float waitTime = Random.Range(28, 48); // slumpar v�ntetider, v�rderna i Random.Range() h�r divideras med 4 - Anton

        for (int i = 0; i < waitTime; i++)
        {
            ball.GetComponent<Rigidbody>().velocity = new Vector3(speed, 0, offset); // ser till att klotet h�ller sig p� banan n�r den skjuts - Anton
            yield return new WaitForSeconds(0.25f); // v�ntar lite s� k�glorna hinner falla omkull innan de �terst�lls - Anton
        }

        allPinsDown = true;  foreach (var item in pins) // kollar om alla k�glor �r nere - Anton
        {
            if (item != null) { allPinsDown = false; } // om n�gon st�r upp s� s�tts v�rdet till false igen - Anton
        }

        if (!allPinsDown) // detta k�rs bara n�r det finns k�glor kvar p� banan - Anton
        {
            for (int i = 0; i < 40; i++) 
            {
                ball.GetComponent<Rigidbody>().AddForce(new Vector3(100, 0, 0)); // vill se till att bollen inte syns n�r den teleporterar, s� jag ser till att den knuffas fram s� mycket som m�jligt - Anton
                foreach (var item in pins)
                {
                    if (item != null)
                    {
                        item.GetComponent<Rigidbody>().isKinematic = true; // fryser k�glorna - Anton
                        item.transform.position += new Vector3(0, 0.1f, 0); // h�r lyfts k�glorna - Anton
                    }

                }
                yield return new WaitForSeconds(0.05f);
            }
        }

        ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); // f�r bollen att stanna - Anton
        ball.transform.position = ballpos; // �terst�ller bollens position - Anton

        if (run == 2) // endast p� andra rundan ska k�glorna helt �terst�llas - Anton
        {
            foreach (var item in pins)
            {
                if (item != null)
                {
                    Destroy(item); // tar bort alla k�glor som finns kvar - Anton
                }

            }
            yield return new WaitForSeconds(Random.Range(2, 7)); // slumpade v�ntetider igen - Anton

            for (int i = 0; i < pins.Length; i++)
            {
                pins[i] = Instantiate(pinprefab, pinpos[i], Quaternion.identity); // skapa en ny k�gla vid gamla k�glans position - Anton
                pins[i].GetComponent<Rigidbody>().isKinematic = true; // frys k�glan - Anton
                pins[i].transform.position += new Vector3(0, 4, 0); // flytta upp k�glan - Anton
            }

            run = 0; // b�rja om fr�n b�rjan med rundr�kningen - Anton
        }

        for (int i = 0; i < 40; i++)
        {
            foreach (var item in pins)
            {
                if (item != null)
                {
                    item.transform.position += new Vector3(0, -0.1f, 0); // h�r s�kns k�glorna igen efter varje runda - Anton
                }

            }
            yield return new WaitForSeconds(0.05f);
        }
        StartCoroutine(Play()); // h�r kallas funktionen igen, s� att den h�ller p� f�r alltid - Anton
    }
}
