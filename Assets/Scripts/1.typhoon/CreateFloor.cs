using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class CreateFloor : MonoBehaviour
{

    public GameObject floor;
    public Material floorMat;
    public GameObject maincamera, aidcamera, curvecamera;

    string DataPath;

    #region 界面变量
    public Dropdown variables_dropdown, show_dropdown;
    public InputField Animation_LayerNumber;
    string dropdown_string;
    public Button button1;
    Coroutine coroutine;
    private int NowPlayNumber = 1;
    private int TotalFrameNumber = 121;
    private int layerMark = 15;
    #endregion

    //public float Opacity = 0.85f,
    //       FloorGap = 0.1f;

    //private Mesh floorMesh;

    //public List<Vector3>[] MeshVertices = new List<Vector3>[] { };
    Ty_Class ty = new Ty_Class();

    List<string> WeatherVariableNames;

    int LayerNumber = 15,
        Logitude = 0,
        Latitude = 0;
    // Use this for initialization
    void Start()
    {
        //if (null == floor)
        //{
        //    Debug.Log("floor is null");
        //}
        //else
        //{
        //    //Debug.Log("Submeshes: " + floor.GetComponent<MeshFilter>().mesh.subMeshCount);
        //}
        //maincamera = GameObject.Find("MainViewCamera");
        //aidcamera = GameObject.Find("AidViewCamera");

        maincamera.SetActive(true);
        aidcamera.SetActive(false);
        curvecamera.SetActive(false);

        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        
        //WeatherVariableNames = new List<string>() { "Cloud" };

        //coroutine = StartCoroutine(ShowAllTimeSimulation());
        //StartCoroutine(IExtractVariables());

        //dropdown_string = "ShowAllTimeSimulation";
        //StartCoroutine(GetNcMax());
    }

    // Update is called once per frame
    void Update()
    {
        //variables_dropdown 变量变化
        if (variables_dropdown.value == 0)
        {
            WeatherVariableNames = new List<string>() { "Cloud" };
        }
        else if (variables_dropdown.value == 1)
        {
            WeatherVariableNames = new List<string>() { "Rain" };
        }
        else if (variables_dropdown.value == 2)
        {
            WeatherVariableNames = new List<string>() { "Ice" };
        }
        else if (variables_dropdown.value == 3)
        {
            WeatherVariableNames = new List<string>() { "Snow" };
        }
        else if (variables_dropdown.value == 4)
        {
            WeatherVariableNames = new List<string>() { "Graupel" };
        }
        else if (variables_dropdown.value == 5)
        {
            WeatherVariableNames = new List<string>() { "All" };
        }

        //variables_dropdown 变量变化
        if (show_dropdown.value == 0)
        {
            dropdown_string = "ShowAllTimeSimulation";
        }
        else if (show_dropdown.value == 1)
        {
            dropdown_string = "ShowAllTimeSomeLayerSimulation";
        }

        //Animation_LayerNumber 变量变化
        if (Animation_LayerNumber.text != "")
        {
            //Debug.Log(Animation_LayerNumber.text);
            int L = Convert.ToInt16(Animation_LayerNumber.text);
            if (L > -1 && L < 450)
                LayerNumber = L;
            //Debug.Log("LayerNumber: " + LayerNumber);
        }
        
    }

    public void AnimationShow()
    {
        //foreach (string s in WeatherVariableNames)
        //    Debug.Log(s);
        StopAllCoroutines();
        ClearMesh();

        StartCoroutine(dropdown_string);

        maincamera.SetActive(true);
        aidcamera.SetActive(false);
        curvecamera.SetActive(false);
    }

    public void Clear()
    {
        //StopAllCoroutines();
        StopCoroutine("ShowAllTimeSimulation");
        StopCoroutine("ShowAllTimeSomeLayerSimulation");
        Debug.Log("Stop Coroutines");
        ClearMesh();
    }


    void OnGUI()
    {
        //if (GUI.Button(new Rect(50, 100, 80, 40), "全模型显示"))
        //{
        //    StopAllCoroutines();
        //    StartCoroutine("ShowAllTimeSimulation");
        //}
        //if (GUI.Button(new Rect(50, 100, 80, 40), "雨显示"))
        //{
        //    WeatherVariableNames = new List<string>() { "Rain" };
        //    StartCoroutine(ShowAllTimeSimulation(ty));
        //}
        //if (GUI.Button(new Rect(50,150, 80, 40), "冰显示"))
        //{
        //    WeatherVariableNames = new List<string>() { "Ice" };
        //    StartCoroutine(ShowAllTimeSimulation(ty));
        //}
        if (GUI.Button(new Rect(200, 150, 80, 40), "停止"))
        {
            StopAllCoroutines();
        }

        //相机切换
        if (GUI.Button(new Rect(350, 150, 80, 40), "切换相机"))
        {
            if (maincamera.activeInHierarchy == true)
            {
                Debug.Log("main");
                maincamera.SetActive(false);
                aidcamera.SetActive(true);
            }
            else
            {
                Debug.Log("aid");
                aidcamera.SetActive(false);
                maincamera.SetActive(true);
            }
        }

        // 帧数复位
        if (GUI.Button(new Rect(500, 150, 80, 40), "帧数复位"))
        {
            NowPlayNumber = 1;
        }
    }


    IEnumerator ShowAllTimeSimulation()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        for (int i = NowPlayNumber; i < TotalFrameNumber; ++i)
        {
            if (i < 10)
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_00" + i + ".nc";
            else if (i < 100)
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_0" + i + ".nc";
            else
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_" + i + ".nc";
            
            //string DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_d03_3times.nc";
            if (!System.IO.File.Exists(DataPath))
                Debug.Log("File doesn't exist!  " + DataPath);
            else
                Debug.Log(DataPath);
            ty = new Ty_Class(DataPath, floor, floorMat, WeatherVariableNames);

            for (int t = 0; t < ty.timeCount; ++t)
            {
                foreach (string name in WeatherVariableNames)
                {
                    ty.ShowSomeVariableSimulation(name, t);
                    Debug.Log(layerMark);
                    ty.setLayerMark(name, "z", layerMark);
                    yield return null;
                }

                if (WeatherVariableNames.Contains("Cloud"))
                    ty.oriCloudData[t].Clear();
                if (WeatherVariableNames.Contains("Rain"))
                    ty.oriRainData[t].Clear();
                if (WeatherVariableNames.Contains("Ice"))
                    ty.oriIceData[t].Clear();
                if (WeatherVariableNames.Contains("Snow"))
                    ty.oriSnowData[t].Clear();
                if (WeatherVariableNames.Contains("Grauoel"))
                    ty.oriGraupData[t].Clear();
                if (WeatherVariableNames.Contains("All"))
                    ty.oriMixData[t].Clear();
                Debug.Log("已清除");

                yield return new WaitForEndOfFrame();
            }

            ty.oriLatData.Clear();
            ty.oriLogData.Clear();
            foreach (List<Vector3> points in ty.VoltexPoints)
                points.Clear();
            Debug.Log(ty.oriLogData.Count);

            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            Debug.Log("读取进度 " + 0.833 * i + "%...");

            //记录当前读取帧数
            NowPlayNumber = i;

            ///*yield*/ return new WaitForEndOfFrame();
        }
        //播放帧数复位
        NowPlayNumber = 1;

        sw.Stop();
        Debug.Log("用时: " + sw.Elapsed);
    }

    IEnumerator ShowAllTimeSomeLayerSimulation()
    {
        for (int i = NowPlayNumber; i < TotalFrameNumber; ++i) 
        {
            if (i < 10)
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_00" + i + ".nc";
            else if (i < 100)
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_0" + i + ".nc";
            else
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_" + i + ".nc";

            //string DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_d03_3times.nc";
            if (!System.IO.File.Exists(DataPath))
                Debug.Log("File doesn't exist!  " + DataPath);
            else
                Debug.Log(DataPath);
            ty = new Ty_Class(DataPath, floor, floorMat, WeatherVariableNames);

            for (int t = 0; t < ty.timeCount; ++t)
            {
                foreach (string name in WeatherVariableNames)
                {
                    ty.ShowSomeVariableSomeLayerSimulation(name, t, LayerNumber, "z");
                    yield return null;
                }

                if (WeatherVariableNames.Contains("Cloud"))
                    ty.oriCloudData[t].Clear();
                if (WeatherVariableNames.Contains("Rain"))
                    ty.oriRainData[t].Clear();
                if (WeatherVariableNames.Contains("Ice"))
                    ty.oriIceData[t].Clear();
                if (WeatherVariableNames.Contains("Snow"))
                    ty.oriSnowData[t].Clear();
                if (WeatherVariableNames.Contains("Grauoel"))
                    ty.oriGraupData[t].Clear();
                if (WeatherVariableNames.Contains("All"))
                    ty.oriMixData[t].Clear();
                Debug.Log("已清除");

                yield return new WaitForEndOfFrame();
            }
            ty.oriLatData.Clear();
            ty.oriLogData.Clear();
            foreach (List<Vector3> points in ty.VoltexPoints)
                points.Clear();
            Debug.Log(ty.oriLogData.Count);

            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            Debug.Log("读取进度 " + 0.833 * i + "%...");

            //记录当前读取帧数
            NowPlayNumber = i;

            yield return new WaitForEndOfFrame();
        }

        //播放帧数复位
        NowPlayNumber = 1;
    }

    IEnumerator GetPointVariables()
    {
        List<float> EData = new List<float>();
        for (int i = 1; i < 121; ++i)
        {
            if (i < 10)
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_00" + i + ".nc";
            else if (i < 100)
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_0" + i + ".nc";
            else
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_" + i + ".nc";

            //string DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_d03_3times.nc";
            if (!System.IO.File.Exists(DataPath))
                Debug.Log("File doesn't exist!  " + DataPath);
            else
                Debug.Log(DataPath);

            ty = new Ty_Class(DataPath, WeatherVariableNames);

            List<float> tmp = ty.GetCertainPointWholeTimeValues(WeatherVariableNames, Logitude, Latitude, LayerNumber);

            foreach(float ff in tmp)
            {
                EData.Add(ff);
            }

            //清空变量
            tmp.Clear();
            Array.Clear(ty.stackData, 0, ty.allDataCount);

            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            Debug.Log("读取进度 " + 0.833 * i + "%...");

            yield return new WaitForEndOfFrame();

        }
        foreach (string Variable in WeatherVariableNames)
        {
            Debug.Log("Write: " + Variable + "Data...");
            Write_File(EData, Variable + "Data", Variable + "Data.txt");
        }
        yield return null;
        Debug.Log("GetPointVariables Finish!");
    }

    void ClearMesh()
    {
        string[] MeshNames = new string[] { "Cloud", "Rain", "Ice", "Snow", "Graupel", "All" };
        foreach (string Name in MeshNames)
        {
            GameObject root = GameObject.Find(Name + "Mesh");
            foreach (Transform child in root.transform)
            {
                //清除临时生成的mesh
                Mesh m = child.GetComponent<MeshFilter>().mesh;
                m.Clear();
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }
    }


    public static void Write_File(List<float> lf, string VariableName, string FileName)
    {
        FileStream fs = new FileStream(Application.dataPath + @"/StreamingAssets/" + FileName, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine(VariableName);
        sw.WriteLine("数据数量: " + lf.Count);
        int i = 0;
        foreach (float f in lf)
        {
            sw.Write(f + " ");
            ++i;
            if (i == 450)
            {
                sw.WriteLine();
                sw.WriteLine();
                i = 0;
            }
        }
        sw.Close();
    }

    //Mesh[] SetNewMesh(List<Vector3> vertices, List<float> Construct_i_Data)
    //{

    //    int count = 21;
    //    int VerCount = 21;
    //    float MaxData = 0.002351127f,
    //          MinData = 0.0f;
    //    Color c = new Color(1, 1, 1, Opacity);

    //    Mesh[] myMesh = new Mesh[count];
    //    for (int k = 0; k < count; k++)
    //    {
    //        myMesh[k] = new Mesh();
    //        List<Vector3> newVecs = new List<Vector3>();        //初始化顶点坐标
    //        List<Color> newC = new List<Color>();               //初始化顶点颜色

    //        for (int i = 0; i < VerCount; i++)
    //        {
    //            int dataindex = k * VerCount + i;
    //            dataindex = dataindex - k;
    //            if (dataindex > 419)
    //                break;
    //            for (int j = 0; j < 450; j++)
    //            {
    //                newVecs.Add(vertices[dataindex * 450 + j]);         //  添加mesh坐标
    //                newC.Add(c);
    //                float value = (MaxData - Construct_i_Data[dataindex * 450 + j]) / (MaxData - MinData);
    //                value = value < 0 ? 0 : value;
    //                value = value > 1 ? 1 : value;
    //                //将HSV的颜色值转为RGB的颜色值
    //                if (value == 1)
    //                    newC.Add(new Color(0, 0, 0, 0));
    //                else
    //                    newC.Add(Color.HSVToRGB(value * 2 / 3, 1, 1, false));
    //            }
    //        }
    //        myMesh[k].vertices = newVecs.ToArray();
    //        myMesh[k].colors = newC.ToArray();
    //        Debug.Log("Vecs: " + newVecs.Count);

    //        List<int> triagles = new List<int>();
    //        for (int i = 0; i < VerCount - 1; i++)
    //        {

    //            if (i + k * VerCount - k > 418)         //维度索引最多到419, 超出419 out of range
    //                break;

    //            for (int j = 0; j < 449; j++)
    //            {
    //                if (Construct_i_Data[(k * VerCount + i - k) * 450 + j] == 0)            //物理值为0 直接跳过
    //                    continue;
    //                triagles.Add(i * 450 + j + 0);
    //                triagles.Add(i * 450 + j + 1);
    //                triagles.Add(i * 450 + j + 1 + 450);
    //                triagles.Add(i * 450 + j + 1 + 450);
    //                triagles.Add(i * 450 + j + 0 + 450);
    //                triagles.Add(i * 450 + j + 0);
    //                triagles.Add(i * 450 + j + 1);
    //                triagles.Add(i * 450 + j + 0);
    //                triagles.Add(i * 450 + j + 1 + 450);
    //                triagles.Add(i * 450 + j + 0 + 450);
    //                triagles.Add(i * 450 + j + 1 + 450);
    //                triagles.Add(i * 450 + j + 0);
    //            }
    //        }
    //        Debug.Log("Tragles: " + triagles.Count);
    //        myMesh[k].triangles = triagles.ToArray();
    //        myMesh[k].RecalculateNormals();
    //        myMesh[k].RecalculateBounds();
    //    }

    //    return myMesh;
    //}

    //List<Color>[] SetNewColor(List<float> Construct_i_Data)
    //{
    //    //float MaxData = float.MinValue,
    //    //      MinData = float.MaxValue;

    //    //for (int h = 0; h < Construct_i_Data.Count; ++h)
    //    //{
    //    //    for (int i = 0; i < Construct_i_Data[h].Count; ++i)
    //    //    {
    //    //        if (MaxData < Construct_i_Data[h][i])
    //    //            MaxData = Construct_i_Data[h][i];
    //    //        if (MinData > Construct_i_Data[h][i])
    //    //            MinData = Construct_i_Data[h][i];
    //    //    }
    //    //}
    //    //Debug.Log("Max: " + MaxData + ", Min: " + MinData);

    //    int count = 21;
    //    int VerCount = 21;
    //    float MaxData = 0.002351127f,
    //          MinData = 0.0f;

    //    List<Color>[] myColor = new List<Color>[count];

    //    for (int k = 0; k < count; ++k)
    //    {
    //        myColor[k] = new List<Color>();
    //        List<Color> newC = new List<Color>();
    //        for (int i = 0; i < VerCount; i++)
    //        {
    //            int dataindex = k * VerCount + i;
    //            dataindex = dataindex - k;
    //            if (dataindex > 419)
    //                break;
    //            for (int j = 0; j < 450; j++)
    //            {
    //                float value = (MaxData - Construct_i_Data[dataindex * 450 + j]) / (MaxData - MinData);
    //                value = value < 0 ? 0 : value;
    //                value = value > 1 ? 1 : value;
    //                //将HSV的颜色值转为RGB的颜色值
    //                if (value == 1)
    //                    newC.Add(new Color(0, 0, 0, 0));
    //                else
    //                    newC.Add(Color.HSVToRGB(value * 2 / 3, 1, 1, false));
    //            }
    //        }
    //        myColor[k] = newC;

    //    }

    //    return myColor;
    //}
}
