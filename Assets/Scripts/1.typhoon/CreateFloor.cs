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
    public GameObject maincamera, aidcamera, mapcamera, curvecamera;

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
        Longitude = 0,
        Latitude = 0;
    // Use this for initialization
    void Start()
    {
        mapcamera = GameObject.Find("MapCamera");

        maincamera.SetActive(true);
        aidcamera.SetActive(false);
        curvecamera.SetActive(false);

        Resources.UnloadUnusedAssets();
        System.GC.Collect();
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
            mapcamera.SetActive(true);
        }
        else if (show_dropdown.value == 1)
        {
            dropdown_string = "ShowAllTimeSomeLayerSimulation";
            mapcamera.SetActive(false);
        }

        //Animation_LayerNumber 变量变化
        if (Animation_LayerNumber.text != "") 
        {
            //Debug.Log(Animation_LayerNumber.text);
            int L = Convert.ToInt16(Animation_LayerNumber.text);
            if (L > -1 && L < 450)
            {
                if (LayerNumber != L)
                    Debug.Log("LayerNumber: " + L);
                LayerNumber = L;
                layerMark = L;
            }
        }

        //防止Ray UI穿透问题
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            CatchHitPos();
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

    void CatchHitPos()
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hitInfo, 100) && hitInfo.transform.gameObject.name == "Map")
            {
                if (hitInfo.point.x > 108 && hitInfo.point.x < 121 && hitInfo.point.z > 16 && hitInfo.point.z < 27)
                    Debug.Log("hitinfo pos:" + hitInfo.point);
            }
        }
    }

    public void VortexShow()
    {
        StopAllCoroutines();
        ClearMesh();
        Debug.Log("vortexshow");

        DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_001.nc";

        if (!System.IO.File.Exists(DataPath))
            Debug.Log("File doesn't exist!  " + DataPath);
        else
            Debug.Log(DataPath);
        ty = new Ty_Class(DataPath, floor, floorMat, WeatherVariableNames);

        List<Vector3> VortexList = new List<Vector3>();

        for(int i = 0; i < 20;i+=2)
        {
            for(int j=0;j<420;j+=20)
            {
                for (int k = 0; k < 450; k += 20)
                {
                    GameObject sphereObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphereObj.transform.parent = GameObject.Find("PointCloud").transform;
                    sphereObj.transform.position = ty.VoltexPoints[i][450 * j + k];
                    sphereObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    VortexList.Add(ty.VoltexPoints[i][450 * j + k]);
                }
            }
        }

        int count = 0;
        Vector3[] veCluster = new Vector3[15];
        int[] countCluster = new int[15];
        //初始化
        for (int i = 0; i < veCluster.Length; ++i)
        {
            veCluster[i] = VortexList[i];
            countCluster[i] = 1;
        }

        while (true)
        {
            foreach(Vector3 vortex in VortexList)
            {
                int k = 0;
                float dis = float.MaxValue;
                for (int i = 0; i < veCluster.Length; ++i)
                {
                    if ((vortex - veCluster[i]).magnitude < dis)
                    {
                        dis = (vortex - veCluster[i]).magnitude;
                        k = i;
                    }
                }

                veCluster[k] = (veCluster[k] * countCluster[k] + vortex) / (++countCluster[k]);

            }

            ++count;
            if (count > 100)
                break;

        }

        foreach(Vector3 ve in veCluster)
        {
            GameObject sphereObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereObj.transform.parent = GameObject.Find("PointCloud").transform;
            sphereObj.transform.position = ve;
            sphereObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            sphereObj.GetComponent<Renderer>().material.color = Color.red;
        }

        Debug.Log(VortexList.Count);
        Debug.Log(countCluster[0] + " " + countCluster[1]);

        Resources.UnloadUnusedAssets();
        System.GC.Collect();
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
                Debug.Log("aid");
                maincamera.SetActive(false);
                aidcamera.SetActive(true);
            }
            else
            {
                Debug.Log("main");
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
                    //Debug.Log(layerMark);
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
            //foreach (List<Vector3> points in ty.VoltexPoints)
            //    points.Clear();

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
            //foreach (List<Vector3> points in ty.VoltexPoints)
            //    points.Clear();
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

            List<float> tmp = ty.GetCertainPointWholeTimeValues(WeatherVariableNames, Longitude, Latitude, LayerNumber);

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
    
}
