using System;
using System.Linq;
using System.Collections.Generic;
using Google.Protobuf.Reflection;

using ProtoVerify.Common.Models;

namespace ProtoVerify.Common
{
    public class ProtoProcessor
    {
        private readonly List<FileDescriptorProto> _files;
        private readonly ProtoContainer _container;

        public ProtoProcessor(List<FileDescriptorProto> files, ProtoContainer container)
        {
            _files = files;
            _container = container;
        }

        public void ProcessFiles()
        {
            foreach (var file in _files)
            {
                ProcessFile(file);
            }
        }

        public IEnumerable<string> ProcessForChanges()
        {
            List<string> changes = new List<string>();

            changes.Add("******** ADDED ********");
            ProcessItemsForChanges(_container.Packages, changes, ProtoItemState.Added);

            changes.Add("******** MODIFIED ********");
            ProcessItemsForChanges(_container.Packages, changes, ProtoItemState.Modified);

            changes.Add("******** DELETED ********");
            ProcessItemsForChanges(_container.Packages, changes, ProtoItemState.Deleted);

            return changes;
        }

        private string MakeChangeName(string itemType, string itemName, string parentStr)
        {
            if (string.IsNullOrWhiteSpace(parentStr))
                return $"{itemType}:{itemName}";
            else
                return $"{parentStr}|{itemType}:{itemName}";
        }
        private void ProcessItemsForChanges(IEnumerable<BaseProtoItem> items, IList<string> changes, ProtoItemState state, string parentStr = null)
        {
            foreach (var item in items)
            {
                string changeStr = MakeChangeName(item.ProtoItemType, item.Name, parentStr);
                if (item.ItemState == state)
                {
                    changes.Add($"{state} : {changeStr}");
                }

                if (item is ProtoEnum protoEnum)
                {
                    ProcessItemsForChanges(protoEnum.Values, changes, state, changeStr);
                }

                if (item is ProtoMessage protoMessage)
                {
                    ProcessItemsForChanges(protoMessage.NestedEnums, changes, state, changeStr);
                    ProcessItemsForChanges(protoMessage.NestedMessages, changes, state, changeStr);
                }

                if (item is ProtoPackage protoPackage)
                {
                    ProcessItemsForChanges(protoPackage.Enums, changes, state, changeStr);
                    ProcessItemsForChanges(protoPackage.Messages, changes, state, changeStr);
                }
            }
        }

        private void ProcessFile(FileDescriptorProto file)
        {
            foreach (var extInfo in file.Extensions)
            {
                ProcessExtensions(file.Package, extInfo);
            }

            foreach (var enumInfo in file.EnumTypes)
            {
                ProcessEnum(file.Package, enumInfo);
            }

            foreach (var msgInfo in file.MessageTypes)
            {
                ProcessMessage(file.Package, msgInfo);
            }

            foreach (var svcInfo in file.Services)
            {
                ProcessService(file.Package, svcInfo);
            }

            Console.WriteLine($"{file.Package} : Done processing {file.Name}");
        }

        private ProtoItemState GetUpdatedState(ProtoItemState curState, ProtoItemState newState)
        {
            if (curState == ProtoItemState.Deleted || curState == newState)
                return newState;

            if (curState == ProtoItemState.Added)
                return curState;

            if (curState == ProtoItemState.Modified)
                return curState;

            return newState;
        }

        private ProtoPackage GetPackage(string packageName)
        {
            ProtoPackage rslt = _container.Packages.FirstOrDefault(p => string.Equals(p.Name, packageName, StringComparison.InvariantCultureIgnoreCase));
            if (rslt == null)
            {
                rslt = new ProtoPackage { Name = packageName, ItemState = ProtoItemState.Added };
                _container.Packages.Add(rslt);
            }
            else
            {
                rslt.ItemState = GetUpdatedState(rslt.ItemState, ProtoItemState.Existing);
            }
            return rslt;
        }
        private ProtoEnum GetEnum(IList<ProtoEnum> protoEnums, string enumName)
        {
            ProtoEnum rslt = protoEnums.FirstOrDefault(e => string.Equals(e.Name, enumName, StringComparison.InvariantCultureIgnoreCase));
            if (rslt == null)
            {
                rslt = new ProtoEnum { Name = enumName, ItemState = ProtoItemState.Added };
                protoEnums.Add(rslt);
            }
            else
            {
                rslt.ItemState = GetUpdatedState(rslt.ItemState, ProtoItemState.Existing);
            }
            return rslt;
        }
        private void CheckEnumValue(ProtoEnum protoEnum, string valName, int valNum)
        {
            ProtoEnumValue val = protoEnum.Values.FirstOrDefault(v => v.Number == valNum);

            if (val == null)
            {
                val = new ProtoEnumValue { Number = valNum, Name = valName, ItemState = ProtoItemState.Added };
                protoEnum.Values.Add(val);
            }
            else
            {
                if (string.Equals(val.Name, valName, StringComparison.InvariantCultureIgnoreCase))
                {
                    val.ItemState = GetUpdatedState(val.ItemState, ProtoItemState.Existing);
                }
                else
                {
                    val.ItemState = GetUpdatedState(val.ItemState, ProtoItemState.Modified);
                    val.OldName = val.Name;
                    val.Name = valName;
                }
            }
        }

        private ProtoMessage GetMessage(IList<ProtoMessage> protoMessages, string msgName)
        {
            ProtoMessage msg = protoMessages.FirstOrDefault(m => string.Equals(m.Name, msgName, StringComparison.InvariantCultureIgnoreCase));
            if (msg == null)
            {
                msg = new ProtoMessage { Name = msgName, ItemState = ProtoItemState.Added };
                protoMessages.Add(msg);
            }
            else
            {
                msg.ItemState = GetUpdatedState(msg.ItemState, ProtoItemState.Existing);
            }
            return msg;
        }
        private void CheckFieldValue(ProtoMessage protoMsg, FieldDescriptorProto fld)
        {
            ProtoField protoFld = protoMsg.Fields.FirstOrDefault(f => f.Number == fld.Number);
            if (protoFld == null)
            {
                protoFld = new ProtoField { Number = fld.Number, Name = fld.Name, FieldLabel = (int)fld.label, FieldType = (int)fld.type, FieldTypeName = fld.TypeName, ItemState = ProtoItemState.Added };
                protoMsg.Fields.Add(protoFld);
            }
            else
            {
                if (string.Equals(fld.Name, protoFld.Name, StringComparison.InvariantCultureIgnoreCase) && string.Equals(fld.TypeName, protoFld.FieldTypeName, StringComparison.InvariantCultureIgnoreCase) && (int)fld.label == protoFld.FieldLabel && (int)fld.type == protoFld.FieldType)
                {
                    protoFld.ItemState = GetUpdatedState(protoFld.ItemState, ProtoItemState.Existing);
                }
                else
                {
                    protoFld.ItemState = GetUpdatedState(protoFld.ItemState, ProtoItemState.Modified);
                    protoFld.OldName = protoFld.Name;
                    protoFld.OldFieldType = protoFld.FieldType;
                    protoFld.OldFieldTypeName = protoFld.FieldTypeName;
                    protoFld.OldFieldLabel = protoFld.FieldLabel;

                    protoFld.Name = fld.Name;
                    protoFld.FieldType = (int)fld.type;
                    protoFld.FieldTypeName = fld.TypeName;
                    protoFld.FieldLabel = (int)fld.label;
                }
            }
        }


        private void ProcessEnum(string packageName, EnumDescriptorProto enumDescriptor) => ProcessEnum(GetPackage(packageName).Enums, enumDescriptor);
        private void ProcessEnum(IList<ProtoEnum> protoEnums, EnumDescriptorProto enumDescriptor)
        {
            ProtoEnum protoEnum = GetEnum(protoEnums, enumDescriptor.Name);
            foreach (var val in enumDescriptor.Values)
            {
                CheckEnumValue(protoEnum, val.Name, val.Number);
            }
        }
        private void ProcessMessage(string packageName, DescriptorProto descriptorProto) => ProcessMessage(GetPackage(packageName).Messages, descriptorProto);
        private void ProcessMessage(IList<ProtoMessage> protoMessages, DescriptorProto descriptorProto)
        {
            ProtoMessage protoMessage = GetMessage(protoMessages, descriptorProto.Name);

            foreach (var enumDescriptor in descriptorProto.EnumTypes)
            {
                ProcessEnum(protoMessage.NestedEnums, enumDescriptor);
            }

            foreach (var fld in descriptorProto.Fields)
            {
                CheckFieldValue(protoMessage, fld);
            }

            foreach (var nested in descriptorProto.NestedTypes)
            {
                ProcessMessage(protoMessage.NestedMessages, nested);
            }
        }

        private void ProcessExtensions(string packageName, FieldDescriptorProto extensionDescriptor) => Console.WriteLine($"{packageName}: Extension: {extensionDescriptor.Name}");
        private void ProcessService(string packageName, ServiceDescriptorProto serviceDescriptor) => Console.WriteLine($"{packageName}: Service: {serviceDescriptor.Name}");
    }
}
