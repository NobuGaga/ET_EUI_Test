using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class StaticFontMaker : EditorWindow
{

    const int Border = 1;
    const string OutputPath = "DailyFish/ArtAB/Fonts";
    const string OutputFolderSuffix = "";

    static string originFolder, saveFolder;
    static string texSavePath, matSavePath, fontSavePath;
    static Texture2D atlasTex;

    bool isVisible;
    int space;


    //[MenuItem("Assets/Make Texture Font")]
    static void OpenWindow()
    {
        StaticFontMaker window = GetWindow<StaticFontMaker>();
        window.maxSize = window.minSize = new Vector2(350, 160);
        window.titleContent = new GUIContent("创建贴图字体");
        window.isVisible = true;
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("  说明：");
        EditorGUILayout.LabelField("\t1.支持文件名用小数点和中文。");
        EditorGUILayout.LabelField("\t2.小数点命名必须在外部文件夹操作。");
        EditorGUILayout.LabelField("\t3.小数点文件编辑器内不会显示，不影响字体生成。");
        EditorGUI.LabelField(new Rect(10, 90, 50, 30), "字符间距");
        space = EditorGUI.IntField(new Rect(70, 95, 269, 20), space);
        if (GUI.Button(new Rect(10, 130, 330, 20), "确定") || Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
        {
            if (isVisible)
            {
                Close();
                StartCreateFont(space);
                isVisible = false;
            }
        }
    }

    static void StartCreateFont(int space)
    {
        //获取目录内的素材
        Dictionary<string, Texture2D> textures = GetSelectedTextures();
        if (textures == null || textures.Count == 0)
        {
            return;
        }

        //将素材打包成图集
        Dictionary<string, Rect> sprites = TexturesToAtlas(textures);

        //根据图集生成字体
        bool success = CreateFontFiles(sprites, space);
        if (success)
        {
            AssetDatabase.Refresh();

            //定位到导出文件
            Object obj = AssetDatabase.LoadMainAssetAtPath(texSavePath);
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }
    }

    /// <summary>
    /// 创建字体文件和材质球
    /// </summary>
    static bool CreateFontFiles(Dictionary<string, Rect> sprites, int space)
    {
        Material mat;
        if (File.Exists(matSavePath))
        {
            mat = AssetDatabase.LoadAssetAtPath<Material>(matSavePath);
        }
        else
        {
            mat = new Material(Shader.Find("GUI/Text Shader"));
            AssetDatabase.CreateAsset(mat, matSavePath);
        }
        mat.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texSavePath);
        Font font;
        if (File.Exists(fontSavePath))
        {
            font = AssetDatabase.LoadAssetAtPath<Font>(fontSavePath);
        }
        else
        {
            font = new Font();
            AssetDatabase.CreateAsset(font, fontSavePath);
        }
        font.material = mat;
        CharacterInfo[] characterInfo = new CharacterInfo[sprites.Count + 1];
        int i = 0;
        int charW = 0;
        int charH = 0;
        foreach (var item in sprites)
        {
            Rect rect = item.Value;
            charW = (int)rect.width;
            charH = (int)rect.height;
            CharacterInfo info = new CharacterInfo();
            info.index = item.Key.ToCharArray()[0];
            //偏移
            rect.x -= space * 0.5f;
            //设置字符映射到材质上的坐标
            info.uvBottomLeft = new Vector2(rect.x / (float)atlasTex.width, rect.y / (float)atlasTex.height);
            info.uvBottomRight = new Vector2((rect.x + rect.width) / (float)atlasTex.width, rect.y / (float)atlasTex.height);
            info.uvTopLeft = new Vector2(rect.x / (float)atlasTex.width, (rect.y + rect.height) / (float)atlasTex.height);
            info.uvTopRight = new Vector2((rect.x + rect.width) / (float)atlasTex.width, (rect.y + rect.height) / (float)atlasTex.height);
            //字符顶点的偏移位置和宽高
            info.minX = 0;
            info.maxX = (int)rect.width;
            info.minY = -(int)(rect.height * 0.5f);
            info.maxY = (int)(rect.height * 0.5f);
            //字符间距  
            info.advance = (int)rect.width + space;
            characterInfo[i] = info;
            i++;
        }
        //支持空格
        CharacterInfo einfo = new CharacterInfo();
        einfo.index = (" ")[0];
        einfo.advance = charW + space;
        characterInfo[sprites.Count] = einfo;

        font.characterInfo = characterInfo;
        EditorUtility.SetDirty(font);

        AssetDatabase.SaveAssets();

        //某些属性无法设置，直接暴力修改文件内容
        SerializedObject serialized = new SerializedObject(AssetDatabase.LoadAssetAtPath<Object>(fontSavePath));
        serialized.Update();
        serialized.FindProperty("m_LineSpacing").floatValue = charH;
        serialized.FindProperty("m_FontSize").floatValue = 38f;
        serialized.ApplyModifiedProperties();
        serialized.SetIsDifferentCacheDirty();
        AssetDatabase.ImportAsset(fontSavePath);

        return true;
    }

    /// <summary>
    /// 将多张小图拼接成图集并保存,返回每个小图的位置坐标
    /// </summary>
    static Dictionary<string, Rect> TexturesToAtlas(Dictionary<string, Texture2D> textures)
    {
        Dictionary<string, Rect> sprites = new Dictionary<string, Rect>();

        int newTexWidth = 0;
        int newTexHeight = 0;
        foreach (var tex in textures.Values)
        {
            newTexWidth += tex.width;
            if (tex.height > newTexHeight)
            {
                newTexHeight = tex.height;
            }
        }
        newTexWidth += Border * 2 * textures.Values.Count;
        newTexHeight += Border * 2;
        atlasTex = new Texture2D(newTexWidth, newTexHeight, TextureFormat.RGBA32, false);
        int completedX = 0;
        foreach (var item in textures)
        {
            Texture2D tex = item.Value;
            int w = tex.width + Border * 2;
            int h = newTexHeight;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Color color;
                    if (x < Border || x > w - Border || y < Border || y > h - Border)
                    {
                        color = new Color(0, 0, 0, 0);
                    }
                    else
                    {
                        color = tex.GetPixel(x - Border, y - Border);
                    }
                    atlasTex.SetPixel(completedX + x, y, color);
                }
            }

            Rect rect = new Rect();
            rect.x = completedX;
            rect.y = 0;
            rect.width = tex.width + Border * 2;
            rect.height = tex.height + Border * 2;
            sprites.Add(item.Key, rect);

            completedX += Border * 2 + tex.width;
        }
        atlasTex.Apply();

        string texName = originFolder + ".png";
        string matName = originFolder + ".mat";
        string fontName = originFolder + ".fontsettings";
        string saveFolderPath = Application.dataPath + "/" + OutputPath + "/" + saveFolder;
        if (!Directory.Exists(saveFolderPath)) Directory.CreateDirectory(saveFolderPath);
        File.WriteAllBytes(saveFolderPath + "/" + texName, atlasTex.EncodeToPNG());

        texSavePath = "Assets/" + OutputPath + "/" + saveFolder + "/" + texName;
        matSavePath = "Assets/" + OutputPath + "/" + saveFolder + "/" + matName;
        fontSavePath = "Assets/" + OutputPath + "/" + saveFolder + "/" + fontName;

        AssetDatabase.ImportAsset(texSavePath);
        TextureImporter importer = AssetImporter.GetAtPath(texSavePath) as TextureImporter;
        if (importer)
        {
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            AssetDatabase.ImportAsset(texSavePath);
        }

        return sprites;
    }

    /// <summary>
    /// 获取当前选择的图片
    /// </summary>
    static Dictionary<string, Texture2D> GetSelectedTextures()
    {
        Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        DefaultAsset[] objs = Selection.GetFiltered<DefaultAsset>(SelectionMode.Assets);
        if (objs.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "请选择包含字符图片的文件目录", "确定");
            return textures;
        }
        List<string> allTexPaths = new List<string>();
        for (int i = 0; i < objs.Length; i++)
        {
            Object obj = objs[i];
            string folder = AssetDatabase.GetAssetPath(obj);
            string[] pngPaths = Directory.GetFiles(folder, "*.png", SearchOption.TopDirectoryOnly);
            for (int j = 0; j < pngPaths.Length; j++)
            {
                string texPath = pngPaths[j].Replace("\\", "/");
                if (!allTexPaths.Contains(texPath)) allTexPaths.Add(texPath);
            }
        }
        for (int i = 0; i < allTexPaths.Count; i++)
        {
            string texPath = allTexPaths[i];
            int index = texPath.LastIndexOf("/");
            string texName = texPath.Substring(index + 1).Replace(".png", "");
            if (texName.Length != 1)
            {
                EditorUtility.DisplayDialog("提示", "文件名[" + texName + "]必须是1位数的字母或数字或中文", "确定");
                return textures;
            }
            //设置为可读写，取消压缩，以便正确读取图片像素信息
            TextureImporter importer = AssetImporter.GetAtPath(texPath) as TextureImporter;
            if (importer)
            {
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.isReadable = true;
                importer.mipmapEnabled = false;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.textureCompression = TextureImporterCompression.Uncompressed;

                TextureImporterPlatformSettings androidSetting = importer.GetPlatformTextureSettings("Android");
                androidSetting.overridden = false;
                importer.SetPlatformTextureSettings(androidSetting);
                TextureImporterPlatformSettings iosSetting = importer.GetPlatformTextureSettings("iPhone");
                iosSetting.overridden = false;
                importer.SetPlatformTextureSettings(iosSetting);

                AssetDatabase.ImportAsset(texPath);
            }

            //这里不用AssetDatabase.LoadAssetAtPath(),否则读不到小数点命名的图片
            Texture2D texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
            texture.LoadImage(File.ReadAllBytes(texPath));
            textures.Add(texName, texture);
        }
        if (allTexPaths.Count > 0)
        {
            string[] arr = allTexPaths[0].Split('/');
            originFolder = arr[arr.Length - 2];
            saveFolder = originFolder + OutputFolderSuffix;
        }

        Selection.objects = null;

        return textures;
    }
}