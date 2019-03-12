using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowTimeCurves : MonoBehaviour
{

    #region 数据结构变量
    Ty_Class ty = new Ty_Class();
    List<string> WeatherVariableNames;

    string DataPath;
    List<float> EData = new List<float>();      // 存储提取得到的数据

    #endregion

    #region UI界面变量
    public GameObject maincamera, aidcamera, curvecamera;
    public Dropdown variables_dropdown;
    public InputField Curve_LayerNumber, LogitudeNumber, LatitudeNumber;

    int LayerNumber = 15,
    Logitude = 200,
    Latitude = 200;
    #endregion

    string[] w = new string[] { "Cloud", "Rain", "Ice", "Snow", "Graupel", "All" };
    float[] max = new float[] { 0, 0, 0, 0, 0, 0 };
    int[] idx = new int[] { 0, 0, 0, 0, 0, 0 };

    // Use this for initialization
    void Start()
    {
        WeatherVariableNames = new List<string>() { "Cloud" };

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        //for (int j = 0; j < 6; ++j)
        //{
        //    WeatherVariableNames = new List<string>() { w[j] };

        //    Resources.UnloadUnusedAssets();
        //    System.GC.Collect();

        //    for (int i = 1; i < 121; ++i)
        //    {
        //        if (i < 10)
        //            DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_00" + i + ".nc";
        //        else if (i < 100)
        //            DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_0" + i + ".nc";
        //        else
        //            DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_" + i + ".nc";

        //        //string DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_d03_3times.nc";
        //        if (!System.IO.File.Exists(DataPath))
        //            Debug.Log("File doesn't exist!  " + DataPath);
        //        else
        //            Debug.Log(DataPath);

        //        ty = new Ty_Class(DataPath, WeatherVariableNames);
        //        if (ty.MaxData > max[j])
        //        {
        //            max[j] = ty.MaxData;
        //            idx[j] = ty.MaxDataIndex;
        //        }
        //    }
        //}

        //for (int j = 0; j < 6; ++j)
        //{
        //    Debug.Log(w[j] + " max_data: " + max[j] + " , index:  Log " + (idx[j] % (450 * 420)) % 450 + " , Lat " + (idx[j] % (450 * 420)) / 450 + " , Layer " + idx[j] / (450 * 420) + ", " + idx[j]);
        //}

    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(400, 250, 80, 40), "曲线显示"))
        {
            maincamera.SetActive(false);
            aidcamera.SetActive(false);
            curvecamera.SetActive(true);

            StopAllCoroutines();
            Clear();

            StartCoroutine("GetPointVariables");
            //GetPointVariables();

            //LR_DrawCurve(EData);
            Debug.Log("DrawCurve Done");

        }

        if (GUI.Button(new Rect(500, 250, 80, 40), "清空"))
        {
            Clear();
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        //variables_dropdown 变量变化
        if (variables_dropdown.value == 0)
        {
            WeatherVariableNames = new List<string>() { "Cloud" };
            //Debug.Log("WeatherVariableNames: " + WeatherVariableNames[0]);
        }
        else if (variables_dropdown.value == 1)
        {
            WeatherVariableNames = new List<string>() { "Rain" };
            //Debug.Log("WeatherVariableNames: " + WeatherVariableNames[0]);
        }
        else if (variables_dropdown.value == 2)
        {
            WeatherVariableNames = new List<string>() { "Ice" };
            //Debug.Log("WeatherVariableNames: " + WeatherVariableNames[0]);
        }
        else if (variables_dropdown.value == 3)
        {
            WeatherVariableNames = new List<string>() { "Snow" };
            //Debug.Log("WeatherVariableNames: " + WeatherVariableNames[0]);
        }
        else if (variables_dropdown.value == 4)
        {
            WeatherVariableNames = new List<string>() { "Graupel" };
            //Debug.Log("WeatherVariableNames: " + WeatherVariableNames[0]);
        }
        else if (variables_dropdown.value == 5)
        {
            WeatherVariableNames = new List<string>() { "All" };
            //Debug.Log("WeatherVariableNames: " + WeatherVariableNames[0]);
        }

        //Animation_LayerNumber 变量变化
        if (Curve_LayerNumber.text != "")
        {
            //Debug.Log(Animation_LayerNumber.text);
            int L = Convert.ToInt16(Curve_LayerNumber.text);
            if (L > -1 && L < 27)
                LayerNumber = L;
            //Debug.Log("LayerNumber: " + LayerNumber);
        }

        //LogitudeNumber 变量变化
        if (LogitudeNumber.text != "")
        {
            int L = Convert.ToInt16(LogitudeNumber.text);
            if (L > -1 && L < 450)
                Logitude = L;
            //Debug.Log("Logitude: " + Logitude);
        }

        //LatitudeNumber 变量变化
        if (LatitudeNumber.text != "")
        {
            int L = Convert.ToInt16(LatitudeNumber.text);
            if (L > -1 && L < 450)
                Latitude = L;
            //Debug.Log("Latitude: " + Latitude);
        }

    }

    public void DrawCurves()
    {
        StopAllCoroutines();

        StartCoroutine("GetPointVariables");
        //GetPointVariables();

        LR_DrawCurve(EData);
        Debug.Log("DrawCurve Done");
    }

    IEnumerator GetPointVariables()
    {
        if (EData != null)
            EData.Clear();

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

            //Debug.Log("数组长度: " + ty.allDataCount);

            List<float> tmp = ty.GetCertainPointWholeTimeValues(WeatherVariableNames, Logitude, Latitude, LayerNumber);

            foreach (float ff in tmp)
            {
                EData.Add(ff);
            }

            //清空变量
            tmp.Clear();
            Array.Clear(ty.stackData, 0, ty.allDataCount);

            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            // 读取进度
            Debug.Log("读取进度 " + 0.833 * i + "%...");

            yield return new WaitForEndOfFrame();

        }
        foreach (string Variable in WeatherVariableNames)
        {
            Debug.Log("Write: " + Variable + "Data...");
            Debug.Log("Logitude: " + Logitude + ", Latitude: " + Latitude + ", LayerNumber: " + LayerNumber);
            //Write_File(EData, Variable + "Data", Variable + "Data.txt");
            //for (int i = 0; i < EData.Count; i += 5)
            //{
            //    Debug.Log(i + ": " + EData[i] + "," + (i + 1) + ": " + EData[i + 1] + "," + (i + 2) + ": " + EData[i + 2] + "," + (i + 3) + ": " + EData[i + 3] + "," + (i + 4) + ": " + EData[i + 4]);
            //}
            //for (int i = 0; i < EData.Count; ++i)
            //{
            //    Debug.Log(i + ": " + EData[i]);
            //}
        }
        //yield return null;
        Debug.Log("GetPointVariables Finish!");


        //for (int i = 0; i < EData.Count; ++i)
        //{
        //    Debug.Log("EData_" + i + ": " + EData[i]);
        //}

        LR_DrawCurve(EData);
    }

    void LR_DrawCurve(List<float> EData)
    {
        float max = 0, ratio, width, FigureRatio = 2.0f;
        for (int i = 0; i < EData.Count; ++i)
        {
            if (EData[i] > max)
                max = EData[i];
        }
        ratio = EData.Count * FigureRatio * 0.2f / max;
        width = 0.003f * FigureRatio * EData.Count;

        //清空
        GameObject rootc = GameObject.Find("Curves");
        foreach (Transform child in rootc.transform)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
        GameObject roott = GameObject.Find("Text");
        foreach (Transform child in roott.transform)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }

        Vector3 setoff = new Vector3(-400, 40, 0);

        #region 渲染Sample Line
        //GameObject objsample = GameObject.Instantiate(GameObject.Find("curve"));  //实例化floor
        //objsample.name = "Sample";
        //objsample.transform.parent = GameObject.Find("Curves").transform;

        //LineRenderer sample_lineRenderer;

        //if (!objsample.GetComponent<LineRenderer>())          //获取obj lineRender
        //{
        //    sample_lineRenderer = objsample.AddComponent<LineRenderer>();
        //}
        //else
        //{
        //    sample_lineRenderer = objsample.GetComponent<LineRenderer>();
        //}

        //sample_lineRenderer.material = new Material(Shader.Find("Standard"));
        //sample_lineRenderer.material.SetColor("_Color", Color.red);
        //sample_lineRenderer.material.EnableKeyword("_EMISSION");
        //sample_lineRenderer.material.SetColor("_EmissionColor", Color.red);

        //sample_lineRenderer.numPositions = 2;

        //Vector3 samplepos1 = new Vector3(setoff.x + (EData.Count + 3) * FigureRatio, setoff.y + 0.5f * (EData.Count + 1) * FigureRatio, 0);
        //Vector3 samplepos2 = new Vector3(setoff.x + (EData.Count + 7) * FigureRatio, setoff.y + 0.5f * (EData.Count + 1) * FigureRatio, 0);

        //sample_lineRenderer.startWidth = 0.75f * width;
        //sample_lineRenderer.endWidth = 0.75f * width;

        //Vector3[] samplepoints = new Vector3[] { samplepos1, samplepos2 };
        //sample_lineRenderer.SetPositions(samplepoints);

        GameObject SampleText = new GameObject();
        SampleText.transform.parent = GameObject.Find("Text").transform;
        SampleText.transform.position = new Vector3(setoff.x + 0.5f * (EData.Count - 20) * FigureRatio, setoff.y + 0.5f * (EData.Count + 12) * FigureRatio, 0);
        SampleText.AddComponent<TextMesh>();
        SampleText.GetComponent<TextMesh>().text = WeatherVariableNames[0] + "_Data";
        SampleText.GetComponent<TextMesh>().characterSize = 3.0f * FigureRatio;
        #endregion

        #region 渲染X轴
        GameObject objx = GameObject.Instantiate(GameObject.Find("curve"));  //实例化floor
        objx.name = "X";
        objx.transform.parent = GameObject.Find("Curves").transform;

        LineRenderer x_lineRenderer;

        if (!objx.GetComponent<LineRenderer>())          //获取obj lineRender
        {
            x_lineRenderer = objx.AddComponent<LineRenderer>();
        }
        else
        {
            x_lineRenderer = objx.GetComponent<LineRenderer>();
        }

        x_lineRenderer.material = new Material(Shader.Find("Standard"));
        x_lineRenderer.material.SetColor("_Color", Color.white);
        x_lineRenderer.material.EnableKeyword("_EMISSION");
        x_lineRenderer.material.SetColor("_EmissionColor", Color.white);

        x_lineRenderer.numPositions = 2;

        Vector3 xpos1 = new Vector3(setoff.x, setoff.y, 0);
        Vector3 xpos2 = new Vector3(setoff.x + (EData.Count + 1) * FigureRatio, setoff.y, 0);

        x_lineRenderer.startWidth = 0.75f * width;
        x_lineRenderer.endWidth = 0.75f * width;

        Vector3[] xpoints = new Vector3[] { xpos1, xpos2 };
        x_lineRenderer.SetPositions(xpoints);
        
        // x轴标注点
        for (int i = 0; i < EData.Count; i += 10)
        {
            GameObject obj = GameObject.Instantiate(GameObject.Find("curve"));  //实例化floor
            obj.name = "LabelX" + i;
            obj.transform.parent = GameObject.Find("Curves").transform;

            LineRenderer _lineRenderer;

            if (!obj.GetComponent<LineRenderer>())          //获取obj lineRender
            {
                _lineRenderer = obj.AddComponent<LineRenderer>();
            }
            else
            {
                _lineRenderer = obj.GetComponent<LineRenderer>();
            }

            _lineRenderer.material = new Material(Shader.Find("Standard"));
            _lineRenderer.material.SetColor("_Color", Color.white);
            _lineRenderer.material.EnableKeyword("_EMISSION");
            _lineRenderer.material.SetColor("_EmissionColor", Color.white);

            _lineRenderer.numPositions = 2;

            Vector3 pos1 = new Vector3(setoff.x + i * FigureRatio, setoff.y, 0);
            Vector3 pos2 = new Vector3(setoff.x + i * FigureRatio, setoff.y - 1f * FigureRatio, 0);

            _lineRenderer.startWidth = 1.0f * width;
            _lineRenderer.endWidth = 1.0f * width;

            Vector3[] points = new Vector3[] { pos1, pos2 };
            _lineRenderer.SetPositions(points);

            //设置Text
            GameObject text = new GameObject();
            text.transform.parent = GameObject.Find("Text").transform;
            text.transform.position = new Vector3(setoff.x - 5 + i * FigureRatio, setoff.y - 2f * FigureRatio, 0);
            text.AddComponent<TextMesh>();
            text.GetComponent<TextMesh>().text = i.ToString();
            text.GetComponent<TextMesh>().characterSize = 2.0f * FigureRatio;
        }
        #endregion

        #region 渲染Y轴
        GameObject objy = GameObject.Instantiate(GameObject.Find("curve"));  //实例化floor
        objy.name = "Y";
        objy.transform.parent = GameObject.Find("Curves").transform;

        LineRenderer y_lineRenderer;

        if (!objy.GetComponent<LineRenderer>())          //获取obj lineRender
        {
            y_lineRenderer = objy.AddComponent<LineRenderer>();
        }
        else
        {
            y_lineRenderer = objy.GetComponent<LineRenderer>();
        }

        y_lineRenderer.material = new Material(Shader.Find("Standard"));
        y_lineRenderer.material.SetColor("_Color", Color.white);
        y_lineRenderer.material.EnableKeyword("_EMISSION");
        y_lineRenderer.material.SetColor("_EmissionColor", Color.white);

        y_lineRenderer.numPositions = 2;

        Vector3 ypos1 = new Vector3(setoff.x, setoff.y, 0);
        Vector3 ypos2 = new Vector3(setoff.x, setoff.y + 0.5f * (EData.Count + 1) * FigureRatio, 0);

        y_lineRenderer.startWidth = 0.75f * width;
        y_lineRenderer.endWidth = 0.75f * width;

        Vector3[] ypoints = new Vector3[] { ypos1, ypos2 };
        y_lineRenderer.SetPositions(ypoints);

        // y轴标注点
        for (int i = 10; i < EData.Count; i += 10)
        {
            GameObject obj = GameObject.Instantiate(GameObject.Find("curve"));  //实例化floor
            obj.name = "LabelY" + i;
            obj.transform.parent = GameObject.Find("Curves").transform;

            LineRenderer _lineRenderer;

            if (!obj.GetComponent<LineRenderer>())          //获取obj lineRender
            {
                _lineRenderer = obj.AddComponent<LineRenderer>();
            }
            else
            {
                _lineRenderer = obj.GetComponent<LineRenderer>();
            }

            _lineRenderer.material = new Material(Shader.Find("Standard"));
            _lineRenderer.material.SetColor("_Color", Color.white);
            _lineRenderer.material.EnableKeyword("_EMISSION");
            _lineRenderer.material.SetColor("_EmissionColor", Color.white);

            _lineRenderer.numPositions = 2;

            Vector3 pos1 = new Vector3(setoff.x , setoff.y + i * 0.5f*FigureRatio, 0);
            Vector3 pos2 = new Vector3(setoff.x - 1f * FigureRatio, setoff.y + i * 0.5f * FigureRatio, 0);

            _lineRenderer.startWidth = 1.0f * width;
            _lineRenderer.endWidth = 1.0f * width;

            Vector3[] points = new Vector3[] { pos1, pos2 };
            _lineRenderer.SetPositions(points);

            //设置Text
            GameObject text = new GameObject();
            text.transform.parent = GameObject.Find("Text").transform;
            text.transform.position = new Vector3(setoff.x - 20f * FigureRatio, setoff.y + i * 0.5f * FigureRatio + 3, 0);
            text.AddComponent<TextMesh>();
            text.GetComponent<TextMesh>().text = (10 * i / (ratio * FigureRatio)).ToString();
            text.GetComponent<TextMesh>().characterSize = 2.0f * FigureRatio;
        }
        #endregion

        //渲染grid x
        for (int i = 5; i < EData.Count; i += 5)
        {
            GameObject obj = GameObject.Instantiate(GameObject.Find("curve"));  //实例化floor
            obj.name = "GridX" + i;
            obj.transform.parent = GameObject.Find("Curves").transform;

            LineRenderer _lineRenderer;

            if (!obj.GetComponent<LineRenderer>())          //获取obj lineRender
            {
                _lineRenderer = obj.AddComponent<LineRenderer>();
            }
            else
            {
                _lineRenderer = obj.GetComponent<LineRenderer>();
            }

            _lineRenderer.material = new Material(Shader.Find("Standard"));
            _lineRenderer.material.SetColor("_Color", Color.grey);
            _lineRenderer.material.EnableKeyword("_EMISSION");
            _lineRenderer.material.SetColor("_EmissionColor", Color.grey);

            _lineRenderer.numPositions = 2;

            Vector3 pos1 = new Vector3(setoff.x + i * FigureRatio, setoff.y , 0);
            Vector3 pos2 = new Vector3(setoff.x + i * FigureRatio, setoff.y + 0.5f * FigureRatio * (EData.Count - 1), 0);

            _lineRenderer.startWidth = 0.5f * width;
            _lineRenderer.endWidth = 0.5f * width;

            Vector3[] points = new Vector3[] { pos1, pos2 };
            _lineRenderer.SetPositions(points);
        }

        //渲染grid y
        for (int i = 5; i < 0.5f * (EData.Count); i += 5)
        {
            GameObject obj = GameObject.Instantiate(GameObject.Find("curve"));  //实例化floor
            obj.name = "GridY" + i;
            obj.transform.parent = GameObject.Find("Curves").transform;

            LineRenderer _lineRenderer;

            if (!obj.GetComponent<LineRenderer>())          //获取obj lineRender
            {
                _lineRenderer = obj.AddComponent<LineRenderer>();
            }
            else
            {
                _lineRenderer = obj.GetComponent<LineRenderer>();
            }

            _lineRenderer.material = new Material(Shader.Find("Standard"));
            _lineRenderer.material.SetColor("_Color", Color.grey);
            _lineRenderer.material.EnableKeyword("_EMISSION");
            _lineRenderer.material.SetColor("_EmissionColor", Color.grey);

            _lineRenderer.numPositions = 2;

            Vector3 pos1 = new Vector3(setoff.x,setoff.y+ i * FigureRatio, 0);
            Vector3 pos2 = new Vector3(setoff.x + FigureRatio * (EData.Count - 1), setoff.y + i * FigureRatio, 0);

            _lineRenderer.startWidth = 0.5f * width;
            _lineRenderer.endWidth = 0.5f * width;

            Vector3[] points = new Vector3[] { pos1, pos2 };
            _lineRenderer.SetPositions(points);
        }

        if (IsAllZero(EData))
        {
            GameObject obj = GameObject.Instantiate(GameObject.Find("curve"));  //实例化floor
            obj.transform.parent = GameObject.Find("Curves").transform;

            LineRenderer _lineRenderer;

            if (!obj.GetComponent<LineRenderer>())          //获取obj lineRender
            {
                _lineRenderer = obj.AddComponent<LineRenderer>();
            }
            else
            {
                _lineRenderer = obj.GetComponent<LineRenderer>();
            }

            _lineRenderer.material = new Material(Shader.Find("Standard"));
            _lineRenderer.material.SetColor("_Color", Color.red);
            _lineRenderer.material.EnableKeyword("_EMISSION");
            _lineRenderer.material.SetColor("_EmissionColor", Color.red);

            _lineRenderer.numPositions = 2;

            Vector3 pos1 = new Vector3(setoff.x, setoff.y, -1);
            Vector3 pos2 = new Vector3(setoff.x + EData.Count * FigureRatio, setoff.y, -1);

            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;

            Vector3[] points = new Vector3[] { pos1, pos2 };
            _lineRenderer.SetPositions(points);
        }
        else
        {
            //渲染曲线
            for (int i = 0; i < EData.Count - 1; ++i)
            {
                GameObject obj = GameObject.Instantiate(GameObject.Find("curve"));  //实例化floor
                obj.transform.parent = GameObject.Find("Curves").transform;

                LineRenderer _lineRenderer;

                if (!obj.GetComponent<LineRenderer>())          //获取obj lineRender
                {
                    _lineRenderer = obj.AddComponent<LineRenderer>();
                }
                else
                {
                    _lineRenderer = obj.GetComponent<LineRenderer>();
                }

                _lineRenderer.material = new Material(Shader.Find("Standard"));
                _lineRenderer.material.SetColor("_Color", Color.red);
                _lineRenderer.material.EnableKeyword("_EMISSION");
                _lineRenderer.material.SetColor("_EmissionColor", Color.red);

                _lineRenderer.numPositions = 2;

                Vector3 pos1 = new Vector3(setoff.x + i * FigureRatio, setoff.y + ratio * EData[i] * FigureRatio, -1);
                Vector3 pos2 = new Vector3(setoff.x + (i + 1) * FigureRatio, setoff.y + ratio * EData[i + 1] * FigureRatio, -1);

                _lineRenderer.startWidth = width;
                _lineRenderer.endWidth = width;

                Vector3[] points = new Vector3[] { pos1, pos2 };
                _lineRenderer.SetPositions(points);
            }
        }
       
        Debug.Log("线段数: " + GameObject.Find("Curves").transform.childCount);
    }

    void Clear()
    {
        //清空
        GameObject root = GameObject.Find("Curves");
        foreach (Transform child in root.transform)
        {
            //清除临时生成的mesh
            UnityEngine.Object.Destroy(child.gameObject);
        }
        GameObject roott = GameObject.Find("Text");
        foreach (Transform child in roott.transform)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
    }

    // 判断数列中的数值是否全为零
    bool IsAllZero(List<float> Data)
    {
        bool b = true;
        foreach(float d in Data)
        {
            if (d != 0)
                b = false;
        }
        return b;
    }
}
