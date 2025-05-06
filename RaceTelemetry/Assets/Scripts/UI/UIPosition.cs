using UnityEngine;
using UnityEngine.UI;

public class UIPositionShower : MonoBehaviour
{
    [SerializeField] GameObject car;
    UnityEngine.UI.Text text;

    private void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = "Pozycja: (" +  toUIfigure((int)car.transform.position.x) + ", " + toUIfigure((int)car.transform.position.y) + ")";
    }

    string toUIfigure(int number)
    {
        string answer = number >= 0 ? "+" : "-";
        number = Mathf.Abs(number);
        if (number > 99) answer += number;
        else if (number > 9) answer += "0" + number;
        else answer += "00" + number;
        return answer;
    }
}
