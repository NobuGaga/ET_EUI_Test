using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StaticFont;
using UnityEngine.Profiling;

public class StaticFontTextController : MonoBehaviour
{
    static int _AlphaPropertyID = Shader.PropertyToID("_Alpha");

    [HideInInspector]
    public MeshRenderer _render;

    [HideInInspector]
    public StaticFontText _hudTextMesh;

    private float _animAlpha = 0f;
    private bool _isInAlphaAnim = false;
    private float _fadeAnimDuration = 0.5f;

    private float _alpha = 1f;

    public float Alpha
    {
        get
        {
            return _alpha;
        }

        set
        {
            SetAlpha(value);
        }
    }

    private void Awake()
    {
        _render = GetComponent<MeshRenderer>();
        if(_render == null)
        {
            Debug.LogError("HudTextController _render is null", gameObject);
            return;
        }

        _hudTextMesh = GetComponent<StaticFontText>();
        if (_hudTextMesh == null)
        {
            Debug.LogError("HudTextController _hudTextMesh is null", gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        _render = null;
        _hudTextMesh = null;
    }


    private void Update()
    {
        if (!_isInAlphaAnim)
            return;

        _animAlpha -= Time.deltaTime / _fadeAnimDuration;
        SetAlpha(_animAlpha);

        if (_animAlpha < 0)
        {
            _isInAlphaAnim = false;
        }
    }


    #region 外部调用
    public void SetAlpha(float alpha)
    {
        if (_render == null || _hudTextMesh == null)
            return;
        //Debug.Log("_hudTextMesh---"+ alpha);
        _hudTextMesh.SetAlpha(alpha);
        _alpha = alpha;
    }

    public float GetAlpha()
    {
        if (_render == null || _hudTextMesh == null)
            return 1f;

        return _hudTextMesh.color.a;
    }

    public void BeginFadeOut(float startAlpha, float duration, int easeType)
    {
        _fadeAnimDuration = Mathf.Max(duration, 0.01f);

        if (_isInAlphaAnim)
        {
            _animAlpha = startAlpha;
            return;
        }

        _animAlpha = startAlpha;
        _isInAlphaAnim = true;
    }

    public void SetNum(int number)
    {
        if (_render == null || _hudTextMesh == null)
            return;

        //todo 暂时转字符串，会有GC，后续优化,先保证lua层传递过来的是数字
        //todo 自动根据数字的为数解析逗号，正负号
        //_hudTextMesh.text = number.ToString();
        _hudTextMesh.SetNumber(number);
    }
    #endregion
}