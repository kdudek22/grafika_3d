import socket


if __name__ == "__main__":
    host = "127.0.0.1"
    port = 9000
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

    s.bind((host, port))
    while True:
        (data, addr) = s.recvfrom(128 * 1024)
        print(data)
