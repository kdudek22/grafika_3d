import numpy as np
import matplotlib.pyplot as plt
import matplotlib.animation as animation
import requests
import json


def get_x_and_y_from_step(step_data):
    x = [data["X"] for data in step_data]
    y = [data["Y"] for data in step_data]
    return x, y


if __name__ == "__main__":
    fig, ax = plt.subplots()
    ax.set_xlim(-10000, 10000)
    ax.set_ylim(-10000, 10000)

    response = requests.get("http://localhost:5000/events/2024/Australian%20Grand%20Prix")
    data = json.loads(response.text)

    current_index = 0
    x, y = get_x_and_y_from_step(data[current_index])

    sc = ax.scatter(x, y)


    def update(frame):
        x, y = get_x_and_y_from_step(data[frame])
        sc.set_offsets(np.c_[x, y])  # Update scatter plot positions
        return sc,


    # Create animation
    ani = animation.FuncAnimation(fig, update, frames=5000, interval=50, blit=True)

    plt.show()

