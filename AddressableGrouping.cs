using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;

#if UNITY_EDITOR 
public class AddressableGrouping : EditorWindow
{
    [MenuItem("Window/AddressableGrouping")]
    public static void Window()
	{
        var window = EditorWindow.GetWindow(typeof(AddressableGrouping));
        window.maxSize = window.minSize = new Vector2(400,700);
    }

	void OnGUI()
	{

        EditorGUILayout.HelpBox("새로 추가 할 그룹들이 따라갈 설정 값을 넣어주세요",MessageType.Info,true);

        EditorGUILayout.ObjectField("SettingGroup", settingGroup,typeof(AddressableAssetGroup),true);

        GUILayout.Space(10);

        EditorGUILayout.HelpBox("새로 추가 할 항목들이 있는 그룹을 넣어주세요", MessageType.Info, true);

        EditorGUILayout.ObjectField("AddGroup", AddGroup, typeof(AddressableAssetGroup), true);

        GUILayout.Space(10);

        EditorGUILayout.HelpBox("번들에 들어갈 각 항목들의 최댓값 설정", MessageType.Info, true);
        EditorGUILayout.HelpBox("Sprite의 경우 같은 아틀라스가 설정되어 있는 경우 아틀라스별로 같은 번들에 들어갑니다(갯수제한 없음)", MessageType.Warning, true);

        EditorGUILayout.IntField("Font Limit",nFontLimit);
        EditorGUILayout.IntField("Image Limit", nImageLimit);
        EditorGUILayout.IntField("Sprite Limit", nSpriteLimit);
        EditorGUILayout.IntField("Prefab Limit", nPrefabLimit);
        EditorGUILayout.IntField("Ani Limit", nAniLimit);
        EditorGUILayout.IntField("Material Limit", nMaterialLimit);
        EditorGUILayout.IntField("Sound Limit", nSoundLimit);
        EditorGUILayout.IntField("Mesh Limit", nMeshLimit);
        EditorGUILayout.IntField("Shader Limit", nShaderLimit);
        EditorGUILayout.IntField("ETC Limit", nETCLimit);
        EditorGUILayout.IntField("Model Limit", nModelLimit);
        EditorGUILayout.IntField("Asset Limit", nAssetLimit);

        GUILayout.Space(20);

        if (GUILayout.Button("AllAddressable"))
        {
            AllAddressable();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("SelectAddressable"))
        {
            SelectAddressable();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("CheckAddressable"))
        {
            CheckAddressable();
        }
    }

	//폰트 "ttf" "fnt" "fontsettings" "asset(sdf)" "otf"
	//이미지 "png" "tga" "psd" "tif" "jpg" "PNG" "cubemap" "renderTexture" "hdr" "exr" "normal" "tiff"
	//프리팹 "prefab"
	//애니메이션 "anim" "controller"
	//머테리얼 "mat"
	//사운드 "mp3" "ogg" "wav"
	//모델링 "fbx" "FBX" "obj"
	//매쉬 "mesh"
	//쉐이더 "shader","shadervariants"
	//기타 "xml" "terrainlayer" "playable" "signal" "flare"


    public AddressableAssetGroup settingGroup;

    public AddressableAssetGroup AddGroup;

    AddressableAssetSettings m_addressableAssetSettings;
    List<(AddressableAssetGroup, List<AddressableAssetEntry>)> listItem;
    List<CheckList> listCheck;

    class CheckList
    {
        public List<string> listCheck = new List<string>();
        public int nLimit;
        public int nGroupCount;
        public int nCount;
        public string strName;

        //sprite구분용
        public string strNameSprite;
        public int nLimitSprite;
        public int nGroupCountSprite;
        public int nCountSprite;

        public bool bImage = false;

    }


    #region Init

    //[InfoBox("종류별 그룹 최댓값 설정")]
    public int nFontLimit = 100;
    public int nImageLimit = 30;
    public int nSpriteLimit = 600;
    public int nPrefabLimit = 300;
    public int nAniLimit = 1280;
    public int nMaterialLimit = 6000;
    public int nSoundLimit = 150;
    public int nMeshLimit = 50;
    public int nShaderLimit = 100;
    public int nETCLimit = 100;
    public int nModelLimit = 50;
    public int nAssetLimit = 60;

    void InitCheckList(out List<CheckList> listCheck)
    {
        listCheck = new List<CheckList>();

        List<string> strFont = new List<string>() { "ttf", "fnt", "fontsettings", "otf" };
        List<string> strImage = new List<string>() { "png", "tga", "psd", "tif", "jpg", "cubemap", "renderTexture", "hdr", "exr", "normal", "tiff" };
        List<string> strPrefab = new List<string>() { "prefab" };
        List<string> strAni = new List<string>() { "anim", "controller" };
        List<string> strMaterial = new List<string>() { "mat" };
        List<string> strSound = new List<string>() { "mp3", "ogg", "wav" };
        List<string> strMesh = new List<string>() { "mesh" };
        List<string> strModel = new List<string>() { "fbx", "obj" };
        List<string> strShader = new List<string>() { "shader", "shadervariants" };
        List<string> strETC = new List<string>() { "xml", "terrainlayer", "playable", "signal", "flare" };
        List<string> strAsset = new List<string>() { "asset" };

        CheckList checkLisFont = new CheckList();
        checkLisFont.listCheck = strFont;
        checkLisFont.nLimit = nFontLimit;
        checkLisFont.nCount = 0;
        checkLisFont.nGroupCount = 0;
        checkLisFont.strName = "Font_";
        listCheck.Add(checkLisFont);

        CheckList checkListImage = new CheckList();
        checkListImage.listCheck = strImage;
        checkListImage.nLimit = nImageLimit;
        checkListImage.nCount = 0;
        checkListImage.nGroupCount = 0;
        checkListImage.strName = "Image_";

        checkListImage.nLimitSprite = nSpriteLimit;
        checkListImage.nCountSprite = 0;
        checkListImage.nGroupCountSprite = 0;
        checkListImage.strNameSprite = "Sprite_";

        checkListImage.bImage = true;

        listCheck.Add(checkListImage);

        CheckList checkListPrefab = new CheckList();
        checkListPrefab.listCheck = strPrefab;
        checkListPrefab.nLimit = nPrefabLimit;
        checkListPrefab.nCount = 0;
        checkListPrefab.nGroupCount = 0;
        checkListPrefab.strName = "Prefab_";
        listCheck.Add(checkListPrefab);

        CheckList checkListAni = new CheckList();
        checkListAni.listCheck = strAni;
        checkListAni.nLimit = nAniLimit;
        checkListAni.nCount = 0;
        checkListAni.nGroupCount = 0;
        checkListAni.strName = "Animation_";
        listCheck.Add(checkListAni);

        CheckList checkListMaterial = new CheckList();
        checkListMaterial.listCheck = strMaterial;
        checkListMaterial.nLimit = nMaterialLimit;
        checkListMaterial.nCount = 0;
        checkListMaterial.nGroupCount = 0;
        checkListMaterial.strName = "Material_";
        listCheck.Add(checkListMaterial);

        CheckList checkListSound = new CheckList();
        checkListSound.listCheck = strSound;
        checkListSound.nLimit = nSoundLimit;
        checkListSound.nCount = 0;
        checkListSound.nGroupCount = 0;
        checkListSound.strName = "Sound_";
        listCheck.Add(checkListSound);

        CheckList checkListMesh = new CheckList();
        checkListMesh.listCheck = strMesh;
        checkListMesh.nLimit = nMeshLimit;
        checkListMesh.nCount = 0;
        checkListMesh.nGroupCount = 0;
        checkListMesh.strName = "Mesh_";
        listCheck.Add(checkListMesh);

        CheckList checkListShader = new CheckList();
        checkListShader.listCheck = strShader;
        checkListShader.nLimit = nShaderLimit;
        checkListShader.nCount = 0;
        checkListShader.nGroupCount = 0;
        checkListShader.strName = "Shader_";
        listCheck.Add(checkListShader);

        CheckList checkListETC = new CheckList();
        checkListETC.listCheck = strETC;
        checkListETC.nLimit = nETCLimit;
        checkListETC.nCount = 0;
        checkListETC.nGroupCount = 0;
        checkListETC.strName = "ETC_";
        listCheck.Add(checkListETC);

        CheckList checkListModel = new CheckList();
        checkListModel.listCheck = strModel;
        checkListModel.nLimit = nModelLimit;
        checkListModel.nCount = 0;
        checkListModel.nGroupCount = 0;
        checkListModel.strName = "Model_";
        listCheck.Add(checkListModel);

        CheckList checkListAsset = new CheckList();
        checkListAsset.listCheck = strAsset;
        checkListAsset.nLimit = nAssetLimit;
        checkListAsset.nCount = 0;
        checkListAsset.nGroupCount = 0;
        checkListAsset.strName = "AssetFile_";
        listCheck.Add(checkListAsset);
    }

    #endregion


    public void AllAddressable()
    {
        if (settingGroup == null)
        {
            Debug.LogError("settingGroup = null");
        }
        else
        {
            AllSettingAddressable();

        }
    }

    void SelectAddressable()
    {
        if (settingGroup == null || AddGroup == null)
        {
            Debug.LogError("settingGroup = null || AddGroup = null");
            return;
        }

        InitCheckList(out listCheck);

        m_addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
        float per = 0;
        int nMaxCount = 0;
        int nCount = 0;

        foreach (var entry in AddGroup.entries)
        {
            nMaxCount++;
        }

        var entries = new List<AddressableAssetEntry>(AddGroup.entries);

        EditorUtility.DisplayProgressBar("loading", $"({nCount} / {nMaxCount})", per);
        foreach (var entry in entries)
        {
            if (entry.IsScene)//씬
            {
                var findGroup = m_addressableAssetSettings.FindGroup("SceneAsset");
                if (findGroup == null)
                {
                    findGroup = m_addressableAssetSettings.CreateGroup("SceneAsset", false, false, true, settingGroup.Schemas, settingGroup.SchemaTypes.ToArray());
                }

                if (entry.parentGroup != findGroup)
                {
                    m_addressableAssetSettings.MoveEntry(entry, findGroup);
                }

                continue;
            }

            var strSplit = entry.AssetPath.Split('.');
            int nLength = strSplit.Length;
            var extension = strSplit[nLength - 1];

            Check2(extension, entry);

            nCount++;
        }

        EditorUtility.ClearProgressBar();
    }

    void CheckAddressable()
    {
        m_addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
        var a = m_addressableAssetSettings.groups;

        string[] test = new string[] { "mesh", "tiff", "hdr", "normal", "exr", "flare", "signal", "obj", "playable", "renderTexture", "cubemap", "shader", "shadervariants", "jpg", "ttf", "fnt", "fontsettings", "asset", "otf", "png", "tga", "wav", "TGA", "psd", "prefab", "anim", "controller", "mat", "mp3", "ogg", "tif", "fbx", "terrainlayer", "xml" };

        foreach (var b in a)
        {
            foreach (var c in b.entries)
            {
                if (c.IsScene)
                {
                    break;
                }

                var d = c.AssetPath.Split('.');
                bool check = false;
                foreach (var g in test)
                {
                    int n = d.Length;
                    if (d[n - 1].ToLower() == g.ToLower())
                    {
                        check = true;
                        break;
                    }
                }

                if (!check)
                {
                    if (d.Length >= 2)
                    {
                        Debug.LogError(d[0] + " " + d[1]);
                    }
                    else
                    {
                        Debug.LogError(d[0] + "" + c.ToString());
                    }

                }

            }
        }
    }



    void AllSettingAddressable()
    {
        InitCheckList(out listCheck);

        m_addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
        var groups = new List<AddressableAssetGroup>(m_addressableAssetSettings.groups);
        listItem = new List<(AddressableAssetGroup, List<AddressableAssetEntry>)>();
        float per = 0;
        int nMaxCount = 0;
        int nCount = 0;

        foreach (var group in groups)
        {
            foreach (var entry in group.entries)
            {
                nMaxCount++;
            }
        }

        foreach (var group in groups)
        {
            EditorUtility.DisplayProgressBar("loading", $"({nCount} / {nMaxCount})", per);
            foreach (var entry in group.entries)
            {
                if (entry.IsScene)//씬
                {
                    var findGroup = m_addressableAssetSettings.FindGroup("SceneAsset");
                    if (findGroup == null)
                    {
                        findGroup = m_addressableAssetSettings.CreateGroup("SceneAsset", false, false, true, settingGroup.Schemas, settingGroup.SchemaTypes.ToArray());
                    }

                    if (entry.parentGroup != findGroup)
                    {
                        int index = listItem.FindIndex(a => a.Item1 == findGroup);
                        if (index == -1)
                        {
                            List<AddressableAssetEntry> addressableAssetEntries = new List<AddressableAssetEntry>();
                            addressableAssetEntries.Add(entry);
                            listItem.Add((findGroup, addressableAssetEntries));
                        }
                        else
                        {
                            listItem[index].Item2.Add(entry);
                        }
                    }

                    continue;
                }

                var strSplit = entry.AssetPath.Split('.');
                int nLength = strSplit.Length;
                var extension = strSplit[nLength - 1];

                Check(extension, entry);

                nCount++;
            }
        }

        MoveEntry();

        EditorUtility.ClearProgressBar();
    }

    TextureImporter textureImporter;
    void Check(string extension, AddressableAssetEntry entry)
    {
        if (listCheck == null)
        {
            Debug.LogError("listCheck == null");
            return;
        }

        bool bCheck = false;
        bool bSprite = false;
        string groupName = "";
        bool bAtlas = false;
        string AtlasName = "";

        for (int i = 0; i < listCheck.Count; ++i)
        {
            if (listCheck[i].listCheck.FindIndex(a => a.ToLower() == extension.ToLower()) != -1)
            {
                //sprite
                if (listCheck[i].bImage)
                {
                    textureImporter = AssetImporter.GetAtPath(entry.AssetPath) as TextureImporter;

                    if (textureImporter != null)
                    {
                        if (textureImporter.textureType == TextureImporterType.Sprite)
                        {
                            bSprite = true;
                            AtlasName = textureImporter.spritePackingTag;
                        }

                    }
                    else
                    {
                        Debug.LogError(entry.AssetPath + " : image not find TextureImporter");
                    }
                }
                if (AtlasName == "")
                {
                    groupName = bSprite ? listCheck[i].strNameSprite + listCheck[i].nGroupCountSprite : listCheck[i].strName + listCheck[i].nGroupCount;
                }
                else
                {
                    groupName = listCheck[i].strNameSprite + AtlasName;
                    bAtlas = true;
                }

                var group = m_addressableAssetSettings.FindGroup(groupName);
                if (group == null)
                {
                    group = m_addressableAssetSettings.CreateGroup(groupName, false, false, true, settingGroup.Schemas, settingGroup.SchemaTypes.ToArray());
                }

                //               if (entry.parentGroup == group)
                //{
                //                   bCheck = true;
                //                   break;
                //               }


                //if (group.entries.Count >= listCheck[i].nLimit)
                //{
                //	if (bSprite)
                //	{
                //                       listCheck[i].nGroupCountSprite++;
                //                   }
                //	else
                //	{
                //                       listCheck[i].nGroupCount++;
                //                   }

                //                   --i;
                //                   continue;
                //}

                int index = listItem.FindIndex(a => a.Item1 == group);
                if (index == -1)
                {
                    List<AddressableAssetEntry> addressableAssetEntries = new List<AddressableAssetEntry>();
                    addressableAssetEntries.Add(entry);
                    listItem.Add((group, addressableAssetEntries));
                }
                else
                {
                    listItem[index].Item2.Add(entry);
                }

                if (!bAtlas)
                {
                    if (bSprite)
                    {
                        listCheck[i].nCountSprite++;
                    }
                    else
                    {
                        listCheck[i].nCount++;
                    }

                    if (bSprite)
                    {
                        if (listCheck[i].nCountSprite >= listCheck[i].nLimitSprite)
                        {
                            listCheck[i].nCountSprite = 0;
                            listCheck[i].nGroupCountSprite++;
                        }
                    }
                    else
                    {
                        if (listCheck[i].nCount >= listCheck[i].nLimit)
                        {
                            listCheck[i].nCount = 0;
                            listCheck[i].nGroupCount++;
                        }
                    }
                }

                bCheck = true;
                break;
            }
        }

        if (!bCheck)
        {
            Debug.LogError("포함되는것이 없음 : " + entry.AssetPath);
        }

    }
    void Check2(string extension, AddressableAssetEntry entry)
    {
        if (listCheck == null)
        {
            Debug.LogError("listCheck == null");
            return;
        }

        bool bCheck = false;
        bool bSprite = false;
        bool bAtlas = false;
        string groupName = "";
        string AtlasName = "";
        for (int i = 0; i < listCheck.Count; ++i)
        {
            if (listCheck[i].listCheck.FindIndex(a => a.ToLower() == extension.ToLower()) != -1)
            {
                //sprite
                if (listCheck[i].bImage)
                {
                    textureImporter = AssetImporter.GetAtPath(entry.AssetPath) as TextureImporter;

                    if (textureImporter != null)
                    {
                        if (textureImporter.textureType == TextureImporterType.Sprite)
                        {
                            bSprite = true;
                            AtlasName = textureImporter.spritePackingTag;
                        }

                    }
                    else
                    {
                        Debug.LogError(entry.AssetPath + " : image not find TextureImporter");
                    }
                }

                if (AtlasName == "")
                {
                    groupName = bSprite ? listCheck[i].strNameSprite + listCheck[i].nGroupCountSprite : listCheck[i].strName + listCheck[i].nGroupCount;
                }
                else
                {
                    groupName = listCheck[i].strNameSprite + AtlasName;
                    bAtlas = true;
                }

                var group = m_addressableAssetSettings.FindGroup(groupName);
                if (group == null)
                {
                    group = m_addressableAssetSettings.CreateGroup(groupName, false, false, true, settingGroup.Schemas, settingGroup.SchemaTypes.ToArray());
                }

                if (!bAtlas)
                {
                    if (group.entries.Count >= listCheck[i].nLimit)
                    {
                        if (bSprite)
                        {
                            listCheck[i].nGroupCountSprite++;
                        }
                        else
                        {
                            listCheck[i].nGroupCount++;
                        }

                        --i;
                        continue;
                    }
                }

                m_addressableAssetSettings.MoveEntry(entry, group);

                bCheck = true;
                break;
            }
        }

        if (!bCheck)
        {
            Debug.LogError("포함되는것이 없음 : " + entry.AssetPath);
        }

    }
    void MoveEntry()
    {
        float per = 0;
        float i = 0;
        EditorUtility.DisplayProgressBar("MoveEntry", $"({i} / {listItem.Count})", per);
        foreach (var item in listItem)
        {
            per = i / listItem.Count;
            m_addressableAssetSettings.MoveEntries(item.Item2, item.Item1);
            i++;
            EditorUtility.DisplayProgressBar("loading", $"({i} / {listItem.Count})", per);
        }
    }
}
#endif
