import time
import json
from utils import get_physics, get_graphics
from flask import Flask, jsonify


app = Flask(__name__)


def save_data(seconds=120, timestep=0.3):
    res = []

    start, last = time.time(), time.time()

    while time.time() - start < seconds:
        if time.time() - last >= timestep:
            graphics_reading = get_graphics()
            res.append({
                "x": graphics_reading.carCoordinates[graphics_reading.playerCarID][0],
                "y": graphics_reading.carCoordinates[graphics_reading.playerCarID][1],
                "z": graphics_reading.carCoordinates[graphics_reading.playerCarID][2],
            })

            print(res[-1])

            last = time.time()

    with open("data_new.json", "w") as f:
        json.dump(res, f)
        print("Saved file")


@app.route("/telemetry")
def live_data():
    physics_reading = get_physics()

    res = {
        "x": (physics_reading.TyreContactPoint.FL.x + physics_reading.TyreContactPoint.RR.x) / 2,
        "y": (physics_reading.TyreContactPoint.FL.y + physics_reading.TyreContactPoint.RR.y) / 2,
        "z": (physics_reading.TyreContactPoint.FL.z + physics_reading.TyreContactPoint.RR.z) / 2,
        "heading": physics_reading.Heading,
        "gear": physics_reading.Gear,
        "rmp": physics_reading.Rpm,
        "speed": physics_reading.SpeedKmh,

    }
    return jsonify(res)


if __name__ == "__main__":
    app.run(debug=True)
