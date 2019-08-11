using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadNC : MonoBehaviour {

    public void Read_NCFile()
    {
        Ty_Class ty;
        int rd = 2; //选择读取的变量，一个nc文件保存有6个变量float值，选择一个读取保存入ty.stackData，rd = 0为"Cloud"，rd = 1为"Rain"，之后分别为"Ice"，"Snow"，"Graupel"，"All" 
        List<string> WeatherVariableNames;

        if (rd % 6 == 0)
        {
            WeatherVariableNames = new List<string>() { "Cloud" };
        }
        else if (rd % 6 == 1)
        {
            WeatherVariableNames = new List<string>() { "Rain" };
        }
        else if (rd % 6 == 2)
        {
            WeatherVariableNames = new List<string>() { "Ice" };
        }
        else if (rd % 6 == 3)
        {
            WeatherVariableNames = new List<string>() { "Snow" };
        }
        else if (rd % 6 == 4)
        {
            WeatherVariableNames = new List<string>() { "Graupel" };
        }
        else
        {
            WeatherVariableNames = new List<string>() { "All" };
        }

        string DataPath = Application.dataPath + @"/StreamingAssets/1.typhoon/wrfout_typhoon_001" + ".nc";     //选择读取的文件

        ty = new Ty_Class(DataPath, WeatherVariableNames);  //调用Ty_class.ReadNetCDFStackDataOnly 读取nc文件，坐标数据存入ty.VortexPoints，float value数据存入ty.stackData

        Debug.Log(ty.VortexPoints.Length);
        Debug.Log(ty.stackData.Length);

    }
}
