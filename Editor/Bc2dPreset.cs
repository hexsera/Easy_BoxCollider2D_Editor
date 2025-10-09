using UnityEngine;
using System.Collections.Generic;

namespace HexseraBc2dEditor
{
    [System.Serializable]
    public class Bc2dData //Bc2d preset data container
    {
        public string preset_name;
        public bool pixel_snap;
        public float ppr;
        public Vector2 handle_position;
        public Vector2 offset;
        public Vector2 size;
        public Vector2 margin_leftup;
        public Vector2 margin_rightdown;
    }


    public class Bc2dPreset : ScriptableObject //Bc2d preset data management class
    {
        public List<Bc2dData> list = new List<Bc2dData>(); //Bc2d preset data

        public void Save(Bc2dData data, bool new_save = true)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].preset_name == data.preset_name)
                {
                    list[i] = data;
                    return;
                }
            }

            if (new_save)
            {
                list.Add(data);
            }

        }

        public Bc2dData Load(string name)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].preset_name == name)
                {
                    return list[i];
                }
            }

            return null;
        }

        public void DeleteData(string name)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].preset_name == name)
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}
