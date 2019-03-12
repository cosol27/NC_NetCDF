using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;
using System.Threading;

using NETCDF_CDL;//调用netcdf

public class Ty_Class:IDisposable // : MonoBehaviour 
{
    public GameObject floor;
    public Material floorMat;

    public float Opacity = 0.9f,
                 cloud_mask_rate = 0.04f,      //云雨变量 剔除后百分之100*cloud_mask_rate的数据，0为不剔除
                 rain_mask_rate = 0.03f,
                 ice_mask_rate = 0.15f,
                 snow_mask_rate = 0.03f,
                 graup_mask_rate = 0.01f,
                 all_mask_rate = 0.02f,   //其余变量 剔除后百分之100*mask_rate的数据，0为不剔除
                 FloorGap = 0.10f;

    internal int timeCount, fromtopCount, latlogCount, allDataCount, lat, log;
    internal string DataPath;
    public List<string> WeatherVariableNames;
    public float MaxData = float.MinValue;
    public int MaxDataIndex = 0;

    public float[] stackData;
    [NonSerialized]
    public List<float> oriLatData = new List<float> ();						//lat
	[NonSerialized]
    public List<float> oriLogData = new List<float> ();                      //log
    public List<List<float>>[] oriCloudData = new List<List<float>>[]{};     //clouddata
    public List<List<float>>[] oriRainData = new List<List<float>>[]{};      //raindata
    public List<List<float>>[] oriIceData = new List<List<float>>[]{};       //icedata
    public List<List<float>>[] oriSnowData = new List<List<float>>[]{};      //snowdata
    public List<List<float>>[] oriGraupData = new List<List<float>>[]{};     //graupdata
    public List<List<float>>[] oriMixData = new List<List<float>>[] { };     //mixdata

    public List<Vector3>[] VoltexPoints = new List<Vector3>[]{ };           //mesh vertices

    public bool bReadFinish = false;

    //confirm:
    // height is as 0.1m as from-to-top
    //from-to-top is 34, as 3.4m in VR

    // Use this for initialization
    //void Start()
    //{
    //    //floor = GameObject.Find("floor");

    //    //StartCoroutine(ie_NetCDF());
    //    if (DataPath != null)
    //    {
    //        ReadNetCDF(DataPath);
    //    }
    //}

    //   IEnumerator ie_NetCDF()                 //string _path) 
    //   {
    //       for (int i = 0; i < 1; i++)
    //       {
    //           string DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_" + i + ".nc";
    //           Debug.Log("DataPath -> " + DataPath);
    //           StartCoroutine(ReadNetCDF(DataPath));
    //           yield return new WaitForEndOfFrame();
    //		bReadFinish = true;
    //       }
    //    }

    /// <summary>
    /// 构造函数，实例化Typhoon时即读取文件
    /// </summary>
    /// <param name="s">NetCDF文件路径</param>
    public Ty_Class(string s, GameObject obj, Material mat, List<string> Names)
    {
        DataPath = s;
        floor = obj;
        floorMat = mat;
        WeatherVariableNames = Names;

        ReadNetCDF(DataPath);
        
    }

    /// <summary>
    /// 构造函数2，实例化Typhoon时即读取文件,无渲染模式
    /// </summary>
    /// <param name="s">NetCDF文件路径</param>
    /// <param name="Names">所需提取的变量名称</param>
    public Ty_Class(string s, List<string> Names)
    {
        DataPath = s;
        WeatherVariableNames = Names;

        ReadNetCDFStackDataOnly(DataPath);
    }


    public Ty_Class()
    {
        //Debug.Log("Ty已创建");
    }

    ~Ty_Class()
    {
        Dispose();
    }

    public void Dispose()
    {
        //Debug.Log("已销毁");
    }

    public void ReadNetCDF(string path)
	{
		int ncidp;
		int openflag = _netcdf.nc_open(path, _netcdf.CreateMode.NC_NOWRITE, out ncidp);
		Debug.Log("openflag : " + openflag + " -> ncidp : " + ncidp);

		int ndims;//dimension
		int nvars;//变量长度
		int ngatts;
		int unlimdimid;
		int inqflag = _netcdf.nc_inq(ncidp, out ndims, out nvars, out ngatts, out unlimdimid);

        Debug.Log("result: " + inqflag + " -> ndims : " + ndims + " // nvars : " + nvars + " // ngatts : " + ngatts + " // unlimdimid : " + unlimdimid);

        //dimension
        StringBuilder[] dimNames = new StringBuilder[ndims];
		int[] dimData = new int[ndims];

		for (int i = 0; i < ndims; i++)
		{
			StringBuilder dimname = new StringBuilder();
			_netcdf.nc_inq_dim(ncidp, i, dimname, out dimData[i]);
			dimNames[i] = dimname;
            Debug.Log("dimData : " + dimNames[i] + "  " + dimData[i]);
        }
        //variable
        StringBuilder[] varNames = new StringBuilder[nvars];
		_netcdf.NcType[] varTypes = new _netcdf.NcType[nvars];
		int varid = 0;
		StringBuilder varname;
		for (int i = 0; i < nvars; i++)
		{
			varname = new StringBuilder();
			_netcdf.nc_inq_varname(ncidp, i, varname);									//Name
			_netcdf.nc_inq_vartype(ncidp, i, out varTypes[i]);							//type
			_netcdf.nc_inq_varid(ncidp, varname.ToString(), out varid);					//ID
			varNames[i] = varname;
            Debug.Log("varname 指针 : " + varid + "  " + varNames[i] + "  " + varTypes[i]);
        }

        // data length must be 420*450*34*3, otherwise read fails(*************IMPORTANT)
        timeCount = dimData [0];
		fromtopCount = dimData [4];
		latlogCount = dimData [2] * dimData [3];
        lat = dimData[2];
        log = dimData[3];

		allDataCount = dimData [2] * dimData [3] * dimData [4] * dimData [0];

		//lat data
		float[] curdata = new float[allDataCount];
		int getlat = _netcdf.nc_get_var_float(ncidp, 1, curdata);

		if(getlat != 0)
			Debug.Log ("latdata: read with error: " + getlat);
        for (int i = 0; i < latlogCount; i++) {
			oriLatData.Add (curdata[i]);
		}
        //Write_file(oriLatData, "LatData.txt");
        //yield return new WaitForFixedUpdate();

		//log data
		int getlog = _netcdf.nc_get_var_float(ncidp, 2, curdata);
		if(getlog != 0)
			Debug.Log ("logdata: read with error: " + getlog);
        for (int i = 0; i < latlogCount; i++) {
			oriLogData.Add (curdata[i]);
		}
        //Write_file(oriLogData, "LogData.txt");
        //yield return new WaitForFixedUpdate();

		//construct virtual points
		VoltexPoints = new List<Vector3>[fromtopCount];
		for (int i = 0; i < fromtopCount; i++) {
			List<Vector3> plane_points = new List<Vector3> ();
			for (int j = 0; j < latlogCount; j++) {
                Vector3 point = new Vector3(oriLogData[j], i * FloorGap + FloorGap * UnityEngine.Random.Range(-0.1f, 0.1f), oriLatData[j]);
				plane_points.Add (point);
			}
			VoltexPoints [i] = plane_points;
		}
        //yield return new WaitForFixedUpdate();

        //cloud data
        if (WeatherVariableNames.Contains("Cloud"))
        {
            int getcloud = _netcdf.nc_get_var_float(ncidp, 3, curdata);
            if (getcloud != 0)
                Debug.Log("coulddata: read with error: " + getcloud);

            oriCloudData = ReConstructData(curdata, timeCount, fromtopCount, latlogCount);

            //Write_file(oriCloudData[0][0], "CloudData.txt");
        }

        //yield return new WaitForFixedUpdate();

        //rain data
        if (WeatherVariableNames.Contains("Rain"))
        {
            int getrain = _netcdf.nc_get_var_float(ncidp, 4, curdata);
            if (getrain != 0)
                Debug.Log("raindata: read with error: " + getrain);

            oriRainData = ReConstructData(curdata, timeCount, fromtopCount, latlogCount);
            //Write_file(oriCloudData[0][15], "RainData.txt");
        }

        //yield return new WaitForFixedUpdate();

        //ice data
        if (WeatherVariableNames.Contains("Ice"))
        {
            int getice = _netcdf.nc_get_var_float(ncidp, 5, curdata);
            if (getice != 0)
                Debug.Log("icedata: read with error: " + getice);

            oriIceData = ReConstructData(curdata, timeCount, fromtopCount, latlogCount);
            //Write_file(oriCloudData[0][15], "IceData.txt");
        }
        
        //yield return new WaitForFixedUpdate();

        //snow data
        if (WeatherVariableNames.Contains("Snow"))
        {
            int getsnow = _netcdf.nc_get_var_float(ncidp, 6, curdata);
            if (getsnow != 0)
                Debug.Log("snowdata: read with error: " + getsnow);

            oriSnowData = ReConstructData(curdata, timeCount, fromtopCount, latlogCount);
        }
        //yield return new WaitForFixedUpdate();

        //graupel data
        if (WeatherVariableNames.Contains("Graupel"))
        {
            int getgraup = _netcdf.nc_get_var_float(ncidp, 7, curdata);
            if (getgraup != 0)
                Debug.Log("graupdata: read with error: " + getgraup);

            oriGraupData = ReConstructData(curdata, timeCount, fromtopCount, latlogCount); 
        }

        //yield return new WaitForFixedUpdate();

        //Mix data
        if (WeatherVariableNames.Contains("All"))
        {
            float[] curdatax = new float[allDataCount];
            
            //get cloud data
            int getcloud = _netcdf.nc_get_var_float(ncidp, 3, curdatax);
            if (getcloud != 0)
                Debug.Log("coulddata: read with error: " + getcloud);
            //添加变量
            for (int i = 0; i < allDataCount; ++i)
                curdata[i] = curdatax[i];
            //清空变量
            Array.Clear(curdatax, 0, allDataCount);

            //get rain data
            int getrain = _netcdf.nc_get_var_float(ncidp, 4, curdatax);
            if (getrain != 0)
                Debug.Log("raindata: read with error: " + getrain);
            //添加变量
            for (int i = 0; i < allDataCount; ++i)
                curdata[i] += curdatax[i];
            //清空变量
            Array.Clear(curdatax, 0, allDataCount);

            //get ice data
            int getice = _netcdf.nc_get_var_float(ncidp, 5, curdatax);
            if (getice != 0)
                Debug.Log("icedata: read with error: " + getice);
            //添加变量
            for (int i = 0; i < allDataCount; ++i)
                curdata[i] += curdatax[i];
            //清空变量
            Array.Clear(curdatax, 0, allDataCount);

            //get snow data
            int getsnow = _netcdf.nc_get_var_float(ncidp, 6, curdatax);
            if (getsnow != 0)
                Debug.Log("snowdata: read with error: " + getsnow);
            //添加变量
            for (int i = 0; i < allDataCount; ++i)
                curdata[i] += curdatax[i];
            //清空变量
            Array.Clear(curdatax, 0, allDataCount);

            //get graupel data
            int getgraup = _netcdf.nc_get_var_float(ncidp, 7, curdatax);
            if (getgraup != 0)
                Debug.Log("graupdata: read with error: " + getgraup);
            //添加变量
            for (int i = 0; i < allDataCount; ++i)
                curdata[i] += curdatax[i];
            //清空变量
            Array.Clear(curdatax, 0, allDataCount);

            ////mix data
            //for (int i = 0; i < allDataCount; ++i)
            //    curdata[i] = curdata3[i] + curdata4[i] + curdata5[i] + curdata6[i] + curdata7[i];
            ////clear cur data

            //Array.Clear(curdata4, 0, allDataCount);
            //Array.Clear(curdata5, 0, allDataCount);
            //Array.Clear(curdata6, 0, allDataCount);
            //Array.Clear(curdata7, 0, allDataCount);

            oriMixData = ReConstructData(curdata, timeCount, fromtopCount, latlogCount);
            
        }

        // 求得最大值
        for (int i = 0; i < allDataCount; ++i)
        {
            if(curdata[i] > MaxData)
            {
                MaxData = curdata[i];
                MaxDataIndex = i + 1;
            }
        }

        int closeflag = _netcdf.nc_close(ncidp);//close file
		if (closeflag == (int)_netcdf.ResultCode.NC_NOERR) {
			Debug.Log("file indexer release sucessfully! ");
		}

        bReadFinish = true;

    }

    public void ReadNetCDFStackDataOnly(string path)
    {
        int ncidp;
        int openflag = _netcdf.nc_open(path, _netcdf.CreateMode.NC_NOWRITE, out ncidp);
        Debug.Log("openflag : " + openflag + " -> ncidp : " + ncidp);

        int ndims;//dimension
        int nvars;//变量长度
        int ngatts;
        int unlimdimid;
        int inqflag = _netcdf.nc_inq(ncidp, out ndims, out nvars, out ngatts, out unlimdimid);

        Debug.Log("result: " + inqflag + " -> ndims : " + ndims + " // nvars : " + nvars + " // ngatts : " + ngatts + " // unlimdimid : " + unlimdimid);

        //dimension
        StringBuilder[] dimNames = new StringBuilder[ndims];
        int[] dimData = new int[ndims];

        for (int i = 0; i < ndims; i++)
        {
            StringBuilder dimname = new StringBuilder();
            _netcdf.nc_inq_dim(ncidp, i, dimname, out dimData[i]);
            dimNames[i] = dimname;
            Debug.Log("dimData : " + dimNames[i] + "  " + dimData[i]);
        }
        //variable
        StringBuilder[] varNames = new StringBuilder[nvars];
        _netcdf.NcType[] varTypes = new _netcdf.NcType[nvars];
        int varid = 0;
        StringBuilder varname;
        for (int i = 0; i < nvars; i++)
        {
            varname = new StringBuilder();
            _netcdf.nc_inq_varname(ncidp, i, varname);                                  //Name
            _netcdf.nc_inq_vartype(ncidp, i, out varTypes[i]);                          //type
            _netcdf.nc_inq_varid(ncidp, varname.ToString(), out varid);                 //ID
            varNames[i] = varname;
            Debug.Log("varname 指针 : " + varid + "  " + varNames[i] + "  " + varTypes[i]);
        }

        // data length must be 420*450*34*3, otherwise read fails(*************IMPORTANT)
        timeCount = dimData[0];
        fromtopCount = dimData[4];
        latlogCount = dimData[2] * dimData[3];
        lat = dimData[2];
        log = dimData[3];

        allDataCount = dimData[2] * dimData[3] * dimData[4] * dimData[0];

        //stack data
        stackData = new float[allDataCount];
        
        //cloud data
        if (WeatherVariableNames.Contains("Cloud"))
        {
            int getcloud = _netcdf.nc_get_var_float(ncidp, 3, stackData);
            if (getcloud != 0)
                Debug.Log("coulddata: read with error: " + getcloud);
        }

        //yield return new WaitForFixedUpdate();

        //rain data
        if (WeatherVariableNames.Contains("Rain"))
        {
            int getrain = _netcdf.nc_get_var_float(ncidp, 4, stackData);
            if (getrain != 0)
                Debug.Log("raindata: read with error: " + getrain);
        }

        //yield return new WaitForFixedUpdate();

        //ice data
        if (WeatherVariableNames.Contains("Ice"))
        {
            int getice = _netcdf.nc_get_var_float(ncidp, 5, stackData);
            if (getice != 0)
                Debug.Log("icedata: read with error: " + getice);
        }

        //yield return new WaitForFixedUpdate();

        //snow data
        if (WeatherVariableNames.Contains("Snow"))
        {
            int getsnow = _netcdf.nc_get_var_float(ncidp, 6, stackData);
            if (getsnow != 0)
                Debug.Log("snowdata: read with error: " + getsnow);
        }

        //yield return new WaitForFixedUpdate();

        //graupel data
        if (WeatherVariableNames.Contains("Graupel"))
        {
            int getgraup = _netcdf.nc_get_var_float(ncidp, 7, stackData);
            if (getgraup != 0)
                Debug.Log("graupdata: read with error: " + getgraup);
        }

        //yield return new WaitForFixedUpdate();

        //Mix data
        if (WeatherVariableNames.Contains("All"))
        {
            float[] curdatax = new float[allDataCount];
            //get cloud data
            int getcloud = _netcdf.nc_get_var_float(ncidp, 3, curdatax);
            if (getcloud != 0)
                Debug.Log("coulddata: read with error: " + getcloud);
            //添加变量
            for (int i = 0; i < allDataCount; ++i)
                stackData[i] = curdatax[i];
            //清空变量
            Array.Clear(curdatax, 0, allDataCount);

            //get rain data
            int getrain = _netcdf.nc_get_var_float(ncidp, 4, curdatax);
            if (getrain != 0)
                Debug.Log("raindata: read with error: " + getrain);
            //添加变量
            for (int i = 0; i < allDataCount; ++i)
                stackData[i] += curdatax[i];
            //清空变量
            Array.Clear(curdatax, 0, allDataCount);

            //get ice data
            int getice = _netcdf.nc_get_var_float(ncidp, 5, curdatax);
            if (getice != 0)
                Debug.Log("icedata: read with error: " + getice);
            //添加变量
            for (int i = 0; i < allDataCount; ++i)
                stackData[i] += curdatax[i];
            //清空变量
            Array.Clear(curdatax, 0, allDataCount);

            //get snow data
            int getsnow = _netcdf.nc_get_var_float(ncidp, 6, curdatax);
            if (getsnow != 0)
                Debug.Log("snowdata: read with error: " + getsnow);
            //添加变量
            for (int i = 0; i < allDataCount; ++i)
                stackData[i] += curdatax[i];
            //清空变量
            Array.Clear(curdatax, 0, allDataCount);

            //get graupel data
            int getgraup = _netcdf.nc_get_var_float(ncidp, 7, curdatax);
            if (getgraup != 0)
                Debug.Log("graupdata: read with error: " + getgraup);
            //添加变量
            for (int i = 0; i < allDataCount; ++i)
                stackData[i] += curdatax[i];
            //清空变量
            Array.Clear(curdatax, 0, allDataCount);

        }

        // 求得最大值
        for (int i = 0; i < allDataCount; ++i)
        {
            if (stackData[i] > MaxData)
            {
                MaxData = stackData[i];
                MaxDataIndex = i + 1;
            }
        }

        int closeflag = _netcdf.nc_close(ncidp);//close file
        if (closeflag == (int)_netcdf.ResultCode.NC_NOERR)
        {
            Debug.Log("file indexer release sucessfully! ");
        }

        bReadFinish = true;

    }

    //
    /// <summary>
    /// 输入变量名，模拟指定变量全时间状态变化
    /// </summary>
    /// <param name="WeatherVariableName">变量名称</param>
    public void ShowSomeVariableSimulation(string WeatherVariableName,int o)
    {
        Color WeatherColor = Color.clear;
        List<List<float>> curdb = new List<List<float>>();

        if (WeatherVariableName == "Cloud")
        {
            WeatherColor = new Color(1, 1, 1, Opacity);
            curdb = oriCloudData[o];
        }
        else if (WeatherVariableName == "Rain")
        {
            WeatherColor = new Color(0.5f, 0.5f, 0.5f, Opacity);
            curdb = oriRainData[o];
        }
        else if (WeatherVariableName == "Ice")
        {
            WeatherColor = new Color(0.8f, 0, 0, Opacity - 0.05f);
            curdb = oriIceData[o];
        }
        else if (WeatherVariableName == "Snow")
        {
            WeatherColor = new Color(0, 0.8f, 0, Opacity - 0.05f);
            curdb = oriSnowData[o];
        }
        else if (WeatherVariableName == "Graupel")
        {
            WeatherColor = new Color(0, 0, 0.8f, Opacity - 0.05f);
            curdb = oriGraupData[o];
        }
        else if (WeatherVariableName == "All")
        {
            curdb = oriMixData[o];
        }

        if (null == floor)
        {
            Debug.Log("floor is null");
        }

        GameObject root = GameObject.Find(WeatherVariableName + "Mesh");        //比如 WeatherVariableName = Cloud, Gameobject 为 CloudMesh
        if (root == null)
            Debug.Log("Can't find " + WeatherVariableName + "Mesh");


        foreach (Transform child in root.transform)
        {
            //清除临时生成的mesh
            Mesh m = child.GetComponent<MeshFilter>().mesh;
            m.Clear();
            UnityEngine.Object.Destroy(child.gameObject);
        }

        for (int p = 0; p < VoltexPoints.Length; ++p)
        //for (int p = 14; p < 17; p++)
        {
            GameObject obj = GameObject.Instantiate(floor);
            obj.transform.parent = GameObject.Find(WeatherVariableName + "Mesh").transform;
            //obj.transform.SetParent(GameObject.Find("space").transform);

            obj.name = WeatherVariableName + "MeshObject__Z " + p;

            Mesh[] newMesh = SetNewMesh(VoltexPoints[p], curdb[p], WeatherVariableName, !(WeatherVariableName.Contains("Cloud") || WeatherVariableName.Contains("Rain")));  //ALl 使其彩色图显示
            //List<Color>[] newColor = SetNewColor(ty.oriCloudData[1][p]);

            int leng = newMesh.Length;
            for (int i = 0; i < leng; i++)
            {
                GameObject fool = GameObject.Instantiate(floor);
                fool.transform.parent = obj.transform;
                fool.name = "floor_" + i;
                fool.GetComponent<MeshFilter>().mesh = newMesh[i];
                //fool.GetComponent<MeshFilter>().mesh.colors = newColor[i].ToArray();
                fool.GetComponent<MeshRenderer>().material = floorMat;
            }
        }

        //渲染y轴面
        for (int px = 0; px < 420; ++px)
        {
            GameObject obj = GameObject.Instantiate(floor);
            obj.transform.parent = GameObject.Find(WeatherVariableName + "Mesh").transform;
            obj.name = WeatherVariableName + "MeshObject__Y " + px;

            List<Vector3> tmp_Voltexpoints = ReConstruct_VoltexPoints(VoltexPoints, "x", px);
            List<float> tmp_curdb = ReConstruct_Curdb(curdb, "x", px);
            Mesh newMesh = SetNewMeshXY(tmp_Voltexpoints, tmp_curdb, WeatherVariableName, "x", !(WeatherVariableName.Contains("Cloud") || WeatherVariableName.Contains("Rain")));

            //int leng = newMesh.Length;
            for (int i = 0; i < 1; i++)
            {
                GameObject fool = GameObject.Instantiate(floor);
                fool.transform.parent = obj.transform;
                fool.name = "floor_y" + i;
                fool.GetComponent<MeshFilter>().mesh = newMesh;
                //fool.GetComponent<MeshFilter>().mesh.colors = newColor[i].ToArray();
                fool.GetComponent<MeshRenderer>().material = floorMat;
            }
        }

        //渲染x轴面
        for (int py = 0; py < 450; ++py)
        {
            GameObject obj = GameObject.Instantiate(floor);
            obj.transform.parent = GameObject.Find(WeatherVariableName + "Mesh").transform;
            obj.name = WeatherVariableName + "MeshObject__X " + py;

            List<Vector3> tmp_Voltexpoints = ReConstruct_VoltexPoints(VoltexPoints, "y", py);
            List<float> tmp_curdb = ReConstruct_Curdb(curdb, "y", py);
            Mesh newMesh = SetNewMeshXY(tmp_Voltexpoints, tmp_curdb, WeatherVariableName, "y", !(WeatherVariableName.Contains("Cloud") || WeatherVariableName.Contains("Rain")));

            //int leng = newMesh.Length;
            for (int i = 0; i < 1; i++)
            {
                GameObject fool = GameObject.Instantiate(floor);
                fool.transform.parent = obj.transform;
                fool.name = "floor_x" + i;
                fool.GetComponent<MeshFilter>().mesh = newMesh;
                //fool.GetComponent<MeshFilter>().mesh.colors = newColor[i].ToArray();
                fool.GetComponent<MeshRenderer>().material = floorMat;
            }
        }
    }

    //private void Mz(List<List<float>> curdb, string WeatherVariableName)
    //{
    //    for (int p = 0; p < VoltexPoints.Length; ++p)
    //    //for (int p = 14; p < 17; p++)
    //    {
    //        GameObject obj = GameObject.Instantiate(floor);
    //        obj.transform.parent = GameObject.Find(WeatherVariableName + "Mesh").transform;
    //        //obj.transform.SetParent(GameObject.Find("space").transform);

    //        obj.name = WeatherVariableName + "MeshObject__" + p;

    //        Mesh[] newMesh = SetNewMesh(VoltexPoints[p], curdb[p], WeatherVariableName, !(WeatherVariableName.Contains("Cloud") || WeatherVariableName.Contains("Rain")));  //ALl 使其彩色图显示
    //        //List<Color>[] newColor = SetNewColor(ty.oriCloudData[1][p]);

    //        int leng = newMesh.Length;
    //        for (int i = 0; i < leng; i++)
    //        {
    //            GameObject fool = GameObject.Instantiate(floor);
    //            fool.transform.parent = obj.transform;
    //            fool.name = "floor_" + i;
    //            fool.GetComponent<MeshFilter>().mesh = newMesh[i];
    //            //fool.GetComponent<MeshFilter>().mesh.colors = newColor[i].ToArray();
    //            fool.GetComponent<MeshRenderer>().material = floorMat;
    //        }
    //    }
    //}

    /// <summary>
    /// 输入变量名，模拟指定变量某层全时间状态变化
    /// </summary>
    /// <param name="WeatherVariableName">变量名称</param>
    public void ShowSomeVariableSomeLayerSimulation(string WeatherVariableName, int o, int p, string axis)
    {
        List<List<float>> curdb = new List<List<float>>();

        if (WeatherVariableName == "Cloud")
        {
            curdb = oriCloudData[o];
        }
        else if (WeatherVariableName == "Rain")
        {
            curdb = oriRainData[o];
        }
        else if (WeatherVariableName == "Ice")
        {
            curdb = oriIceData[o];
        }
        else if (WeatherVariableName == "Snow")
        {
            curdb = oriSnowData[o];
        }
        else if (WeatherVariableName == "Graupel")
        {
            curdb = oriGraupData[o];
        }
        else if (WeatherVariableName == "All")
        {
            curdb = oriMixData[o];
        }

        if (null == floor)
        {
            Debug.Log("floor is null");
        }

        GameObject root = GameObject.Find(WeatherVariableName + "Mesh");        //比如 WeatherVariableName = Cloud, Gameobject 为 CloudMesh
        if (root == null)
            Debug.Log("Can't find " + WeatherVariableName + "Mesh");


        foreach (Transform child in root.transform)
        {
            //清除临时生成的mesh
            Mesh m = child.GetComponent<MeshFilter>().mesh;
            m.Clear();
            UnityEngine.Object.Destroy(child.gameObject);
        }



        GameObject obj = GameObject.Instantiate(floor);
        obj.transform.parent = GameObject.Find(WeatherVariableName + "Mesh").transform;

        obj.name = WeatherVariableName + "MeshObject__" + p;

        if (axis.ToLower() == "z")
        {
            Mesh[] newMesh = SetNewMesh(VoltexPoints[p], curdb[p], WeatherVariableName, !(WeatherVariableName.Contains("Cloud") || WeatherVariableName.Contains("Rain")));

            int leng = newMesh.Length;
            for (int i = 0; i < leng; i++)
            {
                GameObject fool = GameObject.Instantiate(floor);
                fool.transform.parent = obj.transform;
                fool.name = "floor_" + i;
                fool.GetComponent<MeshFilter>().mesh = newMesh[i];
                //fool.GetComponent<MeshFilter>().mesh.colors = newColor[i].ToArray();
                fool.GetComponent<MeshRenderer>().material = floorMat;
            }
        }
        else
        {
            List<Vector3> tmp_Voltexpoints = ReConstruct_VoltexPoints(VoltexPoints, axis, p);
            List<float> tmp_curdb = ReConstruct_Curdb(curdb, axis, p);

            Mesh newMesh = SetNewMeshXY(tmp_Voltexpoints, tmp_curdb, WeatherVariableName, axis, !(WeatherVariableName.Contains("Cloud") || WeatherVariableName.Contains("Rain")));

            Debug.Log("Mesh Vertices count: " + newMesh.vertices.Length);
            Debug.Log("Mesh Colors count: " + newMesh.colors.Length);
            Debug.Log("Mesh triangles count: " + newMesh.triangles.Length);


            //int leng = newMesh.Length;
            for (int i = 0; i < 1; i++)
            {
                GameObject fool = GameObject.Instantiate(floor);
                fool.transform.parent = obj.transform;
                fool.name = "floor_" + i;
                fool.GetComponent<MeshFilter>().mesh = newMesh;
                //fool.GetComponent<MeshFilter>().mesh.colors = newColor[i].ToArray();
                fool.GetComponent<MeshRenderer>().material = floorMat;
            }
        }

    }

    /// <summary>
    /// 输出所需变量给定位置的变量值
    /// </summary>
    /// <param name="VariableNames">变量名称集合</param>
    /// <param name="Log">经度</param>
    /// <param name="Lat">纬度</param>
    /// <param name="h">层数</param>
    public List<float> GetCertainPointWholeTimeValues(List<string> VariableNames, int LogNum, int LatNum, int h)
    {
        //List<List<float>>[] curdb = new List<List<float>>[] { };
        List<float> tmpdb = new List<float>();

        //if (VariableNames.Contains("Cloud"))
        //{
        //    curdb = oriCloudData;
        //}
        //else if (VariableNames.Contains("Rain"))
        //{
        //    curdb = oriRainData;
        //}
        //else if (VariableNames.Contains("Ice"))
        //{
        //    curdb = oriIceData;
        //}
        //else if (VariableNames.Contains("Snow"))
        //{
        //    curdb = oriSnowData;
        //}
        //else if (VariableNames.Contains("Graupel"))
        //{
        //    curdb = oriGraupData;
        //}
        //else if (VariableNames.Contains("All"))
        //{
        //    curdb = oriMixData;
        //}
        
        for (int t = 0; t < timeCount; ++t)
        {
            if (stackData == null)
                Debug.Log("Null");
            tmpdb.Add(stackData[LogNum + log * LatNum + latlogCount * h]);
        }

        return tmpdb;
        
    }


    /// <summary>
    /// Res the construct data.
    /// </summary>
    /// <returns>The construct data.</returns>
    /// <param name="originaldata">Originaldata.</param>
    /// <param name="timeCount">Time count.</param>
    /// <param name="fromtopCount">Fromtop count.</param>
    /// <param name="latlogCount">Latlog count.</param>
    List<List<float>>[] ReConstructData(float[] originaldata, int timeCount, int fromtopCount, int latlogCount)
	{
        List<List<float>>[] ConstructData = new List<List<float>>[timeCount];
		for (int i = 0; i < timeCount; i++) {
			List<List<float>> fromtop = new List<List<float>> ();
			for (int j = 0; j < fromtopCount; j++) {
				List<float> temp = new List<float> ();
				for (int k = 0; k < latlogCount; k++) {
					temp.Add (originaldata [k + latlogCount * j + latlogCount * fromtopCount * i]);
//					if (temp [k] > 0.001f) {
//						Debug.Log (temp[k]);
//						break;
//					}
				}
				fromtop.Add (temp);
			}
			ConstructData [i] = fromtop;
		}

        return ConstructData;
	}

    List<Vector3> ReConstruct_VoltexPoints(List<Vector3>[] VoltexPoints,string axis,int p)
    {
        List<Vector3> tmp_p = new List<Vector3>();
        if (axis.ToLower() == "x")
        {
            foreach(List<Vector3> v in VoltexPoints)
            {
                for(int i = 0; i < 450; ++i)
                {
                    tmp_p.Add(v[450 * p + i]);
                }
            }
        }
        else if (axis.ToLower() == "y")
        {
            foreach (List<Vector3> v in VoltexPoints)
            {
                for (int i = 0; i < 420; ++i)
                {
                    tmp_p.Add(v[p + 450 * i]);
                }
            }
        }
        
        return tmp_p;
    }
    
    List<float> ReConstruct_Curdb(List<List<float>> curdb, string axis, int p)
    {
        List<float> tmp_db = new List<float>();
        if (axis.ToLower() == "x")
        {
            foreach (List<float> db in curdb)
            {
                for (int i = 0; i < 450; ++i)
                {
                    tmp_db.Add(db[450 * p + i]);
                }
            }

        }
        else if (axis.ToLower() == "y") 
        {
            foreach (List<float> db in curdb)
            {
                for (int i = 0; i < 420; ++i)
                {
                    tmp_db.Add(db[p + 450 * i]);
                }
            }
        }
        
        return tmp_db;
    }

    public void setLayerMark(string WeatherVariableName, string axis, int layerMark)
    {
        try
        {
            GameObject obj = GameObject.Find(WeatherVariableName + "Mesh");
            GameObject objChild;
            for (int i = 0; i < obj.transform.childCount; ++i)
            {
                objChild = obj.transform.GetChild(i).gameObject;
                if (objChild.name.Split(' ')[1] == layerMark.ToString() && objChild.name.Contains(axis.ToUpper()))
                {
                    Debug.Log(objChild.name);
                    foreach (Transform cchild in objChild.transform)
                    {
                        switch (axis.ToLower())
                        {
                            case "x":
                                cchild.gameObject.layer = 9;
                                break;
                            case "y":
                                cchild.gameObject.layer = 10;
                                break;
                            case "z":
                                cchild.gameObject.layer = 11;
                                break;
                            default:
                                Debug.Log("case error");
                                break;
                        }

                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    Mesh[] SetNewMesh(List<Vector3> vertices, List<float> Construct_i_Data, string WeatherVariableName, bool colorful)
    {

        int count = 42;
        int VerCount = 11;
        float proportion;
        Color c = Color.clear;

        Mesh[] myMesh = new Mesh[count];
        for (int k = 0; k < count; k++)
        {
            myMesh[k] = new Mesh();
            List<Vector3> newVecs = new List<Vector3>();        //初始化顶点坐标
            List<Color> newC = new List<Color>();               //初始化顶点颜色

            for (int i = 0; i < VerCount; i++)
            {
                int dataindex = 3 * (k * VerCount + i);
                dataindex = dataindex - 3 * k;
                if (dataindex > 419)
                    break;
                for (int j = 0; j < 450; j += 3)
                {
                    newVecs.Add(vertices[dataindex * 450 + j]);         //  添加mesh坐标
                    //需要彩色显示
                    if (colorful)
                    {
                        float value;
                        if (WeatherVariableName == "Cloud")
                        {
                            value = (MaxData - 2.5f * Construct_i_Data[dataindex * 450 + j]) / (MaxData - cloud_mask_rate * MaxData);
                        }
                        else if (WeatherVariableName == "Rain")
                        {
                            value = (MaxData - 3.0f * Construct_i_Data[dataindex * 450 + j]) / (MaxData - rain_mask_rate * MaxData);
                        }
                        else if (WeatherVariableName == "Ice")
                        {
                            value = (MaxData - 1.0f * Construct_i_Data[dataindex * 450 + j]) / (MaxData - ice_mask_rate * MaxData);
                        }
                        else if (WeatherVariableName == "Snow")
                        {
                            value = (MaxData - 3.0f * Construct_i_Data[dataindex * 450 + j]) / (MaxData - snow_mask_rate * MaxData);
                        }
                        else if (WeatherVariableName == "Graupel")
                        {
                            value = (MaxData - 3.5f * Construct_i_Data[dataindex * 450 + j]) / (MaxData - graup_mask_rate * MaxData);         //4.5f 高亮提升 即台风眼红色区域更加明显  1.0f为显示原图
                        }
                        else
                        {
                            value = (MaxData - 3.5f * Construct_i_Data[dataindex * 450 + j]) / (MaxData - all_mask_rate * MaxData);         //3.5f 高亮提升 即台风眼红色区域更加明显
                        }
                        value = value < 0 ? 0 : value;
                        value = value > 1 ? 1 : value;
                        //将HSV的颜色值转为RGB的颜色值
                        if (value == 1)
                            c = Color.clear;
                        else
                        {
                            c = Color.HSVToRGB(value * 2 / 3, 1, 1);
                            c.a = Opacity;
                        }
                    }
                    //不需要彩色显示
                    else
                    {
                        proportion = 0.75f + 0.25f * Construct_i_Data[dataindex * 450 + j] / MaxData;
                        if (WeatherVariableName == "Cloud" || WeatherVariableName == "All")
                        {
                            c = new Color(proportion, proportion, proportion, Opacity);
                        }
                        else if (WeatherVariableName == "Rain")
                        {
                            c = new Color(0.75f * proportion, 0.75f * proportion, 0.75f * proportion, Opacity - 0.2f);
                        }
                        else if (WeatherVariableName == "Ice")
                        {
                            c = new Color(proportion, 0, 0, Opacity - 0.15f);
                        }
                        else if (WeatherVariableName == "Snow")
                        {
                            c = new Color(0, proportion, 0, Opacity - 0.15f);
                        }
                        else if (WeatherVariableName == "Graupel")
                        {
                            c = new Color(0, 0, proportion, Opacity - 0.15f);
                        }
                    }
                    newC.Add(c);

                }
            }
            myMesh[k].vertices = newVecs.ToArray();
            myMesh[k].colors = newC.ToArray();
            //Debug.Log("Vecs: " + newVecs.Count);

            List<int> triagles = new List<int>();
            for (int i = 0; i < VerCount - 1; i++)
            {
                if (i + k * VerCount - k > 118)         //维度索引最多到419, 超出419 out of range
                    break;

                for (int j = 0; j < 149; j++)
                {
                    //if (Construct_i_Data[2 * (k * VerCount + i - k) * 450 + 2 * j] < 8 * 1e-5)        //物理值为0 直接跳过

                    /*如果是云变量或雨变量 不受mask_rate 影响  显示全模型 物理值后5% 直接跳过 不渲染
                     * 以下判断类似
                     */
                    if (WeatherVariableName.Contains("Cloud") &&
                        Construct_i_Data[3 * (k * VerCount + i - k) * 450 + 3 * j] <= cloud_mask_rate * MaxData &&
                        Construct_i_Data[3 * (k * VerCount + i - k) * 450 + 3 * j + 3] <= cloud_mask_rate * MaxData &&
                        Construct_i_Data[3 * (k * VerCount + i - k + 1) * 450 + 3 * j + 3] <= cloud_mask_rate * MaxData)
                        continue;
                    else if (WeatherVariableName.Contains("Rain") &&
                        Construct_i_Data[3 * (k * VerCount + i - k) * 450 + 3 * j] <= rain_mask_rate * MaxData &&
                        Construct_i_Data[3 * (k * VerCount + i - k) * 450 + 3 * j + 3] <= rain_mask_rate * MaxData &&
                        Construct_i_Data[3 * (k * VerCount + i - k + 1) * 450 + 3 * j + 3] <= rain_mask_rate * MaxData)
                        continue;
                    else if (WeatherVariableName.Contains("Ice") &&
                        Construct_i_Data[3 * (k * VerCount + i - k) * 450 + 3 * j] <= ice_mask_rate * MaxData &&
                        Construct_i_Data[3 * (k * VerCount + i - k) * 450 + 3 * j + 3] <= ice_mask_rate * MaxData &&
                        Construct_i_Data[3 * (k * VerCount + i - k + 1) * 450 + 3 * j + 3] <= ice_mask_rate * MaxData)
                        continue;
                    else if (WeatherVariableName.Contains("Snow") &&
                        Construct_i_Data[3 * (k * VerCount + i - k) * 450 + 3 * j] <= snow_mask_rate * MaxData &&
                        Construct_i_Data[3 * (k * VerCount + i - k) * 450 + 3 * j + 3] <= snow_mask_rate * MaxData &&
                        Construct_i_Data[3 * (k * VerCount + i - k + 1) * 450 + 3 * j + 3] <= snow_mask_rate * MaxData)
                        continue;
                    else if (WeatherVariableName.Contains("Graupel") &&
                        Construct_i_Data[3 * (k * VerCount + i - k) * 450 + 3 * j] <= graup_mask_rate * MaxData &&
                        Construct_i_Data[3 * (k * VerCount + i - k) * 450 + 3 * j + 3] <= graup_mask_rate * MaxData &&
                        Construct_i_Data[3 * (k * VerCount + i - k + 1) * 450 + 3 * j + 3] <= graup_mask_rate * MaxData)
                        continue;
                    else if(WeatherVariableName.Contains("All") &&
                        Construct_i_Data[3 * (k * VerCount + i - k) * 450 + 3 * j] <= all_mask_rate * MaxData &&
                        Construct_i_Data[3 * (k * VerCount + i - k) * 450 + 3 * j + 3] <= all_mask_rate * MaxData &&
                        Construct_i_Data[3 * (k * VerCount + i - k + 1) * 450 + 3 * j + 3] <= all_mask_rate * MaxData)
                        continue;
                    
                    //triagles.Add (i * 450 + j + 0);
                    //triagles.Add (i * 450 + j + 1);
                    //triagles.Add (i * 450 + j + 1 + 450);
                    //triagles.Add (i * 450 + j + 1 + 450);
                    //triagles.Add (i * 450 + j + 0 + 450);
                    //triagles.Add (i * 450 + j + 0);
                    triagles.Add(i * 150 + j + 1);
                    triagles.Add(i * 150 + j + 0);
                    triagles.Add(i * 150 + j + 1 + 150);
                    triagles.Add(i * 150 + j + 0 + 150);
                    triagles.Add(i * 150 + j + 1 + 150);
                    triagles.Add(i * 150 + j + 0);
                }
            }
            //Debug.Log("Tragles: " + triagles.Count);
            myMesh[k].triangles = triagles.ToArray();
            myMesh[k].RecalculateNormals();
            myMesh[k].RecalculateBounds();
        }

        return myMesh;
    }


    Mesh SetNewMeshXY(List<Vector3> vertices, List<float> Construct_i_Data, string WeatherVariableName, string axis, bool colorful)
    {

        //Debug.Log("Vertises count: " + vertices.Count);

        int count = 27;
        int VerCount = 1;

        if (axis.ToLower() == "x")
        {
            VerCount = 450;
        }
        else if (axis.ToLower() == "y")
        {
            VerCount = 420;
        }

        float proportion;
        Color c = Color.clear;

        Mesh myMesh = new Mesh();


        List<Vector3> newVecs = new List<Vector3>();        //初始化顶点坐标
        List<Color> newC = new List<Color>();               //初始化顶点颜色

        for (int i = 0; i < count; i++)
        {
            int dataindex = i;

            for (int j = 0; j < VerCount; j += 3)
            {
                newVecs.Add(vertices[dataindex * VerCount + j]);        //  添加mesh坐标
                                                                        //需要彩色显示
                if (colorful)
                {
                    float value;
                    if (WeatherVariableName == "Cloud")
                    {
                        value = (MaxData - 2.5f * Construct_i_Data[dataindex * VerCount + j]) / (MaxData - cloud_mask_rate * MaxData);
                    }
                    else if (WeatherVariableName == "Rain")
                    {
                        value = (MaxData - 3.0f * Construct_i_Data[dataindex * VerCount + j]) / (MaxData - rain_mask_rate * MaxData);
                    }
                    else if (WeatherVariableName == "Ice")
                    {
                        value = (MaxData - 1.0f * Construct_i_Data[dataindex * VerCount + j]) / (MaxData - ice_mask_rate * MaxData);
                    }
                    else if (WeatherVariableName == "Snow")
                    {
                        value = (MaxData - 3.0f * Construct_i_Data[dataindex * VerCount + j]) / (MaxData - snow_mask_rate * MaxData);
                    }
                    else if (WeatherVariableName == "Graupel")
                    {
                        value = (MaxData - 3.5f * Construct_i_Data[dataindex * VerCount + j]) / (MaxData - graup_mask_rate * MaxData);         //4.5f 高亮提升 即台风眼红色区域更加明显  1.0f为显示原图
                    }
                    else
                    {
                        value = (MaxData - 3.5f * Construct_i_Data[dataindex * VerCount + j]) / (MaxData - all_mask_rate * MaxData);         //3.5f 高亮提升 即台风眼红色区域更加明显
                    }
                    value = value < 0 ? 0 : value;
                    value = value > 1 ? 1 : value;
                    //将HSV的颜色值转为RGB的颜色值
                    if (value == 1)
                        c = Color.clear;
                    else
                    {
                        c = Color.HSVToRGB(value * 2 / 3, 1, 1);
                        c.a = Opacity;
                    }
                }
                //不需要彩色显示
                else
                {
                    proportion = 0.75f + 0.25f * Construct_i_Data[dataindex * 27 + j] / MaxData;
                    if (WeatherVariableName == "Cloud" || WeatherVariableName == "All")
                    {
                        c = new Color(proportion, proportion, proportion, Opacity);
                    }
                    else if (WeatherVariableName == "Rain")
                    {
                        c = new Color(0.75f * proportion, 0.75f * proportion, 0.75f * proportion, Opacity - 0.2f);
                    }
                    else if (WeatherVariableName == "Ice")
                    {
                        c = new Color(proportion, 0, 0, Opacity - 0.15f);
                    }
                    else if (WeatherVariableName == "Snow")
                    {
                        c = new Color(0, proportion, 0, Opacity - 0.15f);
                    }
                    else if (WeatherVariableName == "Graupel")
                    {
                        c = new Color(0, 0, proportion, Opacity - 0.15f);
                    }
                }
                newC.Add(c);

            }
        }
        myMesh.vertices = newVecs.ToArray();
        myMesh.colors = newC.ToArray();

        List<int> triagles = new List<int>();
        for (int i = 0; i < count - 1; i++)
        {
            for (int j = 0; j < VerCount / 3 - 2; j++)
            {
                //if (Construct_i_Data[2 * (k * VerCount + i - k) * 450 + 2 * j] < 8 * 1e-5)        //物理值为0 直接跳过
                if (Construct_i_Data[i * VerCount + 3 * j] == 0 && Construct_i_Data[i * VerCount + 3 * j + 3] == 0 && Construct_i_Data[(i + 1) * VerCount + 3 * j + 3] == 0)       //物理值为0 直接跳过
                    continue;

                if (WeatherVariableName.Contains("Cloud") &&
                    Construct_i_Data[i * VerCount + 3 * j] <= cloud_mask_rate * MaxData &&
                    Construct_i_Data[i * VerCount + 3 * j + 3] <= cloud_mask_rate * MaxData &&
                    Construct_i_Data[(i + 1) * VerCount + 3 * j + 3] <= cloud_mask_rate * MaxData)
                    continue;
                else if (WeatherVariableName.Contains("Rain") &&
                    Construct_i_Data[i * VerCount + 3 * j] <= rain_mask_rate * MaxData &&
                    Construct_i_Data[i * VerCount + 3 * j + 3] <= rain_mask_rate * MaxData &&
                    Construct_i_Data[(i + 1) * VerCount + 3 * j + 3] <= rain_mask_rate * MaxData)
                    continue;
                else if (WeatherVariableName.Contains("Ice") &&
                    Construct_i_Data[i * VerCount + 3 * j] <= ice_mask_rate * MaxData &&
                    Construct_i_Data[i * VerCount + 3 * j + 3] <= ice_mask_rate * MaxData &&
                    Construct_i_Data[(i + 1) * VerCount + 3 * j + 3] <= ice_mask_rate * MaxData)
                    continue;
                else if (WeatherVariableName.Contains("Snow") &&
                    Construct_i_Data[i * VerCount + 3 * j] <= snow_mask_rate * MaxData &&
                    Construct_i_Data[i * VerCount + 3 * j + 3] <= snow_mask_rate * MaxData &&
                    Construct_i_Data[(i + 1) * VerCount + 3 * j + 3] <= snow_mask_rate * MaxData)
                    continue;
                else if (WeatherVariableName.Contains("Graupel") &&
                    Construct_i_Data[i * VerCount + 3 * j] <= graup_mask_rate * MaxData &&
                    Construct_i_Data[i * VerCount + 3 * j + 3] <= graup_mask_rate * MaxData &&
                    Construct_i_Data[(i + 1) * VerCount + 3 * j + 3] <= graup_mask_rate * MaxData)
                    continue;
                else if (WeatherVariableName.Contains("All") &&
                    Construct_i_Data[i * VerCount + 3 * j] <= all_mask_rate * MaxData &&
                    Construct_i_Data[i * VerCount + 3 * j + 3] <= all_mask_rate * MaxData &&
                    Construct_i_Data[(i + 1) * VerCount + 3 * j + 3] <= all_mask_rate * MaxData)
                    continue;

                triagles.Add(i * VerCount / 3 + j + 0);
                triagles.Add(i * VerCount / 3 + j + 1);
                triagles.Add(i * VerCount / 3 + j + 1 + VerCount / 3);
                triagles.Add(i * VerCount / 3 + j + 1 + VerCount / 3);
                triagles.Add(i * VerCount / 3 + j + 0 + VerCount / 3);
                triagles.Add(i * VerCount / 3 + j + 0);
                //triagles.Add(i * VerCount / 3 + j + 1);
                //triagles.Add(i * VerCount / 3 + j + 0);
                //triagles.Add(i * VerCount / 3 + j + 1 + VerCount / 3);
                //triagles.Add(i * VerCount / 3 + j + 0 + VerCount / 3);
                //triagles.Add(i * VerCount / 3 + j + 1 + VerCount / 3);
                //triagles.Add(i * VerCount / 3 + j + 0);
            }
        }
        //Debug.Log("Tragles: " + triagles.Count);

        myMesh.triangles = triagles.ToArray();
        myMesh.RecalculateNormals();
        myMesh.RecalculateBounds();


        return myMesh;
    }

    public static void Write_File(List<float> lf,string VariableName, string FileName)
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
