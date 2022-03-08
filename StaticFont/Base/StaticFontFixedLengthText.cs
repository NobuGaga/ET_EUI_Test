using UnityEngine;
using UnityEngine.Profiling;

namespace StaticFont
{
    /// <summary>
    /// 针对捕鱼游戏，单独写的一套mesh，
    /// 一次性直接分配15个字的空间出来（顶点、顶点色、uv等等）
    /// 同时利用设置顶点色的alpha来实现渐变色功能
    /// 设置 vertexColor.r来控制顶点位移，r>0则会通过顶点移动到看不见的地方，避免被绘制
    /// </summary>
    [ExecuteInEditMode]
    public class StaticFontFixedLengthText : StaticFontBaseText
    {
        //目前最多允许15个font，1个是符号，10个是数字，4个是逗号
        private static int MAX_FONT_LEN = 15;

        public override void UpdateGraphics()
        {
            if (font)
            {
                if (font.dynamic)
                    font.RequestCharactersInTexture(text, GetRealFontSize());

                Profiler.BeginSample("HudTextMeshFixedLength: DoGenerateMeshByArr");
                DoGenerateMeshByArr();
                Profiler.EndSample();
            }
        }

        Vector3[] _vertArr = new Vector3[MAX_FONT_LEN * 4];
        Vector2[] _uvArr = new Vector2[MAX_FONT_LEN * 4];
        int[] _indicesArr = new int[MAX_FONT_LEN * 6];
        Color[] _colorArr = new Color[MAX_FONT_LEN * 4];

        //红色通道决定了顶点的Y轴初始的位置偏移，大于0就会在vertex shader中位移出去
        private Color _hidePosColor = new Color(1, 0, 0, 1);

        void DoGenerateMeshByArr()
        {
            if (text.Length > MAX_FONT_LEN)
            {
                Debug.LogError("Error! text.Length: " + text.Length + " big than 15(MAX_FONT_LEN)", gameObject);
                return;
            }

            m_bound.size = Vector3.zero;
            m_bound.center = Vector3.zero;

            float fontScale = GetFontScale();
            float xOff = 0;
            float yOff = 0;
            CharacterInfo ch;
            Vector3 vector = new Vector3();
            Color vertexColor = new Color(0, 0, 0, 1);

            for (int i = 0; i < MAX_FONT_LEN; ++i)
            {
                if (i < text.Length)
                {
                    bool ret = font.GetCharacterInfo(text[i], out ch, GetRealFontSize());
                    if (ret == false)
                        ch = default;

                    FillSingleWordByArr(fontScale, xOff, yOff, ref ch, ref vector, ref vertexColor, i);
                    xOff += ch.advance * fontScale + characterSpace;
                }
                else
                {
                    bool ret = font.GetCharacterInfo('0', out ch, GetRealFontSize());
                    FillSingleWordByArr(fontScale, xOff, yOff, ref ch, ref vector, ref _hidePosColor, i, false);
                }
                
            }

            for (int i = 0; i < _vertArr.Length; ++i)
                _vertArr[i] = _vertArr[i] - m_bound.center;

            m_mesh.Clear();
            m_mesh.SetVertices(_vertArr);
            m_mesh.SetUVs(0, _uvArr);
            m_mesh.SetColors(_colorArr);
            m_mesh.SetTriangles(_indicesArr, 0);
        }

        private void FillSingleWordByArr(float fontScale, float xOff, float yOffset, ref CharacterInfo ch, ref Vector3 vector,ref  Color color, int chIndex, bool isUpdateBounds = true)
        {
            int firsIndex = chIndex * 4;
            int secondIndex = firsIndex + 1;
            int thirdIndex = secondIndex + 1;
            int fourthIndex = thirdIndex + 1;

            vector.Set(xOff + ch.minX * fontScale, (ch.maxY + yOffset) * fontScale, 0);
            _vertArr[firsIndex] = vector;
            if (isUpdateBounds)
                m_bound.Encapsulate(vector);

            vector.Set(xOff + ch.maxX * fontScale, (ch.maxY + yOffset) * fontScale, 0);
            _vertArr[secondIndex] = vector;
            if (isUpdateBounds)
            m_bound.Encapsulate(vector);

            vector.Set(xOff + ch.maxX * fontScale, (ch.minY + yOffset) * fontScale, 0);
            _vertArr[thirdIndex] = vector;
            if (isUpdateBounds)
                m_bound.Encapsulate(vector);

            vector.Set(xOff + ch.minX * fontScale, (ch.minY + yOffset) * fontScale, 0);
            _vertArr[fourthIndex] = vector;
            if (isUpdateBounds)
                m_bound.Encapsulate(vector);

            _uvArr[firsIndex] = ch.uvTopLeft;
            _uvArr[secondIndex] = ch.uvTopRight;
            _uvArr[thirdIndex] = ch.uvBottomRight;
            _uvArr[fourthIndex] = ch.uvBottomLeft;

            _colorArr[firsIndex] = color;
            _colorArr[secondIndex] = color;
            _colorArr[thirdIndex] = color;
            _colorArr[fourthIndex] = color;

            int baseIndicesIndex = chIndex * 6;
            _indicesArr[baseIndicesIndex++] = firsIndex;
            _indicesArr[baseIndicesIndex++] = secondIndex;
            _indicesArr[baseIndicesIndex++] = thirdIndex;

            _indicesArr[baseIndicesIndex++] = firsIndex;
            _indicesArr[baseIndicesIndex++] = thirdIndex;
            _indicesArr[baseIndicesIndex++] = fourthIndex;
        }

        #region 外部使用的方法
        public void UpdateVertexColor(Color newColor)
        {
            if (m_mesh.vertexCount == 0)
                return;

            int needUpdateCount = text.Length * 4;
            for (int i = 0; i < needUpdateCount; i++)
                _colorArr[i] = newColor;

            //Debug.LogError("mesh.vertexCount = " + mesh.vertexCount + ", _colorArr.Length = " + _colorArr.Length);
            m_mesh.SetColors(_colorArr);
        }

        #endregion
    }

}