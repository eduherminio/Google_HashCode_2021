﻿using GoogleHashCode;

if (args.Length == 0)
{
    const string inputFileName = "f.txt";
    args = new[] { inputFileName };
}

foreach (var inputFileName in args)
{
    new Solver(inputFileName).Solve();
}
