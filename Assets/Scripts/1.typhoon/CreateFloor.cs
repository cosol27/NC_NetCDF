using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class CreateFloor : MonoBehaviour
{

    public GameObject floor, target;
    public Material floorMat;
    public GameObject maincamera, aidcamera, curvecamera;//, mapcamera;

    string DataPath;
    create_mesh cm_script;

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

    #region 颜色谱图 gram
    float[] gramMax_mask = new float[6] { 0.15f, 0.20f, 0.75f, 0.08f, 0.017f, 0.05f };
    float[] gramMax_mask_single = new float[6] { 0.30f, 0.40f, 0.83f, 0.20f, 0.025f, 0.25f };
    float[] dataMin_mask = new float[6] { 0.01f, 0.01f, 0.01f, 0.02f, 0.015f, 0.01f };
    #endregion

    List<Vector3> VortexList;

    //public float Opacity = 0.85f,
    //       FloorGap = 0.1f;

    //private Mesh floorMesh;

    //public List<Vector3>[] MeshVertices = new List<Vector3>[] { };
    Ty_Class ty = new Ty_Class();

    List<string> WeatherVariableNames;

    int LayerNumber = 12,
        Longitude = 0,
        Latitude = 0;
    // Use this for initialization
    void Start()
    {
        //mapcamera = GameObject.Find("MapCamera");

        maincamera.SetActive(true);
        aidcamera.SetActive(false);
        curvecamera.SetActive(false);

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        cm_script = new create_mesh();
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
            //mapcamera.SetActive(true);
        }
        else if (show_dropdown.value == 1)
        {
            dropdown_string = "ShowAllTimeSomeLayerSimulation";
            //mapcamera.SetActive(false);
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

    public void Trans_VortexList()
    {
        StopAllCoroutines();
        ClearMesh();

        DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_001.nc";

        if (!System.IO.File.Exists(DataPath))
            Debug.Log("File doesn't exist!  " + DataPath);
        else
            Debug.Log(DataPath);
        ty = new Ty_Class(DataPath, floor, floorMat, WeatherVariableNames);

        VortexList = new List<Vector3>();

        int random;
        for (int i = 10; i < 11; i += 2)
        {
            for (int j = 0; j < 420; j += 20)
            {
                for (int k = 0; k < 450; k += 20)
                {
                    //random = 450 * UnityEngine.Random.Range(j - 5, j + 5) + UnityEngine.Random.Range(k - 5, k + 5);
                    //random = random < 0 ? 0 : random;
                    //random = random > 420 * 450 - 1 ? 420 * 450 - 1 : random;
                    random = 450 * j + k;

                    GameObject sphereObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphereObj.transform.parent = GameObject.Find("PointCloud").transform;
                    sphereObj.transform.position = ty.VortexPoints[i][random];
                    sphereObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    VortexList.Add(ty.VortexPoints[i][random]);
                }
            }
        }
    }

    public void Triangulation()
    {
        Debug.Log("Triangulation");
        Trans_VortexList();
    }

    public void K_Cluster()
    {
        int K = 15;

        Debug.Log("K_Cluster");
        Trans_VortexList();

        int count = 0;
        Vector3[] ve_Cluster = new Vector3[K],
                  tmp_Cluster = new Vector3[K];
        List<int>[] Cluster;
        //初始化
        for (int i = 0; i < ve_Cluster.Length; ++i)
        {
            ve_Cluster[i] = VortexList[UnityEngine.Random.Range(0, VortexList.Count - 1)];
            Debug.Log("Initialization: " + i + ve_Cluster[i]);
        }

        while (true)
        {
            tmp_Cluster = (Vector3[])ve_Cluster.Clone();

            Cluster = new List<int>[K];

            for (int c = 0; c < Cluster.Length; ++c)
                Cluster[c] = new List<int>();

            //计算样本最近簇
            for (int v = 0; v < VortexList.Count; ++v)
            {
                int k = 0;
                float dis = float.MaxValue;
                for (int i = 0; i < ve_Cluster.Length; ++i)
                {
                    if ((VortexList[v] - ve_Cluster[i]).magnitude < dis)
                    {
                        dis = (VortexList[v] - ve_Cluster[i]).magnitude;
                        k = i;
                    }
                }
                //点划入最近簇
                Cluster[k].Add(v);

            }
            //计算新向量
            for (int c = 0; c < Cluster.Length; ++c)
            {
                if (Cluster[c].Count == 0)
                    continue;

                ve_Cluster[c] = Vector3.zero;
                foreach (int ci in Cluster[c])
                {
                    ve_Cluster[c] += VortexList[ci];
                }
                ve_Cluster[c] /= Cluster[c].Count;
            }

            ++count;
            if (count > 50 || System.Linq.Enumerable.SequenceEqual(tmp_Cluster, ve_Cluster))
                break;

        }
        foreach (List<int> l in Cluster)
            Debug.Log(l.Count);
        Debug.Log(count);

        foreach (Vector3 ve in ve_Cluster)
        {
            GameObject sphereObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereObj.transform.parent = GameObject.Find("PointCloud").transform;
            sphereObj.transform.position = ve;
            sphereObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            sphereObj.GetComponent<Renderer>().material.color = Color.red;
        }

        Debug.Log(VortexList.Count);

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

        //if (GUI.Button(new Rect(700, 150, 80, 40), "show"))
        //{
        //    string DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_00" + 1 + ".nc";

        //    Ty_Class ty = new Ty_Class(DataPath, WeatherVariableNames);  //调用Ty_class.ReadNetCDFStackDataOnly 读取nc文件，坐标数据存入ty.VortexPoints，float value数据存入ty.stackData
        //    Debug.Log(DataPath);

        //    if (WeatherVariableNames.Contains("Cloud") || WeatherVariableNames.Contains("Rain"))
        //    {
        //        target.GetComponent<Renderer>().material = new Material(Shader.Find("Legacy Shaders/Diffuse"));
        //        if (WeatherVariableNames.Contains("Cloud"))
        //        {
        //            target.GetComponent<Renderer>().material.color = new Color(0.8f, 0.8f, 0.8f, 0.9f);
        //        }
        //        else
        //        {
        //            target.GetComponent<Renderer>().material.color = new Color(0.65f, 0.65f, 0.65f, 0.75f);
        //        }
        //    }
        //    else
        //    {
        //        target.GetComponent<Renderer>().material = new Material(Shader.Find("Custom/Transent"));
        //        //target.GetComponent<Renderer>().material = new Material(Shader.Find("Legacy Shaders/Diffuse"));
        //        //target.GetComponent<Renderer>().material.color = new Color(0.8f, 0.8f, 0.8f, 0.9f);
        //    }

        //    float max = 0,
        //          min = 1;
        //    for (int it = 0; it < ty.stackData.Length; it++)
        //    {
        //        if (min > ty.stackData[it])
        //            min = ty.stackData[it];
        //        if (max < ty.stackData[it])
        //            max = ty.stackData[it];
        //    }
        //    create_mesh cm_script = new create_mesh();
        //    cm_script.Input(ty.VortexPoints, ty.stackData, max, min, target, variables_dropdown.value);
        //}
    }


    IEnumerator ShowAllTimeSimulation()
    {
        for (int i = 0; i < target.transform.childCount; ++i)
        {
            GameObject child = target.transform.GetChild(i).gameObject;
            Destroy(child);
        }
        target.GetComponent<MeshFilter>().mesh.Clear();

        for (int i = NowPlayNumber; i < TotalFrameNumber; ++i)
        {
            string DataPath;
            if (i < 10)
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_00" + i + ".nc";
            else if (i < 100)
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_0" + i + ".nc";
            else
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_" + i + ".nc";

            ty = new Ty_Class(DataPath, WeatherVariableNames);  //调用Ty_class.ReadNetCDFStackDataOnly 读取nc文件，坐标数据存入ty.VortexPoints，float value数据存入ty.stackData
            Debug.Log(DataPath);

            if (WeatherVariableNames.Contains("Cloud")) 
            {
                //target.GetComponent<Renderer>().material = new Material(Shader.Find("Legacy Shaders/Diffuse"));
                //if (WeatherVariableNames.Contains("Cloud"))
                //{
                //    target.GetComponent<Renderer>().material.color = new Color(0.8f, 0.8f, 0.8f, 0.9f);
                //}
                //else
                //{
                //    target.GetComponent<Renderer>().material.color = new Color(0.65f, 0.65f, 0.65f, 0.75f);
                //}
                cm_script.colorMode = 1;
            }
            else if (WeatherVariableNames.Contains("Rain"))
            {
                cm_script.colorMode = 2;
            }
            else
            {
                //target.GetComponent<Renderer>().material = new Material(Shader.Find("Custom/Transent"));
                cm_script.colorMode = 0;
            }

            float max = 0,
                  min = 1;
            for (int it = 0; it < ty.stackData.Length; it++)
            {
                if (min > ty.stackData[it])
                    min = ty.stackData[it];
                if (max < ty.stackData[it])
                    max = ty.stackData[it];
            }
            //switch (variables_dropdown.value)
            //{
            //    case 0:
            //        max = min + gramMax_mask[0] * (max - min);
            //        break;
            //    case 1:
            //        min = min + mask_rate[0, 1] * (max - min);
            //        max = min + mask_rate[1, 1] * (max - min);
            //        break;
            //    case 2:
            //        min = min + mask_rate[0, 2] * (max - min);
            //        max = min + mask_rate[1, 2] * (max - min);
            //        break;
            //    case 3:
            //        min = min + mask_rate[0, 3] * (max - min);
            //        max = min + mask_rate[1, 3] * (max - min);
            //        break;
            //    case 4:
            //        min = min + mask_rate[0, 4] * (max - min);
            //        max = min + mask_rate[1, 4] * (max - min);
            //        break;
            //    case 5:
            //        min = min + mask_rate[0, 5] * (max - min);
            //        max = min + mask_rate[1, 5] * (max - min);
            //        break;
            //}
            max = min + gramMax_mask[variables_dropdown.value] * (max - min);
            
            cm_script.Input(ty.VortexPoints, ty.stackData, max, min, target, dataMin_mask[variables_dropdown.value]);
            //cm_script.input_single(ty.VortexPoints, ty.stackData, max, min, target, 1, 12);

            ty.oriLatData.Clear();
            ty.oriLogData.Clear();
            System.Array.Clear(ty.stackData, 0, ty.stackData.Length);
            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            yield return new WaitForEndOfFrame();
            cm_script.Clear();

            //记录当前读取帧数
            NowPlayNumber = i;

            //int[] count = new int[101];
            //for (int c = 0; c < 101; ++c)
            //    count[c] = 0;
            //float stair = (max - min) / 100;
            //int[] acc = new int[101];
            //foreach (float data in ty.stackData)
            //{
            //    if (Convert.ToInt16((data - min) / stair) > 100)
            //    {
            //        Debug.Log(data + ", " + max + ", " + min + ", " + stair);
            //        Debug.Log(Convert.ToInt16((data - min) / stair));
            //    }
            //    count[Convert.ToInt16((data - min) / stair)] += 1;
            //}
            ////Debug.Log(min);
            //FileStream fs = new FileStream(Application.dataPath + @"/StreamingAssets/ice_statistics.txt", FileMode.Create);
            //StreamWriter sw = new StreamWriter(fs);
            //for (int c = 0; c < 101; ++c)
            //{
            //    if (c == 0)
            //        acc[c] = count[c];
            //    else
            //        acc[c] = acc[c - 1] + count[c];
            //    sw.WriteLine(c + ",    " + count[c] + ",    " + stair * c + " - " + stair * (c + 1) + ",    " + acc[c] / 51030 + "%");
            //}
            //sw.Close();
        }

        //播放帧数复位
        NowPlayNumber = 1;

    }

    IEnumerator ShowAllTimeSomeLayerSimulation()
    {
        for (int i = 0; i < target.transform.childCount; ++i)
        {
            GameObject child = target.transform.GetChild(i).gameObject;
            Destroy(child);
        }
        target.GetComponent<MeshFilter>().mesh.Clear();

        for (int i = NowPlayNumber; i < TotalFrameNumber; ++i)
        {
            string DataPath;
            if (i < 10)
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_00" + i + ".nc";
            else if (i < 100)
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_0" + i + ".nc";
            else
                DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_" + i + ".nc";

            ty = new Ty_Class(DataPath, WeatherVariableNames);  //调用Ty_class.ReadNetCDFStackDataOnly 读取nc文件，坐标数据存入ty.VortexPoints，float value数据存入ty.stackData

            if (WeatherVariableNames.Contains("Cloud"))
            {
                cm_script.colorMode = 1;
            }
            else if (WeatherVariableNames.Contains("Rain"))
            {
                cm_script.colorMode = 2;
            }
            else
            {
                cm_script.colorMode = 0;
            }

            float max = 0,
                  min = 1;
            for (int it = 0; it < ty.stackData.Length; it++)
            {
                if (min > ty.stackData[it])
                    min = ty.stackData[it];
                if (max < ty.stackData[it])
                    max = ty.stackData[it];
            }
            //switch (variables_dropdown.value)
            //{
            //    case 0:
            //        min = min + mask_rate[0, 0] * (max - min);
            //        max = min + mask_rate[1, 0] * (max - min);
            //        break;
            //    case 1:
            //        min = min + mask_rate[0, 1] * (max - min);
            //        max = min + mask_rate[1, 1] * (max - min);
            //        break;
            //    case 2:
            //        min = min + mask_rate[0, 2] * (max - min);
            //        max = min + mask_rate[1, 2] * (max - min);
            //        break;
            //    case 3:
            //        min = min + mask_rate[0, 3] * (max - min);
            //        max = min + mask_rate[1, 3] * (max - min);
            //        break;
            //    case 4:
            //        min = min + mask_rate[0, 4] * (max - min);
            //        max = min + mask_rate[1, 4] * (max - min);
            //        break;
            //    case 5:
            //        min = min + mask_rate[0, 5] * (max - min);
            //        max = min + mask_rate[1, 5] * (max - min);
            //        break;
            //}
            //min = min + 0.2f * (max - min);
            max = min + gramMax_mask_single[variables_dropdown.value] * (max - min);
            
            //cm_script.Input(ty.VortexPoints, ty.stackData, max, min, target, variables_dropdown.value);
            cm_script.input_single(ty.VortexPoints, ty.stackData, max, min, target, 1, LayerNumber, dataMin_mask[variables_dropdown.value]);

            ty.oriLatData.Clear();
            ty.oriLogData.Clear();
            System.Array.Clear(ty.stackData, 0, ty.stackData.Length);
            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            yield return new WaitForEndOfFrame();
            cm_script.Clear();
            //记录当前读取帧数
            NowPlayNumber = i;
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

            foreach (float ff in tmp)
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
        //foreach (string Variable in WeatherVariableNames)
        //{
        //    Debug.Log("Write: " + Variable + "Data...");
        //    Write_File(EData, Variable + "Data", Variable + "Data.txt");
        //}
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

        for (int i = 0; i < target.transform.childCount; ++i)
        {
            GameObject child = target.transform.GetChild(i).gameObject;
            Destroy(child);
        }
        target.GetComponent<MeshFilter>().mesh.Clear();
    }


    public void readTXTFile()
    {
        string line;
        string[] arg;
        Vector3[] vortex = new Vector3[450 * 420 * 27];
        float[] value = new float[450 * 420 * 27];

        StreamReader sr = new StreamReader(Application.dataPath + @"/StreamingAssets/typhoon_066_Cloud.txt", System.Text.Encoding.ASCII);
        line = sr.ReadLine();
        for (int i = 0; i < 5103000; ++i)
        {
            line = sr.ReadLine();
            arg = System.Text.RegularExpressions.Regex.Split(line, ",", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            vortex[i] = new Vector3(float.Parse(arg[1]), float.Parse(arg[2]), float.Parse(arg[3]));
            value[i] = float.Parse(arg[4]);
        }

        Debug.Log(vortex.Length + " vortex: " + vortex[0] + vortex[1354] + vortex[5102999]);
        Debug.Log(value.Length + " value: " + value[0] + value[1354] + value[5102999]);

        sr.Close();
    }
}
