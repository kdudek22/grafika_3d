using UnityEngine;

public class UIManager : MonoBehaviour
{
    Timer timer;

    private void Awake()
    {
        timer = GetComponentInChildren<Timer>();
    }
}
