using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;




public class GridGenerator : MonoBehaviour
{
    public static int GridSizeX = 20;
    public static int GridSizeZ = 20;
    public float GridOffset = 1.0f;

    public RoomStruct[] rooms = new RoomStruct[]{
        new RoomStruct(Room.living,100),
        new RoomStruct(Room.Dining,60),
        new RoomStruct(Room.Hall,70),
        new RoomStruct(Room.Bedroom,60),
        new RoomStruct(Room.Bathroom,40),
    };


    //public int hallSize = 200;
    //public int bathRoomSize = 200;
    //public int bedRoomSize = 200;
    //public int livingSize = 200;
    //public int diningSize = 200;
    //private int roomSize;

    //private int minX, minZ, maxX, maxZ;

    public GameObject blockGameObject;

    public Material BedRoomMat;
    public Material LivingRoomMat;
    public Material DiningRoomMat;
    public Material HallMat;
    public Material BathroomMat;

    public GameObject[,] gridArray = new GameObject[GridSizeX, GridSizeZ];



    void ConvertRoom(GameObject room, Room roomName)
    {
        if (room.GetComponent<GridElement>().roomName == Room.Empty) 
        {
            room.GetComponent<GridElement>().roomName = roomName;

           
        } 
        else 
        {
            return;
        }
       
    }


    public void FindCorners(ref RoomStruct room)
    {
        room.minX = GridSizeX;
        room.minZ = GridSizeZ;
        room.maxX = 0;
        room.maxZ = 0;
       
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int z = 0; z < GridSizeZ; z++)
            {
                if (gridArray[x, z].GetComponent<GridElement>().roomName == room.roomName)
                {
                    if (room.minX > x) { room.minX = x; }
                    if (room.minZ > z) { room.minZ = z; }
                    if (room.maxX < x) { room.maxX = x; }
                    if (room.maxZ < z) { room.maxZ = z; }


                }
            }

        }
       
        room.currentSize = (room.maxX + 1 - room.minX) * (room.maxZ + 1 - room.minZ);
        print("room" + room.roomName + "current size = " + room.currentSize);

    }

    public void GrowXPos(ref RoomStruct room)
    {
        FindCorners(ref room);
        if (room.maxX +1 >= GridSizeX) { return; } 
           
        for (int z = room.minZ; z <= room.maxZ; z++){
            if (gridArray[room.maxX + 1, z].GetComponent<GridElement>().roomName != Room.Empty) {
                return;
            }
        }
        for (int z = room.minZ; z <= room.maxZ; z++)
        {
            ConvertRoom(gridArray[room.maxX + 1, z], room.roomName);
        }
    }
    public void GrowXNeg(ref RoomStruct room)
    {
        FindCorners(ref room);
        if (room.minX - 1 < 0) { return; }

        for (int z = room.minZ; z <= room.maxZ; z++)
        {
            if (gridArray[room.minX - 1, z].GetComponent<GridElement>().roomName != Room.Empty)
            {
                return;
            }
        }
        for (int z = room.minZ; z <= room.maxZ; z++)
        {
            ConvertRoom(gridArray[room.minX - 1, z], room.roomName);
           
        }
    }
    public void GrowZPos(ref RoomStruct room)
    {
        FindCorners(ref room);
        if (room.maxZ +1 >= GridSizeZ) { return; }

        for (int x = room.minX; x <= room.maxX; x++)
        {
            if (gridArray[x, room.maxZ + 1].GetComponent<GridElement>().roomName != Room.Empty)
            {
                return;
            }
        }
        for (int x = room.minX; x <= room.maxX; x++)
        {
            ConvertRoom(gridArray[x, room.maxZ +1], room.roomName);
            
        }
    }
    public void GrowZNeg(ref RoomStruct room)
    {
        FindCorners(ref room);
        if (room.minZ - 1 < 0) { return; }

        for (int x = room.minX; x <= room.maxX; x++)
        {
            if (gridArray[x, room.minZ - 1].GetComponent<GridElement>().roomName != Room.Empty)
            {
                return;
            }
        }
        for (int x = room.minX; x <= room.maxX; x++)
        {
            ConvertRoom(gridArray[x, room.minZ - 1], room.roomName);
            
        }
    }

    public void GrowInAllDirections(ref RoomStruct room)
    {
        GrowXNeg(ref room);
        GrowXPos(ref room);
        GrowZNeg(ref room);
        GrowZPos(ref room);
    }

    public void GrowRooms()
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].currentSize <= rooms[i].MaxSize)
            {
                GrowInAllDirections(ref rooms[i]);
            }
        }













    }

    public void GrowRoomsLShape()
    {


    }
    public void GenerateGrid()
    {
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int z = 0; z < GridSizeZ; z++)
            {
                Vector3 pos = new Vector3(x * GridOffset, 0, z * GridOffset);



                GameObject block = Instantiate(blockGameObject, pos, Quaternion.identity) as GameObject;


                block.transform.SetParent(this.transform);

                gridArray[x, z] = block;


            }
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int z = 0; z < GridSizeZ; z++)
            {
                if (gridArray[x, z].GetComponent<GridElement>().roomName != Room.Empty)
                {
                    switch(gridArray[x, z].GetComponent<GridElement>().roomName)
                    {
                        case Room.Bedroom:
                            gridArray[x, z].GetComponent<Renderer>().material = BedRoomMat;
                            break;
                        case Room.Dining:
                            gridArray[x, z].GetComponent<Renderer>().material = DiningRoomMat;
                            break;
                        case Room.living:
                            gridArray[x, z].GetComponent<Renderer>().material = LivingRoomMat;
                            break;
                        case Room.Hall:
                            gridArray[x, z].GetComponent<Renderer>().material = HallMat;
                            break;
                        case Room.Bathroom:
                            gridArray[x, z].GetComponent<Renderer>().material = BathroomMat;
                            break;
                    }
                    
                    
                } 
            }
        }
    }
}