import fastf1
import matplotlib.pyplot as plt
from matplotlib.animation import FuncAnimation
from flask import Flask, jsonify, request

app = Flask(__name__)


@app.route('/events')
def events():
    df = fastf1.get_event_schedule(2024)
    df = df[df["EventFormat"] == "conventional"]
    df = df[["EventName", "EventDate"]]
    df["EventDate"] = df["EventDate"].dt.strftime("%Y-%m-%d")
    df = df.rename(columns={"EventName": "Name", "EventDate": "Date"})

    return jsonify(df.to_dict(orient="records"))


@app.route('/events/<year>/<name>')
def event(year, name):
    session = fastf1.get_session(int(year), name, 'R')
    session.load(telemetry=True)

    drivers = [session.get_driver(driver).Abbreviation for driver in session.drivers]

    res = []

    for driver in drivers:
        driver_laps = session.laps.pick_drivers([driver])
        telemetry = driver_laps.get_telemetry()

        dict_arr = telemetry.to_dict(orient="records")

        for i in range(len(dict_arr)):
            formatted_step = format_telemetry_step(dict_arr[i], driver)

            res.append([formatted_step]) if i >= len(res) else res[i].append(formatted_step)

    return jsonify(res)


def format_telemetry_step(telemetry_step: dict, driver: str) -> dict:
    return {"Brake": telemetry_step["Brake"],
            "DRS": telemetry_step["DRS"],
            "RPM": telemetry_step["RPM"],
            "X": telemetry_step["X"],
            "Y": telemetry_step["Y"],
            "Driver": driver}


if __name__ == '__main__':
    app.run(debug=True)
