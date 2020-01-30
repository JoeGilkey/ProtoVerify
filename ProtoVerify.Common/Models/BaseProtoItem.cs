using System;
using Newtonsoft.Json;

namespace ProtoVerify.Common.Models
{
    public abstract class BaseProtoItem
    {
        public string Name { get; set; }

        [JsonIgnore]
        public ProtoItemState ItemState { get; set; }

        [JsonIgnore]
        public abstract string ProtoItemType { get; }
    }

    public enum ProtoItemState
    {
        Deleted,
        Existing,
        Added,
        Modified
    }
}
