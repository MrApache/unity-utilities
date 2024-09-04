using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Irisu.Utilities
{
    public sealed class MeshCombiner : MonoBehaviour
    {
        [SerializeField] private List<MeshFilter> _meshFilters;
        [SerializeField] private MeshFilter _targetMesh;

        [ContextMenu("Combine Meshes")]
        private void CombineMesh()
        {
            CombineInstance[] combine = new CombineInstance[_meshFilters.Count];
            for (int i = 0; i < _meshFilters.Count; i++)
            {
                combine[i].mesh = _meshFilters[i].sharedMesh;
                combine[i].transform = _meshFilters[i].transform.localToWorldMatrix;
            }
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combine);
            _targetMesh.mesh = mesh;
            SaveMesh(_targetMesh.sharedMesh, gameObject.name, false, true);
            print("<color=#20E7B0>Combine Meshes was Successful!</color>");
        }

        private static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
        {
            string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path);
            Mesh meshToSave = makeNewInstance ? Instantiate(mesh) : mesh;
            if (optimizeMesh)
                MeshUtility.Optimize(meshToSave);
            AssetDatabase.CreateAsset(meshToSave, path);
            AssetDatabase.SaveAssets();
        }        
    }
}