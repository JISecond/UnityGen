using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;




public class GridGenerator : MonoBehaviour
{
    public static int GridSizeX = 30;
    public static int GridSizeZ = 20;
    public float GridOffset = 1.0f;

    public RoomStruct[] rooms = new RoomStruct[]{
        new RoomStruct(Room.living,100,true),
        new RoomStruct(Room.Dining,60,true),
        new RoomStruct(Room.Hall,70, true),
        new RoomStruct(Room.Bedroom,60, true),
        new RoomStruct(Room.Bathroom,40, false),
    };


 

    public GameObject blockGameObject;

    public Material BedRoomMat;
    public Material LivingRoomMat;
    public Material DiningRoomMat;
    public Material HallMat;
    public Material BathroomMat;
    public Material EmptyMat;

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
       

    }

    public void GrowXPos(ref RoomStruct room, bool ignoreNotEmpty)
    {
        FindCorners(ref room);
        if (room.maxX +1 >= GridSizeX) { return; }
        // проверка на свободное пространство
        if (!ignoreNotEmpty)
        {
            for (int z = room.minZ; z <= room.maxZ; z++)
            {
                if (gridArray[room.maxX + 1, z].GetComponent<GridElement>().roomName != Room.Empty)
                {
                    return;
                }
            }
        }
        //увеличение 
        for (int z = room.minZ; z <= room.maxZ; z++)
        {
            ConvertRoom(gridArray[room.maxX + 1, z], room.roomName);
        }
    }
    public void GrowXNeg(ref RoomStruct room, bool ignoreNotEmpty)
    {
        FindCorners(ref room);
        if (room.minX - 1 < 0) { return; }
        if (!ignoreNotEmpty)
        {
            for (int z = room.minZ; z <= room.maxZ; z++)
            {
                if (gridArray[room.minX - 1, z].GetComponent<GridElement>().roomName != Room.Empty)
                {
                    return;
                }
            }
        }
        for (int z = room.minZ; z <= room.maxZ; z++)
        {
            ConvertRoom(gridArray[room.minX - 1, z], room.roomName);
           
        }
    }
    public void GrowZPos(ref RoomStruct room, bool ignoreNotEmpty)
    {
        FindCorners(ref room);
        if (room.maxZ +1 >= GridSizeZ) { return; }
        if (!ignoreNotEmpty)
        {
            for (int x = room.minX; x <= room.maxX; x++)
            {
                if (gridArray[x, room.maxZ + 1].GetComponent<GridElement>().roomName != Room.Empty)
                {
                    return;
                }
            }
        }
        for (int x = room.minX; x <= room.maxX; x++)
        {
            ConvertRoom(gridArray[x, room.maxZ +1], room.roomName);
            
        }
    }
    public void GrowZNeg(ref RoomStruct room, bool ignoreNotEmpty)
    {
        FindCorners(ref room);
        if (room.minZ - 1 < 0) { return; }
        if (!ignoreNotEmpty)
        {
            for (int x = room.minX; x <= room.maxX; x++)
            {
                if (gridArray[x, room.minZ - 1].GetComponent<GridElement>().roomName != Room.Empty)
                {
                    return;
                }
            }
        }
        for (int x = room.minX; x <= room.maxX; x++)
        {
            ConvertRoom(gridArray[x, room.minZ - 1], room.roomName);
            
        }
    }

    public void GrowInAllDirections(ref RoomStruct room,bool ignoreNotEmpty)
    {
        GrowXNeg(ref room, ignoreNotEmpty);
        GrowXPos(ref room, ignoreNotEmpty);
        GrowZNeg(ref room, ignoreNotEmpty);
        GrowZPos(ref room, ignoreNotEmpty);
    }

    public void GrowRooms()
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].currentSize <= rooms[i].maxSize || rooms[i].softSize)
            {
                
                GrowInAllDirections(ref rooms[i], false);
            }
        }

    }
     

    public void GrowRoomsLShape()
    {
        Array.Sort(rooms, (x, y) => x.GetSizePercentage().CompareTo(y.GetSizePercentage()));
 
        for (int i = 0; i < rooms.Length; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GrowInAllDirections(ref rooms[i], true);
            }
            
        }

        //TODO: ищим комнату с максимальным количеством пустых соседних клеток и расширяемся в нее (расширение не должно сужаться по пути)
        // задавать размер комнаты как процент занимаемого пространства
        // не пересчитывать углы при каждом увеличении 
        // вынести массив с комнатами в отдельный класс, присвоить каждому классу комнаты материал и назначать его не входя в режим мгры
    }
    public void GenerateGrid()
    {
        GameObject childObject = new GameObject("Grid");
        childObject.transform.parent = this.transform;
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int z = 0; z < GridSizeZ; z++)
            {
                Vector3 pos = new Vector3(x * GridOffset, 0, z * GridOffset);
                GameObject block = Instantiate(blockGameObject, pos, Quaternion.identity) as GameObject;
                block.transform.SetParent(childObject.transform);
                block.name = x + " " + z;
                gridArray[x, z] = block;
            }
        }
    }

    public void RandomizeRooms()
    {
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int z = 0; z < GridSizeZ; z++)
            {
                gridArray[x, z].GetComponent<GridElement>().roomName = Room.Empty;
            }
        }
        for (int i = 0; i < rooms.Length; i++)
        {
            gridArray[UnityEngine.Random.Range(0, GridSizeX), UnityEngine.Random.Range(0, GridSizeZ)].GetComponent<GridElement>().roomName = rooms[i].roomName;
        }
    }

    public void GenerateWalls()
    {
        GameObject innerWalls = new GameObject("InnerWalls");
        innerWalls.transform.parent = this.transform;
        GameObject outerWalls = new GameObject("OuterWalls");
        outerWalls.transform.parent = this.transform;
        for (int x = 1; x < GridSizeX; x++)
        {
            for (int z = 0; z < GridSizeZ; z++)
            {
                if (gridArray[x, z].GetComponent<GridElement>().roomName != gridArray[x-1, z].GetComponent<GridElement>().roomName)
                {
                    Vector3 pos = new Vector3(((float)x-GridOffset/2) * GridOffset, 3, z * GridOffset);
                    GameObject block = Instantiate(blockGameObject, pos, Quaternion.identity) as GameObject;
                    block.transform.SetParent(innerWalls.transform);
                    block.transform.localScale = new Vector3(0.2f, 6, 1);
                }

            }

        }
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int z = 1; z < GridSizeZ; z++)
            {
                if (gridArray[x, z].GetComponent<GridElement>().roomName != gridArray[x, z-1].GetComponent<GridElement>().roomName)
                {
                    Vector3 pos = new Vector3(x * GridOffset, 3, ((float)z - GridOffset / 2) * GridOffset);
                    GameObject block = Instantiate(blockGameObject, pos, Quaternion.identity) as GameObject;
                    block.transform.SetParent(innerWalls.transform);
                    block.transform.localScale = new Vector3(1, 6, 0.2f);

                }

            }

        }

        for (int x = 0; x < GridSizeX; x++)
        {
            Vector3 pos = new Vector3(x * GridOffset, 3, (0.0f - GridOffset / 2) * GridOffset);
            GameObject block = Instantiate(blockGameObject, pos, Quaternion.identity) as GameObject;
            block.transform.SetParent(outerWalls.transform);
            block.transform.localScale = new Vector3(1, 6, 0.2f);

            pos = new Vector3(x * GridOffset, 3, ((float)GridSizeZ - GridOffset / 2) * GridOffset);
            block = Instantiate(blockGameObject, pos, Quaternion.identity) as GameObject;
            block.transform.SetParent(outerWalls.transform);
            block.transform.localScale = new Vector3(1, 6, 0.2f);

        }
        for (int z = 0; z < GridSizeZ; z++)
        {
            Vector3 pos = new Vector3((0.0f - GridOffset / 2) * GridOffset, 3, z * GridOffset);
            GameObject block = Instantiate(blockGameObject, pos, Quaternion.identity) as GameObject;
            block.transform.SetParent(outerWalls.transform);
            block.transform.localScale = new Vector3(0.2f, 6, 1);

            pos = new Vector3(((float)GridSizeX - GridOffset / 2) * GridOffset, 3,z  * GridOffset);
            block = Instantiate(blockGameObject, pos, Quaternion.identity) as GameObject;
            block.transform.SetParent(outerWalls.transform);
            block.transform.localScale = new Vector3(0.2f, 6, 1);

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
                switch (gridArray[x, z].GetComponent<GridElement>().roomName)
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
                    case Room.Empty:
                        gridArray[x, z].GetComponent<Renderer>().material = EmptyMat;
                        break;
                }
            }
        }
    }
}