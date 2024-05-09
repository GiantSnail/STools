using System;
using System.Collections.Generic;

namespace lightarc 
{
    class ByteArray
    {
        private List<byte> buffer = new List<byte>();
        private int pos = 0;

        public void Append(byte[] data, int length)
        {
            for (int i = 0; i < length; i++)
            {
                buffer.Add(data[i]);
            }
        }

        public uint ReadUint()
        {
            byte[] bytes = { buffer[pos], buffer[pos + 1], buffer[pos + 2], buffer[pos + 3] };
            uint value = BitConverter.ToUInt32(bytes, 0);
            pos += sizeof(uint);
            return value;
        }

        public uint PeekUint()
        {
            byte[] bytes = { buffer[pos], buffer[pos + 1], buffer[pos + 2], buffer[pos + 3] };
            uint value = BitConverter.ToUInt32(bytes, pos);
            return value;
        }

        public int ReadInt()
        {
            byte[] bytes = { buffer[pos], buffer[pos + 1], buffer[pos + 2], buffer[pos + 3] };
            int value = BitConverter.ToInt32(bytes, pos);
            pos += sizeof(int);
            return value;
        }

        public int PeekInt()
        {
            byte[] bytes = { buffer[pos], buffer[pos + 1], buffer[pos + 2], buffer[pos + 3] };
            int value = BitConverter.ToInt32(bytes, pos);
            return value;
        }

        public byte[] ReadBytes(int length)
        {
            byte[] bytes = buffer.GetRange(pos, length).ToArray();
            pos += length;
            return bytes;
        }

        public void WriteUint(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            buffer.AddRange(bytes);
        }

        public void WriteInt(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            buffer.AddRange(bytes);
        }

        public void WriteBytes(byte[] bytes)
        {
            buffer.AddRange(bytes);
        }

        public int Position => pos;

        public int Length => buffer.Count;

        public int LeftCount => buffer.Count - pos;

        public void ResetPosition()
        {
            pos = 0;
        }

        /// <summary>
        /// 根据当前位置进行偏移
        /// </summary>
        /// <param name="offset"></param>
        public void Seek(int offset)
        {
            pos += offset;
        }

        /// <summary>
        /// 清空已读取的数据
        /// </summary>
        public void ClearReaded()
        {
            buffer.RemoveRange(0, pos);
            pos = 0;
        }
    }
}

