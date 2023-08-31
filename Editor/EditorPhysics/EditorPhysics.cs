using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chimp.Editor.EditorPhysics
{
    internal class EditorPhysics : EditorWindow
    {
        /// <summary>
        /// Editor Properties
        /// </summary>
        private int _Tickrate = 33;
        private float _Speed = 1f;
        private Vector3 _Gravity = Physics.gravity;

        private bool _IsSimulating;
        private PhysicsState _PhysicsState;

        private readonly List<HoldRigidbody> _FrozeSceneRigidbodies = new(2048);
        private readonly List<PlayingRigidbody> _SimulatedObjects = new(64);

        [MenuItem("Chimp/Editor/Editor Physics")]
        private static void CreateInstance()
        {
            var window = CreateWindow<EditorPhysics>();
            window.titleContent = new GUIContent("Editor Physics");
            window.minSize = new Vector2(400, 150);
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private void OnDestroy()
        {
            StopSimulation();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            {
                _Tickrate = (int)EditorGUILayout.Slider("Tickrate", _Tickrate, 8, 64);
                _Speed = EditorGUILayout.Slider("Speed", _Speed, 0.001f, 2f);
                GUILayout.Space(8);
                _Gravity = EditorGUILayout.Vector3Field("Gravity", _Gravity);
            }
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.green;

            string buttonText = _IsSimulating ? "Stop Simulate" : "Simulate";
            if (GUILayout.Button(buttonText, GUILayout.MinHeight(50)))
            {
                ToggleSimulation();
            }
        }

        private void StartSimulation()
        {
            if (_IsSimulating) { return; }

            _IsSimulating = true;
            _SimulatedObjects.Clear();

            // Register selected objects
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                _SimulatedObjects.Add(new PlayingRigidbody(gameObject));
            }

            // Selected objects to be undoable
            foreach (PlayingRigidbody rb in _SimulatedObjects)
            {
                Undo.RegisterFullObjectHierarchyUndo(rb.ActualGameObject, "Editor Physics Run");
            }

            // Save current physic state
            _PhysicsState = new PhysicsState
            {
                Gravity = Physics.gravity,
                SimulationMode = Physics.simulationMode
            };

            Physics.simulationMode = SimulationMode.Script;
            Physics.gravity = _Gravity;

            Undo.undoRedoPerformed += OnUndoPerformed;

            _FrozeSceneRigidbodies.Clear();

            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject root in rootObjects)
            {
                Rigidbody[] objs = root.GetComponentsInChildren<Rigidbody>(false);
                foreach (Rigidbody obj in objs)
                {
                    if (!obj.gameObject.activeInHierarchy) { continue; }

                    // If object is not the object we want to simulate
                    bool found = false;

                    foreach (PlayingRigidbody rb in _SimulatedObjects)
                    {
                        if (rb.ActualGameObject == obj.gameObject)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        _FrozeSceneRigidbodies.Add(new HoldRigidbody(obj));
                    }
                }
            }

            _FrozeSceneRigidbodies.ForEach(rb => rb.Freeze());
            _SimulatedObjects.ForEach(rb => rb.Start());
        }

        private void StopSimulation()
        {
            if (!_IsSimulating)
            {
                return;
            }

            _IsSimulating = false;

            Physics.simulationMode = _PhysicsState.SimulationMode;
            Physics.gravity = _PhysicsState.Gravity;

            Undo.undoRedoPerformed -= OnUndoPerformed;

            _FrozeSceneRigidbodies.ForEach(rb => rb.Revert());
            _SimulatedObjects.ForEach(rb => rb.Stop());

            _FrozeSceneRigidbodies.Clear();
            _SimulatedObjects.Clear();
        }

        private void ToggleSimulation()
        {
            if (_IsSimulating)
            {
                StopSimulation();
                return;
            }

            StartSimulation();
        }

        private void OnEditorUpdate()
        {
            if (_IsSimulating)
            {
                Physics.gravity = _Gravity;
                Physics.Simulate((1f / _Tickrate) * _Speed);
            }
        }

        private void OnUndoPerformed()
        {
            if (_IsSimulating)
            {
                StopSimulation();
            }
        }

        private void OnSceneChanged(Scene arg0, Scene arg1)
        {
            StopSimulation();
        }
    }
}