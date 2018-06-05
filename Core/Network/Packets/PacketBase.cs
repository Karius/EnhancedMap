﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancedMap.Core.Network.Packets
{
    public abstract class PacketBase
    {
        protected abstract byte this[int index] { get; set; }
        protected abstract void EnsureSize(int length);

        public abstract int Length { get; }
        public abstract byte ID { get; }
        public int Position { get; protected set; }

        public abstract byte[] ToArray();

        public void Skip(int length)
        {
            EnsureSize(length);
            Position += length;
        }

        public void Seek(int index)
        {
            Position = index;
            EnsureSize(0);
        }

        public void WriteByte(byte v)
        {
            EnsureSize(1);
            this[Position++] = v;
        }

        public void WriteSByte(sbyte v)
        {
            WriteByte((byte)v);
        }

        public void WriteBool(bool v)
        {
            WriteByte(v ? (byte)0x01 : (byte)0x00);
        }

        public void WriteUShort(ushort v)
        {
            EnsureSize(2);
            WriteByte((byte)v);
            WriteByte((byte)(v >> 8));
        }

        public void WriteUInt(uint v)
        {
            EnsureSize(4);
            WriteByte((byte)v);
            WriteByte((byte)(v >> 8));
            WriteByte((byte)(v >> 16));
            WriteByte((byte)(v >> 24));
        }

        public void WriteFloat(float v)
        {
            unsafe
            {
                uint val = *(uint*)(&v);
                WriteUInt(val);
            }
        }

        public void WriteASCII(string value)
        {
            EnsureSize(value.Length + 1);
            foreach (char c in value)
                WriteByte((byte)c);
            //WriteByte(0);
        }

        public void WriteASCII(string value, int length)
        {
            EnsureSize(length);
            if (value.Length > length)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < value.Length; i++)
                WriteByte((byte)value[i]);

            if (value.Length < length)
            {
                WriteByte(0);
                Position += length - value.Length - 1;
            }
        }

        public void WriteUnicode(string value)
        {
            EnsureSize((value.Length + 1) * 2);
            foreach (char c in value)
            {
                WriteByte((byte)(c >> 8));
                WriteByte((byte)(c));
            }
            WriteUShort(0);
        }

        public void WriteUnicode(string value, int length)
        {
            EnsureSize(length);
            if (value.Length > length)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < value.Length; i++)
            {
                WriteByte((byte)(value[i] >> 8));
                WriteByte((byte)value[i]);
            }

            if (value.Length < length)
            {
                WriteUShort(0);
                Position += (length - value.Length - 1) * 2;
            }
        }
    }
}