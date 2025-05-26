import requests
import time

if __name__ == "__main__":
    """Nicely timed camera switches for the mazda_data.json file"""
    time.sleep(10)

    requests.post("http://localhost:8080/camera/move", json={"x": -114, "y": 18, "z": -434, "rotation_x": 0, "rotation_y": 45, "rotation_z": 0})

    time.sleep(8)

    requests.post("http://localhost:8080/camera/type", json={"camera_type": "FollowBirdsEye"})

    time.sleep(10)

    requests.post("http://localhost:8080/camera/type", json={"camera_type": "FollowClose"})

    time.sleep(5)

    requests.post("http://localhost:8080/camera/move",
                  json={"x": -654, "y": 73, "z": 375, "rotation_x": 10, "rotation_y": 104, "rotation_z": 0})

    time.sleep(30)

    requests.post("http://localhost:8080/camera/type", json={"camera_type": "Driver"})

    time.sleep(5)

    requests.post("http://localhost:8080/camera/move",
                  json={"x": 222, "y": 34, "z": 322, "rotation_x": -5, "rotation_y": 242, "rotation_z": 0})

    time.sleep(10)

    requests.post("http://localhost:8080/camera/type", json={"camera_type": "FollowClose"})
