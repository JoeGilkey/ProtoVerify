using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoVerify.Common.Models
{
    public class ProtoContainer
    {
        public List<ProtoPackage> Packages { get; } = new List<ProtoPackage>();
    }
}
