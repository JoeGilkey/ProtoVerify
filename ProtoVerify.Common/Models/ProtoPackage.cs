using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoVerify.Common.Models
{
    public class ProtoPackage: BaseProtoItem
    {
        public override string ProtoItemType => "Package";

        public IList<ProtoEnum> Enums { get; } = new List<ProtoEnum>();
        public IList<ProtoMessage> Messages { get; } = new List<ProtoMessage>();
    }
}
