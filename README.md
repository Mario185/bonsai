# bonsai

A simple cli tool that provides a better 'change directory' and file execution experience.

Also a user interface for explorer like navigation is included (themeable).

![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/Mario185/bonsai/total?logo=github&style=flat-squared)
![GitHub License](https://img.shields.io/github/license/Mario185/bonsai)
![GitHub Release](https://img.shields.io/github/v/release/Mario185/bonsai?include_prereleases)

1. [Installation](#installation)
2. [How to use](#how-to-use) 
3. [Configuration](#configuration)


## Installation

Currently only tested on Windows with PowerShell and Windows Terminal.

1. Install binary

    #### Windows

    The recommended way to install bonsai is via winget.
    ```
    winget install donmar.bonsai
    ```
1. Setup your shell
    #### PowerShell
     
    Add this to your powershell profile (Run `echo $profile` in PowerShell to locate it.):

   ```pwsh
    Invoke-Expression (& { (bonsai init powershell | Out-String) })
    ```
    This will execute the contents of [init.ps1](#init_ps1)

    Note: For already running powershell sessions you have to reload your profile after modifying it. You can do this by executing this 
    ```pwsh
    . $PROFILE
    ```
## How to use

Asume that we have this directory structure and our working directory is `C:`

```pwsh
C:
├─tempor
│ ├─consetetur
│ │  └─sadipscing
│ │      elitr.txt
│ │      amet.ps1
│ │
│ ├─nonumy
│ └─lorem
│   └─ipsum
│     │  amet.ps1
│     │
│     ├─dolor
│     └─nonumy
├-invidunt 
└-ipsum
```

```pwsh
b                     # Opens the explorer app at the current working directory
b tempor/consetetur   # Changes directory to "C:\tempor\consetetur" 
                      # because tempor/consetetur is a subdirectory of C:\ 

# We assume that bonsai have already been used and the database
b nonumy              # will open an UI aksing which of the two nonumy folders you ment
b tem sum             # will change directory to "C:\tempor\lorem\ipsum"
b scing amet.ps1      # will execute "C:\tempor\consetetur\sadipscing\amet.ps1"
```
Everytime you use bonsai to navigate or execute a file this will be stored in the database.

### Default Key Bindings

Key bindings can be modified in [settings.json](#settings_json)

#### common

| Key        | Action              |
| ---------- | ------------------- |
| Esc | Close |
| UpArrow | select previous item in list |
| DownArrow | select next item in list |
| PageUp | select item one page up in list |
| PageDown| select item one page down in list |
| ctrl+Home | select first item in list |
| ctrl+End | select last item in list |

#### explorer

| Key        | Action              |
| ---------- | ------------------- |
| Enter | open directory in explor |
| ctrl+Enter | Select directory/File and exit explorer |
| ctrl+I     | Load sub directories and files from sub directory (! This could take a bit)|
| ctrl+R     | toggle regex search | 
| alt+UpArror | open parent directory |

#### multiple selection ui

| Key        | Action              |
| ---------- | ------------------- |
| Enter | Select directory/File and exit  |

## Configuration

The configuration is stored in

Windows: %userprofile%\\.bonsai

Contents of the folder are:


* <a id="settings_json"></a>settings.json
 
  | Property   | Values              | Description |
  | ---------- | ------------------- | ------- |
  | showParentDirectoryInList | true/false | true to shwo the parent directory in explorer as ".." |
  | theme | e.g default.json, justicons.json | The theme file to use which has to be located in the [themes](#themes-folder) folder |
  | maxEntryAgeInDays| integer | The amount of days after which an entry in the database should be removed.| 
  | maxIndividualScore| integer | The max score an entry should be able to get. If any entry exceeds this value, scores of all entries will be reduced and all falling below 1 are getting deleted. |
  | keyBindings | Key: [KeyBindingContext](#keybindingcontext), Value: List\<[KeyBinding](#keybinding)\> | The keybindings to use |
  | fileCommands | List\<[FileCommand](#filecommand)\> | commands that should be used when selecting files. When not set, the shell decides what to do with file |
  | directoryCommands | List\<[DirectoryCommand](#directorycommand)\> | commands that should be used when selecting directories. When not set change directory to will be executed <br>If you add a directoryCommand you might want to register also the the change directory command for powershell: <br>`{"action": "Set-Location \"[path]\"", "displayName": "Change directory"}` |

  <a id="keybindingcontext"></a>KeyBindingContext:
    ```
    Common,
    ExplorerApp,
    NavigationApp
   ```

  <a id="filecommand"></a>FileCommand
  
  | Property   | Description |
  | ---------- | ------- |
  | extension | the extension (include the . e.g ".ps1") for which the command should be available. "\*" = all files |
  | action | the expression which should be executed =="[path]"== will be replaced with the full qualified name of the file. eg: "code \"[path]\"" will open Visual Studio Code with the given file |
  | displayName | the name which should be displayed if more than one command is available|
  
  <a id="directorycommand"></a>DirectoryCommand
  
  | Property   | Description |
  | ---------- | ------- |
  | action | the expression which should be executed =="[path]"== will be replaced with the full qualified name of the directory. eg: "ii \"[path]\"" will open the windows explorer |
  | displayName | the name which should be displayed if more than one command is available|
   
   <a id="keybinding"></a>KeyBinding:
  
  
  | Property   | Values              | Description |
  | ---------- | ------------------- | ------- |
  | modifier| [ConsoleModifiers](https://learn.microsoft.com/en-us/dotnet/api/system.consolemodifiers?view=net-9.0#fields) | Use "None" if no modifier should be used |
  | key| [ConsoleKeys](https://learn.microsoft.com/en-us/dotnet/api/system.consolekey?view=net-9.0#fields) | Either key or keyChar have to be given (both is fine to) |
  | keyChar| char representing the key | Either key or keyChar have to be given (both is fine to) |
  | action| None<br>Exit<br>OpenDirectory<br> OpenParentDirectory<br> ConfirmSelection<br>ListSelectPreviousItem<br> ListSelectNextItem<br> ListSelectOnePageUp<br> ListSelectOnePageDown<br> ListSelectFirstItem<br> ListSelectLastItem<br> ToggleShowDetailsPanel<br> ToggleIncludeSubDirectories<br> ToggleRegexSearch ||


* <a id="init_ps1"></a>init.ps1

    This is the script which will be executed when `bonsai init [shell]` is executed.
* db.json
* <a id="themes-folder"></a>themes
  * default.json
  * justicons.json
