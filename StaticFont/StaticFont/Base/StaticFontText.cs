using UnityEngine;
using UnityEngine.Profiling;

namespace StaticFont
{
    [ExecuteInEditMode]
    public class StaticFontText : StaticFontBaseText
    {

        public override void UpdateGraphics()
        {
            if (font)
            {
                if (font.dynamic)
                    font.RequestCharactersInTexture(text, GetRealFontSize());

                Profiler.BeginSample("HudTextMesh: DoGenerateMesh");
                DoGenerateMesh();
                Profiler.EndSample();
            }
        }

        void DoGenerateMesh()
        {
            _verts.Clear();
            _uvs.Clear();
            _indices.Clear();
            _colors.Clear();

            m_bound.size = Vector3.zero;
            m_bound.center = Vector3.zero;

            float fontScale = GetFontScale();
            float xOff = 0;
            float yOff = 0;
            CharacterInfo ch;
            Vector3 vector = new Vector3();
            Color newColor = color;

            if (_numberArrLen > 0)
            {
                for (int i = _numberArrLen - 1; i >= 0; --i)
                {
                    bool ret = font.GetCharacterInfo((char)_numberArr[i], out ch, GetRealFontSize());
                    if (ret == false)
                        ch = default;

                    FillSingleWord(fontScale, xOff, yOff, ref ch, ref vector, ref newColor, i, true);
                    xOff += ch.advance * fontScale + characterSpace;
                }

                Vector3 centerOffset = new Vector3(xOff * _numberArrLen / 2 * fontScale, yOff / 2 * fontScale, 0);
                for (int i = 0; i < _verts.Count; ++i)
                    _verts[i] = _verts[i] - m_bound.center;
            }
            else
            {
                for (int i = 0; i < text.Length; ++i)
                {
                    bool ret = font.GetCharacterInfo(text[i], out ch, GetRealFontSize());
                    if (ret == false)
                        ch = default;

                    FillSingleWord(fontScale, xOff, yOff, ref ch, ref vector, ref newColor, i, true);

                    xOff += ch.advance * fontScale + characterSpace;
                }

                for (int i = 0; i < _verts.Count; ++i)
                    _verts[i] = _verts[i] - m_bound.center;
            }

            m_mesh.Clear();
            m_mesh.SetVertices(_verts);
            m_mesh.SetUVs(0, _uvs);
            m_mesh.SetColors(_colors);
            m_mesh.SetTriangles(_indices, 0);
        }

        private void FillSingleWord(float fontScale, float xOff, float yOffset, ref CharacterInfo ch, ref Vector3 vector, ref Color color, int chIndex, bool ifUpdateBounds = false)
        {
            vector.Set(xOff + ch.minX  * fontScale, (ch.maxY + yOffset) * fontScale, 0);
            _verts.Add(vector);
            if (ifUpdateBounds)
                m_bound.Encapsulate(vector);

            vector.Set(xOff + ch.maxX * fontScale, (ch.maxY + yOffset) * fontScale, 0);
            _verts.Add(vector);
            if (ifUpdateBounds)
                m_bound.Encapsulate(vector);

            vector.Set(xOff + ch.maxX * fontScale, (ch.minY + yOffset) * fontScale, 0);
            _verts.Add(vector);
            if (ifUpdateBounds)
                m_bound.Encapsulate(vector);

            vector.Set(xOff + ch.minX  * fontScale, (ch.minY + yOffset) * fontScale, 0);
            _verts.Add(vector);
            if (ifUpdateBounds)
                m_bound.Encapsulate(vector);

            _uvs.Add(ch.uvTopLeft);
            _uvs.Add(ch.uvTopRight);
            _uvs.Add(ch.uvBottomRight);
            _uvs.Add(ch.uvBottomLeft);

            for (int k = 0; k < 4; ++k)
                _colors.Add(color);

            _indices.Add(chIndex * 4);
            _indices.Add(chIndex * 4 + 1);
            _indices.Add(chIndex * 4 + 2);

            _indices.Add(chIndex * 4);
            _indices.Add(chIndex * 4 + 2);
            _indices.Add(chIndex * 4 + 3);
        }

        static int MAX_NUMBER_LEN = 15;
        //提供一个number的方法，当用作number的时候，可以避免gc
        byte[] _numberArr = new byte[MAX_NUMBER_LEN];   //反向存储的
        int _numberArrLen = 0;

        bool _isMinus = false;
        byte _zeroVal = (byte)'0';

        public void SetNumber(int number)
        {
            _isMinus = number < 0 ? true : false;
            _numberArrLen = 0;
            
            //注意，这里是反向存储
            while (number > 0)
            {
                int mod = number % 10;
                _numberArr[_numberArrLen++] = (byte)(mod + _zeroVal);
                number /= 10;

                if (_numberArrLen > MAX_NUMBER_LEN - 4) //数字最多11位，1位给符号，3位给逗号
                {
                    Debug.LogError("Error! _numberArrLen too large: " + _numberArrLen, gameObject);
                    break;
                }
            }

            _numberArr[_numberArrLen++] = _isMinus ? (byte)'-' : (byte)'+';
            dirty = true;
        }

        #region 外部调用方法

        public void SetAlpha(float alpha)
        {
            if (m_mesh.vertexCount <= 0)
            {
                //Debug.LogError("mesh.vertexCount is 0");
                return;
            }

            color = new Color(color.r, color.g, color.b, alpha);
            for (int i = 0; i < _colors.Count; i++)
                _colors[i] = color;

            m_mesh.SetColors(_colors);
        }

        #endregion
    }
}