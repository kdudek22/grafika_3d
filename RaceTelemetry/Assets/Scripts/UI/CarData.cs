using UnityEngine;
using UnityEngine.UI;

public class CarData : MonoBehaviour
{
    Text speed;
    Text gear;
    Text rmp;

    void Start()
    {
        speed = GameObject.Find("Speed").GetComponent<Text>();
        gear = GameObject.Find("Gear").GetComponent<Text>();
        rmp = GameObject.Find("RMP").GetComponent<Text>();
    }

    public void UpdateUI(Reading reading)
    {
        speed.text = "Prêdkoœæ: " + reading.speed + " km/h";
        gear.text = "Bieg: " + reading.gear;
        rmp.text = reading.rmp + " obr./min";
    }
}
