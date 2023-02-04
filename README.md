# Undebugger

Utility tool that integrates into your game and provides valuable options to debug and test on any device. 

<div>
<img src="https://github.com/vape/Undebugger/blob/screenshots/Screenshots/1.jpg?raw=true" alt="Commands tab" width="32%"/>
<img src="https://github.com/vape/Undebugger/blob/screenshots/Screenshots/2.jpg?raw=true" alt="Log tab" width="32%"/>
<img src="https://github.com/vape/Undebugger/blob/screenshots/Screenshots/3.jpg?raw=true" alt="Status tab" width="32%"/>
</div>

## Features

### Commands

It's a way to modify your game behavior without rebuilding right on the target device. Basically - cheats. Currently, supported command types are:

#### Action Command

Simply executes the given delegate

#### Carousel Command

Allows you to scroll between multiple options

#### Dropdown Command

Dropdown menu with multiple option selections

#### Text Command

Provides input field to pass typed value parameter into given delegate

#### Toggle Command

On/off switch for a given option

### Log

Shows you the full application log right on the target device. Has filtering masks for every type of log message. Each message could be opened into an expanded view with stacktrace and option to email it.

### Status

Contains a frametime graph with an option to pin it on screen for profiling, memory usage info, and arbitrary text segments with any info you wish to have a glance time to time. There is 3 segments included by default: **System Info**, **Application Info** and **Device Info**

## Install

This repository is itself a unity package so the easiest way to install it is to simply add it as a git package into your game via Package Manager. You can as well just download and copy-paste paste entire repository into your assets folder or use any other preferred way to install it.

## Usage

By default, undebugger is included in every debug/development build. You can include it in release build by adding `UNDEBUGGER` to compilation symbols list. Also, you can exclude it from debug build with 'UNDEBUGGER_DISABLE'.

### Extending menu

Every time menu is opened, it builds a model which consists of every modifiable property of the menu. For now, it's two: Commands and Statuses. You can hook your code into model building process in a few ways:

1. Any components present on the scene derived from `MonoBehaviour` and implementing `IDebugMenuHandler` will have `OnBuildingModel` method called every time menu is opened. From here, you can modify the menu model as much as you want.
2. All *static* classes as well as all components present on scene derived from `MonoBehaviour` and having `UndebuggerTarget` attribute will be also processed:
    * Properties with `UndebuggerToggle` attribute will be added as toggle commands
    * Properties with `UndebuggerDropdown` attribute will be added as dropdown commands
    * Properties with `UndebuggerCarousel` attribute will be added as carousel commands
    * Methods with `UndebuggerAction` attribute will be added as action commands
    * Methods with `UndebuggerMenuHandler` attribute will be called the same way as `OnBuildingModel` is.

The commands tab consists of pages, page consists of segments, segment consists of commands. Here is an example of how to add commands:

```C#
[UndebuggerTarget]
public static class GraphicsSettings
{
    // attribute syntax

    [UndebuggerToggle]
    [Preserve]
    public static bool ShadowsEnabled
    { get; set; }

    [UndebuggerMenuHandler]
    [Preserve]
    private static void PopulateDebugMenu(MenuModel model)
    {
        // chain syntax

        model
            .WithPage("Graphics")
                .WithMainSegment()
                    .AddCommand(DropdownCommandModel.Create(
                        "Target Frame Rate", 
                        new int[] { 30, 60, 90, 120 }, 
                        Application.targetFrameRate, 
                        (value) => Application.targetFrameRate = value))
                    .AddCommand(CarouselCommandModel.Create(
                        "VSync Count", 
                        new int[] { 0, 1, 2 }, 
                        QualitySettings.vSyncCount, 
                        (value) => QualitySettings.vSyncCount = value));
    }
}

public class GameLogic : MonoBehaviour, IDebugMenuHandler
{
    // ...

    public void OnBuildingModel(MenuModel model)
    {
        // basic syntax

        var page = model.Commands.FindOrCreatePage("Game", priority: 1000);
        var segment = page.FindOrCreateSegment("Economy", priority: 500);
        segment.Commands.Add(new ActionCommandModel("Give Coins Reward", () => { /* ... */ }));
        segment.Commands.Add(new IntTextCommandModel("Add Lives", (numberOfLivesToAdd) => { /* ... */ }));

        // chain syntax

        model
            .WithPage("Social")
                .WithSegment("Chat")
                    .AddCommand(new TextCommandModel("Send Message", SendMessageToFriend));

    }

    private void SendMessageToFriend(string message)
    {
        // ...
    }
}
```

### Triggers

A menu can be triggered with default triggers:

* **F1 key** to open/close
* When menu is open, you can close it with **Escape key**
* On mobile device, single **tap with four fingers** with open or close menu

Additionally, you can add your own triggers by calling `MenuTriggerService.RegisterTrigger`
