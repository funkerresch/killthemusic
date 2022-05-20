using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RenderLayer))]
public class RenderLayerEditor : Editor
{
    string[] layerTypes = System.Enum.GetNames(typeof(LayerType));

    public override void OnInspectorGUI()
    {
        var layer = target as RenderLayer;
        var parameters = layer.parameters;

        serializedObject.Update();


        if (GUILayout.Button("Destroy & Generate"))
        {   
             layer.Generate();
        }

        if (GUILayout.Button("Destroy"))
        {
            layer.DestroyMaze();
        }

        SerializedProperty _layerType = serializedObject.FindProperty("layerType");
        _layerType.intValue = EditorGUILayout.Popup("Layer Type", _layerType.intValue, layerTypes);

        SerializedProperty _dimX = serializedObject.FindProperty("dimensionX");
        _dimX.intValue = (int)EditorGUILayout.Slider("Dimension X-Axis", _dimX.intValue, 5, 99);

        SerializedProperty _dimZ = serializedObject.FindProperty("dimensionZ");
        _dimZ.intValue = (int)EditorGUILayout.Slider("Dimension Z-Axis", _dimZ.intValue, 5, 99);

        SerializedProperty _scaling = serializedObject.FindProperty("scale");
        _scaling.floatValue = EditorGUILayout.Slider("Scaling", _scaling.floatValue, 0.5f, 10f);

        SerializedProperty _baseWallHeight = serializedObject.FindProperty("groundWallHeight");
        _baseWallHeight.floatValue = EditorGUILayout.Slider("Base Wall Height", _baseWallHeight.floatValue, 0.5f, 10f);

        SerializedProperty _wallHeight = serializedObject.FindProperty("wallHeight");
        _wallHeight.floatValue = EditorGUILayout.Slider("Wall Height", _wallHeight.floatValue, 0.5f, 10f);

        SerializedProperty _showPath = serializedObject.FindProperty("showPath");
        _showPath.boolValue = EditorGUILayout.Toggle("ShowPath", _showPath.boolValue);

        layer.UpdateScaling();
        layer.UpdateGroundWallHeight();
        layer.UpdateWallHeight();

        if (layer.showPath)
            layer.ShowPath();
        else
            layer.HidePath();


        serializedObject.ApplyModifiedProperties();
        



    }

}
