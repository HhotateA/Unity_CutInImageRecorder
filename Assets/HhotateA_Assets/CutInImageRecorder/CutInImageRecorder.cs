using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace HhotateA.ImageRecorder
{
    [RequireComponent(typeof(Camera))]
    public class CutInImageRecorder : MonoBehaviour
    {
        public enum OutputType
        {
            png,
            gif,
            webp
        }

        [Range(1, 60)] public int fps = 30;
        public int recordFrames = 30;
        public Vector2Int resolution = new Vector2Int(960, 720);
        public OutputType outputType = OutputType.webp;
        public string outputPath;
        public bool playAndRun = false;
        public bool loop = true;
        public float selfTimer = 0f;
        public string timerDisplay { get; set; }

        private string tempPath = "";
        private string ResetTempPath()
        {
            if (!string.IsNullOrWhiteSpace(tempPath))
            {
                FileUtil.DeleteFileOrDirectory(tempPath);
            }

            tempPath = "";
            return GetTempPath();
        }
        private string GetTempPath()
        {
            if (string.IsNullOrWhiteSpace(tempPath))
            {
                tempPath = FileUtil.GetUniqueTempPathInProject();
            }

            if (Directory.Exists(tempPath) == false)
                Directory.CreateDirectory(tempPath);
            return tempPath;
        }

        private Camera recorderCamera;
        Camera RecorderCamera
        {
            get
            {
                if (recorderCamera == null)
                {
                    recorderCamera = this.GetComponent<Camera>();
                }

                return recorderCamera;
            }
        }

        private void Start()
        {
            if (playAndRun)
            {
                StartCoroutine(StartTimer());
            }
        }

        public IEnumerator StartTimer()
        {
            var timer = selfTimer;
            while (0 < timer)
            {
                yield return new WaitForSecondsRealtime(1f / 30f);
                timerDisplay = " (" + timer.ToString("F2") + "sec)";
                timer -= 1f / 30f;
            }

            yield return Record();
        }

        public IEnumerator Record()
        {
            ResetTempPath();
            var rt = new RenderTexture(resolution.x, resolution.y, 24);
            var renderTargetCamera = RecorderCamera.targetTexture;
            RecorderCamera.targetTexture = rt;

            Debug.Log("ApngRecorder: Start Recording");
            for (int i = 0; i < recordFrames; i++)
            {
                RecorderCamera.Render();
                ScreenShot(rt, Path.Combine(GetTempPath(), i.ToString() + ".png"));
                timerDisplay = " (" + i.ToString() + "/" + recordFrames.ToString() + ")";
                yield return new WaitForSecondsRealtime(1f / (float) fps);
            }

            Debug.Log("ApngRecorder: End Recording " + outputPath);


            timerDisplay = " (Processing...)";
            RecorderCamera.targetTexture = renderTargetCamera;

            outputPath = GetUniqueFilePath(outputPath);

            switch (outputType)
            {
                case OutputType.png:
                    ApngConverter.Generate(GetTempPath(), outputPath, fps, loop);
                    break;
                case OutputType.gif:
                    ApngConverter.Generate(GetTempPath(),Path.Combine(GetTempPath(), "output.png"), fps, loop);
                    GifConverter.Convert(Path.Combine(GetTempPath(), "output.png"),outputPath);
                    break;
                case OutputType.webp:
                    WebpConverter.Generate(GetTempPath(), outputPath, fps, loop);
                    break;
            }

            timerDisplay = "";

            GC.Collect();
            yield return null;
        }

        void ScreenShot(RenderTexture src, string file)
        {
            Texture2D result = new Texture2D(src.width, src.height, TextureFormat.ARGB32, false);
            var renderTargetActive = RenderTexture.active;
            RenderTexture.active = src;
            result.ReadPixels(new Rect(0, 0, src.width, src.height), 0, 0, false);
            result.Apply();
            RenderTexture.active = renderTargetActive;

            File.WriteAllBytes(file, result.EncodeToPNG());
            DestroyImmediate(result);
        }

        string GetUniqueFilePath(string path)
        {
            if (File.Exists(path))
            {
                var extension = Path.GetExtension(path);
                var fileName = Path.GetFileNameWithoutExtension(path);
                var fileDir = Path.GetDirectoryName(path) ?? "";
                var m = Regex.Match(fileName, @"（\d+）$");
                fileName = Regex.Replace(fileName, @"（\d+）$","");
                if (m.Success)
                {
                    var num = int.Parse(Regex.Replace(m.Value, @"[^0-9]", ""));
                    num++;
                    fileName += "（"+num.ToString()+"）";
                }
                else
                {
                    fileName += "（1）";
                }

                return GetUniqueFilePath(Path.Combine(fileDir,fileName + extension));
            }

            return path;
        }

        public void CancellRecord()
        {
            timerDisplay = "";
            StopAllCoroutines();
        }
    }

    [CustomEditor(typeof(CutInImageRecorder))]
    public class ApngRecorderEditor : Editor
    {
    }
}