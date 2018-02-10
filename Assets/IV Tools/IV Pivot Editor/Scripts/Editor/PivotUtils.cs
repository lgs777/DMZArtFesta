using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace IVSoft
{
    public static class PivotUtils
    {
        public static string RemoveExtension(this string path)
        {
            int removeCount = 0;

            int startingIndex = -1;

            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] != '.')
                    removeCount++;
                else
                {
                    startingIndex = i;
                    break;
                }
            }

            return path.Remove(startingIndex, removeCount + 1);
        }

        public static bool IsAPrefab(this GameObject gameObject)
        {
            return PrefabUtility.GetPrefabParent(gameObject) == null && PrefabUtility.GetPrefabObject(gameObject) != null;
        }

        public static bool Freeze(this Transform transform, Vector3 position, Quaternion rotation, Vector3 scale,
            bool saveToFBX)
        {
            MeshFilter filter = transform.GetComponent<MeshFilter>();

            Mesh mesh = filter != null ? filter.sharedMesh : null;


            Collider collider = transform.GetComponent<Collider>();

            PropertyInfo colliderCenterProperty = collider.GetType().GetProperty("center");

            Vector3 initialColliderCenter = Vector3.zero;

            Vector3 initialPos = Vector3.zero;

            if (colliderCenterProperty != null)
            {
                initialColliderCenter = (Vector3)colliderCenterProperty.GetValue(collider, null);
                initialPos = transform.position;
            }

            Transform[] children = new Transform[transform.childCount];

            // remove children so they don't move along
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = transform.GetChild(i);
            }

            // remove children so they don't move along
            for (int i = 0; i < children.Length; i++)
            {
                children[i].parent = null;
            }

            Vector3[] worldVertices = null;

            if (mesh)
            {
                worldVertices = mesh.vertices.Select(v => transform.TransformPoint(v)).ToArray();

                string path = AssetDatabase.GetAssetPath(mesh);

                if (!path.EndsWith(".asset") && !string.IsNullOrEmpty(path))
                {
                    bool cloned = filter.CloneAndSaveMesh(saveToFBX);

                    mesh = filter.sharedMesh;

                    if (!cloned)
                        return false;
                }
            }

            transform.position = position;

            transform.rotation = rotation;

            transform.localScale = scale;

            if (mesh)
            {
                Vector3[] vertices = worldVertices.Select(v => transform.InverseTransformPoint(v)).ToArray();

                mesh.vertices = vertices;

                mesh.RecalculateNormals();

                mesh.RecalculateBounds();
            }

            if (collider)
            {
                if (collider is MeshCollider)
                {
                    MeshCollider m = collider as MeshCollider;
                    m.sharedMesh = mesh;
                    m.convex = !m.convex;
                    m.convex = !m.convex;
                }

                if (colliderCenterProperty != null)
                {
                    colliderCenterProperty.SetValue(collider,
                        initialColliderCenter + (initialPos - transform.position), null);
                }
            }

            // re add children
            for (int i = 0; i < children.Length; i++)
            {
                children[i].parent = transform;
            }


            return true;
        }

        public static bool FreezeMesh(this MeshFilter filter, Vector3 position, Quaternion rotation, Vector3 scale,
            bool saveToFBX)
        {
            Mesh mesh = filter.sharedMesh;

            Transform transform = filter.transform;

            Collider collider = filter.GetComponent<Collider>();

            PropertyInfo colliderCenterProperty = collider.GetType().GetProperty("center");

            Vector3 initialColliderCenter = Vector3.zero;

            Vector3 initialPos = Vector3.zero;

            if (colliderCenterProperty != null)
            {
                initialColliderCenter = (Vector3)colliderCenterProperty.GetValue(collider, null);
                initialPos = transform.position;
            }

            Vector3[] worldVertices = mesh.vertices.Select(v => transform.TransformPoint(v)).ToArray();

            Transform[] children = new Transform[transform.childCount];

            // remove children so they don't move along
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = transform.GetChild(i);
            }

            // remove children so they don't move along
            for (int i = 0; i < children.Length; i++)
            {
                children[i].parent = null;
            }

            string path = AssetDatabase.GetAssetPath(mesh);

            if (!path.EndsWith(".asset") && !string.IsNullOrEmpty(path))
            {
                bool cloned = filter.CloneAndSaveMesh(saveToFBX);

                if (!cloned)
                    return false;
            }

            transform.position = position;

            transform.rotation = rotation;

            transform.localScale = scale;

            Vector3[] vertices = worldVertices.Select(v => transform.InverseTransformPoint(v)).ToArray();

            mesh.vertices = vertices;

            mesh.RecalculateBounds();

            if (collider)
            {
                if (collider is MeshCollider)
                {
                    MeshCollider m = collider as MeshCollider;
                    m.sharedMesh = mesh;
                    m.convex = !m.convex;
                    m.convex = !m.convex;
                }

                if (colliderCenterProperty != null)
                {
                    colliderCenterProperty.SetValue(collider,
                        initialColliderCenter + (initialPos - transform.position), null);
                }
            }

            // re add children
            for (int i = 0; i < children.Length; i++)
            {
                children[i].parent = transform;
            }


            return true;
        }


        public static bool CloneAndSaveMesh(this MeshFilter meshFilter, bool saveMeshToFbxFolder)
        {
            string path = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);

            if (!saveMeshToFbxFolder || path.StartsWith("Library"))
            {
                path = EditorUtility.SaveFilePanelInProject("Save Cloned Mesh", meshFilter.sharedMesh.name, "asset", "");
            }
            else
            {
                path = path.RemoveExtension() + ".asset";
            }

            if (string.IsNullOrEmpty(path))
                return false;

            // clone mesh
            Mesh cloneMesh = GameObject.Instantiate(meshFilter.sharedMesh);

            // save mesh asset
            AssetDatabase.CreateAsset(cloneMesh, AssetDatabase.GenerateUniqueAssetPath(path));

            meshFilter.sharedMesh = cloneMesh;

            MeshCollider meshColl = meshFilter.GetComponent<MeshCollider>();

            if (meshColl)
            {
                meshColl.sharedMesh = cloneMesh;
            }

            return true;
        }
    }


}