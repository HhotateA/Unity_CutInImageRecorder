using UnityEngine;
using System.Diagnostics;
using System.IO;

namespace HhotateA.ImageRecorder
{
    public static class ApngConverter
    {
        public static void Generate(string inputPath, string outputPath, int fps = 30, bool loop = true)
        {
            var ps = new ProcessStartInfo();
            ps.FileName = Path.Combine(Application.dataPath, "HhotateA_Assets/CutInImageRecorder/apngasm/apngasm.exe");
            ps.Arguments = outputPath + " " + Path.Combine(inputPath, "*.png");
            ps.Arguments += " 1 " + fps.ToString();
            ps.Arguments += loop ? " -l0" : " -l1";

            var p = Process.Start(ps);
            p.WaitForExit(5000);
        }
    }
}
