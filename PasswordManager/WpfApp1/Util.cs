using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;


namespace ExtensionX
{
    /// <summary>
    /// 工具类，全局调用，无副作用
    /// </summary>
    public static class Util
    {
        private static string _currentScene;


        private static byte[] _4ByteBuffer = new byte[4];
        private static byte[] _2ByteBuffer = new byte[2];
        private static byte[] _8ByteBuffer = new byte[8];
        private static byte[] _1ByteBuffer = new byte[1];


        /// <summary>
        /// 生成唯一id
        /// </summary>
        /// <returns></returns>
        public static long GenerateId()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }


        /// <summary>
        /// 扫描时调用的回调
        /// </summary>
        static List<Action<Type, Assembly>> m_calls = new List<Action<Type, Assembly>>();

        /// <summary>
        /// 当前的程序集
        /// </summary>
        private static Assembly m_myAssembly;

        /// <summary>
        /// unity的程序集
        /// </summary>
        private static Assembly m_unityAssembly;

        /// <summary>
        /// 计时器
        /// </summary>
        private static Stopwatch _watch;


        /// <summary>
        /// 扫描所有标注的内容
        /// </summary>
        public static void AddScan(Action<Type, Assembly> call)
        {
            //for (int i = 0; i < o.Length; i++)
            //{
            //    LoadAssembly(o[i], call);
            //}
            m_calls.Add(call);
        }


        /// <summary>
        /// 创建程序集实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object CreateInstance(string name, object[] param = null)
        {
            if (m_myAssembly == null)
            {
                m_myAssembly = Assembly.GetAssembly(typeof(Util));
            }

            return m_myAssembly.CreateInstance(name, true, BindingFlags.Default, null, param, null, null);
        }


        /// <summary>
        /// 开始扫描
        /// </summary>
        public static void StartScan()
        {
            if (m_myAssembly == null)
            {
                m_myAssembly = Assembly.GetAssembly(typeof(Util));
            }

            LoadAssembly(m_myAssembly, m_calls);
            m_calls.Clear();
        }

        /// <summary>
        /// 加载程序集 所有继承了ICommand并且有标注CommandInfo的类
        /// </summary>
        /// <param name="ab"></param>
        public static void LoadAssembly(Assembly ab, List<Action<Type, Assembly>> call)
        {
            if (ab == null || call == null)
            {
                return;
            }

            Type[] t = ab.GetTypes();

            for (int i = 0; i < t.Length; i++)
            {
                for (int j = 0; j < call.Count; j++)
                {
                    call[j](t[i], ab);
                }
            }
        }


        /// <summary>
        /// 开始计时
        /// </summary>
        public static void BeginWatch()
        {
            if (_watch == null)
                _watch = new Stopwatch();
            _watch.Start();
        }

        /// <summary>
        /// 结束计时
        /// </summary>
        /// <returns></returns>
        public static double EndWatch()
        {
            if (_watch == null)
                _watch = new Stopwatch();

            _watch.Stop();
            TimeSpan ts2 = _watch.Elapsed;
            _watch.Reset();
            return ts2.TotalMilliseconds;
        }


        /// <summary>
        /// 写入一个SerializedData
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        public static void WriteStreamData(Stream stream, IStreamData data)
        {
            data.WriteStreamData(stream);
        }

        /// <summary>
        /// 读取一个SerializedData
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T ReadStreamData<T>(Stream stream) where T : IStreamData, new()
        {
            T t = new T();
            t.ReadStreamData(stream);
            return t;
        }


        /// <summary>
        /// 读取一个float
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static float ReadFloat(Stream stream)
        {
            stream.Read(_4ByteBuffer, 0, 4);
            return BitConverter.ToSingle(_4ByteBuffer, 0);
        }


        /// <summary>
        /// 以int32长度读取流
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static int ReadInt(Stream stream)
        {
            stream.Read(_4ByteBuffer, 0, 4);
            return BitConverter.ToInt32(_4ByteBuffer, 0);
        }


   

      

        /// <summary>
        /// 读取一个enum
        /// </summary>
        /// <param name="stream"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ReadEnum<T>(Stream stream)
        {
            string value = ReadString(stream);
            return (T) Enum.Parse(typeof(T), value);
        }


        /// <summary>
        /// 读取一个enum array
        /// </summary>
        /// <param name="stream"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] ReadEnumArray<T>(Stream stream)
        {
            int size = ReadInt(stream);
            T[] t = new T[size];
            for (int i = 0; i < size; i++)
            {
                t[i] = ReadEnum<T>(stream);
            }

            return t;
        }

        /// <summary>
        /// 读取一个long
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static long ReadLong(Stream stream)
        {
            stream.Read(_8ByteBuffer, 0, 8);

            return BitConverter.ToInt64(_8ByteBuffer, 0);
        }


        /// <summary>
        /// 读取一个int array
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static int[] ReadIntArray(Stream stream)
        {
            int size = ReadInt(stream);
            int[] temp = new int[size];
            for (int i = 0; i < size; i++)
            {
                temp[i] = ReadInt(stream);
            }

            return temp;
        }

        /// <summary>
        /// 读取一个bool array
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool[] ReadBoolArray(Stream stream)
        {
            int size = ReadInt(stream);
            bool[] temp = new bool[size];
            for (int i = 0; i < size; i++)
            {
                temp[i] = ReadBool(stream);
            }

            return temp;
        }

        /// <summary>
        /// 读取一个string array
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string[] ReadStringArray(Stream stream)
        {
            int size = ReadInt(stream);
            string[] temp = new string[size];
            for (int i = 0; i < size; i++)
            {
                temp[i] = ReadString(stream);
            }

            return temp;
        }

        /// <summary>
        /// 读取一个float array
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static float[] ReadFloatArray(Stream stream)
        {
            int size = ReadInt(stream);
            float[] temp = new float[size];
            for (int i = 0; i < size; i++)
            {
                temp[i] = ReadFloat(stream);
            }

            return temp;
        }

        /// <summary>
        /// 读取一个string
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadString(Stream stream)
        {
            int length = ReadInt(stream);
            byte[] stringByte = new byte[length];
            stream.Read(stringByte, 0, length);
            return Encoding.UTF8.GetString(stringByte);
        }


        /// <summary>
        /// 读取一个bool
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool ReadBool(Stream stream)
        {
            return stream.ReadByte() == 1;
        }

        /// <summary>
        /// 读取一个byte
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static int ReadByte(Stream stream)
        {
            return stream.ReadByte();
        }

        public static byte[] ReadByteArray(Stream stream)
        {
            int count = ReadInt(stream);
            byte[] datas = new byte[count];
            stream.Read(datas, 0, count);
            return datas;
        }




        /// <summary>
        /// 写入一个byte
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteByte(Stream stream, byte i)
        {
            stream.WriteByte(i);
        }

        /// <summary>
        /// 写入一个enum
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteEnum(Stream stream, Enum i)
        {
            WriteString(stream, i.ToString());
        }

        /// <summary>
        /// 写入一个int
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteInt(Stream stream, int i)
        {
            byte[] data = BitConverter.GetBytes(i);
            stream.Write(data, 0, 4);
        }


        /// <summary>
        /// 写入一个double
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteDouble(Stream stream, double i)
        {
            byte[] data = BitConverter.GetBytes(i);
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// 写入一个double
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static double ReadDouble(Stream stream)
        {
            stream.Read(_8ByteBuffer, 0, 8);
            return BitConverter.ToDouble(_8ByteBuffer, 0);
        }


        /// <summary>
        /// 写入一个long
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteLong(Stream stream, long i)
        {
            byte[] data = BitConverter.GetBytes(i);
            stream.Write(data, 0, 8);
        }

        public static void WriteByteArray(Stream stream, byte[] data)
        {
            WriteInt(stream, data.Length);
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// 写入一个string
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteString(Stream stream, string i)
        {
            if (string.IsNullOrEmpty(i))
            {
                WriteInt(stream, 0);
                return;
            }

            string t = i.Replace("\\n", "\n").Replace("%1", ",");
            byte[] data = Encoding.UTF8.GetBytes(t);
            WriteInt(stream, data.Length);
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// 写入一个float
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteFloat(Stream stream, float i)
        {
            byte[] data = BitConverter.GetBytes(i);
            stream.Write(data, 0, data.Length);
        }


        /// <summary>
        /// 写入一个float array
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteFloatArray(Stream stream, float[] i)
        {
            WriteInt(stream, i.Length);
            foreach (var VARIABLE in i)
            {
                WriteFloat(stream, VARIABLE);
            }
        }

        /// <summary>
        /// 写入一个int array
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteIntArray(Stream stream, int[] i)
        {
            WriteInt(stream, i.Length);
            foreach (var VARIABLE in i)
            {
                WriteInt(stream, VARIABLE);
            }
        }

        /// <summary>
        /// 写入一个double array
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteDoubleArray(Stream stream, double[] i)
        {
            WriteInt(stream, i.Length);
            foreach (var VARIABLE in i)
            {
                WriteDouble(stream, VARIABLE);
            }
        }

        /// <summary>
        /// 写入一个long array
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteLongArray(Stream stream, long[] i)
        {
            WriteInt(stream, i.Length);
            foreach (var VARIABLE in i)
            {
                WriteLong(stream, VARIABLE);
            }
        }

        /// <summary>
        /// 写入一个bool array
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteBoolArray(Stream stream, bool[] i)
        {
            WriteInt(stream, i.Length);
            foreach (var VARIABLE in i)
            {
                WriteBool(stream, VARIABLE);
            }
        }

        /// <summary>
        /// 写入一个string array
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteStringArray(Stream stream, string[] i)
        {
            WriteInt(stream, i.Length);
            foreach (var VARIABLE in i)
            {
                WriteString(stream, VARIABLE);
            }
        }


        /// <summary>
        /// 写入一个bool
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="i"></param>
        public static void WriteBool(Stream stream, bool i)
        {
            stream.WriteByte(BitConverter.GetBytes(i)[0]);
        }
    }
}