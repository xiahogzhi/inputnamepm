using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace ExtensionX
{
    /// <summary>
    /// 用于游戏数据保存时记录的自定义数据
    /// </summary>
    [Serializable]
    public class SerializedData : IStreamData
    {
        [Serializable]
        private class Value
        {
            public int type;

            public object data;
        }

        [SerializeField] Dictionary<string, Value> _datas = new Dictionary<string, Value>();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();

            foreach (var VARIABLE in Datas)
            {
                sb.AppendLine("key:" + VARIABLE.Key + "\tvalue:" + VARIABLE.Value.data);
            }

            return sb.ToString();
        }

        private Dictionary<string, Value> Datas
        {
            get
            {
                if (_datas == null)
                {
                    _datas = new Dictionary<string, Value>();
                }

                return _datas;
            }
        }

        /// <summary>
        /// 存储临时对象,保存数据时不会保存此对象,此对象可以是任何对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SaveTempObj(string name, object value)
        {
            if (Datas.ContainsKey(name))
            {
                Datas[name].type = -1;
                Datas[name].data = value;
            }
            else
            {
                Value v = new Value();
                v.data = value;
                v.type = -1;
                Datas.Add(name, v);
            }
        }

        /// <summary>
        /// 获取临时对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetTempObj<T>(string name, object defaultValue = null)
        {
            if (Datas.ContainsKey(name))
            {
                return (T) Datas[name].data;
            }

            return default(T);
        }

        /// <summary>
        /// 是否存在对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasData(string name)
        {
            return Datas.ContainsKey(name);
        }

        /// <summary>
        /// 清除临时对象
        /// </summary>
        public void ClearTempObj()
        {
            List<string> clearList = new List<string>();
            foreach (var VARIABLE in Datas)
            {
                if (VARIABLE.Value.type == -1)
                {
                    clearList.Add(VARIABLE.Key);
                }
            }

            for (int i = 0; i < clearList.Count; i++)
            {
                Datas.Remove(clearList[i]);
            }
        }

        public int[] GetIntArray(string name)
        {
            return GetObject<int[]>(name);
        }

        public float[] GetFloatArray(string name)
        {
            return GetObject<float[]>(name);
        }

        public bool[] GetBoolArray(string name)
        {
            return GetObject<bool[]>(name);
        }

        public string[] GetStringArray(string name)
        {
            return GetObject<string[]>(name);
        }

        public long GetLong(string name, long defaultValue = 0L)
        {
            return GetObject<long>(name, defaultValue);
        }


        public bool GetBool(string name, bool defaultValue = false)
        {
            return GetObject<bool>(name, defaultValue);
        }

        public int GetInt(string name, int defaultvalue = 0)
        {
            return GetObject<int>(name, defaultvalue);
        }

        public float GetFloat(string name, float defaultvalue = 0)
        {
            return GetObject<float>(name, defaultvalue);
        }

        public string GetString(string name, string defaultvalue = "")
        {
            return GetObject<string>(name, defaultvalue);
        }

        private T GetObject<T>(string name, object defaultValue = null)
        {
            if (Datas.ContainsKey(name))
            {
                return (T) Datas[name].data;
            }

            return (T) defaultValue;
        }

        public void SetByteArray(string name, byte[] value)
        {
            SetValue(name, value, 13);
        }

        public void SetBool(string name, bool value)
        {
            SetValue(name, value, 5);
        }

        public void SetIntArray(string name, int[] value)
        {
            SetValue(name, value, 9);
        }

        public void SetStringArray(string name, string[] value)
        {
            SetValue(name, value, 10);
        }

        public void SetFloatArray(string name, float[] value)
        {
            SetValue(name, value, 11);
        }

        public void SetBoolArray(string name, bool[] value)
        {
            SetValue(name, value, 12);
        }

        private void SetValue(string name, object value, int type)
        {
            if (Datas.ContainsKey(name))
            {
                Datas[name].type = type;
                Datas[name].data = value;
            }
            else
            {
                Value v = new Value();
                v.data = value;
                v.type = type;
                Datas.Add(name, v);
            }
        }

        public void SetFloat(string name, float value)
        {
            SetValue(name, value, 2);
        }

        public void SetInt(string name, int value)
        {
            SetValue(name, value, 1);
        }

        public void SetLong(string name, long value)
        {
            SetValue(name, value, 7);
        }

        public void SetString(string name, string value)
        {
            SetValue(name, value, 6);
        }

        public void Remove(string name)
        {
            if (Datas.ContainsKey(name))
            {
                Datas.Remove(name);
            }
        }

        public void Clear()
        {
            Datas.Clear();
        }

        public void WriteStreamData(Stream stream)
        {
            Util.WriteInt(stream, Datas.Count);
            foreach (var ser in Datas)
            {
                if (!string.IsNullOrEmpty(ser.Key))
                {
                    Util.WriteInt(stream, ser.Value.type);
                    Util.WriteString(stream, ser.Key);
                    switch (ser.Value.type)
                    {
                        case 1:
                            Util.WriteInt(stream, (int) ser.Value.data);
                            break;
                        case 2:
                            Util.WriteFloat(stream, (float) ser.Value.data);
                            break;
                        case 5:
                            Util.WriteBool(stream, (bool) ser.Value.data);
                            break;
                        case 6:
                            Util.WriteString(stream, (string) ser.Value.data);
                            break;
                        case 7:
                            Util.WriteLong(stream, (long) ser.Value.data);
                            break;
                        case 9:
                            Util.WriteIntArray(stream, (int[]) ser.Value.data);
                            break;
                        case 10:
                            Util.WriteStringArray(stream, (string[]) ser.Value.data);
                            break;
                        case 11:
                            Util.WriteFloatArray(stream, (float[]) ser.Value.data);
                            break;
                        case 12:
                            Util.WriteBoolArray(stream, (bool[]) ser.Value.data);
                            break;
                        case 13:
                            Util.WriteByteArray(stream, (byte[]) ser.Value.data);
                            break;
                    }
                }
            }
        }

        public void ReadStreamData(Stream stream)
        {
            SerializedData sd = this;
            int count = Util.ReadInt(stream);
            for (int i = 0; i < count; i++)
            {
                Value v = new SerializedData.Value();
                v.type = Util.ReadInt(stream);
                string name = Util.ReadString(stream);
                switch (v.type)
                {
                    case 1:
                        v.data = Util.ReadInt(stream);
                        break;
                    case 2:
                        v.data = Util.ReadFloat(stream);
                        break;
                    case 5:
                        v.data = Util.ReadBool(stream);
                        break;
                    case 6:
                        v.data = Util.ReadString(stream);
                        break;
                    case 7:
                        v.data = Util.ReadLong(stream);
                        break;
                    case 9:
                        v.data = Util.ReadIntArray(stream);
                        break;
                    case 10:
                        v.data = Util.ReadStringArray(stream);
                        break;
                    case 11:
                        v.data = Util.ReadFloatArray(stream);
                        break;
                    case 12:
                        v.data = Util.ReadBoolArray(stream);
                        break;
                    case 13:
                        v.data = Util.ReadByteArray(stream);
                        break;
                }

                sd.Datas.Add(name, v);
            }
        }
    }
}