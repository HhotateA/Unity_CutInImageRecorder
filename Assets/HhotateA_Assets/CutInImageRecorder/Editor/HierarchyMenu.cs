using UnityEditor;
using UnityEngine;

namespace HhotateA.ImageRecorder
{
    public class HierarchyMenu
    {
        [MenuItem("GameObject/Video/ImageRecorder", false, 0)]
        public static void ImageRecorder()
        {
            var imageRecorder = new GameObject();
            imageRecorder.name = "ImageRecorder";
            var camera = imageRecorder.AddComponent<Camera>();
            var recorder = imageRecorder.AddComponent<CutInImageRecorder>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.clear;
            camera.depth = -5;
        }
    }
}