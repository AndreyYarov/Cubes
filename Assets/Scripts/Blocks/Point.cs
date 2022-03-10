using System;
using UnityEngine;

[Serializable]
public struct Point
{
    public int x, y, z;

    public Point(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static explicit operator Point(Vector3 vector3) =>
        new Point(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z));

    public static implicit operator Vector3(Point point) =>
        new Vector3(point.x, point.y, point.z);

    public static Point operator +(Point p1, Point p2) =>
        new Point(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);

    public static Point operator -(Point p1, Point p2) =>
        new Point(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);

    public static Point operator *(Point point, int factor) =>
        new Point(point.x * factor, point.y * factor, point.z * factor);

    public static Point operator *(int factor, Point point) =>
        new Point(point.x * factor, point.y * factor, point.z * factor);

    public static Vector3 operator /(Point point, float v) =>
        (Vector3)point / v;

    public static bool operator >=(Point p1, Point p2) =>
        p1.x >= p2.x && p1.y >= p2.y && p1.z >= p2.z;

    public static bool operator <=(Point p1, Point p2) =>
        p1.x <= p2.x && p1.y <= p2.y && p1.z <= p2.z;

    public static bool operator ==(Point p1, Point p2) =>
        p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;

    public static bool operator !=(Point p1, Point p2) =>
        p1.x != p2.x || p1.y != p2.y || p1.z != p2.z;

    public static Point Clamp(Point p, Point min, Point max) =>
        new Point(Mathf.Clamp(p.x, min.x, max.x), Mathf.Clamp(p.y, min.y, max.y), Mathf.Clamp(p.z, min.z, max.z));

    public override string ToString() =>
        $"({x}, {y}, {z})";
}
