using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVisionLab
{
    internal static class OpenVisionLearnBinarySimulationModel
    {
        public static bool[] CalculateMorphology(
            int[] values,
            int width,
            int height,
            string mode)
        {
            bool[] source = values.Select(value => value > 0).ToArray();
            return mode switch
            {
                "Dilation" => Dilate(source, width, height),
                "Opening" => Dilate(Erode(source, width, height), width, height),
                "Closing" => Erode(Dilate(source, width, height), width, height),
                _ => Erode(source, width, height)
            };
        }

        public static (int[] Labels, int[] Areas) LabelConnectedBlobs(
            int[] values,
            int width,
            int height)
        {
            int[] labels = new int[values.Length];
            List<int> areas = new();
            int nextLabel = 1;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == 0 || labels[i] != 0)
                {
                    continue;
                }

                int area = FloodFillBlob(values, labels, width, height, i, nextLabel);
                areas.Add(area);
                nextLabel++;
            }

            return (labels, areas.ToArray());
        }

        public static bool[] FindContourPixels(bool[] source, int width, int height)
        {
            bool[] result = new bool[source.Length];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    if (!source[index])
                    {
                        continue;
                    }

                    result[index] = !GetBinary(source, width, height, x - 1, y)
                        || !GetBinary(source, width, height, x + 1, y)
                        || !GetBinary(source, width, height, x, y - 1)
                        || !GetBinary(source, width, height, x, y + 1);
                }
            }

            return result;
        }

        public static (int MinX, int MinY, int MaxX, int MaxY)? FindBounds(
            bool[] source,
            int width,
            int height)
        {
            int minX = width;
            int minY = height;
            int maxX = -1;
            int maxY = -1;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!source[y * width + x])
                    {
                        continue;
                    }

                    minX = Math.Min(minX, x);
                    minY = Math.Min(minY, y);
                    maxX = Math.Max(maxX, x);
                    maxY = Math.Max(maxY, y);
                }
            }

            return maxX < 0 ? null : (minX, minY, maxX, maxY);
        }

        public static bool IsOnBounds(
            int index,
            int width,
            (int MinX, int MinY, int MaxX, int MaxY) bounds)
        {
            int x = index % width;
            int y = index / width;
            return x >= bounds.MinX
                && x <= bounds.MaxX
                && y >= bounds.MinY
                && y <= bounds.MaxY
                && (x == bounds.MinX
                    || x == bounds.MaxX
                    || y == bounds.MinY
                    || y == bounds.MaxY);
        }

        private static int FloodFillBlob(
            int[] values,
            int[] labels,
            int width,
            int height,
            int start,
            int label)
        {
            Queue<int> queue = new();
            queue.Enqueue(start);
            labels[start] = label;
            int area = 0;

            while (queue.Count > 0)
            {
                int index = queue.Dequeue();
                area++;
                int x = index % width;
                int y = index / width;

                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }

                        int nx = x + dx;
                        int ny = y + dy;
                        if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                        {
                            continue;
                        }

                        int next = ny * width + nx;
                        if (values[next] == 0 || labels[next] != 0)
                        {
                            continue;
                        }

                        labels[next] = label;
                        queue.Enqueue(next);
                    }
                }
            }

            return area;
        }

        private static bool[] Erode(bool[] source, int width, int height)
        {
            bool[] result = new bool[source.Length];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result[y * width + x] = AllNeighbors(source, width, height, x, y);
                }
            }

            return result;
        }

        private static bool[] Dilate(bool[] source, int width, int height)
        {
            bool[] result = new bool[source.Length];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result[y * width + x] = AnyNeighbor(source, width, height, x, y);
                }
            }

            return result;
        }

        private static bool AllNeighbors(
            bool[] source,
            int width,
            int height,
            int x,
            int y)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (!GetBinary(source, width, height, x + dx, y + dy))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool AnyNeighbor(
            bool[] source,
            int width,
            int height,
            int x,
            int y)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (GetBinary(source, width, height, x + dx, y + dy))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool GetBinary(
            bool[] source,
            int width,
            int height,
            int x,
            int y)
        {
            return x >= 0
                && x < width
                && y >= 0
                && y < height
                && source[y * width + x];
        }
    }
}
