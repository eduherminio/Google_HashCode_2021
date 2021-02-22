using FileParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleRunner
{
    public class Solver
    {
        private readonly string _inputFile;
        private readonly string _outputFile;

        public Solver(string inputFileName)
        {
            _inputFile = Path.Combine("Inputs", inputFileName);
            _outputFile = Path.Combine("Outputs", $"output_{inputFileName}");

            Directory.CreateDirectory("Outputs");
        }

        public void Solve()
        {
            ParseInput();

            PrintResult();
        }

        private void ParseInput()
        {
            foreach (var line in new ParsedFile(_inputFile))
            {
                Console.WriteLine(line.ToSingleString());
            }
        }

        private void PrintResult()
        {
            File.WriteAllText(_outputFile, "text");
        }
    }
}
