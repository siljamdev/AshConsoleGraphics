# AshConsoleGraphics
<img src="res/icon.png" width="200"/>
UIs and user interfaces in your console!

## Installation
To add it to your project, do `dotnet add package ashConsoleGraphics --version 2.2.5`

## Usage
There are Elements that have a graphical buffer, and screens hold these elements and display them. Screens are elements too, allowing for nesting.  
Additionally, there are selectable elements for using with interactive screens. These are buttons, option pickers and even text input.

## Colors & CharFormat
The Color struct used is from the library [AshLib](https://github.com/siljamdev/AshLib)  
Also, the CharFormat is used, that allows for bold, italic, strike-through, underlined and colored text, with ANSI escape sequences. This class is too from the library [AshLib](https://github.com/siljamdev/AshLib)

## NO_COLOR
Because this library uses [AshLib](https://github.com/siljamdev/AshLib), it too supporsts NO_COLOR environment variable. Read more [here](https://no-color.org/)  

## Documentation and Examples
There is a very complete API documentation and tutorials made with DocFx available [here](https://siljamdev.github.io/AshConsoleGraphics/index.html)  
There are also examples in [here](./examples)