# NC_NetCDF
Universal Post Processing Visualization  Toolbox for NC files

This is Unity Project files, and scripts which is used to read NC files has been stored in /Assets/Scripts

The main scripts constrution:
--
- _netcdf.cs
  * Read .nc file in this script
- Ty_Class.cs
  * Main data structure to store .nc nodes data
- CreateFloor.cs
  * Show **3D** structure in Unity Engine
  
Data Path:
--
Now there is **no .nc file** in the data path ***/Assets/StreamingAssets*** due to the Space limitation, or we can add some .nc data to the path, and modify the ***DataPath*** variable in **CreateFloor.cs**

![image] DemoImage/white.jpg  

![image] DemoImage/color.jpg
