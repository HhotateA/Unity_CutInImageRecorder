using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEditor;
#if EditorCoroutines
    using Unity.EditorCoroutines.Editor;
#endif

namespace HhotateA.ImageRecorder
{
    [CustomEditor(typeof(CutInImageRecorder))]
    public class ImageRecorderEditor : Editor
    {
        private float recordSeconds;

        void InitializeValiables()
        {
            var recorder = (CutInImageRecorder) target;

            if (string.IsNullOrWhiteSpace(recorder.outputPath))
            {
                switch (recorder.outputType)
                {
                    case CutInImageRecorder.OutputType.png:
                        recorder.outputPath = Path.Combine(Application.dataPath, "output.png");
                        break;
                    case CutInImageRecorder.OutputType.gif:
                        recorder.outputPath = Path.Combine(Application.dataPath, "output.gif");
                        break;
                    case CutInImageRecorder.OutputType.webp:
                        recorder.outputPath = Path.Combine(Application.dataPath, "output.webp");
                        break;
                }
            }
        }
        
        public override void OnInspectorGUI()
        {
            var recorder = (CutInImageRecorder) target;
            InitializeValiables();
            // base.OnInspectorGUI();

            recorder.resolution = EditorGUILayout.Vector2IntField(new GUIContent("Resolution","解像度。\nccfoliaで使用する場合、\n40:30なら960*720となります。"), recorder.resolution);
            recorder.loop = EditorGUILayout.Toggle(new GUIContent("Loop","繰り返し再生するかどうか"), recorder.loop);

            var fps = EditorGUILayout.IntSlider("FPS",recorder.fps, 1, 60);
            if (fps != recorder.fps)
            {
                recorder.fps = fps;
                recorder.recordFrames = (int)(recordSeconds * (float)recorder.fps);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                var sec = EditorGUILayout.FloatField(new GUIContent("RecordSecond / Frame", "録画時間 / 録画の総フレーム"), recordSeconds);
                if (sec <= 0f)
                {
                    sec = (float) recorder.recordFrames / (float) recorder.fps;
                }
                if (Math.Abs(sec - recordSeconds) > 0.01f)
                {
                    recordSeconds = sec;
                    recorder.recordFrames = (int)(sec * (float)recorder.fps);
                }
                var fra = EditorGUILayout.IntField(GUIContent.none, recorder.recordFrames, GUILayout.Width(100));
                if (fra <= 0)
                {
                    fra = (int)(recordSeconds * (float)recorder.fps);
                }
                if (fra != recorder.recordFrames)
                {
                    recordSeconds = (float) fra / (float) recorder.fps;
                    recorder.recordFrames = fra;
                }
                
            }


            using (new EditorGUILayout.HorizontalScope())
            {
                recorder.outputPath = EditorGUILayout.TextField(new GUIContent("Output Path","出力形式はwebpがおすすめです。"), recorder.outputPath);
                
                var ot = (CutInImageRecorder.OutputType) EditorGUILayout.EnumPopup(GUIContent.none, recorder.outputType,GUILayout.Width(40));
                if (ot != recorder.outputType)
                {
                    switch (ot)
                    {
                        case CutInImageRecorder.OutputType.png:
                            recorder.outputPath = Path.ChangeExtension(recorder.outputPath, ".png");
                            break;
                        case CutInImageRecorder.OutputType.gif:
                            recorder.outputPath = Path.ChangeExtension(recorder.outputPath, ".gif");
                            break;
                        case CutInImageRecorder.OutputType.webp:
                            recorder.outputPath = Path.ChangeExtension(recorder.outputPath, ".webp");
                            break;
                    }
                    recorder.outputType = ot;
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(new GUIContent("Open Folder", "エクスプローラーでフォルダを開きます。")))
                {
                    var fileDir = Path.GetDirectoryName(recorder.outputPath) ?? "";
                    Process.Start(fileDir); 
                }
                if (GUILayout.Button(new GUIContent("Select Output Path", "画像の出力先を選択します。")))
                {
                    switch (recorder.outputType)
                    {
                        case CutInImageRecorder.OutputType.png:
                            recorder.outputPath = EditorUtility.SaveFilePanel("Export Path", "Assets", "output", "png");
                            break;
                        case CutInImageRecorder.OutputType.gif:
                            recorder.outputPath = EditorUtility.SaveFilePanel("Export Path", "Assets", "output", "gif");
                            break;
                        case CutInImageRecorder.OutputType.webp:
                            recorder.outputPath =
                                EditorUtility.SaveFilePanel("Export Path", "Assets", "output", "webp");
                            break;
                    }
                }
            }

            GUILayout.Space(20);
            using (new EditorGUILayout.HorizontalScope())
            {
                recorder.playAndRun = EditorGUILayout.Toggle(new GUIContent("Play To Record / Delay","シーンの再生後自動で録画を開始します。"), recorder.playAndRun);
                recorder.selfTimer = EditorGUILayout.Slider(GUIContent.none, recorder.selfTimer, 0f, 5f);
            }

#if EditorCoroutines
            if (GUILayout.Button("Record" + recorder.timerDisplay))
            {
                recorder.runInEditMode = true;
                recorder.CancellRecord();
                if (EditorApplication.isPlaying)
                {
                    recorder.StartCoroutine(recorder.StartTimer());
                    recorder.StartCoroutine(RepaintGUI());
                }
                else
                {
                        EditorCoroutineUtility.StartCoroutine(recorder.StartTimer(), target);
                        EditorCoroutineUtility.StartCoroutine(RepaintGUI(), target);
                }
                recorder.StartCoroutine(RepaintGUI());
            }
#else
            using (new EditorGUI.DisabledGroupScope(!EditorApplication.isPlaying || !string.IsNullOrWhiteSpace(recorder.timerDisplay)))
            {
                if (GUILayout.Button("Record" + recorder.timerDisplay))
                {
                    recorder.runInEditMode = true;
                    recorder.CancellRecord();
                    recorder.StartCoroutine(recorder.StartTimer());
                    recorder.StartCoroutine(RepaintGUI());
                }
            }
#endif
        }

        IEnumerator RepaintGUI()
        {
            var recorder = (CutInImageRecorder) target;
            recorder.timerDisplay = " (Initialize...)";
            while (!string.IsNullOrWhiteSpace(recorder.timerDisplay))
            {
                Repaint();
                yield return null;
            }
            yield return null;
        }
    }
}