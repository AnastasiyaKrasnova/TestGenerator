using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.IO;
using TestGeneratorDll;

namespace TestsGeneratorConsole
{
    public class Pipeline
    {
        private List<string> _filesfortest;
        private string _testfolder;
        private int _maxThreads;
        private int _maxFilesToRead;
        private int _maxFilesToWrite;
        private TestGenerator _generator;

        public Pipeline(TestGenerator generator,List<string> files, string folder, int maxThreads, int maxFilesToRead, int maxFilesToWrite)
        {
            _generator = generator;
            _filesfortest = files;
            _testfolder = folder;
            _maxThreads = maxThreads;
            _maxFilesToRead = maxFilesToRead;
            _maxFilesToWrite = maxFilesToWrite;
        }

        public async Task GenerateAsync()
        {
            /*var readFile = new TransformBlock<string, string>(async path =>
            {
                string fileContent;
                using (StreamReader reader = File.OpenText(path))
                {
                    fileContent = await reader.ReadToEndAsync();
                }
                return fileContent;

            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _maxFilesToRead
            });

            var getTest = new TransformBlock<string, List<TestFile>>(async source =>
            {
                await Task.Run(() => _generator.Generate(source));

            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _maxThreads
            });


            var writeResult = new ActionBlock<List<TestFile>>(async files =>
            {
                foreach (var file in files)
                {
                    using (StreamWriter writer = File.CreateText(file.FileName))
                    {
                        await writer.WriteAsync(file.Code);
                    }
                }
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _maxFilesToWrite
            });*/


            var readFile = new TransformBlock<string, string>
                (
                    async path => await File.ReadAllTextAsync(path),
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = _maxFilesToRead
                    }
                );
            var getTest = new TransformManyBlock<string, TestFile>
                (
                    async sourceCode => await Task.Run(() => _generator.Generate(sourceCode)),
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = _maxThreads
                    }
                );
            var writeResult = new ActionBlock<TestFile>
                (
                    async filesContent =>
                    {
                        await File.WriteAllTextAsync(Path.Combine(_testfolder, filesContent.FileName), filesContent.Code);

                    },
                   new ExecutionDataflowBlockOptions
                   {
                       MaxDegreeOfParallelism = _maxFilesToWrite
                   }
                );

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            readFile.LinkTo(getTest, linkOptions);
            getTest.LinkTo(writeResult, linkOptions);
            foreach (string file in _filesfortest)
            {
                readFile.Post(file);
            }
            readFile.Complete();

            await writeResult.Completion;
        }
    }
}
