using System;
using System.Collections.Generic;
using CommandLine;

using ProtoVerify.Common;

namespace ProtoVerify
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }

        static void HandleParseError(IEnumerable<Error> errors) { }

        static void RunOptions(Options opts)
        {
            ProtoLoader loader = new ProtoLoader(opts.Directory, opts.IncludeSubDirs);

            loader.ProcessProtos();

            Common.Models.ProtoContainer container = null;
            if (System.IO.File.Exists(opts.ContainerFile))
            {
                try
                {
                    container = Newtonsoft.Json.JsonConvert.DeserializeObject<Common.Models.ProtoContainer>(System.IO.File.ReadAllText(opts.ContainerFile));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error parsing {opts.ContainerFile} : {ex.Message}");
                    container = null;
                }
            }

            if (container == null)
            {
                container = new Common.Models.ProtoContainer();
            }

            ProtoProcessor processor = new ProtoProcessor(loader.Files, container);

            processor.ProcessFiles();

            if (System.IO.File.Exists(opts.ContainerFile))
                System.IO.File.Delete(opts.ContainerFile);

            System.IO.File.WriteAllText(opts.ContainerFile, Newtonsoft.Json.JsonConvert.SerializeObject(container, Newtonsoft.Json.Formatting.Indented));

            var changes = processor.ProcessForChanges();

            System.IO.File.WriteAllText(opts.OutputFile, string.Join(Environment.NewLine, changes));
        }
    }
}
