using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;  // Make sure you have this at the top

public class ServerListener : MonoBehaviour
{
    private HttpListener listener;
    private Thread listenerThread;

    void Start()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();

        listenerThread = new Thread(HandleIncomingConnections);
        listenerThread.Start();

        Debug.Log("Server started at http://localhost:8080/");
    }

    private void HandleIncomingConnections()
    {
        while (listener.IsListening)
        {
            try
            {
                var context = listener.GetContext();
                var request = context.Request;
                var response = context.Response;

                string responseBody;
                int statusCode = 200;

                if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/test"){
                    responseBody = TestView(request);
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/camera/info"){
                    responseBody = GetCameraInfoView(request);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/camera/move")
                {
                    responseBody = MoveCameraView(request);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/camera/type")
                {
                    responseBody = ChangeCameraTypeView(request);
                }
                else
                {
                    statusCode = 404;
                    responseBody = JsonConvert.SerializeObject(new { error = "Not found" });
                }

                byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                response.StatusCode = statusCode;
                response.ContentLength64 = buffer.Length;
                response.ContentType = "application/json";
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                Debug.LogError("Server error: " + ex.Message);
            }
        }
    }

    // GET /hello
    private string TestView(HttpListenerRequest request)
    {
        return JsonConvert.SerializeObject(new { message = "Hello back" });
    }

    // POST /camera/type - allows for the change of the camera type
    // the expected json body is {"camera_type": "Free" | "Follow" | "LockedInPlace"}
    private string ChangeCameraTypeView(HttpListenerRequest request)
    {
        StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding);
        string requestBody = reader.ReadToEnd();
        try
        {
            CameraTypeBody cameraTypeBody = JsonConvert.DeserializeObject<CameraTypeBody>(requestBody);
            UnityMainThreadDispatcher.Enqueue(() => CameraManager.instance.UpdateCameraType(cameraTypeBody.camera_type));
        }
        catch (Exception e)
        {
            return JsonConvert.SerializeObject(new { error = "Invalid body" });
        }
        return JsonConvert.SerializeObject(new { status = "Success" });
    }

    // POST /camera/move
    // moves the camera to the desired position, expected json is {"x": int, "y": int, "z": int, "rotation_x": int, "rotation_y": int, "rotation_z": int}
    private string MoveCameraView(HttpListenerRequest request)
    {
        StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding);
        string requestBody = reader.ReadToEnd();
        try
        {
            CameraMoveBody movebody = JsonConvert.DeserializeObject<CameraMoveBody>(requestBody);
            UnityMainThreadDispatcher.Enqueue(() => CameraManager.instance.MoveCameraToPosition(new Vector3(movebody.x, movebody.y, movebody.z), new Vector3(movebody.rotation_x, movebody.rotation_y, movebody.rotation_z)));

        }
        catch (Exception e)
        {
            return JsonConvert.SerializeObject(new { error = "Invalid body" });
        }

        return JsonConvert.SerializeObject(new { status = "Success" });
    }

    // GET /camera/info
    // gets the cameras basic info like, position, rotation and the current state - Follow | Free | LockedInPlace
    private string GetCameraInfoView(HttpListenerRequest request)
    {
        var infoMap = UnityMainThreadDispatcher.EnqueueSync(() => CameraManager.instance.GetInfoMap());
        return JsonConvert.SerializeObject(infoMap);
    }

    void OnApplicationQuit()
    {
        listener.Stop();
        listenerThread.Abort();
    }
}
