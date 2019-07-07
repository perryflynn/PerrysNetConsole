A collection of C# classes to create fancy console applications.

- Prompt / Multiple Choice Prompt
- Progressbar / Progressbar with status messages
- Loading indicator
- Graph
- Timeout counter
- Scrolltext (works only on windows)
- Grid system for text output
- Automatic text wrapping inside of grid columns
- Tables
- Bordered tables
- Dynamic coloring and alignment with delegates

## Install with nuget

- Full .NET standard support
- Workarounds for operating system specific functions

```
Install-Package PerrysNetConsole -Version 2.1.0
dotnet add package PerrysNetConsole --version 2.1.0
```

[nuget package site](https://www.nuget.org/packages/PerrysNetConsole)

## Demo

![Demo image](./doc/Demo-2019-07-07-17-31.gif)

## Demo Application

The code includes a demo application.

- Clone the repo: `git clone https://github.com/perryflynn/PerrysNetConsole.git`
- Create a new solution for the projects `PerrysNetConsole` and `Demo`
- Add `PerrysNetConsole` as reference to `Demo`
- Set `Demo` as startup project
- Run Demo

See [the demo application](./Demo/Program.cs) for more details.
