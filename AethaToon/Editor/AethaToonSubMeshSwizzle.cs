using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class AethaToonSubMeshSwizzle : EditorWindow
{
    [MenuItem("Window/AethaToon SubMesh Swizzle")]
    public static void SwizzleMeshWindow()
    {
        AethaToonSubMeshSwizzle wnd = GetWindow<AethaToonSubMeshSwizzle>();
        wnd.titleContent = new GUIContent("SubMesh Swizzle");
    }

    private static bool _alsoMoveMaterials = true;

    private void OnGUI()
    {
        if (Selection.gameObjects.Length != 1)
        {
            EditorGUILayout.LabelField("Select a single gameobject to continue");
            return;
        }

        Mesh targetMesh = null;
        MeshFilter mf = Selection.gameObjects[0].GetComponent<MeshFilter>();
        MeshRenderer mr = Selection.gameObjects[0].GetComponent<MeshRenderer>();
        SkinnedMeshRenderer smr = Selection.gameObjects[0].GetComponent<SkinnedMeshRenderer>();
        Material[] sharedMaterials = Array.Empty<Material>();
        if (mf)
        {
            targetMesh = mf.sharedMesh;
            if (mr)
            {
                sharedMaterials = mr.sharedMaterials;
            }
        }
        if (!targetMesh && smr)
        {
            targetMesh = smr.sharedMesh;
            sharedMaterials = smr.sharedMaterials;
        }
        if (!targetMesh)
        {
            EditorGUILayout.LabelField("No MeshFilter of SkinnedMeshRenderer component on the selected gameobject");
            return;
        }
        EditorGUILayout.LabelField($"{targetMesh.name} has {targetMesh.subMeshCount} submeshes and {sharedMaterials.Length} materials");
        _alsoMoveMaterials = EditorGUILayout.Toggle("Swap materials?", _alsoMoveMaterials);

        List<SubMeshDescriptor> submeshes = new List<SubMeshDescriptor>();
        for (int i = 0; i < targetMesh.subMeshCount; i++)
        {
            submeshes.Add(targetMesh.GetSubMesh(i));
        }
        submeshes = submeshes.OrderBy(x => x.vertexCount + x.firstVertex * -133).ToList();
        
        for (int i = 0; i < targetMesh.subMeshCount; i++)
        {
            var subMesh = targetMesh.GetSubMesh(i);
            float h = ((float)submeshes.IndexOf(subMesh) / submeshes.Count);
            Color c = GUI.backgroundColor;
            GUI.backgroundColor = Color.HSVToRGB(h, 0.5f, 1f);
            GUILayout.BeginHorizontal("Box");
            bool canGoLeft = i > 0;
            if (GUILayout.Button("UP", GUILayout.Width(50)) && canGoLeft)
            {
                SwapSubmesh(targetMesh, i, i-1);
                if (_alsoMoveMaterials && sharedMaterials.Length > i-1)
                {
                    if (mr)
                    {
                        SwapMaterial(mr, i, i-1);
                    }
                    else if (smr)
                    {
                        SwapMaterial(smr, i, i-1);
                    }
                }
            }
            string label = $"{targetMesh.GetSubMesh(i).vertexCount} vertices";
            if (sharedMaterials.Length > i && sharedMaterials[i])
            {
                label += ", "+sharedMaterials[i].name;
            }
            GUILayout.Label(label);
            bool canGoRight = i != targetMesh.subMeshCount-1;
            if (GUILayout.Button("DOWN", GUILayout.Width(50)) && canGoRight)
            {
                SwapSubmesh(targetMesh, i, i+1);
                if (_alsoMoveMaterials && sharedMaterials.Length > i+1)
                {
                    if (mr)
                    {
                        SwapMaterial(mr, i, i+1);
                    }
                    else if (smr)
                    {
                        SwapMaterial(smr, i, i+1);
                    }
                }
            }
            GUI.backgroundColor = c;
            GUILayout.EndHorizontal();
        }

        for (int i = submeshes.Count; i < sharedMaterials.Length; i++)
        {
            GUILayout.BeginHorizontal("Box");
            string label = $"{sharedMaterials[i].name}";
            GUILayout.Label(label);
            GUILayout.EndHorizontal();
        }
        GUILayout.Space(15);
    }

    static void SwapMaterial(SkinnedMeshRenderer target, int i1, int i2)
    {
        var mats = target.sharedMaterials;
        var mat1 = mats[i1];
        var mat2 = mats[i2];
        mats[i1] = mat2;
        mats[i2] = mat1;
        target.sharedMaterials = mats;
    }
    
    static void SwapMaterial(MeshRenderer target, int i1, int i2)
    {
        var mats = target.sharedMaterials.ToArray();
        var mat1 = mats[i1];
        var mat2 = mats[i2];
        mats[i1] = mat2;
        mats[i2] = mat1;
        target.sharedMaterials = mats;
    }

    static void SwapSubmesh(Mesh target, int i1, int i2)
    {
        Undo.RecordObject(target, $"Swapped submesh of {target.name}");
        var descriptor1 = target.GetSubMesh(i1);
        var descriptor2 = target.GetSubMesh(i2);
        target.SetSubMesh(i1, descriptor2);
        target.SetSubMesh(i2, descriptor1);
        target.UploadMeshData(false);
    }
}