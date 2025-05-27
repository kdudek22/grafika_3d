import requests
import time

if __name__ == "__main__":
    """Nicely timed camera switches for the mazda_data.json file"""
    requests.post("http://localhost:8095/camera/type", json={"camera_type": "Driver"})

    time.sleep(10)

    requests.post("http://localhost:8095/camera/move", json={"x": -171, "y": 37, "z": 140,
                                                             "rotation_x": 30, "rotation_y": 130, "rotation_z": 0})

    time.sleep(10)

    requests.post("http://localhost:8095/camera/type", json={"camera_type": "FollowClose"})


    time.sleep(13)

    requests.post("http://localhost:8095/camera/move", json={"x": 193, "y": 22, "z": -55,
                                                             "rotation_x": 22, "rotation_y": -80, "rotation_z": 0})



