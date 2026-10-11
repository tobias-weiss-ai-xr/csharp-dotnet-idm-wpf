// Ant Colony Optimization for coordinate measurement path planning
public static class AntColonyOptimizer
{
    public static (int[] Order, double TotalDistance) Optimize(CoordPoint[] points, int iterations = 50)
    {
        int n = points.Length;
        if (n <= 1) return (Array.Empty<int>(), 0);
        if (n == 2) return (new[] { 0, 1 }, Dist(points[0], points[1]));

        var dist = new double[n, n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                dist[i, j] = Math.Sqrt(Math.Pow(points[i].X - points[j].X, 2) + Math.Pow(points[i].Y - points[j].Y, 2));

        var pheromone = new double[n, n];
        for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) pheromone[i, j] = 1.0;

        var rnd = new Random();
        int[] best = null!; double bestDist = double.MaxValue;

        for (int iter = 0; iter < iterations; iter++)
        {
            var path = BuildPath(dist, pheromone, rnd, alpha: 1.0, beta: 2.0);
            var pathDist = CalcDist(path, dist);
            if (pathDist < bestDist) { best = path; bestDist = pathDist; }
            Evaporate(pheromone, 0.1);
            Deposit(pheromone, best!, bestDist);
        }
        return (best!, bestDist);
    }

    static int[] BuildPath(double[,] dist, double[,] pher, Random rnd, double alpha, double beta)
    {
        int n = dist.GetLength(0);
        var path = new List<int>(); var used = new bool[n];
        path.Add(rnd.Next(0, n)); used[path[0]] = true;
        for (int step = 1; step < n; step++)
        {
            int cur = path[^1], next = SelectNext(cur, used, dist, pher, rnd, alpha, beta);
            path.Add(next); used[next] = true;
        }
        return path.ToArray();
    }

    static int SelectNext(int cur, bool[] used, double[,] dist, double[,] pher, Random rnd, double alpha, double beta)
    {
        int n = dist.GetLength(0);
        double[] probs = new double[n]; double sum = 0;
        for (int j = 0; j < n; j++)
        {
            if (used[j] || dist[cur,j] < 1e-10) continue;
            probs[j] = Math.Pow(pher[cur, j], alpha) * Math.Pow(1.0 / dist[cur, j], beta);
            sum += probs[j];
        }
        if (sum < 1e-10) return -1;
        double r = rnd.NextDouble() * sum, cum = 0;
        for (int j = 0; j < n; j++) { if (used[j]) continue; cum += probs[j]; if (cum >= r) return j; }
        return -1;
    }

    static double CalcDist(int[] path, double[,] dist)
    {
        double d = 0;
        for (int i = 1; i < path.Length; i++) d += dist[path[i-1], path[i]];
        return d;
    }

    static void Evaporate(double[,] pher, double rate)
    {
        int n = pher.GetLength(0);
        for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) pher[i, j] *= (1.0 - rate);
    }

    static void Deposit(double[,] pher, int[] path, double dist)
    {
        if (path == null || dist < 1e-10) return;
        for (int i = 0; i < path.Length - 1; i++)
        { pher[path[i], path[i+1]] += 1.0 / dist; pher[path[i+1], path[i]] += 1.0 / dist; }
    }

    static double Dist(CoordPoint a, CoordPoint b) =>
        Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
}
