using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProtoVerify.Common.Models
{
    public class ProtoField : BaseProtoItem
    {
        public override string ProtoItemType => "Field";

        public int Number { get; set; }
        public int FieldType { get; set; }
        public string FieldTypeName { get; set; }
        public int FieldLabel { get; set; }

        [JsonIgnore]
        public string OldName { get; set; }

        [JsonIgnore]
        public int OldFieldType { get; set; }

        [JsonIgnore]
        public string OldFieldTypeName { get; set; }

        [JsonIgnore]
        public int OldFieldLabel { get; set; }
    }
}
