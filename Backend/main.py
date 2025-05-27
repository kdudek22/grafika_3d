import time
import json
from utils import get_physics, get_graphics
from flask import Flask, jsonify
from flask import Flask
from flask_socketio import SocketIO, emit
import time
import threading


app = Flask(__name__)
socketio = SocketIO(app, cors_allowed_origins='*', async_mode="threading")


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


def get_telemetry_data():
    graphics_reading = get_graphics()
    physics_reading = get_physics()

    return {
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


@app.route("/telemetry")
def live_data():
    return jsonify(get_telemetry_data())


@socketio.on('connect')
def handle_connect():
    print("Client connected")

    def send_telemetry():
        while True:
            data = get_telemetry_data()
            socketio.emit('telemetry', data)
            socketio.sleep(0.05)

    threading.Thread(target=send_telemetry, daemon=True).start()

if __name__ == '__main__':
    socketio.run(app, host='127.0.0.1', port=5000)
