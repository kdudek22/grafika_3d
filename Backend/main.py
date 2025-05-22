import time
import json
from utils import get_physics, get_graphics
from flask import Flask, jsonify


app = Flask(__name__)


def save_data(seconds=300, timestep=0.05):
    res = []

    start, last = time.time(), time.time()

    while time.time() - start < seconds:
        if time.time() - last >= timestep:

            graphics_reading = get_graphics()
            physics_reading = get_physics()

            res.append({
                "x": (physics_reading.TyreContactPoint.FL.x + physics_reading.TyreContactPoint.RR.x) / 2,
                "y": (physics_reading.TyreContactPoint.FL.y + physics_reading.TyreContactPoint.RR.y) / 2,
                "z": (physics_reading.TyreContactPoint.FL.z + physics_reading.TyreContactPoint.RR.z) / 2,
                "heading": physics_reading.Heading,
                "pitch": physics_reading.Pitch,
                "roll": physics_reading.Roll,
                "gear": physics_reading.Gear,
                "rpm": physics_reading.Rpm,
                "steer_angle": physics_reading.SteerAngle,
                "speed": physics_reading.SpeedKmh,
                "gas": physics_reading.Gas,
                "break": physics_reading.Brake,
                "velocity_vector": [physics_reading.Velocity.x, physics_reading.Velocity.y, physics_reading.Velocity.z],
                "current_time": graphics_reading.currentTime,
            })

            print(res[-1])

            last = time.time()

    with open("bmw_data.json", "w") as f:
        json.dump(res, f, indent=4)
        print("Saved file")


@app.route("/telemetry")
def live_data():
    graphics_reading = get_graphics()
    physics_reading = get_physics()

    res = {
        "x": (physics_reading.TyreContactPoint.FL.x + physics_reading.TyreContactPoint.RR.x) / 2,
        "y": (physics_reading.TyreContactPoint.FL.y + physics_reading.TyreContactPoint.RR.y) / 2,
        "z": (physics_reading.TyreContactPoint.FL.z + physics_reading.TyreContactPoint.RR.z) / 2,
        "heading": physics_reading.Heading,
        "pitch": physics_reading.Pitch,
        "roll": physics_reading.Roll,
        "gear": physics_reading.Gear,
        "rpm": physics_reading.Rpm,
        "steer_angle": physics_reading.SteerAngle,
        "speed": physics_reading.SpeedKmh,
        "gas": physics_reading.Gas,
        "break": physics_reading.Brake,
        "velocity_vector": [physics_reading.Velocity.x, physics_reading.Velocity.y, physics_reading.Velocity.z],
        "current_time": graphics_reading.currentTime,
    }
    return jsonify(res)


if __name__ == "__main__":
    # save_data()
    app.run(debug=True)
