// Battle Review Before Boss Node

using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    /// <summary> 用于解析配置表的 Vector3 数据拓展方法 </summary>
	public static class VectorStringHelper
    {
        /// <summary> 默认配置表有多少个 VectorString 配置 </summary>
        private const ushort DefaultVectorConfigLength = 64;

        /// <summary> 默认配置表一个 VectorString 配置字符串长度 </summary>
        private const ushort DefaultVectorLength = 64;

        private static readonly List<Vector3> vectorStringList;

        /// <summary> 字符串转换器, 目前只能 string to float, 初始长度为 64 </summary>
        private static StringBuilder vectorStringReader;
        
        static VectorStringHelper()
        {
            vectorStringList = new List<Vector3>(DefaultVectorConfigLength);
            vectorStringReader = new StringBuilder(DefaultVectorLength);
        }

        public static void Clear()
        {
            vectorStringList.Clear();
            vectorStringReader.Clear();
        }

        #region String Array public Parse

        internal static bool TryParseVectorStringArray(string[] vectorStringArray, List<Vector3> vectorList)
        {
            vectorList.Clear();

            string vectorString;
            for (var index = 0; index < vectorStringArray.Length; index++)
            {
                vectorString = vectorStringArray[index];
                try
                {
                    if (TryParseVector(vectorString, out Vector3 vector))
                        vectorList.Add(vector);
                }
                catch (System.Exception exception)
                {
                    Log.Error(exception.Message);
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region String public Parse

        /// <summary> 配置表字符串分隔符 </summary>
        private const char SplitSymbol = ':';

        /// <summary> 这里不定义成拓展方法为了方便外部调用 </summary>
        public static bool TryParseVector(string vectorString, out Vector3 vector)
        {
            vector = Vector3.zero;

            if (string.IsNullOrEmpty(vectorString))
                return false;

            byte vectorFlag = VectorFlagX;
            int splitIndex = 0;
            int vectorStringLen = vectorString.Length;
            for (var index = splitIndex; index < vectorStringLen; index++)
            {
                char @char = vectorString[index];
                if (@char != SplitSymbol)
                    continue;

                vector.SetValue(vectorString, splitIndex, index, vectorFlag);
                splitIndex = index + 1;
                vectorFlag++;
            }

            // 最后一个 z 值在这里进行赋值, 如果一直没找到则只给 x 赋值
            vector.SetValue(vectorString, splitIndex, (ushort)vectorStringLen, vectorFlag);
            return true;
        }

        #endregion

        #region Vector3 private Extention

        /// <summary> 通过分隔符设置 Vector3 对应的值, 定义成拓展方法方便内部调用, 访问权限为私有 </summary>
        /// <param name="vector">需要修改的 Vector3 </param>
        /// <param name="vectorString">Vector3 字符串</param>
        /// <param name="splitIndex">分隔数值初始索引值</param>
        /// <param name="index">分隔数值终止索引值</param>
        /// <param name="vectorFlag">当前赋值标记位</param>
        private static void SetValue(this ref Vector3 vector, string vectorString, int splitIndex, int index, byte vectorFlag)
        {
            vectorStringReader.Clear();
            vectorStringReader.Append(vectorString, splitIndex, index - splitIndex);

            // Battle TODO ToString 有 GC 后期优化
            string floatString = vectorStringReader.ToString();

            if (float.TryParse(floatString, out float value))
                vector.SetValue(vectorFlag, value);
            else
                Log.Error($"Config Vector3 string is not valid config string = { vectorString }");
        }

        private const byte VectorFlagX = 0;
        private const byte VectorFlagY = 1;
        private const byte VectorFlagZ = 2;

        /// <summary> 定义成拓展方法方便内部调用, 访问权限为私有 </summary>
        private static void SetValue(this ref Vector3 vector, byte vectorFlag, float value)
        {
            switch (vectorFlag)
            {
                case VectorFlagX: vector.x = value; break;
                case VectorFlagY: vector.y = value; break;
                case VectorFlagZ: vector.z = value; break;
            }
        }

        #endregion
    }
}