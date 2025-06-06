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
using SocketIOClient;


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

public class APIDataProvider : DataProvider
{
    public List<Reading> readings = new List<Reading>();

    public HttpClient httpClient;

    // Time between api calls
    private int sleepAmount = 3;

    public APIDataProvider()
    {
        this.httpClient = new HttpClient();

        // Start to poll the api
        Task.Run(() => PollAPI());
    }

    private async Task PollAPI()
    {
        while (true)
        {
            Debug.Log("Sending a request");
            var response = await httpClient.GetAsync("http://127.0.0.1:5000/telemetry");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();

                Reading reading = JsonConvert.DeserializeObject<Reading>(data);

                this.readings.Add(reading);
            }

            await Task.Delay(this.sleepAmount);
        }
    }

    public Reading GetReading(int _)
    {
        return this.readings.Last();
    }
}

public class SocketIODataProvider : DataProvider
{
    public List<Reading> readings = new List<Reading>();

    private SocketIOUnity socket;

    public SocketIODataProvider()
    {
        var uri = new System.Uri("http://127.0.0.1:5000");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string> { },
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("SocketIO connected.");
        };

        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("SocketIO disconnected.");
        };

        socket.On("telemetry", response =>
        {
            try
            {
                string json = response.GetValue().ToString();
                Reading reading = JsonConvert.DeserializeObject<Reading>(json);
                readings.Add(reading);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error parsing telemetry: " + ex.Message);
            }
        });

        socket.Connect();
        Debug.Log("Finished the setup");
    }

    public Reading GetReading(int _)
    {
        return readings.Count > 0 ? readings[readings.Count - 1] : null;
    }
}