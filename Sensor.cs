using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using System.Text;
using UnityEngine.UI;

public class Sensor : MonoBehaviour
{

    // 초기 설정
    BluetoothHelper bluetoothHelper;
    string deviceName;
    string received_message;
    private string tmp;

    [Header("Connection Display")]
    public GameObject connectedButtons;         //연결되었을때 표시될 게임오브젝트 그룹
    public GameObject disconnectedButtons;      //연결 안되었을 때 표시될 게임오브젝트 그룹
    public GameObject logTextbox;               //로그를 기록하는 텍스트 오브젝트

    [Header("Sensor Display")]
    public GameObject tempText;

    [Header("FanImageObject")]
    public GameObject fanImageObject;



    /// <summary>
    /// //////////////////////Start ////////////////////////////////////
    /// </summary>

    void Start()
    {

        deviceName = "Park"; //블루투스 모듈이 깜박이는 상태일 것
        try
        {
            BluetoothHelper.BLE = true;
            bluetoothHelper = BluetoothHelper.GetInstance(deviceName);
            bluetoothHelper.OnConnected += OnConnected;
            bluetoothHelper.OnConnectionFailed += OnConnectionFailed;
            bluetoothHelper.OnDataReceived += OnMessageReceived; //read the data
            bluetoothHelper.OnScanEnded += OnScanEnded;

            bluetoothHelper.setTerminatorBasedStream("\n");

            if (!bluetoothHelper.ScanNearbyDevices())
            {
                //scan didnt start (on windows desktop (not UWP))
                //try to connect
                bluetoothHelper.Connect();//this will work only for bluetooth classic.
                //scanning is mandatory before connecting for BLE.

            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            LogWrite(ex.Message);
        }

        LogWrite("자동 연결중...");



    }

    /// <summary>
    /// /////////////// 사용자 함수 /////////////////////////////////
    /// </summary>

    public void LogWrite(string msg)
    {
        logTextbox.GetComponent<Text>().text = msg;
        //tmp += ">" + msg + "\n";
    }

    void OnConnected(BluetoothHelper helper)
    {
        LogWrite(deviceName + "에 연결되었습니다.");
        try
        {
            helper.StartListening();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            LogWrite(ex.Message);

        }
    }
    void OnScanEnded(BluetoothHelper helper, LinkedList<BluetoothDevice> devices)
    {

        if (helper.isDevicePaired()) //we did found our device (with BLE) or we already paired the device (for Bluetooth Classic)
            helper.Connect();
        else
            helper.ScanNearbyDevices(); //we didn't
    }
    void OnConnectionFailed(BluetoothHelper helper)
    {
        LogWrite("Connection Failed");
        Debug.Log("Connection Failed");
    }
    public void connectButtonClick()
    {
        if (bluetoothHelper.isDevicePaired())
            bluetoothHelper.Connect(); // tries to connect
        else
            LogWrite("Cannot connect, device is not found, for bluetooth classic, pair the device\n\tFor BLE scan for nearby devices");
    }
    public void DisconnectButtonClick()
    {
        bluetoothHelper.Disconnect();
        LogWrite("Disconnected");
    }
    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
    }
    /// <summary>
    /// //////////////////////Update/////////////////////////////////
    /// </summary>
    void Update()
    {
        if (bluetoothHelper == null)
            return;

        if (bluetoothHelper.isConnected())
        {
            connectedButtons.gameObject.SetActive(true);
            disconnectedButtons.gameObject.SetActive(false);
        }

        if (!bluetoothHelper.isConnected())
        {
            connectedButtons.gameObject.SetActive(false);
            disconnectedButtons.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (bluetoothHelper != null)
                bluetoothHelper.Disconnect();
            Application.Quit();
        }
    }
    /// <summary>
    /// //////////////////////////////  아두이노에서 시리얼 입력을 받았을 때 문자열 분석  //////////////////////
    /// </summary>
    /// <param name="helper"></param>
    void OnMessageReceived(BluetoothHelper helper)
    {
        received_message = helper.Read();
        LogWrite(received_message);
         if (received_message.Contains("Sensor"))
        {
            tempText.GetComponentInChildren<Text>().text = int.Parse(received_message.Substring(6, 3)) + " ℃";
            
            LogWrite("센서값");
        }

    }
}