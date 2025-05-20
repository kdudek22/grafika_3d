using UnityEngine;
using UnityEngine.UIElements;

public class UIInteractor : MonoBehaviour
{
    public static UIInteractor instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateUI(Reading reading)
    {
        UIDocument doc = GetComponent<UIDocument>();
        if (doc == null)
        {
            Debug.LogError("UIDocument not found on GameObject.");
            return;
        }

        VisualElement root = doc.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Root VisualElement is null.");
            return;
        }

        Label speed = root.Q<Label>("Speed");
        Label gear = root.Q<Label>("Gear");
        Label rpm = root.Q<Label>("RPM");

        speed.text = "Speed: " + reading.speed.ToString();
        gear.text = "Gear: " + reading.gear.ToString();
        rpm.text = "RPM: " + reading.rmp.ToString();
    }
}