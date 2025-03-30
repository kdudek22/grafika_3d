import time
import json
from utils import get_physics
from flask import Flask, jsonify


app = Flask(__name__)


def save_data(seconds=300, timestep=0.3):
    res = []

    start, last = time.time(), time.time()

    while time.time() - start < seconds:
        if time.time() - last >= timestep:
            physics_reading = get_physics()
            res.append({
                "x": physics_reading.TyreContactPoint.FL.x,
                "y": physics_reading.TyreContactPoint.FL.y,
                "z": physics_reading.TyreContactPoint.FL.z,
                "heading": physics_reading.Heading
            })

            last = time.time()

    with open("data.json", "w") as f:
        json.dump(res, f)
        print("Saved file")


@app.route("/telemetry")
def live_data():
    physics_reading = get_physics()

    res = {
        "x": physics_reading.TyreContactPoint.FL.x,
        "y": physics_reading.TyreContactPoint.FL.y,
        "z": physics_reading.TyreContactPoint.FL.z,
        "heading": physics_reading.Heading
    }
    return jsonify(res)


if __name__ == "__main__":
    app.run(debug=True)


class FilePlayer:
    readings: list[int]
    current_index: int

    def get_next(self):
        self.current_index += 1
        return self.readings[self.current_index]

    def get_at_given_index(self, index: int):
        return self.readings[index]


class APIPlayer:
    def get_next(self):
        return # api call here