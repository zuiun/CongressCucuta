# Congress of Cúcuta

Government simulation game/slideshow program, made for nostalgic reasons.

## Installation

Congress of Cúcuta uses [.NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0/runtime).
Whenever 10.0 comes out, it will use that instead.
I have yet to create an `.exe`, as Congress of Cúcuta is not ready for release.

## Usage

Run the provided `.exe` (whenever it is released) and follow the directions.
Congress of Cúcuta requires a `.sim` file to run.
Several are available in this repository and can be found in the `Simulations` folder.

## Creation

If you can decipher the `.sim` files, then you could theoretically write a simulation from scratch.
However, realistically, you should download the source code and make some changes in your preferred C#/.NET editor:
- Create an `ISimulation` of your own.
Some examples are found in the `Simulations` folder.
- `App.xaml` contains the entry point for the program.
This needs to be changed to `Views/CompilerWindow.xaml`.
- `ViewModels/CompilerViewModel.cs` stores the simulation in question.
Change the `Simulation` field to point to your `ISimulation`.
- Build and run the project. The program will ask you to choose a file to save as `.sim`.
    - If the build fails, then this is likely an issue with your `ISimulation`.
    `Simulation` is an extremely rigid data structure that has several constraints built in.
    Ensure that you meet all of them before trying again.
    - If the build succeeds, then you can now run the compiled `.sim` using the (theoretically) provided `.exe`.
