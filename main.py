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
    return jsonify(get_telemetry(int(year), name))


def get_better_telemetry(year: int, name: str) -> list[list[dict]]:
    session = fastf1.get_session(int(year), name, 'R')
    session.load(telemetry=True)

    drivers = [session.get_driver(driver).Abbreviation for driver in session.drivers]

    res = []

    for driver in drivers:
        driver_laps = session.laps.pick_drivers([driver])
        dict_arr = driver_laps.get_pos_data().to_dict(orient="records")

        for i in range(len(dict_arr)):
            formatted_step = new_format(dict_arr[i], driver)
            res.append([formatted_step]) if i >= len(res) else res[i].append(formatted_step)

    return res


def new_format(telemetry_step: dict, driver: str) -> dict:
    return {"Brake": False,
            "DRS": 0,
            "RPM": 100.0,
            "X": telemetry_step["X"],
            "Y": telemetry_step["Y"],
            "Driver": driver}


def get_telemetry(year: int, name: str) -> list[list[dict]]:
    """Returns the telemetry for all drivers"""
    session = fastf1.get_session(int(year), name, 'R')
    session.load(telemetry=True)

    drivers = [session.get_driver(driver).Abbreviation for driver in session.drivers]

    res = []

    for driver in drivers:
        driver_laps = session.laps.pick_drivers([driver])
        telemetry = driver_laps.get_telemetry(frequency=20)

        dict_arr = telemetry.to_dict(orient="records")

        for i in range(len(dict_arr)):
            formatted_step = format_telemetry_step(dict_arr[i], driver)

            res.append([formatted_step]) if i >= len(res) else res[i].append(formatted_step)

    return res


def format_telemetry_step(telemetry_step: dict, driver: str) -> dict:
    return {"Brake": telemetry_step["Brake"],
            "DRS": telemetry_step["DRS"],
            "RPM": telemetry_step["RPM"],
            "X": telemetry_step["X"],
            "Y": telemetry_step["Y"],
            "Driver": driver}


if __name__ == '__main__':
    app.run(debug=True)
