using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(UGUIOscilloscope))]
public class UGUIOscilloscopeEditor : RawImageEditor {
	private SerializedProperty propertyStyle;
	private SerializedProperty propertyFitRectTransform;
	private SerializedProperty propertyCenterWidth;

	protected override void OnEnable ()
	{
		base.OnEnable ();
		propertyStyle = serializedObject.FindProperty ("style");
		propertyFitRectTransform = serializedObject.FindProperty ("fitRectTransform");
		propertyCenterWidth = serializedObject.FindProperty ("centerWidth");
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		serializedObject.Update ();
		EditorGUILayout.PropertyField (propertyStyle);
		EditorGUILayout.PropertyField (propertyFitRectTransform);
		EditorGUILayout.PropertyField (propertyCenterWidth);
		serializedObject.ApplyModifiedProperties ();
	}
}