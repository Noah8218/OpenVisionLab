using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace OpenVisionLab
{
    internal static class OpenVisionToolOpenProfiler
    {
        [ThreadStatic]
        private static List<string> currentPhases;

        public static void Begin()
        {
            currentPhases = new List<string>();
        }

        public static T Measure<T>(string phaseName, Func<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                return action();
            }
            finally
            {
                Record(phaseName, stopwatch.ElapsedMilliseconds);
            }
        }

        public static void Measure(string phaseName, Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                action();
            }
            finally
            {
                Record(phaseName, stopwatch.ElapsedMilliseconds);
            }
        }

        public static void Record(string phaseName, long elapsedMilliseconds)
        {
            if (currentPhases == null || string.IsNullOrWhiteSpace(phaseName))
            {
                return;
            }

            currentPhases.Add(
                "Internal" + phaseName + "Ms=" + elapsedMilliseconds.ToString(CultureInfo.InvariantCulture));
        }

        public static string Consume()
        {
            if (currentPhases == null || currentPhases.Count == 0)
            {
                currentPhases = null;
                return string.Empty;
            }

            string result = string.Join("|", currentPhases);
            currentPhases = null;
            return result;
        }
    }
}
