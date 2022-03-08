using System.Collections.Generic;
using UnityEngine;

namespace StaticFont
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public abstract class StaticFontBaseText : MonoBehaviour
    {

        protected Bounds m_bound;
        public Bounds Bound => m_bound;

        protected Mesh m_mesh = null;

        protected List<Vector3> _verts = new List<Vector3>();
        protected List<Vector2> _uvs = new List<Vector2>();
        protected List<int> _indices = new List<int>();
        protected List<Color> _colors = new List<Color>();

        protected bool m_isDirty = true;
        public bool dirty
        {
            get { return m_isDirty; }
            set { m_isDirty = value; }
        }

        [SerializeField]
        private Color _color = Color.white;
        public Color color
        {
            get { return _color; }
            set
            {
                _color = value;
                m_isDirty = true;
            }
        }

        #region Unity Life Cycle

        void Awake()
        {
            if (m_mesh == null)
            {
                m_mesh = new Mesh();
                m_mesh.name = name;
            }

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = m_mesh;

            // TODO 暂时通过预设实例化原有数据, 后面有时间就改成动态的
            //MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            //Debug.Log("meshRenderer.sharedMaterials.Length = " + meshRenderer.sharedMaterials.Length);

            m_isDirty = true;
        }

        void LateUpdate()
        {
            UpdateGraphicsDirty();
        }

        #endregion

        void UpdateGraphicsDirty()
        {
            //if (_dirty)
            //{
                UpdateGraphics();
                m_isDirty = false;
            //}
        }

        public virtual void UpdateGraphics()
        {

        }

        [SerializeField]
        private string _text = "";
        public string text
        {
            get { return _text.Length > 0 ? _text : " "; }
            set
            {
#if !UNITY_EDITOR
                if (_text != value)
#endif
                {
                    _text = value;
                    m_isDirty = true;
                }
            }
        }

        [SerializeField]
        private Font _font;
        public Font font
        {
            get { return _font; }
            set
            {
#if !UNITY_EDITOR
                if (_font != value)
#endif
                {
                    _font = value;
                    m_isDirty = true;
                }
            }
        }

        [SerializeField]
        private int _fontSize = 32;
        public int fontSize
        {
            get { return _fontSize; }
            set
            {
#if !UNITY_EDITOR
                if (_fontSize != value)
#endif
                {
                    _fontSize = value;
                    m_isDirty = true;
                }
            }
        }

        [SerializeField]
        private int _characterSpace = 0;
        public int characterSpace
        {
            get { return _characterSpace; }
            set
            {
#if !UNITY_EDITOR
                if (_characterSpace != value)
#endif
                {
                    _characterSpace = value;
                    m_isDirty = true;
                }
            }
        }

        [SerializeField]
        private bool _showOutline = true;
        public bool showOutline
        {
            get { return _showOutline; }
            set
            {
#if !UNITY_EDITOR
                if (_showOutline != value)
#endif
                {
                    _showOutline = value;
                    m_isDirty = true;
                }
            }
        }

        [SerializeField]
        private Color _outlineColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        public Color outlineColor
        {
            get { return _outlineColor; }
            set
            {
#if !UNITY_EDITOR
                if (_outlineColor != value)
#endif
                {
                    _outlineColor = value;
                    m_isDirty = true;
                }
            }
        }

        [SerializeField]
        private float _outlineDistance = 0.8f;
        public float outlineDistance
        {
            get { return _outlineDistance; }
            set
            {
#if !UNITY_EDITOR
                if (_outlineDistance != outlineDistance)
#endif
                {
                    _outlineDistance = value;
                    m_isDirty = true;
                }
            }
        }

        protected int GetRealFontSize()
        {
            int fontSize = font.fontSize > 0 ? font.fontSize : 1;
            return font.dynamic ? fontSize : fontSize;
        }

        // when the font is dynamic ,the scale is 1;else the scale is fontsize/font.fontsize
        protected float GetFontScale()
        {
            float fontSize = font.fontSize > 0f ? font.fontSize : 1f;

            return font.dynamic ? 1f : (float)fontSize / fontSize;
        }

        void Start()
        {
            Font.textureRebuilt += OnFontTextureRebuilt;

            m_isDirty = true;
        }

        void OnFontTextureRebuilt(Font changedFont)
        {
            if (changedFont != font)
                return;

            UpdateGraphics();
        }

        void OnDestroy()
        {
            Font.textureRebuilt -= OnFontTextureRebuilt;
        }
    }
}