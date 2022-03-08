using MultiLanguage;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    // 设置菜单位置在 Text 后面 10
    [AddComponentMenu("UI/Text - MultiLanguage", 10)]
    public sealed class MultiLanguageText : Text
    {
        // 文本表 ID
        [SerializeField] string m_Key = string.Empty;

        protected override void Awake()
        {
            base.Awake();
            SetText();
        }

        public string Key
        {
            set
            {
                m_Key = value;
                SetText();
            }
        }

        // TODO 待测试运行时给 Key 值赋值看看读取问题
        private void SetText()
        {
            string text = LanguageHelper.GetText(m_Key);
            base.text = text;
        }
    }
}