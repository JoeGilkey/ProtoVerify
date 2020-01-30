using System;
using System.Collections.Generic;

using CommandLine;

namespace ProtoVerify
{
    public class Options
    {
        [Option('d', "dir", Required = true, HelpText = "Directory containing the proto files.")]
        public string Directory { get; set; }

        [Option('s', "subdirs", Default = true, HelpText = "Include sub-directories when loading proto files.")]
        public bool IncludeSubDirs { get; set; }

        [Option('c', "container", Default = "ProtoVerifyDefs.json", HelpText = "File containing the current proto state.")]
        public string ContainerFile { get; set; }

        [Option('o', "output", Default = "changes.txt", HelpText = "File to output the change log to.")]
        public string OutputFile { get; set; }
    }
}
