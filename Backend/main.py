import time
import json
from utils import get_physics


if __name__ == "__main__":
    pos_x, pos_y, pos_z = [], [], []

    last = time.time()
    first = time.time()

    while True:
        if time.time() - last > 0.3:
            physics_reading = get_physics()
            print(f"\r{physics_reading.SpeedKmh}", end='', flush=True)

            last = time.time()

        if time.time() - first > 120:
            res = {"x": pos_x, "y": pos_y, "z": pos_z}

            with open("data.json", "w") as f:
                json.dump(res, f)

            print("Saved File")
            exit()
