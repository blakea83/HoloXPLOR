﻿#define NONULL

using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace HoloXPLOR.DataForge
{
    public class DataForge
    {
        internal BinaryReader _br;

        public Boolean IsLegacy { get; set; }
        public Int32 FileVersion { get; set; }
        
        public DataForgeStructDefinition[] StructDefinitionTable { get; set; }
        public DataForgePropertyDefinition[] PropertyDefinitionTable { get; set; }
        public DataForgeEnumDefinition[] EnumDefinitionTable { get; set; }
        public DataForgeDataMapping[] DataMappingTable { get; set; }
        public DataForgeRecord[] RecordDefinitionTable { get; set; }
        public DataForgeStringLookup[] EnumOptionTable { get; set; }
        public DataForgeString[] ValueTable { get; set; }

        public DataForgeReference[] Array_ReferenceValues { get; set; }
        public DataForgeGuid[] Array_GuidValues { get; set; }
        public DataForgeStringLookup[] Array_StringValues { get; set; }
        public DataForgeLocale[] Array_LocaleValues { get; set; }
        public DataForgeEnum[] Array_EnumValues { get; set; }
        public DataForgeInt8[] Array_Int8Values { get; set; }
        public DataForgeInt16[] Array_Int16Values { get; set; }
        public DataForgeInt32[] Array_Int32Values { get; set; }
        public DataForgeInt64[] Array_Int64Values { get; set; }
        public DataForgeUInt8[] Array_UInt8Values { get; set; }
        public DataForgeUInt16[] Array_UInt16Values { get; set; }
        public DataForgeUInt32[] Array_UInt32Values { get; set; }
        public DataForgeUInt64[] Array_UInt64Values { get; set; }
        public DataForgeBoolean[] Array_BooleanValues { get; set; }
        public DataForgeSingle[] Array_SingleValues { get; set; }
        public DataForgeDouble[] Array_DoubleValues { get; set; }
        public DataForgePointer[] Array_StrongValues { get; set; }
        public DataForgePointer[] Array_WeakValues { get; set; }

        public Dictionary<UInt32, String> ValueMap { get; set; }
        public Dictionary<UInt32, List<XmlElement>> DataMap { get; set; }
        public List<Tuple<XmlElement, UInt16, Int32>> Require_ClassMapping { get; set; }
        public List<Tuple<XmlElement, UInt16, Int32>> Require_StrongMapping { get; set; }
        public List<Tuple<XmlAttribute, UInt16, Int32>> Require_WeakMapping1 { get; set; }
        public List<Tuple<XmlAttribute, UInt16, Int32>> Require_WeakMapping2 { get; set; }
        public List<XmlElement> DataTable { get; set; }

        public U[] ReadArray<U>(Int32 arraySize) where U : _DataForgeSerializable
        {
            if (arraySize == -1)
            {
                return null;
            }

            return (from i in Enumerable.Range(0, arraySize)
                    let data = (U)Activator.CreateInstance(typeof(U), this)
                    // let hack = data._index = i
                    select data).ToArray();
        }

        public DataForge(BinaryReader br, Boolean legacy = false)
        {
            this._br = br;
            var temp00 = this._br.ReadInt32();
            this.FileVersion = this._br.ReadInt32();
            this.IsLegacy = legacy;

            this.Require_ClassMapping    = new List<Tuple<XmlElement, UInt16, Int32>> { };
            this.Require_StrongMapping   = new List<Tuple<XmlElement, UInt16, Int32>> { };
            this.Require_WeakMapping1    = new List<Tuple<XmlAttribute, UInt16, Int32>> { };
            this.Require_WeakMapping2    = new List<Tuple<XmlAttribute, UInt16, Int32>> { };

            if (!this.IsLegacy)
            {
                var atemp1 = this._br.ReadUInt16();
                var atemp2 = this._br.ReadUInt16();
                var atemp3 = this._br.ReadUInt16();
                var atemp4 = this._br.ReadUInt16();
            }

            var structDefinitionCount    = this._br.ReadUInt16(); var temp03 = this._br.ReadUInt16();  // 0x02d3
            var propertyDefinitionCount  = this._br.ReadUInt16(); var temp04 = this._br.ReadUInt16();  // 0x0602
            var enumDefinitionCount      = this._br.ReadUInt16(); var temp05 = this._br.ReadUInt16();  // 0x0041 : 0x0002 ??? 0xbbad
            var dataMappingCount         = this._br.ReadUInt16(); var temp06 = this._br.ReadUInt16();  // 0x013c
            var recordDefinitionCount    = this._br.ReadUInt16(); var temp07 = this._br.ReadUInt16();  // 0x0b35

            var int8ValueCount           = this._br.ReadUInt16(); var temp08 = this._br.ReadUInt16();  // 0x0014 - Int8
            var int16ValueCount          = this._br.ReadUInt16(); var temp09 = this._br.ReadUInt16();  // 0x0014 - Int16
            var int64ValueCount          = this._br.ReadUInt16(); var temp10 = this._br.ReadUInt16();  // 0x0014 - Int32
            var int32ValueCount          = this._br.ReadUInt16(); var temp11 = this._br.ReadUInt16();  // 0x0024 - Int64
            var uint8ValueCount          = this._br.ReadUInt16(); var temp12 = this._br.ReadUInt16();  // 0x0014 - UInt8
            var uint16ValueCount         = this._br.ReadUInt16(); var temp13 = this._br.ReadUInt16();  // 0x0014 - UInt16
            var uint64ValueCount         = this._br.ReadUInt16(); var temp14 = this._br.ReadUInt16();  // 0x0014 - UInt32
            var uint32ValueCount         = this._br.ReadUInt16(); var temp15 = this._br.ReadUInt16();  // 0x0014 - UInt64
            var booleanValueCount        = this._br.ReadUInt16(); var temp16 = this._br.ReadUInt16();  // 0x0014 - Boolean
            var singleValueCount         = this._br.ReadUInt16(); var temp17 = this._br.ReadUInt16();  // 0x003c - Single
            var doubleValueCount         = this._br.ReadUInt16(); var temp18 = this._br.ReadUInt16();  // 0x0014 - Double
            var guidValueCount           = this._br.ReadUInt16(); var temp19 = this._br.ReadUInt16();  // 0x0014 - Guid Array Values
            var stringValueCount         = this._br.ReadUInt16(); var temp20 = this._br.ReadUInt16();  // 0x0076 - String Array Values
            var localeValueCount         = this._br.ReadUInt16(); var temp21 = this._br.ReadUInt16();  // 0x0034 - Locale Array Values
            var enumValueCount           = this._br.ReadUInt16(); var temp22 = this._br.ReadUInt16();  // 0x006e - Enum Array Values
            var strongValueCount         = this._br.ReadUInt16(); var temp23 = this._br.ReadUInt16();  // 0x1cf3 - ??? Class Array Values
            var weakValueCount           = this._br.ReadUInt16(); var temp24 = this._br.ReadUInt16();  // 0x079d - ??? Pointer Array Values

            var referenceValueCount      = this._br.ReadUInt16(); var temp25 = this._br.ReadUInt16();  // 0x0026 - Reference Array Values
            var enumOptionCount          = this._br.ReadUInt16(); var temp26 = this._br.ReadUInt16();  // 0x0284 - Enum Options
            var textLength               = this._br.ReadUInt32(); var temp27 = (this.IsLegacy) ? 0 : this._br.ReadUInt32();  // 0x2e45 : 0x00066e4b

            this.StructDefinitionTable   = this.ReadArray<DataForgeStructDefinition>(structDefinitionCount);
            this.PropertyDefinitionTable = this.ReadArray<DataForgePropertyDefinition>(propertyDefinitionCount);
            this.EnumDefinitionTable     = this.ReadArray<DataForgeEnumDefinition>(enumDefinitionCount);
            this.DataMappingTable        = this.ReadArray<DataForgeDataMapping>(dataMappingCount);
            this.RecordDefinitionTable   = this.ReadArray<DataForgeRecord>(recordDefinitionCount);

            this.Array_Int8Values        = this.ReadArray<DataForgeInt8>(int8ValueCount);
            this.Array_Int16Values       = this.ReadArray<DataForgeInt16>(int16ValueCount);
            this.Array_Int32Values       = this.ReadArray<DataForgeInt32>(int32ValueCount);
            this.Array_Int64Values       = this.ReadArray<DataForgeInt64>(int64ValueCount);
            this.Array_UInt8Values       = this.ReadArray<DataForgeUInt8>(uint8ValueCount);
            this.Array_UInt16Values      = this.ReadArray<DataForgeUInt16>(uint16ValueCount);
            this.Array_UInt32Values      = this.ReadArray<DataForgeUInt32>(uint32ValueCount);
            this.Array_UInt64Values      = this.ReadArray<DataForgeUInt64>(uint64ValueCount);
            this.Array_BooleanValues     = this.ReadArray<DataForgeBoolean>(booleanValueCount);
            this.Array_SingleValues      = this.ReadArray<DataForgeSingle>(singleValueCount);
            this.Array_DoubleValues      = this.ReadArray<DataForgeDouble>(doubleValueCount);
            this.Array_GuidValues        = this.ReadArray<DataForgeGuid>(guidValueCount);
            this.Array_StringValues      = this.ReadArray<DataForgeStringLookup>(stringValueCount);
            this.Array_LocaleValues      = this.ReadArray<DataForgeLocale>(localeValueCount);
            this.Array_EnumValues        = this.ReadArray<DataForgeEnum>(enumValueCount);
            this.Array_StrongValues      = this.ReadArray<DataForgePointer>(strongValueCount);
            this.Array_WeakValues        = this.ReadArray<DataForgePointer>(weakValueCount);

            this.Array_ReferenceValues   = this.ReadArray<DataForgeReference>(referenceValueCount);
            this.EnumOptionTable         = this.ReadArray<DataForgeStringLookup>(enumOptionCount);

            var buffer = new List<DataForgeString> { };
            var maxPosition = this._br.BaseStream.Position + textLength;
            var startPosition = this._br.BaseStream.Position;
            this.ValueMap = new Dictionary<UInt32,String> { };
            while (this._br.BaseStream.Position < maxPosition)
            {
                var offset = this._br.BaseStream.Position - startPosition;
                var dfString = new DataForgeString(this);
                buffer.Add(dfString);
                this.ValueMap[(UInt32)offset] = dfString.Value;
            }
            this.ValueTable = buffer.ToArray();
            
            this.DataTable = new List<XmlElement> { };
            this.DataMap = new Dictionary<UInt32, List<XmlElement>> { };

            foreach (var dataMapping in this.DataMappingTable)
            {
                this.DataMap[dataMapping.StructIndex] = new List<XmlElement> { };

                var dataStruct = this.StructDefinitionTable[dataMapping.StructIndex];

                for (Int32 i = 0; i < dataMapping.StructCount; i++)
                {
                    var node = dataStruct.Read(dataMapping.Name);
                    
                    this.DataMap[dataMapping.StructIndex].Add(node);
                    this.DataTable.Add(node);
                }
            }

            foreach (var dataMapping in this.Require_ClassMapping)
            {
                if (dataMapping.Item2 == 0xFFFF)
                {
#if NONULL
                    dataMapping.Item1.ParentNode.RemoveChild(dataMapping.Item1);
#else
                    dataMapping.Item1.ParentNode.ReplaceChild(
                        this._xmlDocument.CreateElement("null"),
                        dataMapping.Item1);
#endif
                }
                else
                {
                    dataMapping.Item1.ParentNode.ReplaceChild(
                        this.DataMap[dataMapping.Item2][dataMapping.Item3],
                        dataMapping.Item1);
                }
            }

        }

        private XmlDocument _xmlDocument = new XmlDocument();
        public XmlElement CreateElement(String name) { return this._xmlDocument.CreateElement(name); }
        public XmlAttribute CreateAttribute(String name) { return this._xmlDocument.CreateAttribute(name); }
        public String OuterXML { get { return this._xmlDocument.OuterXml; } }
        public XmlNodeList ChildNodes { get { return this._xmlDocument.DocumentElement.ChildNodes; } }

        public void GenerateXML()
        {
            var root = this._xmlDocument.CreateElement("DataForge");
            this._xmlDocument.AppendChild(root);
            foreach (var record in this.RecordDefinitionTable)
            {
                this.DataMap[record.StructIndex][record.VariantIndex] = this.DataMap[record.StructIndex][record.VariantIndex].Rename(record.Name);
                var recordXml = this.DataMap[record.StructIndex][record.VariantIndex];
                if (!this.IsLegacy)
                {
                    var fileNameAttribute = this._xmlDocument.CreateAttribute("path");
                    fileNameAttribute.Value = record.FileName;
                    recordXml.Attributes.Append(fileNameAttribute);
                }
                root.AppendChild(recordXml);
            }

            foreach (var dataMapping in this.Require_StrongMapping)
            {
                var strong = this.Array_StrongValues[dataMapping.Item3];

                if (strong.Index == 0xFFFFFFFF)
                {
#if NONULL
                    dataMapping.Item1.ParentNode.RemoveChild(dataMapping.Item1);
#else
                    dataMapping.Item1.ParentNode.ReplaceChild(
                        this._xmlDocument.CreateElement("null"),
                        dataMapping.Item1);
#endif
                }
                else
                {
                    dataMapping.Item1.ParentNode.ReplaceChild(
                        this.DataMap[strong.StructType][(Int32)strong.Index],
                        dataMapping.Item1);
                }
            }

            foreach (var dataMapping in this.Require_WeakMapping1)
            {
                var weak = this.Array_WeakValues[dataMapping.Item3];

                var weakAttribute = dataMapping.Item1;

                if (weak.Index == 0xFFFFFFFF)
                {
                    weakAttribute.Value = String.Format("0");
                }
                else
                {
                    var targetElement = this.DataMap[weak.StructType][(Int32)weak.Index];

                    weakAttribute.Value = targetElement.GetPath();
                }
            }

            foreach (var dataMapping in this.Require_WeakMapping2)
            {
                var weakAttribute = dataMapping.Item1;

                if (dataMapping.Item2 == 0xFFFF)
                {
                    weakAttribute.Value = String.Format("0");
                }
                else
                {
                    var targetElement = this.DataMap[dataMapping.Item2][dataMapping.Item3];

                    weakAttribute.Value = targetElement.GetPath();
                }
            }
        }

        public void Save(String filename)
        {
            this.GenerateXML();

            foreach (var node in this._xmlDocument.DocumentElement.ChildNodes)
            {
                if (node is XmlElement)
                {
                    var element = node as XmlElement;

                    var newFile = String.Empty;

                    if (element.Attributes["path"] != null && !String.IsNullOrWhiteSpace(element.Attributes["path"].Value))
                    {
                        newFile = element.Attributes["path"].Value;
                    }
                    else
                    {
                        newFile = String.Format(@"{1}\{0}.xml", element.Name, Path.GetFileNameWithoutExtension(filename));
                    }

                    var newPath = Path.Combine(Path.GetDirectoryName(filename), newFile);
                    if (!Directory.Exists(Path.GetDirectoryName(newPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                    }
                    XmlDocument doc = new XmlDocument { };
                    doc.LoadXml(element.OuterXml);
                    doc.Save(newPath);
                }
            }
            this._xmlDocument.Save(filename);
        }

        public void GenerateSerializationClasses(String path = "AutoGen", String assemblyName = "HoloXPLOR.Data.DataForge")
        {
            path = new DirectoryInfo(path).FullName;

            if (Directory.Exists(path) && path != new DirectoryInfo(".").FullName)
            {
                Directory.Delete(path, true);
                while (Directory.Exists(path))
                {
                    Thread.Sleep(100);
                }
            }

            Directory.CreateDirectory(path);
            while (!Directory.Exists(path))
            {
                Thread.Sleep(100);
            }

            var sb = new StringBuilder();

            sb.AppendLine(@"using System.Xml.Serialization;");
            sb.AppendLine();
            sb.AppendFormat(@"namespace {0}", assemblyName);
            sb.AppendLine();
            sb.AppendLine(@"{");
            foreach (var enumDefinition in this.EnumDefinitionTable)
            {
                sb.Append(enumDefinition.Export());
            }
            sb.AppendLine(@"}");
            
            File.WriteAllText(Path.Combine(path, "Enums.cs"), sb.ToString());

            foreach (var structDefinition in this.StructDefinitionTable)
            {
                var code = structDefinition.Export(assemblyName);
                File.WriteAllText(Path.Combine(path, String.Format("{0}.cs", structDefinition.Name)), code);
            }
        }

        public void CompileSerializationAssembly(String assemblyName = "HoloXPLOR.Data.DataForge")
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                OutputAssembly = String.Format("{0}.dll", assemblyName),
            };

            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.dll");

            List<String> source = new List<String> { };

            var sb = new StringBuilder();

            sb.AppendLine(@"using System.Xml.Serialization;");
            sb.AppendLine();
            sb.AppendFormat(@"namespace {0}", assemblyName);
            sb.AppendLine();
            sb.AppendLine(@"{");
            foreach (var enumDefinition in this.EnumDefinitionTable)
            {
                sb.Append(enumDefinition.Export());
            }
            sb.AppendLine(@"}");

            source.Add(sb.ToString());

            foreach (var structDefinition in this.StructDefinitionTable)
            {
                var code = structDefinition.Export(assemblyName);
                source.Add(code);
            }

            var result = provider.CompileAssemblyFromSource(parameters, source.ToArray());
        }
    }
}
