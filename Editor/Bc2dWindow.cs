using UnityEngine;
using UnityEditor;

namespace HexseraBc2dEditor
{
    public class Bc2dWindow : EditorWindow
    {
        private enum ErrorFlag
        {
            none,
            no_spriterenderer,
            no_boxcollider2d
        }

        private Bc2dHandle handle;
        private GameObject collider_target;
        private bool hook = false;
        private bool advanced_fold = false;

        private ErrorFlag error_flag = ErrorFlag.none;

        private Bc2dPreset preset;
        private bool preset_fold = false;
        private int preset_num = 0;
        private string preset_name = "";
        private Vector2 preset_scroll = Vector2.zero;
        private string path_asset = "Assets/Bc2dEditorPreset/";

        

        [MenuItem("Tools/Bc2d Editor")]
        public static void ShowWindow()
        {
            GetWindow<Bc2dWindow>("Bc2d Editor");
        }

        private void OnGUI()
        {
            //Debug.Log();
            string[] aa = AssetDatabase.GetAllAssetBundleNames();
            //Debug.Log(aa.Length);
            for (int i = 0; i < aa.Length; i++)
            {
                Debug.Log(aa[i]);
            }
            Main();

        }

        private void Main()
        {
            #if UNITY_6000
                if (AssetDatabase.AssetPathExists("Assets/Bc2dEditorPreset") == false)
                {
                    AssetDatabase.CreateFolder("Assets", "Bc2dEditorPreset");

                }
            #elif UNITY_2022_3
                if (AssetDatabase.IsValidFolder("Assets/Bc2dEditorPreset") == false)
                {
                    AssetDatabase.CreateFolder("Assets", "Bc2dEditorPreset");
                }
            #elif UNITY_2021_3
                if (AssetDatabase.IsValidFolder("Assets/Bc2dEditorPreset") == false)
                {
                    AssetDatabase.CreateFolder("Assets", "Bc2dEditorPreset");
                }
            #else
                if (AssetDatabase.IsValidFolder("Assets/Bc2dEditorPreset") == false)
                {
                    AssetDatabase.CreateFolder("Assets", "Bc2dEditorPreset");
                }
            
            #endif

            preset = AssetDatabase.LoadAssetAtPath<Bc2dPreset>(path_asset + "Bc2dPreset.asset");
            if (preset == null)
            {
                preset = ScriptableObject.CreateInstance<Bc2dPreset>();
                AssetDatabase.CreateAsset(preset, path_asset + "Bc2dPreset.asset");

            }

            GameObject temp_collider_target = (GameObject)EditorGUILayout.ObjectField("Target object", collider_target, typeof(GameObject), true);

            if (temp_collider_target != collider_target || temp_collider_target == null)
            {
                hook = false;
                if (handle != null)
                {
                    handle.ClearThis();
                    handle = null;
                    SceneView.RepaintAll();
                }
                preset_name = "";
            }
            collider_target = temp_collider_target;

            if (hook == true && handle == null)
            {
                hook = false;
                preset_name = "";
            }

            if (hook == false)
            {
                if (GUILayout.Button("Attach"))
                {
                    if (handle == null && collider_target != null)
                    {
                        if (collider_target.GetComponent<SpriteRenderer>() == null)
                        {
                            error_flag = ErrorFlag.no_spriterenderer;
                        }
                        else
                        {
                            handle = new Bc2dHandle(collider_target, this);
                            error_flag = ErrorFlag.none;
                            hook = true;
                            SceneView.RepaintAll();
                        }

                    }
                }
            }
            else
            {
                if (GUILayout.Button("Detach"))
                {
                    if (handle != null)
                    {
                        handle.ClearThis();
                        handle = null;
                        collider_target = null;
                        hook = false;
                        SceneView.RepaintAll();
                    }
                }
            }

            
            

            if (error_flag == ErrorFlag.no_spriterenderer)
            {
                EditorGUILayout.HelpBox("This GameObject doesn't have a SpriteRenderer component!", MessageType.Error);
            }


            if (hook == true && handle != null)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                advanced_fold = EditorGUILayout.Foldout(advanced_fold, new GUIContent("Advanced settings"), true);
                if (advanced_fold == true)
                {

                    handle.SetPixelSnap(EditorGUILayout.ToggleLeft("Pixel snap", handle.GetPixelsnap()));
                    EditorGUIUtility.labelWidth = position.width / 3 + 13.5f;


                    EditorGUI.BeginDisabledGroup(!handle.GetPixelsnap());
                    handle.SetPixelPerUnit(EditorGUILayout.FloatField("Pixels Per Unit", handle.GetPixelPerUnit(), GUILayout.Width(position.width * 3 / 6)));



                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Offset:", GUILayout.Width(position.width / 3));
                    EditorGUIUtility.labelWidth = 10;
                    handle.SetOffset(new Vector2(EditorGUILayout.FloatField("X", handle.GetOffset().x, GUILayout.Width(position.width * 3 / 12)), handle.GetOffset().y));
                    handle.SetOffset(new Vector2(handle.GetOffset().x, EditorGUILayout.FloatField("Y", handle.GetOffset().y, GUILayout.Width(position.width * 3 / 12))));
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.EndDisabledGroup();


                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Position:", GUILayout.Width(position.width / 3));
                    handle.SetPosition(new Vector2(EditorGUILayout.FloatField("X", handle.GetPosition().x, GUILayout.Width(position.width * 3 / 12)), handle.GetPosition().y));
                    handle.SetPosition(new Vector2(handle.GetPosition().x, EditorGUILayout.FloatField("Y", handle.GetPosition().y, GUILayout.Width(position.width * 3 / 12))));
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.BeginHorizontal();
                    string sizename = "";
                    if (handle.GetPixelsnap() == true)
                    {
                        sizename = "Size (pixels):";
                    }
                    else
                    {
                        sizename = "Size (units):";
                    }
                    EditorGUILayout.LabelField(sizename, GUILayout.Width(position.width / 3));
                    EditorGUIUtility.labelWidth = 10;
                    handle.SetSize(new Vector2(EditorGUILayout.FloatField("X", handle.GetSize().x, GUILayout.Width(position.width * 3 / 12)), handle.GetSize().y));
                    handle.SetSize(new Vector2(handle.GetSize().x, EditorGUILayout.FloatField("Y", handle.GetSize().y, GUILayout.Width(position.width * 3 / 12))));
                    EditorGUILayout.EndHorizontal();



                    Rect rect = GUILayoutUtility.GetRect(200, 200, GUILayout.ExpandWidth(true));
                    float size = Mathf.Min(rect.width, rect.height) * 0.8f;
                    Rect square = new Rect(
                        rect.x + (rect.width - size) / 2,
                        rect.y + (rect.height - size) / 2 + 30,
                        size,
                        size
                    );

                    Handles.BeginGUI();
                    Handles.color = Color.green;
                    Handles.DrawLine(new Vector3(square.xMin, square.yMin), new Vector3(square.xMax, square.yMin));
                    Handles.DrawLine(new Vector3(square.xMax, square.yMin), new Vector3(square.xMax, square.yMax));
                    Handles.DrawLine(new Vector3(square.xMax, square.yMax), new Vector3(square.xMin, square.yMax));
                    Handles.DrawLine(new Vector3(square.xMin, square.yMax), new Vector3(square.xMin, square.yMin));
                    Handles.EndGUI();

                    handle.SetLeftTop(new Vector2(handle.GetLeftTop().x, EditorGUI.FloatField(new Rect(square.center.x - 25, square.yMin - 28, 50, 18), handle.GetLeftTop().y))); //top
                    handle.SetLeftTop(new Vector2(EditorGUI.FloatField(new Rect(square.xMin - 60, square.center.y - 9, 50, 18), handle.GetLeftTop().x), handle.GetLeftTop().y)); //left
                    handle.SetRightBottom(new Vector2(EditorGUI.FloatField(new Rect(square.xMax + 10, square.center.y - 9, 50, 18), handle.GetRightBottom().x), handle.GetRightBottom().y)); //right
                    handle.SetRightBottom(new Vector2(handle.GetRightBottom().x, EditorGUI.FloatField(new Rect(square.center.x - 25, square.yMax + 10, 50, 18), handle.GetRightBottom().y))); //bottom
                    Vector2 m_s = EditorStyles.label.CalcSize(new GUIContent("Margin"));
                    EditorGUI.LabelField(new Rect(square.center.x - m_s.x / 2, square.center.y - m_s.y / 2, m_s.x, m_s.y), "Margin");
                    GUILayoutUtility.GetRect(200, 80, GUILayout.ExpandWidth(true)); //just padding
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Preset name: ", GUILayout.Width(80));
                    preset_name = EditorGUILayout.TextField(preset_name);
                    EditorGUILayout.EndHorizontal();
                    if (GUILayout.Button("Save preset"))
                    {
                        preset.Save(MakePresetData());
                        EditorUtility.SetDirty(preset);
                    }
                    GUILayoutUtility.GetRect(200, 2, GUILayout.ExpandWidth(true)); //just padding

                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                preset_fold = EditorGUILayout.Foldout(preset_fold, new GUIContent("Presets"), true);

                if (preset_fold == true)
                {
                    preset_scroll = EditorGUILayout.BeginScrollView(preset_scroll, GUILayout.Height(150));
                    for (int i = 0; i < preset.list.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (EditorGUILayout.Toggle(preset_num == i, EditorStyles.radioButton, GUILayout.Width(20)))
                        {
                            preset_num = i;
                        }
                        else
                        {
                            if (preset_num == i)
                            {
                                preset_num = -1;
                            }
                        }
                        EditorGUILayout.LabelField(preset.list[i].preset_name);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Load"))
                    {
                        if (preset_num != -1)
                        {
                            LoadPreset(preset.Load(preset.list[preset_num].preset_name));
                        }
                        preset_num = -1;
                    }

                    if (GUILayout.Button("Delete", GUILayout.Width(60)))
                    {
                        if (preset_num != -1)
                        {
                            preset.DeleteData(preset.list[preset_num].preset_name);
                        }
                        preset_num = -1;

                    }
                    EditorGUILayout.EndHorizontal();

                }

                EditorGUILayout.EndVertical();

                if (GUILayout.Button("Apply"))
                {
                    handle.CreateBox();
                }
            }


        }

        private Bc2dData MakePresetData()
        {
            Bc2dData temp_data = new Bc2dData();
            temp_data.preset_name = preset_name;
            temp_data.pixel_snap = handle.GetPixelsnap();
            temp_data.ppr = handle.GetPixelPerUnit();
            temp_data.handle_position = handle.GetPosition();
            temp_data.offset = handle.GetOffset();
            temp_data.size = handle.GetSize();
            temp_data.margin_leftup = handle.GetLeftTop();
            temp_data.margin_rightdown = handle.GetRightBottom();
            return temp_data;
        }

        private void LoadPreset(Bc2dData data)
        {
            Bc2dData temp_data = data;

            preset_name = temp_data.preset_name;
            handle.SetPixelSnap(temp_data.pixel_snap);
            if (temp_data.pixel_snap == true)
            {
                handle.SetPixelPerUnit(temp_data.ppr);
                handle.SetOffset(temp_data.offset);
            }
            handle.SetPosition(temp_data.handle_position);
            handle.SetSize(temp_data.size);
            handle.SetLeftTop(temp_data.margin_leftup);
            handle.SetRightBottom(temp_data.margin_rightdown);
        }
    }

}