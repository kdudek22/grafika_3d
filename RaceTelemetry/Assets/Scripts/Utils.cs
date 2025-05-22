public class Reading {
    public float x;
    public float y;
    public float z;
    public float heading;
    public float pitch;
    public int gear;
    public int rpm;
    public float steer_angle;
    public float gas;
    public float brake;
    public float speed;
    public string current_time;
}

public enum PlayType
{
    File,
    Live,
}

public enum ReplayState
{
    Idle,
    Starting,
    Playing,
    Paused,
}

public enum CameraType
{
    Free,
    FollowBirdsEye,
    FollowClose,
    LockedInPlace,
    Driver,
}

public class CameraMoveBody
{
    public int x;
    public int y;
    public int z;
    public int rotation_x;
    public int rotation_y;
    public int rotation_z;
}

public class CameraTypeBody
{
    public CameraType camera_type;
}
