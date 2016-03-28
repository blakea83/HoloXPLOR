﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoloXPLOR.DataForge
{
    public class DataForgeDataMapping : _DataForgeSerializable
    {
        public UInt16 StructIndex { get; set; }
        public UInt16 StructCount { get; set; }
        public UInt32 NameOffset { get; set; }
        public String Name { get { return this.DocumentRoot.ValueMap[this.NameOffset]; } }

        public DataForgeDataMapping(DataForge documentRoot)
            : base(documentRoot)
        {
            this.StructCount = this._br.ReadUInt16();
            this.StructIndex = this._br.ReadUInt16();
            this.NameOffset = documentRoot.StructDefinitionTable[this.StructIndex].NameOffset;
        }

        public override String ToString()
        {
            return String.Format("0x{1:X4} {2}[0x{0:X4}]", this.StructCount, this.StructIndex, this.Name);
        }
    }
}
