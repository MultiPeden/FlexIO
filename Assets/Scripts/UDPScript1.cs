using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;


public class UDPScript1 : MonoBehaviour {

    private Rigidbody rb;

    private String message;
    private bool msgUpdated;

    Thread receiveThread;
    UdpClient client;

    // -------------------------------------------------------------------------
    public void Start()
    {
        message = "";
        rb = GetComponent<Rigidbody>();
        //        instruction.text = "started";

        ReceiveData();
        msgUpdated = false;

    }

    // -------------------------------------------------------------------------
    private void ReceiveData()
    {
        int somePort = 11000;


            client = new UdpClient(somePort);
                try
                {
                    client.BeginReceive(new AsyncCallback(recv), null);

                }
                catch (Exception e)
                {
              
                    this.message = "Error: " + e.ToString();
                }
        }



    //CallBack
    private void recv(IAsyncResult res)
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
        byte[] received = client.EndReceive(res, ref RemoteIpEndPoint);

        //Process codes
;
        this.message = Encoding.UTF8.GetString(received);
        client.BeginReceive(new AsyncCallback(recv), null);

        msgUpdated = true;

    }

    // Update is called once per frame
    void Update()
    {

        if (msgUpdated) {
            msgUpdated = false;
         //   Debug.Log(message);



            IRPoint[] irs = JsonHelper.FromJson<IRPoint>(message);



            if (irs != null)
            {
               // instruction.text = "first coords x = : " + irs[0].x;
               // Debug.Log(" first coords x = : " + ((irs[0].x / 100.0f)-5));

                Vector3 movement = new Vector3((irs[0].x/ 50.0f) - 5, ((irs[0].y / 50.0f) - 5)* -1, 0.0f);
                //rb.AddForce(movement);
                rb.position = movement;

            }


      }




    }

    void OnApplicationQuit()
    {
        if (client != null)
            client.Close();
    }




    [Serializable]
    private class IRPoint
    {
        public int id;
        public int x;
        public int y;
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

}
