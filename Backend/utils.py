import ctypes
from ctypes import c_float, c_int, c_wchar
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

        ("Velocity", Vector3),
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


class SPageFileGraphic(ctypes.Structure):
    _fields_ = [
        ("packetId", c_int),
        ("AC_STATUS", c_int),
        ("AC_SESSION_TYPE", c_int),
        ("currentTime", ctypes.c_wchar * 15),
        ("lastTime", c_wchar * 15),
        ("bestTime", c_wchar * 15),
        ("split", c_wchar * 15),
        ("completedLaps", c_int),
        ("position", c_int),
        ("iCurrentTime", c_int),
        ("iLastTime", c_int),
        ("iBestTime", c_int),
        ("sessionTimeLeft", c_float),
        ("distanceTraveled", c_float),
        ("isInPit", c_int),
        ("currentSectorIndex", c_int),
        ("lastSectorTime", c_int),
        ("numberOfLaps", c_int),
        ("tyreCompound", c_wchar * 33),
        ("replayTimeMultiplier", c_float),
        ("normalizedCarPosition", c_float),
        ("activeCars", c_int),
        ("carCoordinates", c_float * 60 * 3),
        ("carID", c_int * 60),
        ("playerCarID", c_int),
        ("penaltyTime", c_float),
        ("flag", c_int),
        ("penalty", c_int),
        ("idealLineOn", c_int),
        ("isInPitLane", c_int),
        ("surfaceGrip", c_float),
        ("mandatoryPitDone", c_int),
        ("windSpeed", c_float),
        ("windDirection", c_float),
        ("isSetupMenuVisible", c_int),
        ("mainDisplayIndex", c_int),
        ("secondaryDisplayIndex", c_int),
        ("TC", c_int),
        ("TCCut", c_int),
        ("EngineMap", c_int),
        ("ABS", c_int),
        ("fuelXLap", c_int),
        ("rainLights", c_int),
        ("flashingLights", c_int),
        ("lightsStage", c_int),
        ("exhaustTemperature", c_float),
        ("wiperLV", c_int),
        ("DriverStintTotalTimeLeft", c_int),
        ("DriverStintTimeLeft", c_int),
        ("rainTypes", c_int),
    ]

    def to_dict(self):
        return {
            "test": self.carCoordinates
        }


def get_graphics() -> SPageFileGraphic:
    buf = mmap.mmap(-1, ctypes.sizeof(SPageFileGraphic), u"Local\\acpmf_graphics")
    data = SPageFileGraphic.from_buffer_copy(buf)

    return data


def get_physics() -> SPageFilePhysics:
    map_file = "acpmf_physics"
    size = ctypes.sizeof(SPageFilePhysics)

    with mmap.mmap(-1, size, map_file, access=mmap.ACCESS_READ) as mm:
        data = mm.read(size)
        physics = SPageFilePhysics.from_buffer_copy(data)

    return physics
