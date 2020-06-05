﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class RoomInfo
{
    public string name;
    public int X;
    public int Y;
}

public class RoomController : MonoBehaviour
{
    public static RoomController instance;

    string currentWorldName = "Basement";

    RoomInfo currentLoadRoomData;

    Room currRoom;

    Queue<RoomInfo> loadRoomQueue = new Queue<RoomInfo>();

    public List<Room> loadedRooms = new List<Room>();
    bool isLoadingRoom = false;

    public bool DoesRoomExist( int x, int y)
    {
        return loadedRooms.Find( item => item.X == x && item.Y == y) != null;
    }

    public void LoadRoom( string name, int x, int y)
    {
        if(DoesRoomExist(x, y) == true)
        {
            return;
        }

        RoomInfo newRoomData = new RoomInfo();
        newRoomData.name = name;
        newRoomData.X = x;
        newRoomData.Y = y;

        loadRoomQueue.Enqueue(newRoomData);
    }

    IEnumerator LoadRoomRoutine(RoomInfo info)
    {
        string roomName = currentWorldName + info.name;

        AsyncOperation loadRoom = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);

        while(loadRoom.isDone == false)
        {
            yield return null;
        }
    }

    public void RegisterRoom(Room room) 
    {
        if(!DoesRoomExist(currentLoadRoomData.X, currentLoadRoomData.Y))
        {
            room.transform.position = new Vector3(
                currentLoadRoomData.X * room.Width,
                currentLoadRoomData.Y * room.Height,
                0
            );

            room.X = currentLoadRoomData.X;
            room.Y = currentLoadRoomData.Y;
            room.name = currentWorldName + "-" + currentLoadRoomData.name + " " + room.X + ", " + room.Y;
            room.transform.parent = transform;
            room.gameObject.layer = LayerMask.NameToLayer("Minimap");
            //...
            Transform[] grandFa;
            grandFa = room.GetComponentsInChildren<Transform>();
            foreach (Transform child in grandFa)
            {
                //child.gameObject.layer = LayerMask.NameToLayer("Minimap");
                if (child.gameObject.name == "outline" || child.gameObject.name == "wall2")
                {
                    //child.gameObject.GetComponent<Renderer>().enabled = false;
                }
                if (child.gameObject.name == "outline")
                {
                    child.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;

                }
            }


            isLoadingRoom = false;

            if(loadedRooms.Count == 0)
            {
                CameraController.instance.currRoom = room;
            }

            loadedRooms.Add(room);
        }
        else
        {
            Destroy(room.gameObject);
            isLoadingRoom = false;
        }
    }

    void Awake() 
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // LoadRoom("1", 0, 0);
        // LoadRoom("2", 1, 0);
        // LoadRoom("2", -1, 0);
        // LoadRoom("2", 0, 1);
        // LoadRoom("2", 0, -1);
        LoadRoom("1", 0, 0);
        LoadRoom("2", 1, 0);
        LoadRoom("3", 2, 0);
        LoadRoom("4", 0, -1);
        LoadRoom("5", 1, -1);
        LoadRoom("6", 0, 1);
        LoadRoom("7", 1, 1);
        LoadRoom("8", 2, 1);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRoomQueue();
    }

    void UpdateRoomQueue()
    {
        if(isLoadingRoom)
        {
            return;
        }

        if(loadRoomQueue.Count == 0)
        {
            return;
        }

        currentLoadRoomData = loadRoomQueue.Dequeue();
        isLoadingRoom = true;

        StartCoroutine(LoadRoomRoutine(currentLoadRoomData));
    }

    public void OnPlayerEnterRoom(Room room)
    {
        CameraController.instance.currRoom = room;
        currRoom = room;
        Transform[] grandFa;
        grandFa = room.GetComponentsInChildren<Transform>();
        foreach (Transform child in grandFa)
        {
            //child.gameObject.layer = LayerMask.NameToLayer("Minimap");
            if(child.gameObject.name == "outline" || child.gameObject.name == "wall2")
            {
                child.gameObject.GetComponent<Renderer>().enabled = true;
            }
            if (child.gameObject.name == "outline")
            {
                child.gameObject.GetComponent<SpriteRenderer>().color = Color.white; 

            }
        }
        // StartCoroutine(RoomCoroutine());
    }
    public void OnPlayerExitRoom(Room room)
    {
        CameraController.instance.currRoom = room;
        currRoom = room;
        Transform[] grandFa;
        grandFa = room.GetComponentsInChildren<Transform>();
        foreach (Transform child in grandFa)
        {
            //child.gameObject.layer = LayerMask.NameToLayer("Minimap");
            if (child.gameObject.name == "outline" || child.gameObject.name == "wall2")
            {
                child.gameObject.GetComponent<Renderer>().enabled = true;
            }
            if (child.gameObject.name == "outline")
            {
                child.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;

            }
        }
        // StartCoroutine(RoomCoroutine());
    }

    //public IEnumerator RoomCoroutine()
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    UpdateRooms();
    //}
}
