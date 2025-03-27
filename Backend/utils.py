import ctypes
from ctypes import c_float, c_int
import mmap


class Coordinates(ctypes.Structure):
    _fields_ = [
        ("x", ctypes.c_float),
        ("y", ctypes.c_float),
        ("z", ctypes.c_float)
    ]


class Coordinates4(ctypes.Structure):
    _fields_ = [
        ("FL", Coordinates),
        ("FR", Coordinates),
        ("RL", Coordinates),
        ("RR", Coordinates),
    ]


class Vector3(ctypes.Structure):
    _fields_ = [
        ("x", ctypes.c_float),
        ("y", ctypes.c_float),
        ("z", ctypes.c_float)
    ]


class RideHeight(ctypes.Structure):
    _fields_ = [
        ("front", ctypes.c_float),
        ("rear", ctypes.c_float)
    ]


class Reading4(ctypes.Structure):
    _fields_ = [
        ("a", ctypes.c_float),
        ("b", ctypes.c_float),
        ("c", ctypes.c_float),
        ("d", ctypes.c_float)
    ]


class Reading5(ctypes.Structure):
    _fields_ = [
        ("a", ctypes.c_float),
        ("b", ctypes.c_float),
        ("c", ctypes.c_float),
        ("d", ctypes.c_float),
        ("f", ctypes.c_float)
    ]


class SPageFilePhysics(ctypes.Structure):
    _fields_ = [
        ("PacketId", ctypes.c_int),
        ("Gas", ctypes.c_float),
        ("Brake", ctypes.c_float),
        ("Fuel", ctypes.c_float),
        ("Gear", ctypes.c_int),
        ("Rpm", ctypes.c_int),
        ("SteerAngle", ctypes.c_float),
        ("SpeedKmh", ctypes.c_float),

        ("velocity", Vector3),
        ("AccG", Vector3),
        ("WheelSlip", Reading4),
        ("WheelLoad", Reading4),
        ("WheelPressure", Reading4),
        ("WheelAngularSpeed", Reading4),
        ("TyreWear", Reading4),
        ("TyreDirtLevel", Reading4),
        ("TyreTemperature", Reading4),
        ("CamberRad", Reading4),
        ("SuspensionTravel", Reading4),

        ("Drs", ctypes.c_float),
        ("TC", ctypes.c_float),
        ("Heading", ctypes.c_float),
        ("Pitch", ctypes.c_float),
        ("Roll", ctypes.c_float),
        ("CgHeight", ctypes.c_float),

        ("CarDamage", Reading5),

        ("TyresOut", ctypes.c_int),
        ("PitLimiter", ctypes.c_int),
        ("Abs", ctypes.c_float),

        ("KersCharge", ctypes.c_float),
        ("KersInput", ctypes.c_float),
        ("AutoShift", ctypes.c_int),
        ("RideHeight", RideHeight),

        ("TurboBoost", ctypes.c_float),
        ("Ballast", ctypes.c_float),
        ("AirDensity", ctypes.c_float),

        ("AirTemp", ctypes.c_float),
        ("RoadTemp", ctypes.c_float),
        ("LocalAngularVelocity", Vector3),
        ("FinalFF", ctypes.c_float),

        ("PerformanceMeter", ctypes.c_float),
        ("EngineBrake", ctypes.c_int),
        ("ErsRecoveryLevel", c_int),
        ("ErsPowerLevel", c_int),
        ("ErsHeatCharging", c_int),
        ("ErsisCharging", c_int),
        ("KersCurrentKJ", c_float),
        ("DrsAvailable", c_int),
        ("DrsEnabled", c_int),
        ("BrakeTemp", Reading4),

        ("Clutch", c_float),

        ("TyreTempI", Reading4),
        ("TyreTempM", Reading4),
        ("TyreTempO", Reading4),

        ("IsAiControlled", c_int),

        ("TyreContactPoint", Coordinates4),
        ("TyreContactNormal", Coordinates4),
        ("TyreContactHeading", Coordinates4),
        ("BrakeBias", c_float),
    ]


def get_physics() -> SPageFilePhysics:
    map_file = "acpmf_physics"
    size = ctypes.sizeof(SPageFilePhysics)

    with mmap.mmap(-1, size, map_file, access=mmap.ACCESS_READ) as mm:
        data = mm.read(size)
        physics = SPageFilePhysics.from_buffer_copy(data)

    return physics
