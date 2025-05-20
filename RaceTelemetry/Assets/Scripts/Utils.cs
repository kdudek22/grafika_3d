public class Reading{
    public float x;
    public float y;
    public float z;
    public float heading;
    public int gear;
    public int rmp;
    public float speed; 
}

public enum PlayType{
    File,
    Live,
}

public enum CameraType
{
    Free,
    Follow,
    LockedInPlace,
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
