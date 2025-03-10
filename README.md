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

Key bindings can be modified in [settings.json](#configuration)

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