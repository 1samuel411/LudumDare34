using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ObjectPoolManager {
    /****************UNDER CONSTRUCTION*******************
     * You can ignore this for now
     * You can ignore this for now
     * You can ignore this for now
     * You can ignore this for now
     * You can ignore this for now
     * */
    public class PoolEditorWindow : EditorWindow
    {
        private string mystring = "Hello World";
        private bool groupEnabled;
        private bool myBool = true;
        private float myFloat = 1.23f;
        public GameObject obj;

        [MenuItem("Window/Pool Editor Window")]
        static void Init()
        {
            PoolEditorWindow window = (PoolEditorWindow) EditorWindow.GetWindow(typeof (PoolEditorWindow));
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            mystring = EditorGUILayout.TextField("Text Field", mystring);

            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle("Toggle", myBool);
            myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            obj = EditorGUILayout.ObjectField(obj, typeof(GameObject), false, null) as GameObject;
            EditorGUILayout.EndToggleGroup();
        }
    }
}
