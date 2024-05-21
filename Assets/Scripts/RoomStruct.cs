using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RoomStruct 
{
    public RoomStruct(Room roomName,int MaxSize, bool softSize )
    {
        this.roomName = roomName;
        this.maxSize = MaxSize;
        minX = 1000;
        minZ = 1000;
        maxX = 0;
        maxZ = 0;
        currentSize = 0;
        this.softSize = softSize;
       

    }
    public Room roomName;
    public int maxSize, currentSize;
    public int minX, minZ, maxX, maxZ;
    public bool softSize; // может ли комната выходить за максимальный размер

    public float GetSizePercentage()
    {
        return  (float)currentSize/(float)maxSize ;
    }
    
}
