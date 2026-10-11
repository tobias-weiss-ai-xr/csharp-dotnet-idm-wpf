public static class PathOptimizer
{
    public static double Dist(CoordPoint a, CoordPoint b) =>
        Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

    public static int[] GreedyNn(CoordPoint[] pts)
    {
        if (pts.Length == 0) return Array.Empty<int>();
        var used = new bool[pts.Length];
        var path = new List<int>();
        path.Add(0); used[0] = true;
        for (int i = 1; i < pts.Length; i++)
        {
            var last = path[^1]; int nearest = -1; double min = double.MaxValue;
            for (int j = 0; j < pts.Length; j++)
                if (!used[j]) { var d = Dist(pts[last], pts[j]); if (d < min) { min = d; nearest = j; } }
            path.Add(nearest); used[nearest] = true;
        }
        return path.ToArray();
    }
}
