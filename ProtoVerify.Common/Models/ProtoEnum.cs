using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProtoVerify.Common.Models
{
    public class ProtoEnum : BaseProtoItem
    {
        public override string ProtoItemType => "Enum";

        public IList<ProtoEnumValue> Values { get; } = new List<ProtoEnumValue>();
    }

    public class ProtoEnumValue : BaseProtoItem
    {
        public override string ProtoItemType => "Enum Value";

        public int Number { get; set; }

        [JsonIgnore]
        public string OldName { get; set; }
    }
}
