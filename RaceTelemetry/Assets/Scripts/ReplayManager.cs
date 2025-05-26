using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ReplayManager : MonoBehaviour
{
    public static ReplayManager instance;
    public int currentReadingIndex = 0;
    public float timeBetweenReadings = 0.3f;
    private PlayType playType = PlayType.File;
    private float timer = 0f;
    private ReplayState state = ReplayState.Idle;
    private Reading currentReading;
    private Reading lastReading;
    public DataProvider dataProvider;
    private UIInteractor uiInteractor;
    private float playbackSpeed = 1f;


    private void Awake()
    {
        instance = this;
    }

    public void Setup()
    {
        this.state = ReplayState.Starting;

        this.dataProvider = playType == PlayType.File ? new FileDataProvider("mazda_data.json") : new APIDataProvider();

        this.uiInteractor = UIInteractor.instance;

        this.state = ReplayState.Playing;
    }

    public void Pause()
    {
        this.state = ReplayState.Paused;
    }

    public void SetPlaybackSpeed(float playbackSpeed)
    {
        this.playbackSpeed = playbackSpeed;
    }

    public void Update()
    {
        this.timer += Time.deltaTime;

        if (this.state != ReplayState.Playing)
        {
            return;
        }

        // When the time interval is lager than the frame rate, update the index of the reading
        if (this.timer >= this.timeBetweenReadings / this.playbackSpeed)
        {
            this.currentReadingIndex = (this.currentReadingIndex + 1);
            this.lastReading = this.currentReading;
            this.currentReading = this.dataProvider.GetReading(this.currentReadingIndex);

            uiInteractor.UpdateUI(this.currentReading);

            this.timer = 0f;
        }

        // This is to make sure that we have the prevoius reading, as the movement calculations require 2 readings
        if (this.lastReading == null)
        {
            return;
        }

        float lerpMoveRation = (timer % (this.timeBetweenReadings / this.playbackSpeed)) / (this.timeBetweenReadings / this.playbackSpeed);

        this.UpdateGameObject(GameObject.Find("Car"), this.currentReading, this.lastReading, lerpMoveRation);

    }


    public void UpdateGameObject(GameObject obj, Reading currentReading, Reading lastReading, float moveRatio){
        // Target and last positions from readings
        Vector3 targetPos = new Vector3(currentReading.x, currentReading.y, -1 * currentReading.z);
        Vector3 lastPos = new Vector3(lastReading.x, lastReading.y, -1 * lastReading.z);

        // Convert heading and pitch from radians to degrees, invert pitch for correct orientation
        float targetHeadingDegrees = currentReading.heading * Mathf.Rad2Deg;
        float lastHeadingDegrees = lastReading.heading * Mathf.Rad2Deg;

        float targetPitchDegrees = -currentReading.pitch * Mathf.Rad2Deg;
        float lastPitchDegrees = -lastReading.pitch * Mathf.Rad2Deg;

        // Create target and last rotations using pitch (X-axis) and heading (Y-axis)
        Quaternion targetRot = Quaternion.Euler(targetPitchDegrees, targetHeadingDegrees + 180, 0);
        Quaternion lastRot = Quaternion.Euler(lastPitchDegrees, lastHeadingDegrees + 180, 0);

        // Smoothly interpolate position and rotation
        obj.transform.position = Vector3.Lerp(lastPos, targetPos, moveRatio);
        obj.transform.rotation = Quaternion.Slerp(lastRot, targetRot, moveRatio);
    }
}
