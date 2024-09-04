using System;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using JetBrains.Annotations;

namespace Irisu.Utilities
{
    [PublicAPI]
    public static class EditorShortcuts
    {
        [MenuItem("Utils/Init full compilation")]
        private static void InitFullCompilation()
        {
            CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
        }

        [MenuItem("Utils/Init compilation")]
        private static void InitCompilation()
        {
            CompilationPipeline.RequestScriptCompilation();
        }
        
        [MenuItem("Utils/Capture screenshot")]
        public static void CreateScreenshot()
        {
            ScreenCapture.CaptureScreenshot($"Screenshot{DateTime.UtcNow.Second}.png");
        }
    }
}