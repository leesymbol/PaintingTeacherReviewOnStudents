using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using SFB;
public class GenAssessment : MonoBehaviour {
    enum GenType
    {
        Type1,
        Type2

    }
    GenType genType = GenType.Type2;
    public Text info;
    public Text goodInfo;
    public Text badInfo;
    public Text nameInfo;

    public Slider goodSlider;
    public Slider badSlider;


    private string[] nameArr;
    private string[] startArr;
    private string[] goodArr;
    private string[] badArr;
    private string[] endArr;
    public int goodItemNum = 3;
    public int badItemNum = 1;
    private string final = string.Empty;
    private string textPath;
    private string startFile = "start";
    private string goodFile = "good";
    private string badFile = "bad";
    private string endFile = "end";
    private string suffix = ".txt";
    private string savePath = "";
    void Start () {

        //PrepareStrings();
        UpdateBadInfo(badSlider.value);
        UpdateGoodInfo(goodSlider.value);

        goodSlider.onValueChanged.AddListener(delegate { OnGoodValueChanged(); });
        badSlider.onValueChanged.AddListener(delegate { OnBadValueChanged(); });

    }
    private string namePath;
    public void StartReadName()
    {
        namePath = string.Empty;
        nameInfo.text = "";
        var paths = StandaloneFileBrowser.OpenFilePanel("读取学生姓名", "", "txt", false);
        if (paths.Length > 0) {
            namePath = paths[0];
            StartCoroutine(OutputRoutine(new System.Uri(namePath).AbsoluteUri));
        }
    }
    private IEnumerator OutputRoutine(string url) {
        var loader = new WWW(url);
        yield return loader;
        Debug.Log("读取到学生姓名为：" + loader.text);
        nameInfo.text = loader.text;
    }
    public void StartGen()
    {
        final = string.Empty;
        if(genType == GenType.Type1)
            PrepareStrings();
        else
            PrepareClass();

    }
    public void StartSave()
    {
        savePath = string.Empty;
        Debug.Log("Start Save !");
            var extensionList = new [] {
                new ExtensionFilter("Text", "txt"),
            };
            string date =  DateTime.Now.ToString("yyy-MM-dd-HH-mm-ss-ffff");
            string preName;
            preName = System.IO.Path.GetFileNameWithoutExtension(namePath);
            savePath = StandaloneFileBrowser.SaveFilePanel("保存路径", "", preName + "_" + date, extensionList);
            // Debug.Log("Final " + final);
            info.text = "生成评语保存在 " + savePath;
            try
            {
                WriteBytesToFile(savePath, GetBytes(final));
            }
            catch
            {

            }
    }
    void UpdateGoodInfo(float _value){
        goodInfo.text = "积极评语数量：" + _value;
    }
    void UpdateBadInfo(float _value)
    {
        badInfo.text = "消极评语数量：" + _value;
    }
    public void OnGoodValueChanged()
    {
        UpdateGoodInfo(goodSlider.value);
    }
    public void OnBadValueChanged()
    {
        UpdateBadInfo(badSlider.value);
    }
    void PrepareClass()
    {
        info.text = "";
        textPath = Application.streamingAssetsPath + "/Content/";
        string classPath = Application.streamingAssetsPath + "/Name/";

        string startPath = textPath + startFile + suffix;
        string goodPath = textPath + goodFile + suffix;
        string badPath = textPath + badFile + suffix;
        string endPath = textPath + endFile + suffix;


        startArr = FromFileReadByText(startPath).Split('\n');
        goodArr = FromFileReadByText(goodPath).Split('\n');
        badArr = FromFileReadByText(badPath).Split('\n');
        endArr = FromFileReadByText(endPath).Split('\n');
       

            Debug.Log("namePath " + namePath);

            nameArr = FromFileReadByText(namePath).Split('\n');
            for (int i = 0; i < nameArr.Length; i++)
            {
                string person = GetName(i) + ": " + GetStart() + GetGood() + GetBad() + GetEnd() + "\n";
                final += person;
            }
            if (!string.IsNullOrEmpty(final))
            {
                info.text += "合成成功: " + "\n";
                Debug.Log("合成成功: " + final);
                
            }
            else
            {

                info.text = "合成失败!";
                Debug.LogError("合成失败!");

            }
    }
    void PrepareStrings()
    {
        info.text = "";
        textPath = Application.streamingAssetsPath + "/Content/";
        string classPath = Application.streamingAssetsPath + "/Name/";
        string namePath;
        GetClass(classPath);
        string startPath = textPath + startFile + suffix;
        string goodPath = textPath + goodFile + suffix;
        string badPath = textPath + badFile + suffix;
        string endPath = textPath + endFile + suffix;


        startArr = FromFileReadByText(startPath).Split('\n');
        goodArr = FromFileReadByText(goodPath).Split('\n');
        badArr = FromFileReadByText(badPath).Split('\n');
        endArr = FromFileReadByText(endPath).Split('\n');
       
        for (int j = 0; j < classList.Count; j++)
        {
            namePath = classPath + classList[j];
            Debug.Log("namePath " + namePath);

            nameArr = FromFileReadByText(namePath).Split('\n');
            for (int i = 0; i < nameArr.Length; i++)
            {
                string person = GetName(i) + ": " + GetStart() + GetGood() + GetBad() + GetEnd() + "\n";
                final += person;
            }
            if (!string.IsNullOrEmpty(final))
            {
                info.text += "合成成功: " + "\n";
                Debug.Log("合成成功: ");
                
            }
            else
            {

                info.text = "合成失败!";
                Debug.LogError("合成失败!");

            }
            final = string.Empty;
        }

    }

    List<string> classList = new List<string>();
    void GetClass(string _path)
    {
        classList.Clear();
        DirectoryInfo root = new DirectoryInfo(_path);
        FileInfo[] files = root.GetFiles();
        for (int i = 0; i < files.Length;i++)
        {
            if(!classList.Contains(files[i].Name))
            {
                if(files[i].Name.EndsWith(".txt"))
                    classList.Add(files[i].Name);
                Debug.Log("files[i].Name " + files[i].Name);
            }

        }
    }
    IEnumerator LoadAssets(string path,Action<string> loadAssets)
    {
        yield return new WaitForEndOfFrame();
        WWW load = new WWW(path);
        yield return load;
        if (load.error != null)
        {
            Debug.LogError("LoadAsset Error " + load.error);
            yield break;
        }
        else
        {
            if (loadAssets != null)
                loadAssets(load.text);
        }
        load.Dispose();
    }
    public string GetStart()
    {
        string startStr = string.Empty;
        int rand = UnityEngine.Random.Range(0,2);
        if (rand == 1)
            startStr = RndomStr(startArr, 1);
        return startStr;
    }
    public string GetName(int i)
    {
        string nameStr = nameArr[i].Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
        return nameStr;
    }
    public string GetGood()
    {
       string goodStr = RndomStr(goodArr, goodItemNum);
       return goodStr;
    }
    public string GetBad()
    {
        string badStr = RndomStr(badArr, badItemNum);
        return badStr;
    }
    public string GetEnd()
    {
        string endStr = RndomStr(endArr, 1);
        return endStr;
    }
    private string RndomStr(string[] charArr, int codeLength)
    {
      

        string sResult = string.Empty;
        if (codeLength == 0)
            return sResult;

        List<int> result = new List<int>();
        while (true)
        {
            System.Random rd = new System.Random(Guid.NewGuid().GetHashCode());
            int a = rd.Next(charArr.Length);
            if (result.Contains(a))
                continue;
            result.Add(a);
            if (result.Count == codeLength)
                break;
        }

        for (int i = 0; i < result.Count; i++)
        {
            string infoTxt = charArr[result[i]];
            infoTxt = infoTxt.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
            sResult += infoTxt;
        }
        result.Clear();
        return sResult;
    }

    public static byte[] GetBytes(string str)
    {
        return System.Text.Encoding.UTF8.GetBytes(str);
    }

    public static string GetString(byte[] byts)
    {
        return System.Text.Encoding.UTF8.GetString(byts);
    }

    public static byte[] FromFileReadBytes(string path)
    {
        return File.ReadAllBytes(path);
    }

    /// <summary>
    /// 读取文件 读取字符串
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string FromFileReadByText(string path)
    {
        if (File.Exists(path))//如果有这个文件
        {
            return File.ReadAllText(path).Trim();
        }
        return null;
    }

    public static void WriteBytesToFile(string filePath, byte[] bytes)
    {
        string dirName = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dirName))
        {
            Directory.CreateDirectory(dirName);
        }
        Loom.RunAsync(() =>
        {
            File.WriteAllBytes(filePath, bytes);
        });
    }
}
