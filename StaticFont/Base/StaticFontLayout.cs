using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StaticFont
{
    /// <summary>
    /// 静态字体自动布局组件
    /// </summary>
    [ExecuteInEditMode]
    public class StaticFontLayout : MonoBehaviour
    {
        // 使用新 API 获取组件, 减少 GC, 用一个静态列表储存
        private static List<StaticFontBaseText> m_layoutElementCache = new List<StaticFontBaseText>(128);
        private static Vector3 m_cacheVector3 = Vector3.zero;

        // 布局排列设置方法
        private delegate float SetElementPositionFunc(float nextPos, StaticFontBaseText element);

        // 多个变量修改使用一个脏标记, 防止重复调用多次渲染
        private bool m_isDirty = true;

        // 布局排列方式
        [SerializeField]
        private Scrollbar.Direction m_layoutType = Scrollbar.Direction.LeftToRight;
        public Scrollbar.Direction LayoutType
        {
            get => m_layoutType;
            set
            {
                m_layoutType = value;
                m_isDirty = true;
            }
        }

        // 每个元素间隔
        [SerializeField]
        private float m_space = 0;
        public float Space
        {
            get => m_space;
            set
            {
                m_space = value;
                m_isDirty = true;
            }
        }

        // 开始布局排列的偏移量
        [SerializeField]
        private Vector2 m_offset = Vector2.zero;
        public Vector2 Offset
        {
            get => m_offset;
            set
            {
                m_offset = value;
                m_isDirty = true;
            }
        }

        void LateUpdate()
        {
#if UNITY_EDITOR
            RefreshLayout();
#else
            if (!m_isDirty)
                return;

            RefreshLayout();
            m_isDirty = false;
#endif
        }

        // 刷新布局, 根据布局类型使用不同布局方法进行设置
        private void RefreshLayout()
        {
            GetComponentsInChildren(m_layoutElementCache);
            if (m_layoutElementCache.Count <= 0)
                return;

            float startPosX = m_offset.x;
            float startPosY = m_offset.y;
            float nextPos = 0;

            SetElementPositionFunc func = null;
            switch (m_layoutType)
            {
                case Scrollbar.Direction.LeftToRight:
                    nextPos = startPosX;
                    func = SetLeftToRightElementPos;
                    break;
                case Scrollbar.Direction.RightToLeft:
                    nextPos = startPosX;
                    func = SetRightToLeftElementPos;
                    break;
                case Scrollbar.Direction.TopToBottom:
                    nextPos = startPosY;
                    func = SetTopToBottomElementPos;
                    break;
                case Scrollbar.Direction.BottomToTop:
                    nextPos = startPosY;
                    func = SetBottomToTopElementPos;
                    break;
            }

            m_cacheVector3.Set(startPosX, startPosY, 0);
            foreach (StaticFontBaseText element in m_layoutElementCache)
            {
                if (!element.gameObject.activeSelf)
                    continue;

                element.transform.localPosition = m_cacheVector3;
                nextPos = func(nextPos, element);
            }
        }

        #region 具体设置方法实现不在方法里进行判断, 直接使用指定方法名进行对应的计算, 提高循环执行效率

        private float SetLeftToRightElementPos(float nextPos, StaticFontBaseText element)
        {
            nextPos += element.Bound.size.x + m_space;
            m_cacheVector3.x = nextPos;
            return nextPos;
        }

        private float SetRightToLeftElementPos(float nextPos, StaticFontBaseText element)
        {
            nextPos -= element.Bound.size.x + m_space;
            m_cacheVector3.x = nextPos;
            return nextPos;
        }

        private float SetTopToBottomElementPos(float nextPos, StaticFontBaseText element)
        {
            nextPos -= element.Bound.size.y + m_space;
            m_cacheVector3.y = nextPos;
            return nextPos;
        }

        private float SetBottomToTopElementPos(float nextPos, StaticFontBaseText element)
        {
            nextPos += element.Bound.size.y + m_space;
            m_cacheVector3.y = nextPos;
            return nextPos;
        }

        #endregion
    }
}