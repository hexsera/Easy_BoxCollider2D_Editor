using UnityEngine;
using UnityEditor;

namespace HexseraBc2dEditor
{
    public class Bc2dHandle
    {
        private enum HandleMouse 
        {
            wait,
            on,
            click,
            drag
        }

        private string path_asset = "Packages/com.hexseragames.bc2deditor/Sprite/";
        //private string path_asset = "Assets/Easy Bc2d Editor/Sprite/";

        private Bc2dWindow parent_window;
        private GameObject target;
        private Sprite sp;
        private float pixel_per_unit = 0;

        private Vector3 before_position;
        private Vector3 handle_move_position = new Vector3(0, 0, 0);
        private Vector3 handle_move_position_before = new Vector3(0, 0, 0);
        private Vector3 handle_scale_position = new Vector3(0, 0, 0);

        private Vector2 collider_offset = new Vector2(0, 0);
        private Vector2 collider_margin_left_top = new Vector2(0, 0);
        private Vector2 collider_margin_right_bottom = new Vector2(0, 0);

        private Vector2 collider_pixel = new Vector2(0, 0);

        private bool pixel_snap = true;


        private HandleMouse cross_arrow_state = HandleMouse.wait;
        private Rect cross_arrow_rect;
        private Texture2D cross_arrow;
        private Texture2D[] cross_arrows = new Texture2D[3];

        private HandleMouse scale_arrow_state = HandleMouse.wait;
        private Rect scale_arrow_rect;
        private Texture2D scale_arrow;
        private Texture2D[] scale_arrows = new Texture2D[3];

        public Bc2dHandle(GameObject tar, Bc2dWindow par)
        {
            target = tar;
            parent_window = par;
            SceneView.duringSceneGui += OnSceneGUI;
            sp = target.GetComponent<SpriteRenderer>().sprite;
            pixel_per_unit = sp.pixelsPerUnit;

            before_position = SpriteLeftup();
            handle_move_position = new Vector3(target.transform.position.x + sp.bounds.center.x * target.transform.lossyScale.x - sp.rect.width * target.transform.lossyScale.x / 2 * (1 / pixel_per_unit),
                                                target.transform.position.y + sp.bounds.center.y * target.transform.lossyScale.y + sp.rect.height * target.transform.lossyScale.y / 2 * (1 / pixel_per_unit));
            handle_scale_position = new Vector3(target.transform.position.x + sp.bounds.center.x * target.transform.lossyScale.x + sp.rect.width * target.transform.lossyScale.x / 2 * (1 / pixel_per_unit),
                                                target.transform.position.y + sp.bounds.center.y * target.transform.lossyScale.y - sp.rect.height * target.transform.lossyScale.y / 2 * (1 / pixel_per_unit));

            cross_arrows[0] = AssetDatabase.LoadAssetAtPath<Texture2D>(path_asset + "farrow.png");
            cross_arrows[1] = AssetDatabase.LoadAssetAtPath<Texture2D>(path_asset + "farrow2.png");
            cross_arrows[2] = AssetDatabase.LoadAssetAtPath<Texture2D>(path_asset + "farrow3.png");

            cross_arrow = cross_arrows[0];

            scale_arrows[0] = AssetDatabase.LoadAssetAtPath<Texture2D>(path_asset + "scale.png");
            scale_arrows[1] = AssetDatabase.LoadAssetAtPath<Texture2D>(path_asset + "scale2.png");
            scale_arrows[2] = AssetDatabase.LoadAssetAtPath<Texture2D>(path_asset + "scale3.png");

            scale_arrow = scale_arrows[0];
        }

        ~Bc2dHandle()
        {
            ClearThis();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (before_position != SpriteLeftup()) //sprite moved
            {
                handle_move_position += SpriteLeftup() - before_position;
                handle_scale_position += SpriteLeftup() - before_position;
                before_position = SpriteLeftup();

            }

            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.Layout:
                    break;
                case EventType.MouseMove:
                    if (cross_arrow_state != HandleMouse.click && MouseOnTexture(scale_arrow_rect, scale_arrows[0], evt.mousePosition)) //scale action
                    {
                        scale_arrow_state = HandleMouse.on;
                    }
                    else
                    {
                        scale_arrow_state = HandleMouse.wait;
                    }

                    if (scale_arrow_state == HandleMouse.wait && MouseOnTexture(cross_arrow_rect, cross_arrows[0], evt.mousePosition)) //cross action
                    {
                        cross_arrow_state = HandleMouse.on;
                    }
                    else
                    {
                        cross_arrow_state = HandleMouse.wait;
                    }
                    break;

                case EventType.MouseDown:
                    if (evt.button == 0)
                    {
                        if (MouseOnTexture(scale_arrow_rect, scale_arrows[0], evt.mousePosition)) //scale action
                        {
                            scale_arrow_state = HandleMouse.click;
                        }

                        if (scale_arrow_state == HandleMouse.wait && MouseOnTexture(cross_arrow_rect, cross_arrows[0], evt.mousePosition)) //cross action
                        {
                            cross_arrow_state = HandleMouse.click;
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (evt.button == 0)
                    {
                        if (scale_arrow_state == HandleMouse.click)
                        {
                            if (MouseOnTexture(scale_arrow_rect, scale_arrows[0], evt.mousePosition)) //scale action
                            {
                                scale_arrow_state = HandleMouse.on;
                            }
                            else
                            {
                                scale_arrow_state = HandleMouse.wait;
                            }
                        }

                        if (cross_arrow_state == HandleMouse.click)
                        {
                            if (scale_arrow_state != HandleMouse.on && MouseOnTexture(cross_arrow_rect, cross_arrows[0], evt.mousePosition)) //cross action
                            {
                                cross_arrow_state = HandleMouse.on;
                            }
                            else
                            {
                                cross_arrow_state = HandleMouse.wait;
                            }
                        }
                    }
                    break;

                case EventType.Repaint:
                    switch (cross_arrow_state)
                    {
                        case HandleMouse.wait:
                            cross_arrow = cross_arrows[0];
                            break;
                        case HandleMouse.on:
                            cross_arrow = cross_arrows[2];
                            break;
                        case HandleMouse.click:
                            cross_arrow = cross_arrows[1];
                            break;
                        case HandleMouse.drag:

                            break;
                    }

                    switch (scale_arrow_state)
                    {
                        case HandleMouse.wait:
                            scale_arrow = scale_arrows[0];
                            break;
                        case HandleMouse.on:
                            scale_arrow = scale_arrows[2];
                            break;
                        case HandleMouse.click:
                            scale_arrow = scale_arrows[1];
                            break;
                        case HandleMouse.drag:

                            break;
                    }
                    break;
            }

            EditorGUI.BeginChangeCheck();
            Vector3 temp_handle_move_position = Handles.Slider2D(
                handle_move_position,
                Vector3.forward,
                Vector3.right,
                Vector3.up,
                HandleUtility.GetHandleSize(handle_move_position) * 0.1f,
                CrossArrowCap,
                new Vector2(0, 0)
            );
            Vector3 temp_handle_scale_position = Handles.Slider2D(
                handle_scale_position,
                Vector3.forward,
                Vector3.right,
                Vector3.up,
                HandleUtility.GetHandleSize(handle_scale_position) * 0.1f,
                ScaleArrowCap,
                new Vector2(0, 0)
            );

            if (EditorGUI.EndChangeCheck())
            {
                if (pixel_snap == true)
                {
                    handle_move_position = VectorSnap(temp_handle_move_position, new Vector3(1 / pixel_per_unit * target.transform.lossyScale.x,
                                                    1 / pixel_per_unit * target.transform.lossyScale.y,
                                                    1 / pixel_per_unit * target.transform.lossyScale.z),
                                                    SpriteLeftup(true) + (Vector3)collider_offset);
                }
                else
                {
                    handle_move_position = temp_handle_move_position;
                }

                if (handle_move_position_before != handle_move_position)
                {
                    temp_handle_scale_position += handle_move_position - handle_move_position_before;
                }

                if (pixel_snap == true)
                {
                    handle_scale_position = VectorSnap(temp_handle_scale_position, new Vector3(1 / pixel_per_unit * target.transform.lossyScale.x,
                                                                    1 / pixel_per_unit * target.transform.lossyScale.y,
                                                                    1 / pixel_per_unit * target.transform.lossyScale.z),
                                                                    SpriteLeftup(true) + (Vector3)collider_offset);
                }
                else
                {
                    handle_scale_position = temp_handle_scale_position;
                }

                if (handle_move_position.x >= handle_scale_position.x)
                {
                    handle_scale_position.x = handle_move_position.x;
                }
                if (handle_move_position.y <= handle_scale_position.y)
                {
                    handle_scale_position.y = handle_move_position.y;
                }
                parent_window.Repaint();
            }

            Rect handle_rect = new Rect(handle_move_position.x, handle_move_position.y, handle_scale_position.x - handle_move_position.x, handle_scale_position.y - handle_move_position.y);
            Rect handle_extend_rect = new Rect(handle_move_position.x - collider_margin_left_top.x, handle_move_position.y + collider_margin_left_top.y,
                                                        (handle_scale_position.x + collider_margin_right_bottom.x) - (handle_move_position.x - collider_margin_left_top.x),
                                                        (handle_scale_position.y - collider_margin_right_bottom.y) - (handle_move_position.y + collider_margin_left_top.y));
            handle_move_position_before = handle_move_position;

            Handles.color = Color.white;
            DrawRect(handle_extend_rect, 1);
            Handles.color = Color.red;
            DrawRect(handle_rect, 2);

            if (pixel_snap == true)
            {
                collider_pixel = new Vector2(Mathf.Round(handle_rect.width / (1 / pixel_per_unit * target.transform.lossyScale.x)),
                                Mathf.Round(-handle_rect.height / (1 / pixel_per_unit * target.transform.lossyScale.y)));
            }
            else
            {
                collider_pixel = new Vector2(handle_rect.width,
                                -handle_rect.height);
            }
        }

        private Vector3 VectorSnap(Vector3 pos_now, Vector3 snap, Vector3 offset)
        {
            //Snaps the given position and returns the result.
            Vector3 return_v = pos_now - offset;

            if (snap.x >= 0)
            {
                return_v.x = Mathf.Round(return_v.x / snap.x) * snap.x;
            }
            else
            {
                return_v.x = pos_now.x;
            }

            if (snap.y >= 0)
            {
                return_v.y = Mathf.Round(return_v.y / snap.y) * snap.y;
            }
            else
            {
                return_v.y = pos_now.y;
            }

            if (snap.z >= 0)
            {
                return_v.z = Mathf.Round(return_v.z / snap.z) * snap.z;
            }
            else
            {
                return_v.z = pos_now.z;
            }

            return return_v + offset;
        }

        private Vector3 SpriteLeftup(bool z_zero = false)
        {
            //Returns the sprite's top-left corner in world space.
            Vector3 return_v;
            return_v.x = target.transform.position.x + sp.bounds.center.x * target.transform.lossyScale.x
                            - sp.rect.width * target.transform.lossyScale.x / 2 * (sp.pixelsPerUnit / 100);
            return_v.y = target.transform.position.y + sp.bounds.center.y * target.transform.lossyScale.y
                            + sp.rect.height * target.transform.lossyScale.y / 2 * (sp.pixelsPerUnit / 100);
            return_v.z = target.transform.position.z;
            if (z_zero == true)
            {
                return_v.z = 0;
            }
            return return_v;
        }

        private void DrawRect(Rect rect, float thick)
        {
            Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y), thick);
            Handles.DrawLine(new Vector2(rect.x + rect.width, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), thick);
            Handles.DrawLine(new Vector2(rect.x + rect.width, rect.y + rect.height), new Vector2(rect.x, rect.y + rect.height), thick);
            Handles.DrawLine(new Vector2(rect.x, rect.y + rect.height), new Vector2(rect.x, rect.y), thick);
        }

        private bool MouseOnTexture(Rect rect, Texture2D tex, Vector2 m_pos)
        {
            //Checks whether the mouse cursor is over the texture.
            if (rect == null || tex == null) return false;

            if (rect.Contains(m_pos))
            {
                int px = Mathf.FloorToInt((m_pos.x - rect.x) / rect.width * tex.width);
                int py = Mathf.FloorToInt((rect.height - (m_pos.y - rect.y)) / rect.height * tex.height);

                Color pixel = tex.GetPixel(px, py);
                if (pixel.a > 0.1f)
                {
                    return true;
                }
            }
            return false;
        }

        private void CrossArrowCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            Texture2D tex = cross_arrows[0];
            Vector2 guiPoint = HandleUtility.WorldToGUIPoint(position);
            float texSize = 64f;
            Rect rect = new Rect(guiPoint.x - texSize * 0.5f, guiPoint.y - texSize * 0.5f, texSize, texSize);
            cross_arrow_rect = rect;

            switch (eventType)
            {
                case EventType.Layout:

                    if (rect.Contains(Event.current.mousePosition))
                    {
                        int px = Mathf.FloorToInt((Event.current.mousePosition.x - rect.x) / rect.width * tex.width);
                        int py = Mathf.FloorToInt((rect.height - (Event.current.mousePosition.y - rect.y)) / rect.height * tex.height);

                        Color pixel = tex.GetPixel(px, py);
                        if (pixel.a > 0.1f)
                        {
                            HandleUtility.AddControl(controlID, 0f);
                        }
                    }
                    break;

                case EventType.Repaint:
                    Handles.BeginGUI();
                    GUI.DrawTexture(rect, cross_arrow);
                    Handles.EndGUI();
                    break;
            }
        }

        private void ScaleArrowCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            Texture2D tex = scale_arrows[0];
            Vector2 guiPoint = HandleUtility.WorldToGUIPoint(position);
            float texSize = 64f;
            Rect rect = new Rect(guiPoint.x - texSize * 0.5f, guiPoint.y - texSize * 0.5f, texSize, texSize);
            scale_arrow_rect = rect;

            switch (eventType)
            {
                case EventType.Layout:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        int px = Mathf.FloorToInt((Event.current.mousePosition.x - rect.x) / rect.width * tex.width);
                        int py = Mathf.FloorToInt((rect.height - (Event.current.mousePosition.y - rect.y)) / rect.height * tex.height);

                        Color pixel = tex.GetPixel(px, py);
                        if (pixel.a > 0.1f)
                        {
                            HandleUtility.AddControl(controlID, 0f);
                        }
                    }
                    break;

                case EventType.Repaint:
                    Handles.BeginGUI();
                    GUI.DrawTexture(rect, scale_arrow);
                    Handles.EndGUI();
                    break;
            }
        }

        public void CreateBox()
        {
            //Applies the settings to the BoxCollider2D.
            float width = (handle_scale_position.x + collider_margin_right_bottom.x) - (handle_move_position.x - collider_margin_left_top.x);
            float height = (handle_move_position.y + collider_margin_left_top.y) - (handle_scale_position.y - collider_margin_right_bottom.y);
            float offset_x = (handle_move_position.x - collider_margin_left_top.x) - target.transform.position.x + width / 2;
            float offset_y = (handle_move_position.y + collider_margin_left_top.y) - target.transform.position.y - height / 2;

            BoxCollider2D col = target.GetComponent<BoxCollider2D>();
            if (col == null)
            {
                target.AddComponent<BoxCollider2D>();
                col = target.GetComponent<BoxCollider2D>();
            }

            col.size = new Vector2(width / target.transform.lossyScale.x, height / target.transform.lossyScale.y);
            col.offset = new Vector2(offset_x / target.transform.lossyScale.x, offset_y / target.transform.lossyScale.y);
        }

        public void SetPosition(Vector2 pos)
        {
            handle_move_position = pos + (Vector2)SpriteLeftup() + GetOffset();
        }


        public void SetPixelSnap(bool b)
        {
            if (pixel_snap != b)
            {
                if (b == false)
                {
                    collider_offset = new Vector2(0, 0);
                    collider_pixel = new Vector2(
                        handle_scale_position.x - handle_move_position.x,
                        handle_move_position.y - handle_scale_position.y
                    );
                }
                else
                {
                    handle_move_position = VectorSnap(handle_move_position, new Vector3(1 / pixel_per_unit * target.transform.lossyScale.x,
                                                                    1 / pixel_per_unit * target.transform.lossyScale.y,
                                                                    1 / pixel_per_unit * target.transform.lossyScale.z),
                                                                    SpriteLeftup(true) + (Vector3)collider_offset);

                    handle_scale_position = VectorSnap(handle_scale_position, new Vector3(1 / pixel_per_unit * target.transform.lossyScale.x,
                                                                    1 / pixel_per_unit * target.transform.lossyScale.y,
                                                                    1 / pixel_per_unit * target.transform.lossyScale.z),
                                                                    SpriteLeftup(true) + (Vector3)collider_offset);


                    collider_pixel = new Vector2(
                        Mathf.Round((handle_scale_position.x - handle_move_position.x) / (1 / pixel_per_unit * target.transform.lossyScale.x)),
                        Mathf.Round((handle_move_position.y - handle_scale_position.y) / (1 / pixel_per_unit * target.transform.lossyScale.y))
                    );


                }
                pixel_snap = b;
            }
        }

        public void SetOffset(Vector2 pos)
        {
            Vector2 shift = pos - collider_offset;
            handle_move_position += (Vector3)shift;
            handle_scale_position += (Vector3)shift;
            collider_offset = pos;
            SceneView.RepaintAll();
        }

        public void SetPixelPerUnit(float p)
        {
            if (pixel_per_unit != p)
            {
                pixel_per_unit = p;
                handle_move_position = VectorSnap(handle_move_position, new Vector3(1 / pixel_per_unit * target.transform.lossyScale.x,
                                                                    1 / pixel_per_unit * target.transform.lossyScale.y,
                                                                    1 / pixel_per_unit * target.transform.lossyScale.z),
                                                                    SpriteLeftup(true) + (Vector3)collider_offset);

                handle_scale_position = VectorSnap(handle_scale_position, new Vector3(1 / pixel_per_unit * target.transform.lossyScale.x,
                                                                1 / pixel_per_unit * target.transform.lossyScale.y,
                                                                1 / pixel_per_unit * target.transform.lossyScale.z),
                                                                SpriteLeftup(true) + (Vector3)collider_offset);
            }

        }

        public void SetLeftTop(Vector2 v)
        {
            collider_margin_left_top = v;
        }

        public void SetRightBottom(Vector2 v)
        {
            collider_margin_right_bottom = v;
        }

        public void SetSize(Vector2 s)
        {
            float temp_sx = s.x;
            float temp_sy = s.y;

            if (s.x <= 0)
            {
                temp_sx = 0;
            }
            if (s.y <= 0)
            {
                temp_sy = 0;
            }

            if (pixel_snap == true)
            {
                float temp_width = Mathf.Round(temp_sx) * target.transform.lossyScale.x * 1 / pixel_per_unit;
                float temp_height = -Mathf.Round(temp_sy) * target.transform.lossyScale.y * 1 / pixel_per_unit;

                handle_scale_position = new Vector3(handle_move_position.x + temp_width, handle_move_position.y + temp_height, 0);
                collider_pixel = new Vector2(Mathf.Round(temp_sx), Mathf.Round(temp_sy));
            }
            else
            {
                float temp_width = temp_sx;
                float temp_height = -temp_sy;
                handle_scale_position = new Vector3(handle_move_position.x + temp_width, handle_move_position.y + temp_height, 0);
                collider_pixel = new Vector2(temp_sx, temp_sy);
            }
        }

        public Vector2 GetPosition()
        {
            return (Vector2)(handle_move_position - SpriteLeftup() - (Vector3)GetOffset());
        }


        public bool GetPixelsnap()
        {
            return pixel_snap;
        }

        public Vector2 GetOffset()
        {
            return collider_offset;
        }

        public float GetPixelPerUnit()
        {
            return pixel_per_unit;
        }

        public Vector2 GetLeftTop()
        {
            return collider_margin_left_top;
        }

        public Vector2 GetRightBottom()
        {
            return collider_margin_right_bottom;
        }

        public Vector2 GetSize()
        {
            return collider_pixel;
        }

        public void ClearThis()
        {
            //Called when the class is released from memory.
            SceneView.duringSceneGui -= OnSceneGUI;
        }


    }

}
