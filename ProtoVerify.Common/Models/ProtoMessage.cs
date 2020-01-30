using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoVerify.Common.Models
{
    public class ProtoMessage : BaseProtoItem
    {
        public override string ProtoItemType => "Message";
        public IList<ProtoField> Fields { get; } = new List<ProtoField>();
        public IList<ProtoMessage> NestedMessages { get; } = new List<ProtoMessage>();
        public IList<ProtoEnum> NestedEnums { get; } = new List<ProtoEnum>();
    }
}
