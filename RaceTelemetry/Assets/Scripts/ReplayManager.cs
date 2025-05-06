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

    private Reading currentReading;
    private Reading lastReading;


    public DataProvider dataProvider;



    private void Awake(){
        instance = this;
    }

    public void Setup(){
        Debug.Log("Setting up :)");

        // this.dataProvider = new FileDataProvider("data_bmw_20fps.json");
        this.dataProvider = new APIDataProvider();

        // var x = new APIDataProvider();

        this.setupFinished = true;
    }

    public void Update(){
        this.timer += Time.deltaTime;

        if(!this.setupFinished){
            return;
        }

        // When the time interval is lager than the frame rate, update the index of the reading
        if(this.timer >= this.timeBetweenReadings){
            Debug.Log("This is still running :)");
            this.currentReadingIndex = (this.currentReadingIndex + 1);
            this.lastReading = this.currentReading;
            this.currentReading = this.dataProvider.GetReading(this.currentReadingIndex);
            this.timer = 0f;
        }

        // This is to make sure that we have the prevoius reading, as the movement calculations require 2 readings
        if(this.lastReading == null){
            return;
        }

        float lerpMoveRation = (timer % this.timeBetweenReadings) / this.timeBetweenReadings;
        
        this.UpdateGameObject(GameObject.Find("Car"), this.currentReading, this.lastReading, lerpMoveRation);
    }


    public void UpdateGameObject(GameObject obj, Reading currentReading, Reading lastReading, float moveRatio){
        // Target position from reading
        Vector3 targetPos = new Vector3(currentReading.x, currentReading.y, -1 * currentReading.z);
        Vector3 lastPos = new Vector3(lastReading.x, lastReading.y, -1 * lastReading.z);

        // Target rotation from heading
        float targetHeadingDegrees = currentReading.heading * Mathf.Rad2Deg;
        float lastHeadingDegrees = lastReading.heading * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, targetHeadingDegrees + 180, 0);
        Quaternion lastRot = Quaternion.Euler(0, lastHeadingDegrees + 180, 0);

        // Smooth position and rotation using moveRatio
        obj.transform.position = Vector3.Lerp(lastPos, targetPos, moveRatio);
        obj.transform.rotation = Quaternion.Slerp(lastRot, targetRot, moveRatio);
    }

}
