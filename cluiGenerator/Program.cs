// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;
using clui.New;


string projectDir = args[0];
string rootNameSpace = args[1];
string cmlFile = args[2];

var fileDirectory = Path.GetDirectoryName(cmlFile)!;


var directoryNs = string.Join(".",
  Path.GetRelativePath(projectDir, fileDirectory).Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries));



var x = fileDirectory.Substring(projectDir.Length);

var fileNs = (rootNameSpace + "." + directoryNs).Trim(".").ToString();
CreateUserFile(fileNs, cmlFile);
CreateDesignerFile(fileNs, cmlFile);


static void CreateDesignerFile(string ns, string sourceFileName)
{
  var designerFileName = sourceFileName + ".designer.cs";

  if (File.Exists(designerFileName))
    File.Move(designerFileName, designerFileName);

  var name = Path.GetFileNameWithoutExtension(sourceFileName);


  var windowElement = XDocument.Load(sourceFileName).Root!;
  var controls = Recursive(windowElement, null);


  StringBuilder propertyWriter = new StringBuilder();
  StringBuilder initWriter = new StringBuilder();
  StringBuilder addWriter = new StringBuilder();
  StringBuilder addWriter1 = new StringBuilder();

  foreach (var control in controls)
  {
    if (control.IsPublic)
    {
      propertyWriter.AppendLine("    public " + control.Type + " " + control.Name + " { get; } = new();");
    }
    else
    {
      initWriter.AppendLine("      " + control.Type + " "  + control.Name + " = new();");
    }

    if (control.Parent == null)
    {
      addWriter.AppendLine($"      this.Controls.Add({control.GetThisQualifiedName()});");
    }
    else
    {
      addWriter1.AppendLine($"      {control.Parent.GetThisQualifiedName()}.Controls.Add({control.GetThisQualifiedName()});");
    }
  }

  var content = $$"""
                   using global::clui.New;
                   
                   namespace {{ns}}
                   {
                     public partial class {{name}}: global::clui.New.Window
                     {
                   {{propertyWriter.ToString()}}
                       protected void Initialize()
                       {
                   {{initWriter}}
                   {{addWriter}}
                   {{addWriter1}}
                       }
                     }
                   }
                   """;

  File.WriteAllText(designerFileName, content);
}



static List<ControlPoco> Recursive(XElement element, ControlPoco? parent)
{
  List<ControlPoco> result = new List<ControlPoco>();
  foreach (var child in element.Elements())
  {
    var name = child.Attribute("Name")?.Value;
    var isPublic = !string.IsNullOrEmpty(name);

    if (string.IsNullOrWhiteSpace(name))
    {
      name = "ctrl_" + child.Name.LocalName.ToLower() + "_" + ControlPoco.GetControlCount(child);
    }

    switch (child.Name.LocalName)
    {
      case "Panel":
        ControlPoco panelPoco = new ControlPoco()
        {
          Name = name,
          IsPublic = isPublic,
          Type = "global::clui.New.Panel",
          Parent = parent
        };
        result.Add(panelPoco);

        result.AddRange(Recursive(child, panelPoco));

        break;

      case "Border":
        ControlPoco borderPoco = new ControlPoco()
        {
          Name = name,
          IsPublic = isPublic,
          Type = "global::clui.New.Border",
          Parent = parent
        };
        result.Add(borderPoco);

        result.AddRange(Recursive(child, borderPoco));

        break;

      case "Label":
        result.Add(new ControlPoco()
        {
          Name = name,
          IsPublic = isPublic,
          Parent = parent,
          Type = "global::clui.New.Label",
        });
        break;

      case "TextBox":
        result.Add(new ControlPoco()
        {
          Name = name,
          IsPublic = isPublic,
          Parent = parent,
          Type = "global::clui.New.TextBox",
        });
        break;
    }
  }

  return result;
}

static void CreateUserFile(string ns, string sourceFileName)
{
  var userFileName = sourceFileName + ".cs";

  if (File.Exists(userFileName))
  {
    File.Move(userFileName, userFileName);
    return;
  }


  var name = Path.GetFileNameWithoutExtension(sourceFileName);

  
  var content = $$"""
                using clui.New;

                namespace {{ns}}
                {
                  public partial class {{name}} : Window
                  {
                    public {{name}}()
                    {
                      Initialize();
                    }
                  }
                }
                """;

  File.WriteAllText(userFileName, content);
}


public class ControlPoco
{
  private static readonly ConcurrentDictionary<string, int> _controlCount = new();
  public static int GetControlCount(XElement element)
  {
    return _controlCount.AddOrUpdate(element.Name.LocalName, s => 1, (s, i) => i + 1);
  }

  public bool IsPublic { get; set; }
  public required string Name { get; init; }

  public required string Type { get; init; }

  public ControlPoco? Parent { get; init; }
  public List<ControlPoco> Controls { get; set; } = new();

  public string GetThisQualifiedName() => IsPublic ? "this." + Name : Name;
}