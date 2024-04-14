using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

//SENT FROM SERVER TO CLIENT (RECEIVING PACKETS - NEEDS TO BE HANDLED)
public enum ServerPackets
{
    sendPlayerWelcomeMessage = 1,
    spawnPlayer,
    playerPosition,
    playerRotation,
    playerDisconnected,
    playerHealth,
    playerMana,
    playerRespawned,
    attackSpawned,
    attackPosition,
    attackDestroyed,
    itemSpawnerCreated,
    itemSpawned,
    itemPickedUp,
    spawnProjectile,
    projectilePosition,
    projectileExploded,
    spawnEnemy,
    enemyPositionAndRotation,
    enemyHealth,
    serverSendChatMessage,
    updatePlayerLevel,
    playerShieldOperations
}

//SENT FROM CLIENT TO SERVER (SENDING PACKETS)
public enum ClientPackets
{
    playerReceivedWelcomeMessage = 1,
    playerMovement,
    playerShoot,
    playerThrowItem,
    playerSendChatMessage,
    playerLevelUp,
    playerShieldActivated
}

public class Packet : IDisposable
{
    private List<byte> buffer;
    private byte[] byteArrayData;
    private int readingIndexPosition;

    //CREATES NEW AND EMPTY PACKET WITHOUT AN ID
    public Packet()
    {
        buffer = new List<byte>(); //INITIALIZE BYTE LIST
        readingIndexPosition = 0;
    }

    //CREATES A NEW PACKET WITH GIVEN ID (USED FOR SENDING)
    public Packet(int _id)
    {
        buffer = new List<byte>(); //INITIALIZE BYTE LIST
        readingIndexPosition = 0;

        Write(_id); //WRITE PACKET ID TO THE BUFFER
    }
    //CREATES A PACKET TO BE READ (USED FOR RECEIVING)
    public Packet(byte[] _data) //DATA TO BE ADDED TO THE PACKET
    {
        buffer = new List<byte>(); //INITIALIZE BYTE LIST
        readingIndexPosition = 0;

        SetBytes(_data);
    }

    #region Functions
    //SETS THE PACKET'S CONTENT AND PREPARES IT TO BE READ
    public void SetBytes(byte[] _data) //DATA TO BE ADDED TO THE PACKET
    {
        Write(_data);
        byteArrayData = buffer.ToArray();
    }

    //INSERTS THE LENGTH OF THE PACKET'S CONTENT AT THE START OF THE BUFFER
    public void InsertPacketLengthAtBeginning()
    {
        buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
    }

    //INSERTS THE GIVEN INT AT THE START OF THE BUFFER
    public void InsertIntAtBeginning(int _value)
    {
        buffer.InsertRange(0, BitConverter.GetBytes(_value));
    }

    //CONVERTS PACKET'S CONTENTS TO ARRAY
    public byte[] ConvertByteListToByteArray()
    {
        byteArrayData = buffer.ToArray();
        return byteArrayData;
    }

    //GETS THE LENGTH OF THE PACKET CONTENTS
    public int GetLengthOfThePacketContents()
    {
        return buffer.Count;
    }

    //GETS THE LENGTH OF THE UNREAD DATA IN THE PACKET 
    public int GetUnreadDataLengthInPacket()
    {
        return GetLengthOfThePacketContents() - readingIndexPosition;
    }

    //RESETS THE PACKET SO THAT IT CAN BE REUSED IN THE FUTURE
    public void ResetPacketContents(bool _shouldReset = true)
    {
        if (_shouldReset)
        {
            buffer.Clear(); //CLEAR BUFFER LIST
            byteArrayData = null;
            readingIndexPosition = 0;
        }
        else
        {
            readingIndexPosition -= 4; //MAKE LAST READ INT "UNREAD"
        }
    }
    #endregion

    #region Write Data
    //APPENDS A BYTE TO THE PACKET AT THE END OF THE PACKET
    public void Write(byte _value)
    {
        buffer.Add(_value);
    }
    //ADDENDS A BYTE ARRAY AT THE END OF THE PACKET
    public void Write(byte[] _value)
    {
        buffer.AddRange(_value);
    }
    //ADDENDS A SHORT AT THE END OF THE PACKET
    public void Write(short _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    //ADDENDS AN INT AT THE END OF THE PACKET
    public void Write(int _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    //ADDENDS A LONG AT THE END OF THE PACKET
    public void Write(long _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    //ADDENDS A FLOAT AT THE END OF THE PACKET
    public void Write(float _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    //ADDENDS A BOOLEAN AT THE END OF THE PACKET
    public void Write(bool _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    //ADDENDS A STRING AT THE END OF THE PACKET
    public void Write(string _value)
    {
        Write(_value.Length); //ADD THE LENGTH OF THE STRING AT THE END OF THE PACKET
        buffer.AddRange(Encoding.UTF32.GetBytes(_value));
    }
    //ADDENDS A VECTOR3 AT THE END OF THE PACKET
    public void Write(Vector3 _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
    }
    //ADDENDS A QUATERNION AT THE END OF THE PACKET
    public void Write(Quaternion _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
        Write(_value.w);
    }
    #endregion

    #region Read Data
    //READS PACKET IN BYTE FORM
    //moveReadingPosition --> WHETHER OR NOT TO MOVE THE BUFFER'S READ POSITION
    public byte ReadByte(bool _moveReadingPosition = true)
    {
        if (buffer.Count > readingIndexPosition)//IF THERE ARE UNREAD BYTES
        {
            byte value = byteArrayData[readingIndexPosition]; //GET THE BYTE AT readPos
            if (_moveReadingPosition == true)
            {
                readingIndexPosition += 1;
            }
            return value; //RETURN THE BYTE
        }
        else
        {
            throw new Exception("[Error]: 'BYTE' reading is failed!");
        }
    }

    //READS AN ARRAY OF BYTES FROM THE PACKET
    //moveReadingPosition --> WHETHER OR NOT TO MOVE THE BUFFER'S READ POSITION
    public byte[] ReadByteArray(int _arrayLength, bool _moveReadingPosition = true) 
    {
        if (buffer.Count > readingIndexPosition)//IF THERE ARE UNREAD BYTES
        {
            //GET THE BYTES AT readPos' POSITION WITH THE RANGE OF ARRAYLENGTH
            byte[] value = buffer.GetRange(readingIndexPosition, _arrayLength).ToArray();
            if (_moveReadingPosition == true)
            {
                readingIndexPosition += _arrayLength;
            }
            return value;
        }
        else
        {
            throw new Exception("[Error]: 'BYTE[]' reading is failed!");
        }
    }

    //READS SHORT BYTES FROM THE PACKET
    //moveReadingPosition --> WHETHER OR NOT TO MOVE THE BUFFER'S READ POSITION
    public short ReadShort(bool _moveReadingPosition = true)
    {
        if (buffer.Count > readingIndexPosition) //IF THERE ARE UNREAD BYTES
        {
            short value = BitConverter.ToInt16(byteArrayData, readingIndexPosition); //CONVERT BYTES TO SHORT
            if (_moveReadingPosition == true)
            {
                readingIndexPosition += 2;
            }
            return value;
        }
        else
        {
            throw new Exception("[Error]: 'SHORT' reading is failed!");
        }
    }

    //READS INT BYTES FROM THE PACKET
    //moveReadingPosition --> WHETHER OR NOT TO MOVE THE BUFFER'S READ POSITION
    public int ReadInt(bool _moveReadingPosition = true)
    {
        if (buffer.Count > readingIndexPosition) //IF THERE ARE UNREAD BYTES
        {
            int value = BitConverter.ToInt32(byteArrayData, readingIndexPosition); //CONVERT BYTES TO INT
            if (_moveReadingPosition == true)
            {
                readingIndexPosition += 4;
            }
            return value;
        }
        else
        {
            throw new Exception("[Error]: 'INT' reading is failed!");
        }
    }

    //READS LONG BYTES FROM THE PACKET
    //moveReadingPosition --> WHETHER OR NOT TO MOVE THE BUFFER'S READ POSITION
    public long ReadLong(bool _moveReadingPosition = true)
    {
        if (buffer.Count > readingIndexPosition)//IF THERE ARE UNREAD BYTES
        {
            long value = BitConverter.ToInt64(byteArrayData, readingIndexPosition); //CONVERT BYTES TO LONG
            if (_moveReadingPosition)
            {
                readingIndexPosition += 8;
            }
            return value;
        }
        else
        {
            throw new Exception("[Error]: 'LONG' reading is failed!");
        }
    }

    //READS FLOAT BYTES FROM THE PACKET
    //moveReadingPosition --> WHETHER OR NOT TO MOVE THE BUFFER'S READ POSITION
    public float ReadFloat(bool _moveReadingPosition = true)
    {
        if (buffer.Count > readingIndexPosition)//IF THERE ARE UNREAD BYTES
        {
            float value = BitConverter.ToSingle(byteArrayData, readingIndexPosition);  //CONVERT BYTES TO FLOAT
            if (_moveReadingPosition)
            {
                readingIndexPosition += 4;
            }
            return value;
        }
        else
        {
            throw new Exception("[Error]: 'FLOAT' reading is failed!");
        }
    }

    //READS BOOLEAN BYTES FROM THE PACKET
    //moveReadingPosition --> WHETHER OR NOT TO MOVE THE BUFFER'S READ POSITION
    public bool ReadBool(bool _moveReadingPosition = true)
    {
        if (buffer.Count > readingIndexPosition) //IF THERE ARE UNREAD BYTES
        {
            bool value = BitConverter.ToBoolean(byteArrayData, readingIndexPosition); //CONVERT BYTES TO FLOAT
            if (_moveReadingPosition)
            {
                readingIndexPosition += 1;
            }
            return value;
        }
        else
        {
            throw new Exception("[Error]: 'BOOL' reading is failed!");
        }
    }

    //READS STRING BYTES FROM THE PACKET
    //moveReadingPosition --> WHETHER OR NOT TO MOVE THE BUFFER'S READ POSITION
    public string ReadString(bool _moveReadingPosition = true)
    {
        try
        {
            int stringLength = ReadInt(); //GET THE LENGTH OF THE STRING
            string value = Encoding.UTF32.GetString(byteArrayData, readingIndexPosition, stringLength * 4);
            if (_moveReadingPosition && value.Length > 0) //IF _moveReadingPosition is TRUE AND STRING IS NOT EMPTY
            {
                readingIndexPosition += stringLength * 4;
            }
            return value;
        }
        catch
        {
            throw new Exception("[Error]: 'STRING' reading is failed!");
        }
    }

    //READS VECTOR3 FROM THE PACKET
    //moveReadingPosition --> WHETHER OR NOT TO MOVE THE BUFFER'S READ POSITION
    public Vector3 ReadVector3(bool _moveReadingPosition = true)
    {
        return new Vector3(ReadFloat(_moveReadingPosition), ReadFloat(_moveReadingPosition), ReadFloat(_moveReadingPosition));
    }

    //READS QUATERNION BYTES FROM THE PACKET
    //moveReadingPosition --> WHETHER OR NOT TO MOVE THE BUFFER'S READ POSITION
    public Quaternion ReadQuaternion(bool _moveReadingPosition = true)
    {
        return new Quaternion(ReadFloat(_moveReadingPosition), ReadFloat(_moveReadingPosition), ReadFloat(_moveReadingPosition), ReadFloat(_moveReadingPosition));
    }
    #endregion

    private bool disposed = false;

    protected virtual void Dispose(bool _disposing)
    {
        if (!disposed)
        {
            if (_disposing)
            {
                buffer = null;
                byteArrayData = null;
                readingIndexPosition = 0;
            }
            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
