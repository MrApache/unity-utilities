using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Irisu.Utilities
{
    [InitializeOnLoad]
    public sealed class CompilationCounter : IPostprocessBuildWithReport
    {
        private static readonly CounterData _counter;
        public int callbackOrder => 0;

        static CompilationCounter()
        {
            _counter = CounterData.instance;
            _counter.Compilations++;
            if(!_counter.DisableDebugMessages)
                Debug.Log($"Compilation: {_counter.Compilations}");
        }

        [PostProcessBuild(1)]
        private static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            _counter.Compilations++;
            if(!_counter.DisableDebugMessages)
                Debug.Log($"Build: {_counter.Builds}");
        }

        public void OnPostprocessBuild(BuildReport report)
        { }
        
#if UNITY_EDITOR_WINDOWS
        [InitializeOnLoad]
        private static class EditorTitle
        {
            private static readonly System.IntPtr _windowPtr;

            static EditorTitle()
            {
                _windowPtr = GetActiveWindow();
                UpdateWindowText();
                EditorApplication.update += UpdateWindowText;
                UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (_, _) => UpdateWindowText();
            }

            [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowText")]
            private static extern bool SetWindowText(System.IntPtr ptr, string lpString);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern System.IntPtr GetActiveWindow();
            
            [MenuItem("Utils/Refresh editor title")]
            private static void UpdateWindowText()
            {
                string text = Application.productName + $" [v0.0:{_counter.Builds}.{_counter.Compilations}]";
                SetWindowText(_windowPtr, text);
            }
        }
#endif
    }
}

