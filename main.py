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
    session.load()
    return jsonify({"year": year, "name": name})



if __name__ == '__main__':
    app.run(debug=True)

    # x = fastf1.get_event_schedule(2024)


    # def init():
    #     ax.set_xlim(-10_000, 10_000)
    #     ax.set_ylim(-10_000, 10_000)
    #     return ln,
    #
    #
    # def step(i: int):
    #     ln.set_data([x[i + 2]], [y[i + 2]])
    #     return ln,
    #
    # session = fastf1.get_session(2024, 'Austrian Grand Prix', 'R')
    # session.load()
    #
    # laps = session.laps.pick_drivers(["VER"])
    # x, y = laps.telemetry["X"], laps.telemetry["Y"]
    #
    # fig, ax = plt.subplots()
    # xdata, ydata = [], []
    # ln, = plt.plot([], [], 'ro')
    #
    # ani = FuncAnimation(fig, step, frames=2000, init_func=init)
    # plt.show()




# import numpy as np
# import matplotlib.pyplot as plt
# from matplotlib.animation import FuncAnimation
#
# fig, ax = plt.subplots()
# xdata, ydata = [], []
# ln, = plt.plot([], [], 'ro')
#
# def init():
#     ax.set_xlim(-10, 10)
#     ax.set_ylim(-10, 10)
#     return ln,
#
# def update(frame: float):
#     ln.set_data([1 * frame], [2*frame])
#     return ln,
#
# ani = FuncAnimation(fig, update, frames=200, init_func=init)
# plt.show()
#
#
