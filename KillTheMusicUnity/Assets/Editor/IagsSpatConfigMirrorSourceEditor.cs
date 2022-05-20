//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(IagsSpatConfigMirrorSource)), CanEditMultipleObjects]
//public class IagsSpatConfigMirrorSourceEditor : Editor
//{
//    string[] hrtfChoice = new[] { "Fabian", "Neumann", "Custom" };

//    public override void OnInspectorGUI()
//    {
//        var configMirrorSource = target as IagsSpatConfigMirrorSource;
//        serializedObject.Update();

//        EditorGUILayout.LabelField("HRTF Settings", EditorStyles.boldLabel);

//        SerializedProperty serializedhrtfForDirectSoundIndex = serializedObject.FindProperty("hrtfForDirectSoundIndex");
//        serializedhrtfForDirectSoundIndex.intValue = EditorGUILayout.Popup("HRTF for direct sound", serializedhrtfForDirectSoundIndex.intValue, hrtfChoice);

//        if (serializedhrtfForDirectSoundIndex.intValue == (int)IagsHrtf.Custom)
//        {
//            SerializedProperty serializedHrtfForDirectSoundName = serializedObject.FindProperty("IrSet");
//            serializedHrtfForDirectSoundName.stringValue = EditorGUILayout.TextField("Name", serializedHrtfForDirectSoundName.stringValue);
//        }

//        SerializedProperty _hrtfForEarlyReflectionsIndex = serializedObject.FindProperty("hrtfForEarlyReflectionsIndex");
//        _hrtfForEarlyReflectionsIndex.intValue = EditorGUILayout.Popup("HRTF for early reflections", _hrtfForEarlyReflectionsIndex.intValue, hrtfChoice);

//        if (_hrtfForEarlyReflectionsIndex.intValue == (int)IagsHrtf.Custom)
//        {
//            SerializedProperty _hrtfForEarlyReflectionsName = serializedObject.FindProperty("hrtfEarlyReflectionsName");
//            _hrtfForEarlyReflectionsName.stringValue = EditorGUILayout.TextField("Name", _hrtfForEarlyReflectionsName.stringValue);
//        }

//        SerializedProperty _binauralReflectionCount = serializedObject.FindProperty("binauralReflectionCount");
//        _binauralReflectionCount.intValue = EditorGUILayout.IntSlider("Binaural Reflection Count", _binauralReflectionCount.intValue, 0, 1000);

//        /*---------------------------------------------------------------------------------------------------------------------------------------------*/

//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Mixing Settings", EditorStyles.boldLabel);

//        SerializedProperty _mute = serializedObject.FindProperty("mute");
//        _mute.boolValue = EditorGUILayout.Toggle("Mute", _mute.boolValue);

//        SerializedProperty _muteThreshold = serializedObject.FindProperty("muteThreshold");
//        _muteThreshold.floatValue = EditorGUILayout.Slider("Auto-Mute Threshold", _muteThreshold.floatValue, -80.0f, 0.0f);

//        SerializedProperty _scalingDirect = serializedObject.FindProperty("scalingDirect");
//        _scalingDirect.floatValue = EditorGUILayout.Slider("Scaling direct sound", _scalingDirect.floatValue, 0, 1f);

//        SerializedProperty _scalingEarly = serializedObject.FindProperty("scalingEarly");
//        _scalingEarly.floatValue = EditorGUILayout.Slider("Scaling early reflections", _scalingEarly.floatValue, 0, 1f);

//        SerializedProperty _scalingLate = serializedObject.FindProperty("scalingLate");
//        _scalingLate.floatValue = EditorGUILayout.Slider("Scaling late reverb", _scalingLate.floatValue, 0, 1f);

//        /*---------------------------------------------------------------------------------------------------------------------------------------------*/

//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Orientation Settings", EditorStyles.boldLabel);

//        SerializedProperty _inverseAzimuth = serializedObject.FindProperty("inverseAzimuth");
//        _inverseAzimuth.boolValue = EditorGUILayout.Toggle("Inverse Azimuth", _inverseAzimuth.boolValue);
//        SerializedProperty _inverseElevation = serializedObject.FindProperty("inverseElevation");
//        _inverseElevation.boolValue = EditorGUILayout.Toggle("Inverse Elevation", _inverseElevation.boolValue);
//        SerializedProperty _listenerOrientationOnly = serializedObject.FindProperty("listenerOrientationOnly");
//        _listenerOrientationOnly.boolValue = EditorGUILayout.Toggle("Listener orientation only", _listenerOrientationOnly.boolValue);

//        /*---------------------------------------------------------------------------------------------------------------------------------------------*/

//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Late Reverb (To be depricated)", EditorStyles.boldLabel);

//        SerializedProperty _reverbClip = serializedObject.FindProperty("ReverbClip");
//        EditorGUILayout.PropertyField(_reverbClip);

//        SerializedProperty _reverbGroup = serializedObject.FindProperty("reverbGroup");
//        _reverbGroup.intValue = EditorGUILayout.IntSlider("Reverb Group", _reverbGroup.intValue, -1, 63);

//        /*---------------------------------------------------------------------------------------------------------------------------------------------*/

//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Environment and Distance Settings", EditorStyles.boldLabel);

//        SerializedProperty _temperature = serializedObject.FindProperty("Temperature");
//        _temperature.floatValue = EditorGUILayout.Slider("Temperature", _temperature.floatValue, -50f, 50f);
//        var tooltipTemp = GUILayoutUtility.GetLastRect();
//        GUI.Label(tooltipTemp, new GUIContent("", "Temperature [°C]. Only has an effect if Distance Damping is enabled."));

//        SerializedProperty _humidity = serializedObject.FindProperty("Humidity");
//        _humidity.floatValue = EditorGUILayout.Slider("Humidity", _humidity.floatValue, 0f, 100f);
//        var tooltipHum = GUILayoutUtility.GetLastRect();
//        GUI.Label(tooltipHum, new GUIContent("", "Relative humidity [%]. Only has an effect if Distance Damping is enabled."));

//        SerializedProperty _pressure = serializedObject.FindProperty("Pressure");
//        _pressure.floatValue = EditorGUILayout.Slider("Pressure", _pressure.floatValue, 0f, 2f);
//        var tooltipPressure = GUILayoutUtility.GetLastRect();
//        GUI.Label(tooltipPressure, new GUIContent("", "Normalized pressure [pressure/1atm]. 1.0 is standard sea level pressure (101,325 Pa). Only has an effect if Distance Damping is enabled."));

//        SerializedProperty _distanceAttenuation = serializedObject.FindProperty("DistanceAttenuation");
//        _distanceAttenuation.boolValue = EditorGUILayout.Toggle("Distance Attenuation", _distanceAttenuation.boolValue);
//        var tooltipDistAtt = GUILayoutUtility.GetLastRect();
//        GUI.Label(tooltipDistAtt, new GUIContent("", "If enabled, Unity's Rolloff curve is bypassed and physically accurate attenuation is calculated instead."));

//        SerializedProperty _distanceDamping = serializedObject.FindProperty("DistanceDamping");
//        _distanceDamping.boolValue = EditorGUILayout.Toggle("Distance Damping", _distanceDamping.boolValue);
//        var tooltipDistDamping = GUILayoutUtility.GetLastRect();
//        GUI.Label(tooltipDistDamping, new GUIContent("", "If enabled, physically accurate damping of high frequencies is applied."));

//        SerializedProperty _distanceAttenuationScaling = serializedObject.FindProperty("DistanceAttenuationScaling");
//        _distanceAttenuationScaling.floatValue = EditorGUILayout.Slider("Distance Attenuation Scaling", _distanceAttenuationScaling.floatValue, 0.1f, 10f);

//        /*---------------------------------------------------------------------------------------------------------------------------------------------*/

//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Reflection Settings", EditorStyles.boldLabel);

//        SerializedProperty _wallTag = serializedObject.FindProperty("wallTag");
//        _wallTag.stringValue = EditorGUILayout.TextField("Wall Tag", _wallTag.stringValue);
//        var tooltipWallTag = GUILayoutUtility.GetLastRect();
//        GUI.Label(tooltipWallTag, new GUIContent("", "Apply a Tag with the same name to every wall, that should generate mirror sources."));

//        SerializedProperty _materialAbsorption = serializedObject.FindProperty("MaterialAbsorption");
//        _materialAbsorption.boolValue = EditorGUILayout.Toggle("Material Absorption", _materialAbsorption.boolValue);
//        var tooltipMaterialAbsorption = GUILayoutUtility.GetLastRect();
//        GUI.Label(tooltipMaterialAbsorption, new GUIContent("", "If enabled, individual material absorption is applied for each reflection, otherwise one filter is applied for all " +
//        "early reflections."));

//        SerializedProperty _visualizeMirrorSource = serializedObject.FindProperty("visualizeMirrorSources");
//        _visualizeMirrorSource.boolValue = EditorGUILayout.Toggle("Visualize mirror sources", _visualizeMirrorSource.boolValue);
//        var tooltipVisualizeMirrorSources = GUILayoutUtility.GetLastRect();
//        GUI.Label(tooltipVisualizeMirrorSources, new GUIContent("", "Visualizes the reflection positions, should be disabled for a build."));

//        SerializedProperty _reflectionOrder = serializedObject.FindProperty("reflectionOrder");
//        _reflectionOrder.intValue = EditorGUILayout.IntSlider("Reflection Order", _reflectionOrder.intValue, 0, 10);

//        SerializedProperty _earlyRef2DirectRatioMaxDist = serializedObject.FindProperty("earlyRef2DirectRatioMaxDist");
//        _earlyRef2DirectRatioMaxDist.floatValue = EditorGUILayout.Slider("Max Dist Ratio", _earlyRef2DirectRatioMaxDist.floatValue, 1, 2000);

//        SerializedProperty _earlyRef2DirectRatio = serializedObject.FindProperty("earlyRef2DirectRatio");
//        _earlyRef2DirectRatio.animationCurveValue = EditorGUILayout.CurveField("Early Ref2Direct Ratio", _earlyRef2DirectRatio.animationCurveValue);

//        if (GUILayout.Button("Reset Ratio Curve"))
//            configMirrorSource.ResetReflectionRatioAnimationCurve();

//        SerializedProperty _reflectionColor = serializedObject.FindProperty("reflectionColor");
//        _reflectionColor.colorValue = EditorGUILayout.ColorField("Reflection Color", _reflectionColor.colorValue);

//        if (GUILayout.Button("Recalulate Mirror Sources"))
//        {
//            if(Application.isPlaying)
//                configMirrorSource.CalculateImageSources();
//        }

//        serializedObject.ApplyModifiedProperties();

//        if (configMirrorSource.DistanceAttenuation != configMirrorSource._distanceAttenuation)
//            configMirrorSource.UpdateRolloffCurve();

//        if(configMirrorSource.mute != configMirrorSource._mute)
//            configMirrorSource.MuteSpat(configMirrorSource.mute);

//        if (configMirrorSource.listenerOrientationOnly != configMirrorSource._listenerOrientationOnly)
//            configMirrorSource.ListenerOrientationOnly(configMirrorSource.listenerOrientationOnly);

//        if (configMirrorSource.inverseAzimuth != configMirrorSource._inverseAzimuth)
//            configMirrorSource.InverseAzimuth(configMirrorSource.inverseAzimuth);

//        if (configMirrorSource.inverseElevation != configMirrorSource._inverseElevation)
//            configMirrorSource.InverseElevation(configMirrorSource.inverseElevation);

//        if (configMirrorSource.earlyRef2DirectRatioMaxDist != configMirrorSource._earlyRef2DirectRatioMaxDist)
//        {
//            configMirrorSource.ResetReflectionRatioAnimationCurve();
//            configMirrorSource._earlyRef2DirectRatioMaxDist = configMirrorSource.earlyRef2DirectRatioMaxDist;
//        }

//        //DrawDefaultInspector();
//    }

//}
