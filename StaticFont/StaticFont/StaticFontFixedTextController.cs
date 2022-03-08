using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StaticFont;
using UnityEngine.Profiling;

public class StaticFontFixedTextController : MonoBehaviour
{
    public MeshRenderer _render;

    StaticFontFixedLengthText _hudTextMesh;
    MaterialPropertyBlock _mpb;

    static int _AlphaPropertyID = Shader.PropertyToID("_Alpha");

    private void Awake()
    {
        _render = GetComponent<MeshRenderer>();
        if(_render == null)
        {
            Debug.LogError("HudTextController _render is null", gameObject);
            return;
        }

        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();

        _hudTextMesh = GetComponent<StaticFontFixedLengthText>();
    }

    public void SetAlpha(float alpha)
    {
        if (_render == null)
            return;

        _render.GetPropertyBlock(_mpb);
        _mpb.SetFloat(_AlphaPropertyID, alpha);
        _render.SetPropertyBlock(_mpb);
    }

    bool _isSet0 = true;

    //todo 临时使用Update，后续抽象一个通用的接口，通过tick调用，减少c++到c#层的打断
    private void Update()
    {
        var alpha = Random.Range(0.1f, 2f);
        //SetAlpha(alpha);

        if (_hudTextMesh)
        {
            Profiler.BeginSample("HudFixedTextController.UpdateVertex");
            _hudTextMesh.UpdateVertexColor(new Color(0, 0, 0, alpha));
            Profiler.EndSample();

            Profiler.BeginSample("HudFixedTextController.UpdateText");
            _isSet0 = !_isSet0;
            if (_isSet0)
                _hudTextMesh.text = "200000";
            else
                _hudTextMesh.text = "100000";

            Profiler.EndSample();
        }
            
    }
}
