using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ReplayManager : MonoBehaviour
{
    public static ReplayManager instance;
    public int currentReadingIndex = 0;
    public List<Reading> readings = new List<Reading>();
    public bool setupFinished = false;
    public float timeBetweenReadings = 0.3f;
    public PlayType playType = PlayType.File;
    private float timer = 0f;

    public FileDataProvider dataProvider;



    private void Awake(){
        instance = this;
    }

    public void Setup(){
        Debug.Log("Setting up :)");

        this.dataProvider = new FileDataProvider("data_bmw_20fps.json");

        // var x = new APIDataProvider();

        this.setupFinished = true;
    }

    public void Update(){
        this.timer += Time.deltaTime;

        if(!this.setupFinished){
            return;
        }

        if(this.timer >= this.timeBetweenReadings){
            this.currentReadingIndex = (this.currentReadingIndex + 1) % this.dataProvider.maxIndex;
            this.timer = 0f;
        }

        float lerpMoveRation = timer % this.timeBetweenReadings;
        Reading currentReading = this.dataProvider.GetReading(this.currentReadingIndex);
        this.UpdateGameObject(GameObject.Find("Car"), currentReading, lerpMoveRation);

    }


    public void UpdateGameObject(GameObject obj, Reading reading, float moveRatio){
        // Target position from reading
        Vector3 targetPos = new Vector3(reading.x, reading.y, -1 * reading.z);

        // Target rotation from heading
        float headingDegrees = reading.heading * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, headingDegrees + 180, 0);

        // Smooth position and rotation using moveRatio
        obj.transform.position = Vector3.Lerp(obj.transform.position, targetPos, moveRatio);
        obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, targetRot, moveRatio);
    }

}
