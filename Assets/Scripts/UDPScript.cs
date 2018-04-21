using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UDPScript : MonoBehaviour
{

    private Text instruction;

    public String message;
    private bool msgUpdated;

    Thread receiveThread;
    UdpClient client;
    private IRPoint[] irs;
    int portNumber = 11000;


    public bool autoStartKinect;
    Process kinectProcess;
    float[] floatArray;

    // -------------------------------------------------------------------------
    public void Start()
    {
        if (autoStartKinect)
        {
            kinectProcess = new Process();
            kinectProcess.StartInfo.FileName = "C:/Users/MultiPeden/Documents/GitHub/infraredKinectData/bin/AnyCPU/Debug/InfraredKinectData-WPF.exe";
            //  kinectProcess.StartInfo.Arguments = "-s";
            kinectProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            kinectProcess.Start();
        }



        message = "";
        //      instruction = GetComponent<Text>();
        //        instruction.text = "started";
        ReceiveData();
        msgUpdated = false;

    }

    // -------------------------------------------------------------------------
    private void ReceiveData()
    {



        client = new UdpClient(portNumber);
        try
        {
            client.BeginReceive(new AsyncCallback(Recv), null);

        }
        catch (Exception e)
        {

            //  this.message = "Error: " + e.ToString();
        }
    }



    //CallBack
    private void Recv(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
        byte[] received = client.EndReceive(res, ref RemoteIpEndPoint);



        // create a second float array and copy the bytes into it...
        int len = (int)(received.Length * .25);
        if (floatArray == null || len != floatArray.Length)
        {
            floatArray = new float[received.Length / 4];
        }


        Buffer.BlockCopy(received, 0, floatArray, 0, received.Length);

        SetIrs(floatArray);
        msgUpdated = true;
        client.BeginReceive(new AsyncCallback(Recv), null);

        //Process codes

        //this.message = Encoding.ASCII.GetString(received)
        // this.irs = JsonHelper.FromJson<IRPoint3D>(message);
        //   UnityEngine.Debug.Log(this.irs.Length);
    }


    private void SetIrs(float[] floatArray)
    {



        int len = floatArray.Length / 3;

        message = "hej " + len + " " + (floatArray.Length);
        IRPoint[] irs2 = new IRPoint[len];
        IRPoint ir;
        int id = 0;

        int i = 0;
        for (int j = 0; j < len; j++)
        {
            ir = new IRPoint
            {
                id = j,
                x = -1 * floatArray[i],
                y = floatArray[i + 1],
                z = floatArray[i + 2]
            };
            i += 3;

            irs2[j] = ir;

        }

        this.irs = irs2;
    }


    // Update is called once per frame
    void Update1()
    {

        if (msgUpdated)
        {
            msgUpdated = false;
            //  Debug.Log(message);



            //  IRPoint3D[] irs = JsonHelper.FromJson<IRPoint3D>(message);



            if (irs != null)
                instruction.text = "first coords x = : " + irs[0].x;
            //Debug.Log(" first coords x = : " + irs[0].x );

        }




    }

    void OnApplicationQuit()
    {
        if (client != null)
            client.Close();
        if (kinectProcess != null)
            kinectProcess.Kill();
    }


    public String getMessage()
    {
        return this.message;
    }

    public IRPoint[] GetIRs()
    {


        return this.irs;
    }




}