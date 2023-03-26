using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Debug = UnityEngine.Debug;

namespace HhotateA.ImageRecorder
{
    public static class WebpConverter
    {
        public static void Generate(string inputPath, string outputPath, int fps = 30, bool loop = true)
        {
            var ps = new ProcessStartInfo();
            ps.FileName = Path.Combine(Application.streamingAssetsPath, "CutInImageRecorder/libwebp/img2webp.exe");
            ps.Arguments = loop ? "-loop 0" : "-loop 1";
            
            string[] pngs = Directory.GetFiles(inputPath, "*.png",SearchOption.TopDirectoryOnly);
            pngs = pngs.OrderBy(e =>
            {
                var fileName = Regex.Match(e, @"\d+.png$");
                return int.Parse(fileName.Value.Replace(".png", ""));
            }).ToArray();
            foreach (var png in pngs)
            {
                ps.Arguments += " -d " + (1000 / fps).ToString();
                ps.Arguments += " " + png;
                Debug.Log(png);
            }
            ps.Arguments += " -o " + outputPath;

            var p = Process.Start(ps);
            p.WaitForExit(5000);
        }
    }
}
