import time
import json
from utils import get_physics
from flask import Flask


if __name__ == "__main__":
    data = []

    last = time.time()
    first = time.time()

    while True:
        if time.time() - last > 0.05:
            physics_reading = get_physics()
            print(f"\r{physics_reading.Heading}", end='', flush=True)

            res = {
                   "x": physics_reading.TyreContactPoint.FL.x,
                   "y": physics_reading.TyreContactPoint.FL.y,
                   "z": physics_reading.TyreContactPoint.FL.z,
                   "heading": physics_reading.Heading
                }

            data.append(res)

            last = time.time()

        if time.time() - first > 300:

            with open("data_bmw_20fps.json", "w") as f:
                json.dump(data, f)

            print("Saved File")
            exit()


