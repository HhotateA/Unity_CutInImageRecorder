using UnityEngine;
using System.Diagnostics;
using System.IO;

namespace HhotateA.ImageRecorder
{
    public static class GifConverter
    {
        public static void Convert(string inputPath, string outputPath)
        {
            var ps = new ProcessStartInfo();
            ps.FileName = Path.Combine(Application.streamingAssetsPath, "CutInImageRecorder/apng2gif/apng2gif.exe");
            ps.Arguments = inputPath + " " + outputPath;

            var p = Process.Start(ps);
            p.WaitForExit(5000);
        }
    }
}
