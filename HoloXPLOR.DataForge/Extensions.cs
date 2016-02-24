﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DDRIT = Dolkens.Framework.BinaryExtensions.ExtensionMethods;
using System.Security.Cryptography;
using Dolkens.Framework.BinaryExtensions;

namespace Dolkens.Framework.BinaryExtensions
{
    public enum StringSizeEnum
    {
        Int8 = 1,
        Int16 = 2,
        Int32 = 4,
    }

    /// <summary>
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2008 Peter Dolkens
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Stream Extensions

        /// <summary>
        /// Read a Length-prefixed string from the stream
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="byteLength">Size of the Length representation</param>
        /// <returns></returns>
        public static String ReadPString(this BinaryReader binaryReader, StringSizeEnum byteLength = StringSizeEnum.Int32)
        {
            Int32 stringLength = 0;

            switch (byteLength)
            {
                case StringSizeEnum.Int8:
                    stringLength = binaryReader.ReadByte();
                    break;
                case StringSizeEnum.Int16:
                    stringLength = binaryReader.ReadInt16();
                    break;
                case StringSizeEnum.Int32:
                    stringLength = binaryReader.ReadInt32();
                    break;
                default:
                    throw new NotSupportedException("Only Int8, Int16, and Int32 string sizes are supported");
            }

            // If there is actually a string to read
            if (stringLength > 0)
            {
                return new String(binaryReader.ReadChars(stringLength));
            }

            return null;
        }

        /// <summary>
        /// Read a NULL-Terminated string from the stream
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        public static String ReadCString(this BinaryReader binaryReader)
        {
            Int32 stringLength = 0;

            while (binaryReader.ReadChar() != 0 && binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                stringLength++;

            Int64 nul = binaryReader.BaseStream.Position;

            binaryReader.BaseStream.Seek(0 - stringLength - 1, SeekOrigin.Current);

            Char[] chars = binaryReader.ReadChars(stringLength + 1);

            binaryReader.BaseStream.Seek(nul, SeekOrigin.Begin);

            // If there is actually a string to read
            if (stringLength > 0)
            {
                return new String(chars, 0, stringLength).Replace("\u0000", "");
            }

            return null;
        }

        /// <summary>
        /// Read a Fixed-Length string from the stream
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="stringLength">Size of the String</param>
        /// <returns></returns>
        public static String ReadFString(this BinaryReader binaryReader, Int32 stringLength)
        {
            Char[] chars = binaryReader.ReadChars(stringLength);

            for (Int32 i = 0; i < stringLength; i++)
            {
                if (chars[i] == 0)
                {
                    return new String(chars, 0, i);
                }
            }

            return new String(chars);
        }

        public static Byte[] ReadAllBytes(this Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Int64 oldPosition = stream.Position;
                stream.Position = 0;
                stream.CopyTo(ms);
                stream.Position = oldPosition;
                return ms.ToArray();
            }
        }

        #endregion
    }
}

#region Namespace Proxies

namespace System
{
    public static class _Proxy
    {
        /// <summary>
        /// Read a Length-prefixed string from the stream
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="byteLength">Size of the Length representation</param>
        /// <returns></returns>
        public static String ReadPString(this BinaryReader binaryReader, StringSizeEnum byteLength = StringSizeEnum.Int32) { return DDRIT.ReadPString(binaryReader, byteLength); }

        /// <summary>
        /// Read a NULL-Terminated string from the stream
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        public static String ReadCString(this BinaryReader binaryReader) { return DDRIT.ReadCString(binaryReader); }

        /// <summary>
        /// Read a Fixed-Length string from the stream
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="stringLength">Size of the String</param>
        /// <returns></returns>
        public static String ReadFString(this BinaryReader binaryReader, Int32 stringLength) { return DDRIT.ReadFString(binaryReader, stringLength); }
    }
}

#endregion