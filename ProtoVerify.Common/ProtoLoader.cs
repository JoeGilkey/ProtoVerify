using System;
using System.IO;
using System.Collections.Generic;
using Google.Protobuf.Reflection;

namespace ProtoVerify.Common
{
    public class ProtoLoader
    {
        readonly string _protoDir;
        readonly bool _includeSubDirs;
        readonly List<FileDescriptorProto> _protoFiles = new List<FileDescriptorProto>();
        int _errorCount = 0;

        public ProtoLoader(string path, bool includeSubDirs)
        {
            if (!Directory.Exists(path))
            {
                throw new ArgumentOutOfRangeException();
            }

            _protoDir = path;
            _includeSubDirs = includeSubDirs;
        }

        public void ProcessProtos()
        {
            var set = new FileDescriptorSet
            {
                AllowNameOnlyImport = _includeSubDirs
            };

            set.AddImportPath(_protoDir);
            set.AddImportPath(Path.GetDirectoryName(typeof(ProtoLoader).Assembly.Location));

            SearchOption searchOption = _includeSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (string path in Directory.EnumerateFiles(_protoDir, "*.proto", searchOption))
            {
                set.Add(path, true);
            }

            set.Process();

            var errors = set.GetErrors();
            foreach (var err in errors)
            {
                if (err.IsError) _errorCount++;
                Console.Error.WriteLine(err.ToString());
            }

            if (_errorCount > 0) return;

            foreach (var file in set.Files)
            {
                _protoFiles.Add(file);
            }
        }

        public List<FileDescriptorProto> Files { get => _protoFiles; }
    }
}
