using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using UnityEngine;


public interface DataProvider{
    public abstract Reading GetReading(int index);
}


public class FileDataProvider: DataProvider{
    public List<Reading> readings = new List<Reading>();
    public int maxIndex;

    public FileDataProvider(string fileName){
        this.readings = GetReadingsFromFile(fileName);
        this.maxIndex = this.readings.Count;
    }

    public Reading GetReading(int index){
        return this.readings[index];
    }

    public static List<Reading> GetReadingsFromFile(string fileName){
        Debug.Log("Reading from file " + fileName);
        string path = Path.Combine(Application.dataPath, "ReplayData", fileName);

        string jsonString = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<List<Reading>>(jsonString);
    }
}

public class APIDataProvider : MonoBehaviour, DataProvider
{
    public List<Reading> readings = new List<Reading>();
    private HttpClient httpClient;
    private float sleepAmount = 5f;

    private void Start()
    {
        httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(5);
        StartCoroutine(PollAPI());
    }

    private IEnumerator PollAPI()
    {
        while (true)
        {
            Debug.Log("Sending a request");

            Task<HttpResponseMessage> requestTask = httpClient.GetAsync("http://127.0.0.1:5000/telemetry");
            yield return new WaitUntil(() => requestTask.IsCompleted);

            if (requestTask.IsFaulted)
            {
                Debug.LogError($"HTTP request failed: {requestTask.Exception?.GetBaseException().Message}");
            }
            else if (requestTask.Result.IsSuccessStatusCode)
            {
                Task<string> readTask = requestTask.Result.Content.ReadAsStringAsync();
                yield return new WaitUntil(() => readTask.IsCompleted);

                try
                {
                    Reading reading = JsonConvert.DeserializeObject<Reading>(readTask.Result);
                    if (reading != null)
                        readings.Add(reading);
                    else
                        Debug.LogWarning("Deserialized reading is null.");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"JSON Deserialize Error: {ex}");
                }
            }
            else
            {
                Debug.LogWarning($"HTTP error: {requestTask.Result.StatusCode}");
            }

            yield return new WaitForSeconds(sleepAmount);
        }
    }

    public Reading GetReading(int _)
    {
        return readings.Count > 0 ? readings[^1] : null;
    }
}