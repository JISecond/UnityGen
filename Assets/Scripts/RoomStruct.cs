using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RoomStruct 
{
    public RoomStruct(Room roomName,int MaxSize)
    {
        this.roomName = roomName;
        this.MaxSize = MaxSize;
        minX = 1000;
        minZ = 1000;
        maxX = 0;
        maxZ = 0;
        currentSize = 0;
    }
    public Room roomName;
    public int MaxSize, currentSize;
    public int minX, minZ, maxX, maxZ;
    
}
