using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;


public class create_mesh
{
    public int colorMode;

    List<List<Vector3>> lst;
    float[,,] value;
    Vector3[,,] vortex;
    Vector3[,,] normal;

    bool[,,] b_v;
    float max, min;
    //	public GameObject target;

    bool testd = true;
    List<Vector3> l1 = new List<Vector3>();
    List<Vector3> l2 = new List<Vector3>();
    List<Color> l3 = new List<Color>();
    List<Mesh> L = new List<Mesh>();

    public List<Vector3> v_l = new List<Vector3>();
    public List<Color> c_l = new List<Color>();
    public List<Vector3> n_l = new List<Vector3>();


    public create_mesh()
    {
        colorMode = 0;
        l1 = new List<Vector3>();
        l2 = new List<Vector3>();
        l3 = new List<Color>();
        bool test_d = true;
        value = new float[27, 420, 450];
        vortex = new Vector3[27, 420, 450];
        normal = new Vector3[27, 420, 450];
        b_v = new bool[27, 420, 450];
        List<Vector3> v_l = new List<Vector3>();
        List<Color> c_l = new List<Color>();
        List<Vector3> n_l = new List<Vector3>();
    }

    public void Clear()
    {
        for (int i = 0; i < L.Count; ++i)
            L[i].Clear();
        l1.Clear();
        l2.Clear();
        l3.Clear();
        v_l.Clear();
        c_l.Clear();
        n_l.Clear();
    }

    public void input_single(List<Vector3>[] VortexPoints, float[] stackData, float max, float min, GameObject target, int mod, int index, float mask_rate)
    {
        //float mask_rate = 0f;
        //switch (rd)
        //{
        //    case 0:
        //        mask_rate = 0.01f;
        //        break;
        //    case 1:
        //        mask_rate = 0.01f;
        //        break;
        //    case 2:
        //        mask_rate = 0.15f;
        //        break;
        //    case 3:
        //        mask_rate = 0.01f;
        //        break;
        //    case 4:
        //        mask_rate = 0.01f;
        //        break;
        //    case 5:
        //        mask_rate = 0.02f;
        //        break;
        //}

        this.max = max;
        this.min = min;
        for (int k = 0; k < 27; k++)
        {
            for (int j = 0; j < 420; j++)
            {
                for (int i = 0; i < 450; i++)
                {

                    vortex[k, j, i] = VortexPoints[k][j * 450 + i];
                    value[k, j, i] = stackData[k * 420 * 450 + j * 450 + i];

                }
            }
        }

        for (int k = 0; k < 27; k++)
        {
            for (int j = 0; j < 420; j++)
            {
                for (int i = 0; i < 450; i++)
                {
                    if (value[k, j, i] > mask_rate * max)
                    {
                        b_v[k, j, i] = true;
                    }
                    else
                    {
                        b_v[k, j, i] = false;

                    }

                }

            }

        }
        Vector3 nx = vortex[0, 0, 1] - vortex[0, 0, 0];
        Vector3 nz = vortex[0, 1, 0] - vortex[0, 0, 0];

        if (mod == 1)
        {
            int x = index;
            for (int j = 0; j < 420 - 1; j++)
            {
                for (int i = 0; i < 450 - 1; i++)
                {

                    if (create_tria(vortex[x, j, i], vortex[x, j, i + 1], vortex[x, j + 1, i], vortex[x, j + 1, i + 1], Vector3.up, Vector3.up, Vector3.up, Vector3.up, b_v[x, j, i], b_v[x, j, i + 1], b_v[x, j + 1, i], b_v[x, j + 1, i + 1], value[x, j, i], value[x, j, i + 1], value[x, j + 1, i], value[x, j + 1, i + 1], v_l, n_l, c_l))
                    {

                    }

                }
            }
            create_obj1(v_l, c_l, n_l, target);
        }
        else if (mod == 2)
        {
            int j = index;
            for (int x = 0; x < 27 - 1; x++)
            {
                for (int i = 0; i < 450 - 1; i++)
                {
                    Vector3 v1, v2, v3, v4;
                    Vector3 n1, n2, n3, n4;
                    bool b1, b2, b3, b4;
                    float f1, f2, f3, f4;
                    b1 = b_v[x, j, i];
                    b2 = b_v[x, j, i + 1];
                    b3 = b_v[x + 1, j, i];
                    b4 = b_v[x + 1, j, i + 1];

                    n1 = normal[x, j, i];
                    n2 = normal[x, j, i + 1];
                    n3 = normal[x + 1, j, i];
                    n4 = normal[x + 1, j, i + 1];
                    f1 = value[x, j, i];
                    f2 = value[x, j, i + 1];
                    f3 = value[x + 1, j, i];
                    f4 = value[x + 1, j, i + 1];
                    v1 = vortex[x, j, i];
                    v2 = vortex[x, j, i + 1];
                    v3 = vortex[x + 1, j, i];
                    v4 = vortex[x + 1, j, i + 1];
                    if (create_tria(v1, v2, v3, v4, Vector3.back, Vector3.back, Vector3.back, Vector3.back, b1, b2, b3, b4, f1, f2, f3, f4, v_l, n_l, c_l))
                    {

                    }

                }
            }
            create_obj1(v_l, c_l, n_l, target);
        }

        else if (mod == 3)
        {
            int i = index;
            for (int x = 0; x < 27 - 1; x++)
            {
                for (int j = 0; j < 420 - 1; j++)
                {
                    if (create_tria(vortex[x, j, i], vortex[x + 1, j, i], vortex[x, j + 1, i], vortex[x + 1, j + 1, i], Vector3.left, Vector3.left, Vector3.left, Vector3.left, b_v[x, j, i], b_v[x + 1, j, i], b_v[x, j + 1, i], b_v[x + 1, j + 1, i], value[x, j, i], value[x + 1, j, i], value[x, j + 1, i], value[x + 1, j + 1, i], v_l, n_l, c_l))
                    {
                    }
                }
            }
            create_obj1(v_l, c_l, n_l, target);
        }
    }


    public void Input(List<Vector3>[] VortexPoints, float[] stackData, float max, float min, GameObject target, float mask_rate)
    {
        //float mask_rate = 0f;
        //switch (rd)
        //{
        //    case 0:
        //        mask_rate = 0.01f;
        //        break;
        //    case 1:
        //        mask_rate = 0.01f;
        //        break;
        //    case 2:
        //        mask_rate = 0.15f;
        //        break;
        //    case 3:
        //        mask_rate = 0.04f;
        //        break;
        //    case 4:
        //        mask_rate = 0.02f;
        //        break;
        //    case 5:
        //        mask_rate = 0.03f;
        //        break;
        //}

        this.max = max;
        this.min = min;
        for (int k = 0; k < 27; k++)
        {
            for (int j = 0; j < 420; j++)
            {
                for (int i = 0; i < 450; i++)
                {

                    vortex[k, j, i] = VortexPoints[k][j * 450 + i];
                    value[k, j, i] = stackData[k * 420 * 450 + j * 450 + i];

                }
            }
        }

        for (int k = 0; k < 27; k++)
        {
            for (int j = 0; j < 420; j++)
            {
                for (int i = 0; i < 450; i++)
                {
                    if (value[k, j, i] > mask_rate * max)
                    {
                        b_v[k, j, i] = true;
                    }
                    else
                    {
                        b_v[k, j, i] = false;

                    }

                }

            }

        }
        Vector3 nx = vortex[0, 0, 1] - vortex[0, 0, 0];
        Vector3 nz = vortex[0, 1, 0] - vortex[0, 0, 0];

        Vector3 n_right = vortex[1, 0, 1] - vortex[0, 0, 0]; Vector3 n_left = vortex[1, 0, 0] - vortex[0, 0, 1];
        Vector3 n_front = vortex[1, 1, 0] - vortex[0, 0, 0]; Vector3 n_back = vortex[1, 0, 0] - vortex[0, 1, 0];


        nx.Normalize();
        nz.Normalize();

        for (int k = 0; k < 27; k++)
        {
            for (int j = 0; j < 420; j++)
            {
                for (int i = 0; i < 450; i++)
                {
                    if (b_v[k, j, i])
                    {
                        Vector3 n = Vector3.zero;
                        //						if (k > 0 && !b_v [k - 1, j, i]) {
                        //							n += new Vector3 (0, -1, 0);
                        //						}
                        if ((k < 26 && !b_v[k + 1, j, i]) || k == 26)
                        {
                            n += new Vector3(0, 1, 0);
                        }

                        //												if ( k == 25 && j == 3 && i ==308) 
                        //						{
                        //							Debug.Log (n);
                        //						}
                        if ((j > 0 && !b_v[k, j - 1, i]) || j == 0)
                        {
                            n += -nz;
                        }
                        //												if ( k == 25 && j == 3 && i ==308) 
                        //						{
                        //							Debug.Log (n);
                        //						}
                        if ((j < 419 && !b_v[k, j + 1, i]) || j == 419)
                        {
                            n += nz;
                        }
                        //												if ( k == 25 && j == 3 && i ==308) 
                        //						{
                        //							Debug.Log (n);
                        //						}
                        if ((i > 0 && !b_v[k, j, i - 1]) || i == 0)
                        {
                            n += -nx;
                        }
                        //												if ( k == 25 && j == 3 && i ==308) 
                        //						{
                        //							Debug.Log (n);
                        //						}
                        if ((i < 449 && !b_v[k, j, i + 1]) || i == 449)
                        {
                            n += nx;
                        }
                        //						if ( k == 25 && j == 3 && i ==308) 
                        //						{
                        //							Debug.Log (n);
                        //						}

                        if (n.x != 0 || n.y != 0 || n.z != 0)
                        {


                            //							if (testd && k == 25&&j>0&&i>0) {
                            //								testd = false;
                            //								Debug.Log (i+" "+j+" "+k);
                            //								Debug.Log (!b_v [k - 1, j, i]+" "+!b_v [k + 1, j, i]);
                            //								Debug.Log (!b_v [k , j- 1, i]+" "+!b_v [k, j+1, i]);
                            //								Debug.Log (!b_v [k , j, i-1]+" "+!b_v [k , j, i+1]);
                            //								Debug.Log (normal [k, j, i]);
                            //
                            //							}
                            n.Normalize();
                            normal[k, j, i] = n;

                            if (n.x == 0 && n.y == 0 && n.z == 0)
                                n = new Vector3(0, 1, 0);

                            //							if ( k == 25 && j == 3 && i ==308) 
                            //							{
                            //								Debug.Log (n);
                            //							}
                            //							n = nx;
                        }
                        else
                        {
                            //							n = new Vector3 (0, 0.1f, 0);
                            //							normal [k, j, i] = n;
                            ////							n = nx;
                            //
                            //							if ( k == 26 && j == 3 && i ==308) 
                            //							{
                            //								Debug.Log (n);
                            //							}
                            if (i < 449 && k < 26)
                            {
                                if (!b_v[k + 1, j, i + 1])
                                    n += n_right;

                            }

                            if (i > 0 && k < 26)
                            {
                                if (!b_v[k + 1, j, i - 1])
                                    n += n_left;
                            }

                            if (j < 419 && k < 26)
                            {
                                if (!b_v[k + 1, j + 1, i])
                                    n += n_front;
                            }

                            if (j > 0 && k < 26)
                            {
                                if (!b_v[k + 1, j - 1, i])
                                    n += n_back;
                            }

                            if (n.x != 0 || n.y != 0 || n.z != 0)
                            {
                                n.Normalize();
                                normal[k, j, i] = n.normalized;
                                //								if ( k == 25 && j == 3 && i ==308) 
                                //								{
                                //									Debug.Log (normal [k, j, i]);
                                //								}
                            }
                            else
                            {
                                normal[k, j, i] = new Vector3(0, 1, 0);
                            }

                        }




                    }
                    else
                    {
                        normal[k, j, i] = new Vector3(0, 1, 0);
                    }

                }

            }
        }


        search_value(value, vortex, normal, b_v, v_l, n_l, c_l);
        create_obj1(v_l, c_l, n_l, target);
    }

    //	void Start () {
    //		l1= new List<Vector3> ();
    //		l2 = new List<Vector3> ();
    //		l3 = new List<Color> ();	
    //		bool test_d = true;
    //		value = new float[27 , 420 , 450];
    //		vortex = new Vector3[27 , 420 ,450];
    //		normal= new Vector3[27 , 420 ,450];
    //		b_v=new bool[27 , 420 , 450];
    //		nc = this.GetComponent<ReadNC> ();
    //
    //		//			Debug.LogError(k+" count="+count);
    //
    //		path=new string[120];
    //		for (int i = 1; i < 121; i++) {
    //			if (i < 10)
    //				path [i-1] = "wrfout_typhoon_00" + i.ToString ();
    //			else if(i<100)
    //				path [i-1] = "wrfout_typhoon_0" + i.ToString ();
    //			else	
    //				path [i-1] = "wrfout_typhoon_" + i.ToString ();	
    //		}
    //		Debug.Log (path[0]);
    //
    //		List<Vector3> v_l = new List<Vector3> ();
    //		List<Color> c_l = new List<Color> ();
    //		List<Vector3> n_l = new List<Vector3> ();
    //
    //	}
    //
    //	// Update is called once per frame
    //	void Update () {
    //		if (Input.GetMouseButtonDown (2)) 
    //		{
    //			mod = true;
    //
    //		}
    //
    //
    //
    //		if (mod) 
    //		{
    //			max = 0;
    //			min = 1;
    //			v_l.Clear ();
    //			c_l.Clear (); 
    //			n_l.Clear ();
    //			readFile (value,vortex,b_v,normal,path[turn]);
    //			turn++;
    //			int count = 0;
    //			for (int k = 0; k < 27; k++) 
    //			{
    //				for (int j= 0; j < 420;j++)
    //				{		
    //					for (int i = 0; i< 450; i++) 
    //					{
    //						if(j==0&&i==0)
    //						{
    //							//						Debug.Log (k);
    //							count=0;
    //						}
    //						if(b_v[k,j,i])
    //						{
    //							count++;
    //						}
    //
    //
    //
    //					}
    //				}
    //
    //			}
    //
    //			Debug.LogWarning ("here"+Time.realtimeSinceStartup);
    //
    //			search_value (value,vortex,normal, b_v, v_l,n_l,c_l);
    //			create_obj1 (v_l,c_l,n_l);
    //			Debug.LogWarning ("wow"+Time.realtimeSinceStartup);
    //			//			turn++;
    //			mod = false;
    //		}
    //	}
    //
    //	public void readFile(float[,,] value,Vector3[,,]vortex,bool[,,] b_v,Vector3[,,]normal,string path)
    //	{
    //		string line;
    //		string[] arg;
    //
    //		nc.Read_NCFile (path);
    //
    //		for (int k = 0; k < 27; k++)
    //			for (int j = 0; j < 420; j++)
    //				for (int i = 0; i < 450; i++)
    //				{
    //
    //
    //					{
    //
    //
    //						{
    //							//	line = sr.ReadLine ();
    //							//	arg = System.Text.RegularExpressions.Regex.Split (line, ",", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    //
    //							vortex [k, j, i] = nc.ty.VortexPoints[k] [ j * 450 + i]-new Vector3(108,0,16);
    //							value [k, j, i] = nc.ty.stackData[k * 420 * 450 + j * 450 + i];
    //							if (min > value [k, j, i]) {
    //								min = value [k, j, i];
    //							}
    //							if(max<value [k, j, i])
    //								max = value [k, j, i];
    //
    //
    //						}
    //					}
    //
    //				}
    //		//
    //		for (int k = 0; k < 27; k++)
    //			for (int j = 0; j < 420; j++)
    //				for (int i = 0; i < 450; i++) {
    //					if (value [k, j, i] >0.15*max) {
    //						b_v [k, j, i] = true;
    //						if (k == 0) 
    //						{
    //							//									Debug.Log (j+" "+i);
    //						}
    //					} else {
    //						b_v [k, j, i] = false;
    //					}
    //				}
    //		Vector3 nx = vortex [0, 0, 1] - vortex [0, 0, 0];
    //		Vector3 nz =vortex [0, 1, 0] - vortex [0, 0, 0];
    //
    //		Vector3 n_right= vortex [1, 0, 1] - vortex [0, 0, 0];Vector3 n_left= vortex [1, 0, 0] - vortex [0, 0, 1];
    //		Vector3 n_front= vortex [1, 1, 0] - vortex [0, 0, 0];Vector3 n_back= vortex [1, 0, 0] - vortex [0, 1, 0];
    //
    //
    //		nx.Normalize ();
    //		nz.Normalize ();
    //		//		Debug.Log ("nx"+nx.ToString("0.00000000"));
    //		//		Debug.Log ("nz"+nz.ToString("0.00000000"));
    //		for (int k = 0; k < 27; k++) {
    //			for (int j = 0; j < 420; j++) {
    //				for (int i = 0; i < 450; i++) 
    //				{
    //					if (b_v [k, j, i]) {
    //						Vector3 n = Vector3.zero;
    //						//						if (k > 0 && !b_v [k - 1, j, i]) {
    //						//							n += new Vector3 (0, -1, 0);
    //						//						}
    //						if ((k < 26 && !b_v [k + 1, j, i]) || k == 26) {
    //							n += new Vector3 (0, 1, 0);
    //						}
    //
    //						//												if ( k == 25 && j == 3 && i ==308) 
    //						//						{
    //						//							Debug.Log (n);
    //						//						}
    //						if ((j > 0 && !b_v [k, j - 1, i]) || j == 0) {
    //							n += -nz;
    //						}
    //						//												if ( k == 25 && j == 3 && i ==308) 
    //						//						{
    //						//							Debug.Log (n);
    //						//						}
    //						if ((j < 419 && !b_v [k, j + 1, i]) || j == 419) {
    //							n += nz;
    //						}
    //						//												if ( k == 25 && j == 3 && i ==308) 
    //						//						{
    //						//							Debug.Log (n);
    //						//						}
    //						if ((i > 0 && !b_v [k, j, i - 1]) || i == 0) {
    //							n += -nx;
    //						}
    //						//												if ( k == 25 && j == 3 && i ==308) 
    //						//						{
    //						//							Debug.Log (n);
    //						//						}
    //						if ((i < 449 && !b_v [k, j, i + 1]) || i == 449) {
    //							n += nx;
    //						}
    //						//						if ( k == 25 && j == 3 && i ==308) 
    //						//						{
    //						//							Debug.Log (n);
    //						//						}
    //
    //						if (n.x != 0 || n.y != 0 || n.z != 0) {
    //
    //
    //							//							if (testd && k == 25&&j>0&&i>0) {
    //							//								testd = false;
    //							//								Debug.Log (i+" "+j+" "+k);
    //							//								Debug.Log (!b_v [k - 1, j, i]+" "+!b_v [k + 1, j, i]);
    //							//								Debug.Log (!b_v [k , j- 1, i]+" "+!b_v [k, j+1, i]);
    //							//								Debug.Log (!b_v [k , j, i-1]+" "+!b_v [k , j, i+1]);
    //							//								Debug.Log (normal [k, j, i]);
    //							//
    //							//							}
    //							n.Normalize ();
    //							normal [k, j, i] = n;
    //
    //							if(n.x==0&&n.y==0&&n.z==0)
    //								n = new Vector3 (0, 1, 0);
    //
    //							//							if ( k == 25 && j == 3 && i ==308) 
    //							//							{
    //							//								Debug.Log (n);
    //							//							}
    //							//							n = nx;
    //						} else {
    //							//							n = new Vector3 (0, 0.1f, 0);
    //							//							normal [k, j, i] = n;
    //							////							n = nx;
    //							//
    //							//							if ( k == 26 && j == 3 && i ==308) 
    //							//							{
    //							//								Debug.Log (n);
    //							//							}
    //							if (i < 449&&k<26) 
    //							{
    //								if (!b_v [k + 1, j, i + 1])
    //									n += n_right;
    //
    //							}
    //
    //							if (i > 0 && k < 26)
    //							{
    //								if (!b_v [k + 1, j, i - 1])
    //									n += n_left;
    //							}
    //
    //							if (j < 419 && k < 26) 
    //							{
    //								if (!b_v [k + 1, j+1, i ])
    //									n += n_front;
    //							}
    //
    //							if (j > 0 && k < 26) {
    //								if (!b_v [k + 1, j-1, i ])
    //									n += n_back;
    //							}
    //
    //							if (n.x != 0 || n.y != 0 || n.z != 0) {
    //								n.Normalize ();
    //								normal [k, j, i] = n.normalized;
    //								//								if ( k == 25 && j == 3 && i ==308) 
    //								//								{
    //								//									Debug.Log (normal [k, j, i]);
    //								//								}
    //							}
    //							else {
    //								normal [k, j, i] = new Vector3 (0, 1, 0);
    //							}
    //
    //						}
    //
    //
    //
    //
    //					} else {
    //						normal [k, j, i] = new Vector3 (0, 1, 0);
    //					}
    //
    //				}
    //
    //			}
    //		}
    //
    //
    //		//		
    //		//		Debug.Log(vortex.Length + " vortex: " + vortex[0,0,0] );
    //		//	//	Debug.Log(value.Length + " value: " + value[0] + value[1354] + value[5102999]);
    //		//		
    //		//		sr.Close();
    //	}




    void create_obj(List<Vector3> v_l, List<Color> c_l, List<Vector3> n_l)
    {

        GameObject b = new GameObject("haha", typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider));
        Mesh mr = b.GetComponent<MeshFilter>().mesh;//获取物体的mesh
        int len = v_l.Count;
        Debug.LogError(len + "len");

        int[] idx = new int[len];
        Vector3[] v = new Vector3[len];
        Color[] c = new Color[len];
        for (int i = 0; i < len; i++)
        {
            v[i] = v_l[i];
            c[i] = c_l[i];
            idx[i] = i;
            //			if(i<9)
            //			Debug.Log (v[i].ToString("0.0000000"));
        }


        mr.Clear();//清空当前mesh
        mr.vertices = v;//设置顶点
        mr.triangles = idx;//设置顶点
        mr.colors = c;//设置顶点颜色
        L.Add(mr);
    }
    void create_obj1(List<Vector3> v_l, List<Color> c_l, List<Vector3> n_l, GameObject target)
    {
        int num = v_l.Count / 60000;

        //		if (v_l.Count > num * 60000)
        num += 1;


        if (num == 1)
        {
            Mesh mr = target.GetComponent<MeshFilter>().mesh;//获取物体的mesh


            int len = v_l.Count;
            Debug.LogError(len + "len");

            int[] idx = new int[len];
            Vector3[] v = new Vector3[len];
            Color[] c = new Color[len];
            Vector3[] n = new Vector3[len];
            for (int i = 0; i < len; i++)
            {
                v[i] = v_l[i];
                c[i] = c_l[i];
                n[i] = n_l[i];
                idx[i] = i;

            }


            mr.Clear();//清空当前mesh
            mr.vertices = v;//设置顶点
            mr.normals = n;
            mr.triangles = idx;//设置顶点
            mr.colors = c;//设置顶点颜色

            L.Add(mr);
        }
        else
        {
            bool test = true;
            Mesh mr = target.GetComponent<MeshFilter>().mesh;//获取物体的mesh

            mr.subMeshCount = num;


            int len = 60000;
            int[] idx = new int[len];
            Vector3[] v = new Vector3[len];
            Color[] c = new Color[len];
            Vector3[] n = new Vector3[len];
            for (int i = 0; i < len; i++)
            {
                v[i] = v_l[i];
                c[i] = c_l[i];
                n[i] = n_l[i];
                idx[i] = i;

            }

            Debug.Log(v_l.Count + " " + n_l.Count + " " + num);
            mr.Clear();//清空当前mesh






            mr.vertices = v;//设置顶点
            mr.triangles = idx;//设置顶点
            mr.normals = n;
            mr.colors = c;//设置顶点颜色

            L.Add(mr);

            for (int i = 1; i < num; i++)
            {
                test = true;
                int[] idx1;
                Vector3[] v1;
                Color[] c1;
                Vector3[] n1;
                if (i < num - 1)
                {
                    idx1 = new int[len];
                    v1 = new Vector3[len];
                    c1 = new Color[len];
                    n1 = new Vector3[len];
                    for (int j = 0; j < len; j++)
                    {
                        v1[j] = v_l[j + 60000 * i];
                        c1[j] = c_l[j + 60000 * i];
                        n1[j] = n_l[j + 60000 * i];
                        idx1[j] = j;
                        //						if(test)//&&(n1 [i].x==0&&n1[i].y==0&&n1[i].z==0)
                        //						{
                        //							test = false;
                        //							Debug.LogWarning (n1 [i] + " " + i);
                        //						}

                    }
                    //					Debug.Log(n1[0]+" DD"+i);
                }
                else
                {
                    len = v_l.Count - 60000 * i;
                    idx1 = new int[len];
                    v1 = new Vector3[len];
                    c1 = new Color[len];
                    n1 = new Vector3[len];
                    for (int j = 0; j < len; j++)
                    {
                        v1[j] = v_l[j + 60000 * i];
                        c1[j] = c_l[j + 60000 * i];
                        n1[j] = n_l[j + 60000 * i];
                        idx1[j] = j;
                        //						if(test)//&&		(n1[i].x==0&&n1[i].y==0&&n1[i].z==0)
                        //						{
                        //							test = false;
                        //							Debug.LogWarning(n1[i]+" "+i);
                        //						}
                    }
                    //					Debug.Log(n1[0]+" DD"+i);
                }


                GameObject b = GameObject.Find((target.name + i.ToString()));
                if (b == null)
                {
                    b = new GameObject(target.name + i.ToString(), typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider));
                }

                Mesh _mymesh2;

                _mymesh2 = b.GetComponent<MeshFilter>().mesh;
                _mymesh2.Clear();//清空当前mesh
                _mymesh2.vertices = v1;//设置顶点
                _mymesh2.normals = n1;
                //				Debug.LogWarning(n1[0]+" DD"+i);
                _mymesh2.triangles = idx1;//设置顶点

                _mymesh2.colors = c1;//设置顶点颜色
                b.transform.parent = target.transform;
                b.transform.position = target.transform.position;
                b.GetComponent<Renderer>().material = target.GetComponent<Renderer>().material;

                L.Add(_mymesh2);
            }
        }
    }


    void top_down(float[,,] value, Vector3[,,] vortex, Vector3[,,] normal, bool[,,] b_v, List<Vector3> v_l, List<Vector3> n_l, List<Color> c_l)
    {
        for (int j = 0; j < 420 - 1; j++)
        {
            for (int i = 0; i < 450 - 1; i++)
            {
                for (int x = 26; x >= 0; x--)
                {
                    bool show = false;
                    int count = 0;
                    int true_count = 0;
                    if (x == 26)
                    {
                        show = true;
                        if (b_v[x, j, i])
                        {
                            true_count++;
                        }
                        if (b_v[x, j, i + 1])
                        {
                            true_count++;
                        }
                        if (b_v[x, j + 1, i])
                        {
                            true_count++;
                        }
                        if (b_v[x, j + 1, i + 1])
                        {
                            true_count++;
                        }
                    }
                    else
                    {


                        if (b_v[x, j, i])
                        {
                            true_count++;
                        }
                        if (b_v[x, j, i + 1])
                        {
                            true_count++;
                        }
                        if (b_v[x, j + 1, i])
                        {
                            true_count++;
                        }
                        if (b_v[x, j + 1, i + 1])
                        {
                            true_count++;
                        }
                        if (true_count >= 3)
                        {
                            if (!b_v[x + 1, j, i] && b_v[x, j, i])
                            {
                                count++;
                            }
                            if (!b_v[x + 1, j, i + 1] && b_v[x, j, i + 1])
                            {
                                count++;
                            }

                            if (!b_v[x + 1, j + 1, i] && b_v[x, j + 1, i])

                            {
                                count++;
                            }

                            if (!b_v[x + 1, j + 1, i + 1] && b_v[x, j + 1, i + 1])
                            {
                                count++;
                            }

                            if (count >= 1)
                            {
                                //								if (count == 1) 
                                //								{
                                //									if (true_count == 4) 
                                //									{
                                //									
                                //									}
                                //								}
                                show = true;
                            }

                        }


                    }
                    if (true_count >= 3 && show)
                        if (create_tria(vortex[x, j, i], vortex[x, j, i + 1], vortex[x, j + 1, i], vortex[x, j + 1, i + 1], normal[x, j, i], normal[x, j, i + 1], normal[x, j + 1, i], normal[x, j + 1, i + 1], b_v[x, j, i], b_v[x, j, i + 1], b_v[x, j + 1, i], b_v[x, j + 1, i + 1], value[x, j, i], value[x, j, i + 1], value[x, j + 1, i], value[x, j + 1, i + 1], v_l, n_l, c_l))
                        {
                            //						break;

                            //						if(i==308&&(j==3||j==2||j==4)&&x==26)
                            //							create_tria (vortex [x, j, i], vortex [x, j, i + 1], vortex [x, j + 1, i], vortex [x, j + 1, i + 1], normal [x, j, i], normal [x, j, i + 1],normal [x, j + 1, i], normal [x, j + 1, i + 1], b_v [x, j, i], b_v [x, j, i + 1], b_v [x, j + 1, i], b_v [x, j + 1, i + 1], value [x, j, i], value [x, j, i + 1], value [x, j + 1, i], value [x, j + 1, i + 1], l1, l2, l3);
                        }

                }
            }
        }
    }

    void down_top(float[,,] value, Vector3[,,] vortex, Vector3[,,] normal, bool[,,] b_v, List<Vector3> v_l, List<Vector3> n_l, List<Color> c_l)
    {
        for (int j = 0; j < 420 - 1; j++)
        {
            for (int i = 0; i < 450 - 1; i++)
            {
                for (int x = 0; x < 27; x++)
                {
                    bool show = false;
                    int count = 0;
                    int true_count = 0;
                    if (x == 0)
                    {
                        show = true;
                        if (b_v[x, j, i])
                        {
                            true_count++;
                        }
                        if (b_v[x, j, i + 1])
                        {
                            true_count++;
                        }
                        if (b_v[x, j + 1, i])
                        {
                            true_count++;
                        }
                        if (b_v[x, j + 1, i + 1])
                        {
                            true_count++;
                        }
                    }
                    else
                    {


                        if (b_v[x, j, i])
                        {
                            true_count++;
                        }
                        if (b_v[x, j, i + 1])
                        {
                            true_count++;
                        }
                        if (b_v[x, j + 1, i])
                        {
                            true_count++;
                        }
                        if (b_v[x, j + 1, i + 1])
                        {
                            true_count++;
                        }
                        if (true_count >= 3)
                        {
                            if (!b_v[x - 1, j, i] && b_v[x, j, i])
                            {
                                count++;
                            }
                            if (!b_v[x - 1, j, i + 1] && b_v[x, j, i + 1])
                            {
                                count++;
                            }

                            if (!b_v[x - 1, j + 1, i] && b_v[x, j + 1, i])

                            {
                                count++;
                            }

                            if (!b_v[x - 1, j + 1, i + 1] && b_v[x, j + 1, i + 1])
                            {
                                count++;
                            }

                            if (count >= 1)
                            {
                                //								if (count == 1) 
                                //								{
                                //									if (true_count == 4) 
                                //									{
                                //									
                                //									}
                                //								}
                                show = true;
                            }

                        }


                    }
                    if (true_count >= 3 && show)
                        if (create_tria(vortex[x, j, i], vortex[x, j, i + 1], vortex[x, j + 1, i], vortex[x, j + 1, i + 1], normal[x, j, i], normal[x, j, i + 1], normal[x, j + 1, i], normal[x, j + 1, i + 1], b_v[x, j, i], b_v[x, j, i + 1], b_v[x, j + 1, i], b_v[x, j + 1, i + 1], value[x, j, i], value[x, j, i + 1], value[x, j + 1, i], value[x, j + 1, i + 1], v_l, n_l, c_l))
                        {
                            //						break;
                        }

                }
            }
        }
    }

    void front_back(float[,,] value, Vector3[,,] vortex, Vector3[,,] normal, bool[,,] b_v, List<Vector3> v_l, List<Vector3> n_l, List<Color> c_l)
    {
        for (int x = 0; x < 27 - 1; x++)
        {
            for (int i = 0; i < 450 - 1; i++)
            {
                for (int j = 0; j < 420; j++)
                {

                    bool show = false;
                    //					if (j == 0 || !b_v [x, j - 1, i] ||! b_v [x, j-1, i + 1] || !b_v [x + 1, j-1, i] || !b_v [x + 1, j-1, i + 1])

                    {
                        //						show = true;

                        int count = 0;
                        if (j == 0)
                        {
                            show = true;
                        }
                        else
                        {
                            if (b_v[x, j, i])
                            {
                                if (!b_v[x, j - 1, i])
                                {
                                    count++;
                                }
                            }
                            else if (j < 419 && b_v[x, j + 1, i])
                            {
                                count++;
                            }


                            if (b_v[x, j, i + 1])
                            {
                                if (!b_v[x, j - 1, i + 1])
                                {
                                    count++;
                                }
                            }
                            else if (j < 419 && b_v[x, j + 1, i + 1])
                            {
                                count++;
                            }


                            if (b_v[x + 1, j, i])
                            {
                                if (!b_v[x + 1, j - 1, i])
                                {
                                    count++;
                                }
                            }
                            else if (j < 419 && b_v[x + 1, j + 1, i])
                            {
                                count++;
                            }


                            if (b_v[x + 1, j, i + 1])
                            {
                                if (!b_v[x + 1, j - 1, i + 1])
                                {
                                    count++;
                                }
                            }
                            else if (j < 419 && b_v[x + 1, j + 1, i + 1])
                            {
                                count++;
                            }

                            if (count >= 2)
                            {
                                //								show = true;
                                if (count == 2)
                                {
                                    if (!b_v[x, j, i] && !b_v[x, j, i + 1])
                                    {
                                        show = false;
                                    }
                                    else if (!b_v[x + 1, j, i] && !b_v[x + 1, j, i + 1])
                                    {
                                        show = false;
                                    }
                                    else if (!b_v[x, j, i] && !b_v[x + 1, j, i])
                                    {
                                        show = false;
                                    }
                                    else if (!b_v[x, j, i + 1] && !b_v[x + 1, j, i + 1])
                                    {
                                        show = false;
                                    }
                                    else
                                    {
                                        show = false;

                                        //										show = true;

                                        if (b_v[x, j, i] && b_v[x, j, i + 1] && b_v[x + 1, j, i] && b_v[x + 1, j, i + 1])
                                        {


                                            if (!b_v[x, j - 1, i] && !b_v[x, j - 1, i + 1])
                                            {
                                                show = true;
                                            }
                                            else if (!b_v[x + 1, j - 1, i] && !b_v[x + 1, j - 1, i + 1])
                                            {
                                                show = true;
                                            }
                                            else if (!b_v[x, j - 1, i] && !b_v[x + 1, j - 1, i])
                                            {

                                                //												show = true;
                                                Vector3 v1, v2, v3, v4;
                                                Vector3 n1, n2, n3, n4;
                                                bool b1, b2, b3, b4;
                                                float f1, f2, f3, f4;
                                                b1 = b_v[x, j, i];
                                                b2 = b_v[x, j - 1, i + 1];
                                                b3 = b_v[x + 1, j, i];
                                                b4 = b_v[x + 1, j - 1, i + 1];
                                                n1 = normal[x, j, i];
                                                n2 = normal[x, j - 1, i + 1];
                                                n3 = normal[x + 1, j, i];
                                                n4 = normal[x + 1, j - 1, i + 1];

                                                f1 = value[x, j, i];
                                                f2 = value[x, j - 1, i + 1];
                                                f3 = value[x + 1, j, i];
                                                f4 = value[x + 1, j - 1, i + 1];
                                                v1 = vortex[x, j, i];
                                                v2 = vortex[x, j - 1, i + 1];
                                                v3 = vortex[x + 1, j, i];
                                                v4 = vortex[x + 1, j - 1, i + 1];
                                                if (create_tria(v1, v2, v3, v4, n1, n2, n3, n4, b1, b2, b3, b4, f1, f2, f3, f4, v_l, n_l, c_l))
                                                {

                                                }
                                            }
                                            else if (!b_v[x, j - 1, i + 1] && !b_v[x + 1, j - 1, i + 1])
                                            {
                                                //												show = true;
                                                Vector3 v1, v2, v3, v4;
                                                bool b1, b2, b3, b4;
                                                Vector3 n1, n2, n3, n4;
                                                float f1, f2, f3, f4;
                                                b1 = b_v[x, j - 1, i];
                                                b2 = b_v[x, j, i + 1];
                                                b3 = b_v[x + 1, j - 1, i];
                                                b4 = b_v[x + 1, j, i + 1];

                                                n1 = normal[x, j - 1, i];
                                                n2 = normal[x, j, i + 1];
                                                n3 = normal[x + 1, j - 1, i];
                                                n4 = normal[x + 1, j, i + 1];
                                                f1 = value[x, j - 1, i];
                                                f2 = value[x, j, i + 1];
                                                f3 = value[x + 1, j - 1, i];
                                                f4 = value[x + 1, j, i + 1];
                                                v1 = vortex[x, j - 1, i];
                                                v2 = vortex[x, j, i + 1];
                                                v3 = vortex[x + 1, j - 1, i];
                                                v4 = vortex[x + 1, j, i + 1];
                                                if (create_tria(v1, v2, v3, v4, n1, n2, n3, n4, b1, b2, b3, b4, f1, f2, f3, f4, v_l, n_l, c_l))
                                                {

                                                }
                                            }

                                        }


                                        //										Debug.Log ();
                                    }

                                    //									show = false;
                                }
                                else
                                {
                                    show = true;
                                    int real_count = 0;
                                    if (b_v[x, j, i])
                                    {
                                        real_count++;
                                    }
                                    if (b_v[x, j, i + 1])
                                    {
                                        real_count++;
                                    }
                                    if (b_v[x + 1, j, i])
                                    {
                                        real_count++;
                                    }
                                    if (b_v[x + 1, j, i + 1])
                                    {
                                        real_count++;
                                    }
                                    if (real_count <= 3)
                                        show = false;

                                }
                            }
                            else if (count == 1)
                            {
                                Vector3 v1, v2, v3, v4;
                                bool b1, b2, b3, b4;
                                float f1, f2, f3, f4;
                                Vector3 n1, n2, n3, n4;
                                bool exist = false;
                                if (b_v[x, j, i] && b_v[x, j, i + 1] && b_v[x + 1, j, i] && b_v[x + 1, j, i + 1])
                                {
                                    if (!b_v[x, j - 1, i])
                                    {
                                        exist = true;
                                        b1 = b_v[x, j, i];
                                        b2 = b_v[x, j - 1, i + 1];
                                        b3 = b_v[x + 1, j, i];
                                        b4 = b_v[x + 1, j - 1, i + 1];


                                        n1 = normal[x, j, i];
                                        n2 = normal[x, j - 1, i + 1];
                                        n3 = normal[x + 1, j, i];
                                        n4 = normal[x + 1, j - 1, i + 1];
                                        f1 = value[x, j, i];
                                        f2 = value[x, j - 1, i + 1];
                                        f3 = value[x + 1, j, i];
                                        f4 = value[x + 1, j - 1, i + 1];
                                        v1 = vortex[x, j, i];
                                        v2 = vortex[x, j - 1, i + 1];
                                        v3 = vortex[x + 1, j, i];
                                        v4 = vortex[x + 1, j - 1, i + 1];
                                    }
                                    else if (!b_v[x, j - 1, i + 1])
                                    {
                                        exist = true;
                                        b1 = b_v[x, j - 1, i];
                                        b2 = b_v[x, j, i + 1];
                                        b3 = b_v[x + 1, j - 1, i];
                                        b4 = b_v[x + 1, j, i + 1];


                                        n1 = normal[x, j - 1, i];
                                        n2 = normal[x, j, i + 1];
                                        n3 = normal[x + 1, j - 1, i];
                                        n4 = normal[x + 1, j, i + 1];
                                        f1 = value[x, j - 1, i];
                                        f2 = value[x, j, i + 1];
                                        f3 = value[x + 1, j - 1, i];
                                        f4 = value[x + 1, j, i + 1];
                                        v1 = vortex[x, j - 1, i];
                                        v2 = vortex[x, j, i + 1];
                                        v3 = vortex[x + 1, j - 1, i];
                                        v4 = vortex[x + 1, j, i + 1];
                                    }
                                    else if (!b_v[x + 1, j - 1, i])
                                    {
                                        exist = true;
                                        b1 = b_v[x, j, i];
                                        b2 = b_v[x, j - 1, i + 1];
                                        b3 = b_v[x + 1, j, i];
                                        b4 = b_v[x + 1, j - 1, i + 1];

                                        n1 = normal[x, j, i];
                                        n2 = normal[x, j - 1, i + 1];
                                        n3 = normal[x + 1, j, i];
                                        n4 = normal[x + 1, j - 1, i + 1];
                                        f1 = value[x, j, i];
                                        f2 = value[x, j - 1, i + 1];
                                        f3 = value[x + 1, j, i];
                                        f4 = value[x + 1, j - 1, i + 1];
                                        v1 = vortex[x, j, i];
                                        v2 = vortex[x, j - 1, i + 1];
                                        v3 = vortex[x + 1, j, i];
                                        v4 = vortex[x + 1, j - 1, i + 1];
                                    }
                                    else //if (!b_v [x + 1, j-1, i + 1]) 
                                    {
                                        exist = true;
                                        b1 = b_v[x, j - 1, i];
                                        b2 = b_v[x, j, i + 1];
                                        b3 = b_v[x + 1, j - 1, i];
                                        b4 = b_v[x + 1, j, i + 1];

                                        n1 = normal[x, j - 1, i];
                                        n2 = normal[x, j, i + 1];
                                        n3 = normal[x + 1, j - 1, i];
                                        n4 = normal[x + 1, j, i + 1];
                                        f1 = value[x, j - 1, i];
                                        f2 = value[x, j, i + 1];
                                        f3 = value[x + 1, j - 1, i];
                                        f4 = value[x + 1, j, i + 1];
                                        v1 = vortex[x, j - 1, i];
                                        v2 = vortex[x, j, i + 1];
                                        v3 = vortex[x + 1, j - 1, i];
                                        v4 = vortex[x + 1, j, i + 1];
                                    }
                                    if (exist)
                                    {
                                        if (create_tria(v1, v2, v3, v4, n1, n2, n3, n4, b1, b2, b3, b4, f1, f2, f3, f4, v_l, n_l, c_l))
                                        {

                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (show)
                    {

                        Vector3 v1, v2, v3, v4;
                        Vector3 n1, n2, n3, n4;
                        bool b1, b2, b3, b4;
                        float f1, f2, f3, f4;

                        b1 = b_v[x, j, i];
                        b2 = b_v[x, j, i + 1];
                        b3 = b_v[x + 1, j, i];
                        b4 = b_v[x + 1, j, i + 1];

                        n1 = normal[x, j, i];
                        n2 = normal[x, j, i + 1];
                        n3 = normal[x + 1, j, i];
                        n4 = normal[x + 1, j, i + 1];
                        if (!b1 || !b2 || !b3 || !b4)
                        {
                            if (!b1 && j < 420 - 1)
                            {
                                v1 = vortex[x, j + 1, i];
                                b1 = b_v[x, j + 1, i];
                                f1 = value[x, j + 1, i];
                                n1 = normal[x, j + 1, i];
                            }
                            else
                            {
                                v1 = vortex[x, j, i];
                                f1 = value[x, j, i];

                            }

                            if (!b2 && j < 420 - 1)
                            {
                                v2 = vortex[x, j + 1, i + 1];
                                b2 = b_v[x, j + 1, i + 1];
                                f2 = value[x, j + 1, i + 1];
                                n2 = normal[x, j + 1, i + 1];
                            }
                            else
                            {
                                v2 = vortex[x, j, i + 1];
                                f2 = value[x, j, i + 1];
                            }
                            if (!b3 && j < 420 - 1)
                            {
                                v3 = vortex[x + 1, j + 1, i];
                                b3 = b_v[x + 1, j + 1, i];
                                f3 = value[x + 1, j + 1, i];
                                n3 = normal[x + 1, j + 1, i];
                            }
                            else
                            {
                                v3 = vortex[x + 1, j, i];
                                f3 = value[x + 1, j, i];
                            }
                            if (!b4 && j < 420 - 1)
                            {
                                v4 = vortex[x + 1, j + 1, i + 1];
                                b4 = b_v[x + 1, j + 1, i + 1];
                                f4 = value[x + 1, j + 1, i + 1];
                                n4 = normal[x + 1, j + 1, i + 1];
                            }
                            else
                            {
                                v4 = vortex[x + 1, j, i + 1];
                                f4 = value[x + 1, j, i + 1];
                            }

                            if (create_tria(v1, v2, v3, v4, n1, n2, n3, n4, b1, b2, b3, b4, f1, f2, f3, f4, v_l, n_l, c_l))
                            {

                            }
                        }
                        else
                        {
                            b1 = b_v[x, j, i];
                            b2 = b_v[x, j, i + 1];
                            b3 = b_v[x + 1, j, i];
                            b4 = b_v[x + 1, j, i + 1];

                            n1 = normal[x, j, i];
                            n2 = normal[x, j, i + 1];
                            n3 = normal[x + 1, j, i];
                            n4 = normal[x + 1, j, i + 1];
                            f1 = value[x, j, i];
                            f2 = value[x, j, i + 1];
                            f3 = value[x + 1, j, i];
                            f4 = value[x + 1, j, i + 1];
                            v1 = vortex[x, j, i];
                            v2 = vortex[x, j, i + 1];
                            v3 = vortex[x + 1, j, i];
                            v4 = vortex[x + 1, j, i + 1];
                            if (create_tria(v1, v2, v3, v4, n1, n2, n3, n4, b1, b2, b3, b4, f1, f2, f3, f4, v_l, n_l, c_l))
                            {

                            }

                        }

                    }


                    //					if (create_tria_flip (vortex [x, j, i], vortex [x, j, i + 1], vortex [x+ 1, j , i], vortex [x+ 1, j , i + 1], b_v [x, j, i], b_v [x, j, i + 1], b_v [x + 1, j, i], b_v [x+ 1, j , i + 1], value [x, j, i], value [x, j, i + 1], value [x+ 1, j , i], value [x+ 1, j , i + 1], v_l, c_l)) {
                    //						break;
                    //					}
                    //					else if(j<420-1)
                    //					{
                    //						if (create_tria_flip (vortex [x, j, i], vortex [x, j, i + 1], vortex [x+ 1, j+1 , i], vortex [x+ 1, j+1 , i + 1], b_v [x, j, i], b_v [x, j, i + 1], b_v [x + 1, j+1, i], b_v [x+ 1, j+1 , i + 1], value [x, j, i], value [x, j, i + 1], value [x+ 1, j+1 , i], value [x+ 1, j+1 , i + 1], v_l, c_l)) {
                    //						break;
                    //						}
                    //						else if (create_tria_flip (vortex [x, j+1, i], vortex [x, j+1, i + 1], vortex [x+ 1, j , i], vortex [x+ 1, j , i + 1], b_v [x, j+1, i], b_v [x, j+1, i + 1], b_v [x + 1, j, i], b_v [x+ 1, j , i + 1], value [x, j+1, i], value [x, j+1, i + 1], value [x+ 1, j , i], value [x+ 1, j , i + 1], v_l, c_l)) {
                    //						break;
                    //						}
                    //					}

                }
            }
        }
    }

    void back_front(float[,,] value, Vector3[,,] vortex, Vector3[,,] normal, bool[,,] b_v, List<Vector3> v_l, List<Vector3> n_l, List<Color> c_l)
    {
        for (int x = 0; x < 27 - 1; x++)
        {
            for (int i = 0; i < 450 - 1; i++)
            {
                for (int j = 419; j >= 0; j--)
                {

                    bool show = false;
                    //					if (j == 0 || !b_v [x, j - 1, i] ||! b_v [x, j-1, i + 1] || !b_v [x + 1, j-1, i] || !b_v [x + 1, j-1, i + 1])

                    {
                        //						show = true;

                        int count = 0;
                        if (j == 419)
                        {
                            show = true;
                        }
                        else
                        {
                            if (b_v[x, j, i])
                            {
                                if (!b_v[x, j + 1, i])
                                {
                                    count++;
                                }
                            }
                            else if (j > 0 && b_v[x, j - 1, i]) //##################################################33
                            {
                                count++;
                            }


                            if (b_v[x, j, i + 1])
                            {
                                if (!b_v[x, j + 1, i + 1])
                                {
                                    count++;
                                }
                            }
                            else if (j > 0 && b_v[x, j - 1, i + 1])
                            {
                                count++;
                            }


                            if (b_v[x + 1, j, i])
                            {
                                if (!b_v[x + 1, j + 1, i])
                                {
                                    count++;
                                }
                            }
                            else if (j > 0 && b_v[x + 1, j - 1, i])
                            {
                                count++;
                            }


                            if (b_v[x + 1, j, i + 1])
                            {
                                if (!b_v[x + 1, j + 1, i + 1])
                                {
                                    count++;
                                }
                            }
                            else if (j > 0 && b_v[x + 1, j - 1, i + 1])
                            {
                                count++;
                            }

                            if (count >= 2)
                            {
                                show = true;
                                if (count == 2)
                                {

                                    show = false;
                                    if (!b_v[x, j, i] && !b_v[x, j, i + 1])
                                    {
                                        show = false;
                                    }
                                    else if (!b_v[x + 1, j, i] && !b_v[x + 1, j, i + 1])
                                    {
                                        show = false;
                                    }
                                    else if (!b_v[x, j, i] && !b_v[x + 1, j, i])
                                    {
                                        show = false;
                                    }
                                    else if (!b_v[x, j, i + 1] && !b_v[x + 1, j, i + 1])
                                    {
                                        show = false;
                                    }
                                    //									if((!b_v [x, j+1, i] && !b_v [x, j, i + 1])||(!b_v [x, j, i] && !b_v [x, j+1, i + 1])) {
                                    //										show = false;
                                    //									} else if ((!b_v [x + 1, j+1, i] && !b_v [x + 1, j, i + 1])||(!b_v [x + 1, j, i] && !b_v [x + 1, j+1, i + 1]) ){
                                    //										show = false;
                                    //									} else if ((!b_v [x, j+1, i] && !b_v [x + 1, j, i])||(!b_v [x, j, i] && !b_v [x + 1, j+1, i])) {
                                    //										show = false;
                                    //									} else if( (!b_v [x, j+1, i + 1] && !b_v [x + 1, j, i + 1])||(!b_v [x, j, i + 1] && !b_v [x + 1, j+1, i + 1])) {
                                    //										show = false;
                                    //									} 

                                    if (b_v[x, j, i] && b_v[x, j, i + 1] && b_v[x + 1, j, i] && b_v[x + 1, j, i + 1])
                                    {

                                        if (!b_v[x, j + 1, i] && !b_v[x, j + 1, i + 1])
                                        {
                                            show = true;
                                        }
                                        else if (!b_v[x + 1, j + 1, i] && !b_v[x + 1, j + 1, i + 1])
                                        {
                                            show = true;
                                        }
                                        else if (!b_v[x, j + 1, i] && !b_v[x + 1, j + 1, i])
                                        {
                                            Vector3 v1, v2, v3, v4;
                                            Vector3 n1, n2, n3, n4;
                                            bool b1, b2, b3, b4;
                                            float f1, f2, f3, f4;

                                            b1 = b_v[x, j, i];
                                            b2 = b_v[x, j + 1, i + 1];
                                            b3 = b_v[x + 1, j, i];
                                            b4 = b_v[x + 1, j + 1, i + 1];
                                            n1 = normal[x, j, i];
                                            n2 = normal[x, j + 1, i + 1];
                                            n3 = normal[x + 1, j, i];
                                            n4 = normal[x + 1, j + 1, i + 1];
                                            f1 = value[x, j, i];
                                            f2 = value[x, j + 1, i + 1];
                                            f3 = value[x + 1, j, i];
                                            f4 = value[x + 1, j + 1, i + 1];
                                            v1 = vortex[x, j, i];
                                            v2 = vortex[x, j + 1, i + 1];
                                            v3 = vortex[x + 1, j, i];
                                            v4 = vortex[x + 1, j + 1, i + 1];
                                            if (create_tria_flip(v1, v2, v3, v4, n1, n2, n3, n4, b1, b2, b3, b4, f1, f2, f3, f4, v_l, n_l, c_l))
                                            {

                                            }

                                        }
                                        else if (!b_v[x, j + 1, i + 1] && !b_v[x + 1, j + 1, i + 1])
                                        {
                                            Vector3 v1, v2, v3, v4;
                                            Vector3 n1, n2, n3, n4;
                                            bool b1, b2, b3, b4;
                                            float f1, f2, f3, f4;


                                            b1 = b_v[x, j + 1, i];
                                            b2 = b_v[x, j, i + 1];
                                            b3 = b_v[x + 1, j + 1, i];
                                            b4 = b_v[x + 1, j, i + 1];

                                            n1 = normal[x, j + 1, i];
                                            n2 = normal[x, j, i + 1];
                                            n3 = normal[x + 1, j + 1, i];
                                            n4 = normal[x + 1, j, i + 1];
                                            f1 = value[x, j + 1, i];
                                            f2 = value[x, j, i + 1];
                                            f3 = value[x + 1, j + 1, i];
                                            f4 = value[x + 1, j, i + 1];
                                            v1 = vortex[x, j + 1, i];
                                            v2 = vortex[x, j, i + 1];
                                            v3 = vortex[x + 1, j + 1, i];
                                            v4 = vortex[x + 1, j, i + 1];

                                            if (create_tria_flip(v1, v2, v3, v4, n1, n2, n3, n4, b1, b2, b3, b4, f1, f2, f3, f4, v_l, n_l, c_l))
                                            {

                                            }

                                        }
                                        else
                                        {

                                        }

                                    }





                                    //									else {
                                    //										show = true;
                                    //										//										Debug.Log ();
                                    //									}
                                }
                                else
                                {
                                    show = true;
                                    int real_count = 0;
                                    if (b_v[x, j, i])
                                    {
                                        real_count++;
                                    }
                                    if (b_v[x, j, i + 1])
                                    {
                                        real_count++;
                                    }
                                    if (b_v[x + 1, j, i])
                                    {
                                        real_count++;
                                    }
                                    if (b_v[x + 1, j, i + 1])
                                    {
                                        real_count++;
                                    }
                                    if (real_count <= 3)
                                        show = false;

                                }
                            }
                            else if (count == 1)
                            {
                                Vector3 v1, v2, v3, v4;
                                Vector3 n1, n2, n3, n4;
                                bool b1, b2, b3, b4;
                                float f1, f2, f3, f4;
                                bool exist = false;
                                if (b_v[x, j, i] && b_v[x, j, i + 1] && b_v[x + 1, j, i] && b_v[x + 1, j, i + 1])
                                {
                                    if (!b_v[x, j + 1, i])
                                    {
                                        exist = true;
                                        b1 = b_v[x, j, i];
                                        b2 = b_v[x, j + 1, i + 1];
                                        b3 = b_v[x + 1, j, i];
                                        b4 = b_v[x + 1, j + 1, i + 1];

                                        n1 = normal[x, j, i];
                                        n2 = normal[x, j + 1, i + 1];
                                        n3 = normal[x + 1, j, i];
                                        n4 = normal[x + 1, j + 1, i + 1];
                                        f1 = value[x, j, i];
                                        f2 = value[x, j + 1, i + 1];
                                        f3 = value[x + 1, j, i];
                                        f4 = value[x + 1, j + 1, i + 1];
                                        v1 = vortex[x, j, i];
                                        v2 = vortex[x, j + 1, i + 1];
                                        v3 = vortex[x + 1, j, i];
                                        v4 = vortex[x + 1, j + 1, i + 1];
                                    }
                                    else if (!b_v[x, j + 1, i + 1])
                                    {
                                        exist = true;
                                        b1 = b_v[x, j + 1, i];
                                        b2 = b_v[x, j, i + 1];
                                        b3 = b_v[x + 1, j + 1, i];
                                        b4 = b_v[x + 1, j, i + 1];

                                        n1 = normal[x, j + 1, i];
                                        n2 = normal[x, j, i + 1];
                                        n3 = normal[x + 1, j + 1, i];
                                        n4 = normal[x + 1, j, i + 1];
                                        f1 = value[x, j + 1, i];
                                        f2 = value[x, j, i + 1];
                                        f3 = value[x + 1, j + 1, i];
                                        f4 = value[x + 1, j, i + 1];
                                        v1 = vortex[x, j + 1, i];
                                        v2 = vortex[x, j, i + 1];
                                        v3 = vortex[x + 1, j + 1, i];
                                        v4 = vortex[x + 1, j, i + 1];
                                    }
                                    else if (!b_v[x + 1, j + 1, i])
                                    {
                                        exist = true;
                                        b1 = b_v[x, j, i];
                                        b2 = b_v[x, j + 1, i + 1];
                                        b3 = b_v[x + 1, j, i];
                                        b4 = b_v[x + 1, j + 1, i + 1];

                                        n1 = normal[x, j, i];
                                        n2 = normal[x, j + 1, i + 1];
                                        n3 = normal[x + 1, j, i];
                                        n4 = normal[x + 1, j + 1, i + 1];
                                        f1 = value[x, j, i];
                                        f2 = value[x, j + 1, i + 1];
                                        f3 = value[x + 1, j, i];
                                        f4 = value[x + 1, j + 1, i + 1];
                                        v1 = vortex[x, j, i];
                                        v2 = vortex[x, j + 1, i + 1];
                                        v3 = vortex[x + 1, j, i];
                                        v4 = vortex[x + 1, j + 1, i + 1];
                                    }
                                    else //if (!b_v [x + 1, j-1, i + 1]) 
                                    {
                                        exist = true;
                                        b1 = b_v[x, j + 1, i];
                                        b2 = b_v[x, j, i + 1];
                                        b3 = b_v[x + 1, j + 1, i];
                                        b4 = b_v[x + 1, j, i + 1];

                                        n1 = normal[x, j + 1, i];
                                        n2 = normal[x, j, i + 1];
                                        n3 = normal[x + 1, j + 1, i];
                                        n4 = normal[x + 1, j, i + 1];
                                        f1 = value[x, j + 1, i];
                                        f2 = value[x, j, i + 1];
                                        f3 = value[x + 1, j + 1, i];
                                        f4 = value[x + 1, j, i + 1];
                                        v1 = vortex[x, j + 1, i];
                                        v2 = vortex[x, j, i + 1];
                                        v3 = vortex[x + 1, j + 1, i];
                                        v4 = vortex[x + 1, j, i + 1];
                                    }
                                    if (exist)
                                    {
                                        if (create_tria_flip(v1, v2, v3, v4, n1, n2, n3, n4, b1, b2, b3, b4, f1, f2, f3, f4, v_l, n_l, c_l))
                                        {

                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (show)
                    {

                        Vector3 v1, v2, v3, v4;
                        Vector3 n1, n2, n3, n4;
                        bool b1, b2, b3, b4;
                        float f1, f2, f3, f4;

                        b1 = b_v[x, j, i];
                        b2 = b_v[x, j, i + 1];
                        b3 = b_v[x + 1, j, i];
                        b4 = b_v[x + 1, j, i + 1];

                        n1 = normal[x, j, i];
                        n2 = normal[x, j, i + 1];
                        n3 = normal[x + 1, j, i];
                        n4 = normal[x + 1, j, i + 1];
                        if (!b1 || !b2 || !b3 || !b4)
                        {
                            if (!b1 && j > 0)
                            {
                                v1 = vortex[x, j - 1, i];
                                b1 = b_v[x, j - 1, i];
                                f1 = value[x, j - 1, i];
                                n1 = normal[x, j - 1, i];
                            }
                            else
                            {
                                v1 = vortex[x, j, i];
                                f1 = value[x, j, i];
                            }

                            if (!b2 && j > 0)
                            {
                                v2 = vortex[x, j - 1, i + 1];
                                b2 = b_v[x, j - 1, i + 1];
                                f2 = value[x, j - 1, i + 1];
                                n2 = normal[x, j - 1, i + 1];
                            }
                            else
                            {
                                v2 = vortex[x, j, i + 1];
                                f2 = value[x, j, i + 1];
                            }
                            if (!b3 && j > 0)
                            {
                                v3 = vortex[x + 1, j - 1, i];
                                b3 = b_v[x + 1, j - 1, i];
                                f3 = value[x + 1, j - 1, i];
                                n3 = normal[x + 1, j - 1, i];
                            }
                            else
                            {
                                v3 = vortex[x + 1, j, i];
                                f3 = value[x + 1, j, i];
                            }
                            if (!b4 && j > 0)
                            {
                                v4 = vortex[x + 1, j - 1, i + 1];
                                b4 = b_v[x + 1, j - 1, i + 1];
                                f4 = value[x + 1, j - 1, i + 1];
                                n4 = normal[x + 1, j - 1, i + 1];
                            }
                            else
                            {
                                v4 = vortex[x + 1, j, i + 1];
                                f4 = value[x + 1, j, i + 1];
                            }

                            if (create_tria_flip(v1, v2, v3, v4, n1, n2, n3, n4, b1, b2, b3, b4, f1, f2, f3, f4, v_l, n_l, c_l))
                            {

                            }
                        }
                        else
                        {
                            b1 = b_v[x, j, i];
                            b2 = b_v[x, j, i + 1];
                            b3 = b_v[x + 1, j, i];
                            b4 = b_v[x + 1, j, i + 1];

                            n1 = normal[x, j, i];
                            n2 = normal[x, j, i + 1];
                            n3 = normal[x + 1, j, i];
                            n4 = normal[x + 1, j, i + 1];
                            f1 = value[x, j, i];
                            f2 = value[x, j, i + 1];
                            f3 = value[x + 1, j, i];
                            f4 = value[x + 1, j, i + 1];
                            v1 = vortex[x, j, i];
                            v2 = vortex[x, j, i + 1];
                            v3 = vortex[x + 1, j, i];
                            v4 = vortex[x + 1, j, i + 1];
                            if (create_tria_flip(v1, v2, v3, v4, n1, n2, n3, n4, b1, b2, b3, b4, f1, f2, f3, f4, v_l, n_l, c_l))
                            {

                            }

                        }

                    }


                    //					if (create_tria_flip (vortex [x, j, i], vortex [x, j, i + 1], vortex [x+ 1, j , i], vortex [x+ 1, j , i + 1], b_v [x, j, i], b_v [x, j, i + 1], b_v [x + 1, j, i], b_v [x+ 1, j , i + 1], value [x, j, i], value [x, j, i + 1], value [x+ 1, j , i], value [x+ 1, j , i + 1], v_l, c_l)) {
                    //						break;
                    //					}
                    //					else if(j<420-1)
                    //					{
                    //						if (create_tria_flip (vortex [x, j, i], vortex [x, j, i + 1], vortex [x+ 1, j+1 , i], vortex [x+ 1, j+1 , i + 1], b_v [x, j, i], b_v [x, j, i + 1], b_v [x + 1, j+1, i], b_v [x+ 1, j+1 , i + 1], value [x, j, i], value [x, j, i + 1], value [x+ 1, j+1 , i], value [x+ 1, j+1 , i + 1], v_l, c_l)) {
                    //						break;
                    //						}
                    //						else if (create_tria_flip (vortex [x, j+1, i], vortex [x, j+1, i + 1], vortex [x+ 1, j , i], vortex [x+ 1, j , i + 1], b_v [x, j+1, i], b_v [x, j+1, i + 1], b_v [x + 1, j, i], b_v [x+ 1, j , i + 1], value [x, j+1, i], value [x, j+1, i + 1], value [x+ 1, j , i], value [x+ 1, j , i + 1], v_l, c_l)) {
                    //						break;
                    //						}
                    //					}

                }
            }
        }
    }


    void left_right(float[,,] value, Vector3[,,] vortex, Vector3[,,] normal, bool[,,] b_v, List<Vector3> v_l, List<Vector3> n_l, List<Color> c_l)
    {
        for (int j = 0; j < 420 - 1; j++)
        {
            for (int x = 0; x < 27 - 1; x++)
            {
                for (int i = 0; i < 450 - 1; i++)
                {
                    bool show = false;
                    int count = 0;
                    int true_count = 0;
                    if (i == 0)
                    {
                        show = true;
                        if (b_v[x, j, i])
                        {
                            true_count++;
                        }
                        if (b_v[x + 1, j, i])
                        {
                            true_count++;
                        }
                        if (b_v[x, j + 1, i])
                        {
                            true_count++;
                        }
                        if (b_v[x + 1, j + 1, i])
                        {
                            true_count++;
                        }
                    }
                    else
                    {


                        if (b_v[x, j, i])
                        {
                            true_count++;
                        }
                        if (b_v[x + 1, j, i])
                        {
                            true_count++;
                        }
                        if (b_v[x, j + 1, i])
                        {
                            true_count++;
                        }
                        if (b_v[x + 1, j + 1, i])
                        {
                            true_count++;
                        }
                        if (true_count > 3)
                        {
                            if (!b_v[x, j, i - 1] && b_v[x, j, i])
                            {
                                count++;
                            }
                            if (!b_v[x + 1, j, i - 1] && b_v[x + 1, j, i])
                            {
                                count++;
                            }

                            if (!b_v[x, j + 1, i - 1] && b_v[x, j + 1, i])

                            {
                                count++;
                            }

                            if (!b_v[x + 1, j + 1, i - 1] && b_v[x + 1, j + 1, i])
                            {
                                count++;
                            }

                            if (count >= 1)
                            {

                                show = true;
                            }

                        }
                        else if (true_count == 3)
                        {
                            //							if (!b_v [x , j, i- 1]&&b_v [x , j, i]) 
                            //							{
                            //								count++;
                            //							}
                            //							if (!b_v [x + 1, j, i - 1] && b_v [x+1, j, i ]) 
                            //							{
                            //								count++;
                            //							}
                            //
                            //							if (!b_v [x , j + 1, i-1] && b_v [x, j + 1, i]) 
                            //
                            //							{
                            //								count++;
                            //							}
                            //
                            //							if (!b_v [x + 1, j + 1, i - 1] && b_v [x+1, j + 1, i ]) 
                            //							{
                            //								count++;
                            //							}
                            //
                            //							if (count >= 2)
                            //							{
                            //
                            //								show=true;
                            //							}

                        }


                    }
                    if ((true_count >= 3 && show) && testd && x == 25 && i != 0 && j > 2)
                    {
                        //						Debug.Log (i+" "+j);
                        //						Debug.Log (b_v [x, j, i]+ " "+b_v [x+ 1, j, i ]+ " "+b_v [x, j + 1, i]+ " "+b_v [x + 1, j + 1, i]);
                        //						Debug.Log (normal [x, j, i]+ " "+normal [x+ 1, j, i ]+ " "+normal [x, j + 1, i]+ " "+normal [x + 1, j + 1, i]);
                        testd = false;

                        create_tria(vortex[x, j, i], vortex[x + 1, j, i], vortex[x, j + 1, i], vortex[x + 1, j + 1, i], normal[x, j, i], normal[x + 1, j, i], normal[x, j + 1, i], normal[x + 1, j + 1, i], b_v[x, j, i], b_v[x + 1, j, i], b_v[x, j + 1, i], b_v[x + 1, j + 1, i], value[x, j, i], value[x + 1, j, i], value[x, j + 1, i], value[x + 1, j + 1, i], l1, l2, l3);
                        create_obj(l1, l3, l2);
                    }


                    if (true_count >= 3 && show)
                        if (create_tria(vortex[x, j, i], vortex[x + 1, j, i], vortex[x, j + 1, i], vortex[x + 1, j + 1, i], normal[x, j, i], normal[x + 1, j, i], normal[x, j + 1, i], normal[x + 1, j + 1, i], b_v[x, j, i], b_v[x + 1, j, i], b_v[x, j + 1, i], b_v[x + 1, j + 1, i], value[x, j, i], value[x + 1, j, i], value[x, j + 1, i], value[x + 1, j + 1, i], v_l, n_l, c_l))
                        {
                            //						break;
                        }

                }
            }
        }
    }


    void right_left(float[,,] value, Vector3[,,] vortex, Vector3[,,] normal, bool[,,] b_v, List<Vector3> v_l, List<Vector3> n_l, List<Color> c_l)
    {
        for (int j = 0; j < 420 - 1; j++)
        {
            for (int x = 0; x < 27 - 1; x++)
            {
                for (int i = 449; i >= 0; i--)
                {
                    bool show = false;
                    int count = 0;
                    int true_count = 0;
                    if (i == 449)
                    {
                        show = true;
                        if (b_v[x, j, i])
                        {
                            true_count++;
                        }
                        if (b_v[x + 1, j, i])
                        {
                            true_count++;
                        }
                        if (b_v[x, j + 1, i])
                        {
                            true_count++;
                        }
                        if (b_v[x + 1, j + 1, i])
                        {
                            true_count++;
                        }
                    }
                    else
                    {


                        if (b_v[x, j, i])
                        {
                            true_count++;
                        }
                        if (b_v[x + 1, j, i])
                        {
                            true_count++;
                        }
                        if (b_v[x, j + 1, i])
                        {
                            true_count++;
                        }
                        if (b_v[x + 1, j + 1, i])
                        {
                            true_count++;
                        }
                        if (true_count > 3)
                        {
                            if (!b_v[x, j, i + 1] && b_v[x, j, i])
                            {
                                count++;
                            }
                            if (!b_v[x + 1, j, i + 1] && b_v[x + 1, j, i])
                            {
                                count++;
                            }

                            if (!b_v[x, j + 1, i + 1] && b_v[x, j + 1, i])
                            {
                                count++;
                            }

                            if (!b_v[x + 1, j + 1, i + 1] && b_v[x + 1, j + 1, i])
                            {
                                count++;
                            }

                            if (count >= 1)
                            {

                                show = true;
                            }

                        }
                        else if (true_count == 3)
                        {
                            //							if (!b_v [x, j, i + 1] && b_v [x, j, i]) {
                            //								count++;
                            //							}
                            //							if (!b_v [x + 1, j, i + 1] && b_v [x + 1, j, i]) {
                            //								count++;
                            //							}
                            //
                            //							if (!b_v [x, j + 1, i + 1] && b_v [x, j + 1, i]) {
                            //								count++;
                            //							}
                            //
                            //							if (!b_v [x + 1, j + 1, i + 1] && b_v [x + 1, j + 1, i]) {
                            //								count++;
                            //							}
                            //
                            //							if (count >= 2) {
                            //
                            //								show = true;
                            //							}
                        }


                    }
                    if (true_count >= 3 && show)
                        if (create_tria_flip(vortex[x, j, i], vortex[x + 1, j, i], vortex[x, j + 1, i], vortex[x + 1, j + 1, i], normal[x, j, i], normal[x + 1, j, i], normal[x, j + 1, i], normal[x + 1, j + 1, i], b_v[x, j, i], b_v[x + 1, j, i], b_v[x, j + 1, i], b_v[x + 1, j + 1, i], value[x, j, i], value[x + 1, j, i], value[x, j + 1, i], value[x + 1, j + 1, i], v_l, n_l, c_l))
                        {
                            //						break;
                        }

                }
            }
        }
    }

    //	void right_left(float[,,] value,Vector3[,,]vortex,bool[,,] b_v,List<Vector3> v_l,List<Color> c_l)
    //	{
    //		for (int x = 0; x < 27-1; x++)
    //		{	
    //			for (int j = 0; j < 420-1 ; j++)
    //			{
    //				 for (int i = 0; i < 450 ; i++) 
    //				{
    //
    //					bool show = false;
    //					if (i == 0 || !b_v [x, j, i - 1] ||! b_v [x, j+ 1, i-1 ]|| ! b_v [x+ 1, j , i-1] || ! b_v [x+ 1, j+ 1 , i-1 ])
    //					{
    ////						show = true;
    //
    //						int count = 0;
    //						if (i == 0) 
    //						{
    //							show = true;
    //						}
    //						else 
    //						{
    //							if (b_v [x, j, i]) 
    //							{
    //								if (!b_v [x, j, i - 1]) 
    //								{
    //									count++;
    //								}
    //							} 
    //							else if (i<449&&b_v [x, j, i + 1]) 
    //							{
    //								count++;
    //							}
    //
    //
    //							if (b_v [x, j + 1, i]) {
    //								if (!b_v [x, j + 1, i - 1]) {
    //									count++;
    //								}
    //							} else if (i<449&&b_v [x, j + 1, i + 1]) 
    //							{
    //								count++;
    //							}
    //
    //
    //							if (b_v [x + 1, j, i]) {
    //								if (!b_v [x + 1, j, i - 1]) {
    //									count++;
    //								}
    //							} else if (i<449&&b_v [x + 1, j, i + 1]) 
    //							{
    //								count++;
    //							}
    //
    //
    //							if (b_v [x + 1, j + 1, i]) {
    //								if (!b_v [x + 1, j + 1, i - 1]) {
    //									count++;
    //								}
    //							} else if (i<449&&b_v [x + 1, j + 1, i+1])
    //							{
    //								count++;
    //							}
    //
    //							if (count >= 2)
    //							{
    //								show = true;
    //								if (count == 2) 
    //								{
    //									if (!b_v [x, j, i] && !b_v [x, j + 1, i]) {
    //										show = false;
    //									} else if (!b_v [x + 1, j, i] && !b_v [x + 1, j + 1, i]) {
    //										show = false;
    //									} else if (!b_v [x, j, i] && !b_v [x + 1, j, i]) {
    //										show = false;
    //									} else if (!b_v [x, j + 1, i] && !b_v [x + 1, j + 1, i]) 
    //									{
    //										show = false;
    //									}
    //								}
    //							}
    //
    //						}
    //					}
    //					if (show) {
    //						Vector3 v1, v2, v3, v4;
    //						bool b1, b2, b3, b4;
    //						float f1, f2, f3, f4;
    //
    //						b1 = b_v [x, j, i];
    //						b2 = b_v [x, j + 1, i];
    //						b3 = b_v [x + 1, j, i];
    //						b4 = b_v [x + 1, j + 1, i];
    //						if (!b1 || !b2 || !b3 || b4 || !b4) {
    //							if (!b1 && i < 450 - 1) {
    //								v1 = vortex [x, j, i + 1];
    //								b1 = b_v [x, j, i + 1];
    //								f1 = value [x, j, i + 1];
    //							} else {
    //								v1 = vortex [x, j, i];
    //								f1 = value [x, j, i];
    //							}
    //
    //							if (!b2 && i < 450 - 1) {
    //								v2 = vortex [x, j + 1, i + 1];
    //								b2 = b_v [x, j + 1, i + 1];
    //								f2 = value [x, j + 1, i + 1];
    //							} else {
    //								v2 = vortex [x, j + 1, i];
    //								f2 = value [x, j + 1, i];
    //							}
    //							if (!b3 && i < 450 - 1) {
    //								v3 = vortex [x + 1, j, i + 1];
    //								b3 = b_v [x + 1, j, i + 1];
    //								f3 = value [x + 1, j, i + 1];
    //							} else {
    //								v3 = vortex [x + 1, j, i];
    //								f3 = value [x + 1, j, i];
    //							}
    //							if (!b4 && i < 420 - 1) {
    //								v4 = vortex [x + 1, j + 1, i + 1];
    //								b4 = b_v [x + 1, j + 1, i + 1];
    //								f4 = value [x + 1, j + 1, i + 1];
    //							} else {
    //								v4 = vortex [x + 1, j + 1, i];
    //								f4 = value [x + 1, j + 1, i];
    //							}
    //
    //							if (create_tria_flip (v1, v2, v3, v4, b1, b2, b3, b4, f1, f2, f3, f4, v_l, c_l)) 
    //							{
    //								//break;
    //							}
    //						}
    //
    //					}
    //
    //
    //
    ////					if (create_tria (vortex [x, j, i], vortex [x, j+ 1, i ], vortex [x+ 1, j , i], vortex [x+ 1, j+ 1 , i ], b_v [x, j, i], b_v [x, j+ 1, i ], b_v [x + 1, j, i], b_v [x+ 1, j+ 1 , i ], value [x, j, i], value [x, j+ 1, i ], value [x+ 1, j , i], value [x+ 1, j+ 1 , i ], v_l, c_l)) {
    ////						break;
    ////					}
    ////					else if(i<450-1)
    ////					{
    ////						 if (create_tria (vortex [x, j, i], vortex [x, j+ 1, i ], vortex [x+ 1, j , i+1], vortex [x+ 1, j+ 1 , i+1 ], b_v [x, j, i], b_v [x, j+ 1, i ], b_v [x + 1, j, i+1], b_v [x+ 1, j+ 1 , i +1], value [x, j, i], value [x, j+ 1, i ], value [x+ 1, j , i+1], value [x+ 1, j+ 1 , i+1 ], v_l, c_l)) {
    ////							break;
    ////						}
    ////						else if (create_tria (vortex [x, j, i+1], vortex [x, j+ 1, i+1 ], vortex [x+ 1, j , i], vortex [x+ 1, j+ 1 , i ], b_v [x, j, i+1], b_v [x, j+ 1, i+1 ], b_v [x + 1, j, i], b_v [x+ 1, j+ 1 , i ], value [x, j, i+1], value [x, j+ 1, i ], value [x+ 1, j , i+1], value [x+ 1, j+ 1 , i ], v_l, c_l)) {
    ////							break;
    ////						}
    ////					}
    //				}
    //			}
    //		}
    //	}


    void search_value(float[,,] value, Vector3[,,] vortex, Vector3[,,] normal, bool[,,] b_v, List<Vector3> v_l, List<Vector3> n_l, List<Color> c_l)
    {
        //		int x = 26;
        //		bool yes=true;
        ////		for (int x = 0; x < 27; x++)
        //		{
        //			for (int j = 0; j < 420 - 1; j++) {	
        //				for (int i = 0; i < 450 - 1; i++) {	
        //					
        //					try {
        //						create_tria (vortex [x, j, i], vortex [x, j, i + 1], vortex [x, j + 1, i], vortex [x, j + 1, i + 1], b_v [x, j, i], b_v [x, j, i + 1], b_v [x, j + 1, i], b_v [x, j + 1, i + 1], value [x, j, i], value [x, j, i + 1], value [x, j + 1, i], value [x, j + 1, i + 1], v_l, c_l);
        //					} catch (System.Exception e) {
        //						if (yes) {
        //							Debug.Log (x + " " + j + " " + i);
        //							yes = false;
        //							break;
        //						}
        //					}
        //
        //
        ////				if (i > 0 && b_v [x, j, i - 1]) {
        //////					Debug.LogError (j + " " + i);
        ////				} else if (i < 449&& b_v [x, j, i + 1])
        ////				{
        //////					Debug.LogWarning (j + " right " + i);
        ////				}
        //				}
        //
        //				if (!yes) {
        //				
        //					break;
        //				}
        //			}
        //
        ////			if (!yes) {
        ////
        ////				break;
        ////			}
        //		}
        top_down(value, vortex, normal, b_v, v_l, n_l, c_l);
        ////		down_top(value,vortex,b_v,v_l,c_l);
        front_back(value, vortex, normal, b_v, v_l, n_l, c_l);
        back_front(value, vortex, normal, b_v, v_l, n_l, c_l);
        left_right(value, vortex, normal, b_v, v_l, n_l, c_l);
        right_left(value, vortex, normal, b_v, v_l, n_l, c_l);
    }



    bool create_tria_0(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, bool t1, bool t2, bool t3, bool t4, float f1, float f2, float f3, float f4, List<Vector3> v_l, List<Vector3> n_l, List<Color> c_l)
    {
        int size = 1;
        float f = max - min;
        int count = 0;

        Vector3 n = Vector3.up;
        if (t1 && t2 && t3 && t4)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);
            v_l.Add(p3 * size);
            v_l.Add(p2 * size);
            v_l.Add(p1 * size);

            n_l.Add(n);
            n_l.Add(n);
            n_l.Add(n);
            c_l.Add(c3);
            c_l.Add(c2);
            c_l.Add(c1);

            v_l.Add(p3 * size);
            v_l.Add(p4 * size);
            v_l.Add(p2 * size);

            n_l.Add(n);
            n_l.Add(n);
            n_l.Add(n);

            c_l.Add(c3);
            c_l.Add(c4);
            c_l.Add(c2);
            return true;

        }
        else if (t1 && t2 && t3)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);

            v_l.Add(p3 * size);
            v_l.Add(p2 * size);
            v_l.Add(p1 * size);

            n_l.Add(n);
            n_l.Add(n);
            n_l.Add(n);
            c_l.Add(c3);
            c_l.Add(c2);
            c_l.Add(c1);
            return false;
        }
        else if (t1 && t2 && t4)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);
            v_l.Add(p4 * size);
            v_l.Add(p2 * size);
            v_l.Add(p1 * size);

            n_l.Add(n);
            n_l.Add(n);
            n_l.Add(n);
            c_l.Add(c4);
            c_l.Add(c2);
            c_l.Add(c1);
            return false;
        }
        else if (t1 && t3 && t4)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);
            v_l.Add(p3 * size);
            v_l.Add(p4 * size);
            v_l.Add(p1 * size);

            n_l.Add(n);
            n_l.Add(n);
            n_l.Add(n);
            c_l.Add(c3);
            c_l.Add(c4);
            c_l.Add(c1);
            return false;
        }
        else if (t2 && t3 && t4)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);
            v_l.Add(p3 * size);
            v_l.Add(p4 * size);
            v_l.Add(p2 * size);

            n_l.Add(n);
            n_l.Add(n);
            n_l.Add(n);
            c_l.Add(c3);
            c_l.Add(c4);
            c_l.Add(c2);
            return true;
        }
        return false;
    }

    bool create_tria(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 n1, Vector3 n2, Vector3 n3, Vector3 n4, bool t1, bool t2, bool t3, bool t4, float f1, float f2, float f3, float f4, List<Vector3> v_l, List<Vector3> n_l, List<Color> c_l)
    {
        int size = 1;
        float f = max - min;
        int count = 0;
        if (t1 && t2 && t3 && t4)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);
            v_l.Add(p3 * size);
            v_l.Add(p2 * size);
            v_l.Add(p1 * size);
            //			Vector3 n = Vector3.Cross (p2-p3,p1-p3);
            //			n.Normalize ();
            n_l.Add(n3);
            n_l.Add(n2);
            n_l.Add(n1);

            c_l.Add(c3);
            c_l.Add(c2);
            c_l.Add(c1);

            v_l.Add(p3 * size);
            v_l.Add(p4 * size);
            v_l.Add(p2 * size);

            //			Vector3 n1 = Vector3.Cross (p4-p3,p2-p3);
            //			n1.Normalize ();
            n_l.Add(n3);
            n_l.Add(n4);
            n_l.Add(n2);
            c_l.Add(c3);
            c_l.Add(c4);
            c_l.Add(c2);
            return true;

        }
        else if (t1 && t2 && t3)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);

            v_l.Add(p3 * size);
            v_l.Add(p2 * size);
            v_l.Add(p1 * size);
            //			Vector3 n = Vector3.Cross (p2-p3,p1-p3);
            //			n.Normalize ();
            n_l.Add(n3);
            n_l.Add(n2);
            n_l.Add(n1);

            c_l.Add(c3);
            c_l.Add(c2);
            c_l.Add(c1);
            return false;
        }
        else if (t1 && t2 && t4)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);
            v_l.Add(p4 * size);
            v_l.Add(p2 * size);
            v_l.Add(p1 * size);

            //			Vector3 n = Vector3.Cross (p2-p4,p1-p4);
            //			n.Normalize ();
            n_l.Add(n4);
            n_l.Add(n2);
            n_l.Add(n1);
            c_l.Add(c4);
            c_l.Add(c2);
            c_l.Add(c1);
            return false;
        }
        else if (t1 && t3 && t4)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);
            v_l.Add(p3 * size);
            v_l.Add(p4 * size);
            v_l.Add(p1 * size);
            //			Vector3 n = Vector3.Cross (p4-p3,p1-p3);
            //			n.Normalize ();
            n_l.Add(n3);
            n_l.Add(n4);
            n_l.Add(n1);
            c_l.Add(c3);
            c_l.Add(c4);
            c_l.Add(c1);
            return false;
        }
        else if (t2 && t3 && t4)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);
            v_l.Add(p3 * size);
            v_l.Add(p4 * size);
            v_l.Add(p2 * size);

            //			Vector3 n = Vector3.Cross (p4-p3,p2-p3);
            //			n.Normalize ();
            n_l.Add(n3);
            n_l.Add(n4);
            n_l.Add(n2);
            c_l.Add(c3);
            c_l.Add(c4);
            c_l.Add(c2);
            return true;
        }
        return false;
    }


    bool create_tria_flip(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 n1, Vector3 n2, Vector3 n3, Vector3 n4, bool t1, bool t2, bool t3, bool t4, float f1, float f2, float f3, float f4, List<Vector3> v_l, List<Vector3> n_l, List<Color> c_l)
    {
        int size = 1;
        float f = max - min;
        int count = 0;
        if (t1 && t2 && t3 && t4)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);
            v_l.Add(p1 * size);
            v_l.Add(p2 * size);

            v_l.Add(p3 * size);
            //			Vector3 n = Vector3.Cross (p2-p1,p3-p1);
            //			n.Normalize ();
            n_l.Add(n1);
            n_l.Add(n2);
            n_l.Add(n3);
            c_l.Add(c1);
            c_l.Add(c2);
            c_l.Add(c3);

            v_l.Add(p2 * size);
            v_l.Add(p4 * size);
            v_l.Add(p3 * size);

            //			Vector3 n1 = Vector3.Cross (p4-p2,p3-p2);
            //			n1.Normalize ();
            n_l.Add(n2);
            n_l.Add(n4);
            n_l.Add(n3);
            c_l.Add(c2);
            c_l.Add(c4);
            c_l.Add(c3);
            return true;

        }
        else if (t1 && t2 && t3)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);

            v_l.Add(p1 * size);
            v_l.Add(p2 * size);
            v_l.Add(p3 * size);

            //			Vector3 n = Vector3.Cross (p2-p1,p3-p1);
            //			n.Normalize ();
            n_l.Add(n2);
            n_l.Add(n2);
            n_l.Add(n3);
            c_l.Add(c1);
            c_l.Add(c2);
            c_l.Add(c3);
            return true;
        }
        else if (t1 && t2 && t4)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);
            v_l.Add(p1 * size);
            v_l.Add(p2 * size);
            v_l.Add(p4 * size);
            //			Vector3 n = Vector3.Cross (p2-p1,p3-p1);
            //			n.Normalize ();
            n_l.Add(n1);
            n_l.Add(n2);
            n_l.Add(n4);
            c_l.Add(c1);
            c_l.Add(c2);
            c_l.Add(c4);
            return true;
        }
        else if (t1 && t3 && t4)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);
            v_l.Add(p1 * size);
            v_l.Add(p4 * size);
            v_l.Add(p3 * size);

            //			Vector3 n = Vector3.Cross (p4-p1,p3-p1);
            //			n.Normalize ();
            n_l.Add(n1);
            n_l.Add(n4);
            n_l.Add(n3);
            c_l.Add(c1);
            c_l.Add(c4);
            c_l.Add(c3);
            return true;
        }
        else if (t2 && t3 && t4)
        {
            Color c1 = get_color(f1);
            Color c2 = get_color(f2);
            Color c3 = get_color(f3);
            Color c4 = get_color(f4);
            v_l.Add(p2 * size);
            v_l.Add(p4 * size);
            v_l.Add(p3 * size);

            //			Vector3 n = Vector3.Cross (p4-p2,p3-p2);
            //			n.Normalize ();
            n_l.Add(n2);
            n_l.Add(n4);
            n_l.Add(n3);
            c_l.Add(c2);
            c_l.Add(c4);
            c_l.Add(c3);
            return true;
        }
        return false;
    }

    Color get_color(float va)
    {
        float f = max - min;       //max ÎªvalueÊý×é×î´óÖµ£¬minÎªvalueÊý×é³ý0Íâ×îÐ¡Öµ
        Color cor;
        if (colorMode == 0)         //彩色模式
        {
            float color_value = (max - va) / f;
            color_value = color_value < 0 ? 0 : color_value;
            color_value = color_value > 1 ? 1 : color_value;

            cor = Color.HSVToRGB((float)color_value * 2 / 3, 1, 1);//½«HSVµÄÑÕÉ«Öµ×ªÎªRGBµÄÑÕÉ«Öµ
        }
        else if(colorMode == 1)         //白色
        {
            float color_value = (max - va) / f;
            color_value = color_value < 0 ? 0 : color_value;
            color_value = color_value > 1 ? 1 : color_value;
            cor = new Color(0.8f + 0.18f * color_value, 0.8f + 0.18f * color_value, 0.8f + 0.18f * color_value);
        }
        else                          //灰色
        {
            float color_value = (max - va) / f;
            color_value = color_value < 0 ? 0 : color_value;
            color_value = color_value > 1 ? 1 : color_value;
            cor = new Color(0.35f + 0.45f * color_value, 0.35f + 0.45f * color_value, 0.35f + 0.45f * color_value, 0.98f);
        }
        
        return cor;

    }

}