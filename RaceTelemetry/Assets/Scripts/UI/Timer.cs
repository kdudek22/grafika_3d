using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [HideInInspector] public bool active; //póki co jest stale aktywny
    float timer = 0f;
    int seconds = 0;

    void Start()
    {
        active = true;
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            seconds += 1;
            timer = 0f;
            UpdateTimer();
        }
    }

    public void UpdateTimer()
    {
        int min = seconds/60;
        int sec = seconds % 60;
        GetComponent<Text>().text = "Czas Trwania: " + (min < 10 ? "0" : "") + min + ":" + (sec < 10 ? "0" : "") + sec;
    }
}
