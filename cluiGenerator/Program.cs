// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;
using clui.New;

CreateUserFile(args[0]);
CreateDesignerFile(args[0]);


static void CreateDesignerFile(string sourceFileName)
{
  var designerFileName = sourceFileName + ".designer.cs";

  if (File.Exists(designerFileName))
    File.Move(designerFileName, designerFileName);

  var name = Path.GetFileNameWithoutExtension(sourceFileName);


  var windowElement = XDocument.Load(sourceFileName).Root!;
  var controls = Recuresive(windowElement, null);


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
      addWriter.AppendLine($"      this.Controls.Add({control.Name});");
    }
    else
    {
      addWriter1.AppendLine($"      {control.Parent.Name}.Controls.Add({control.Name});");
    }
  }

  var content = $$"""
                   using global::clui.New;
                   
                   namespace consoleToolsTestApp
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



static List<ControlPoco> Recuresive(XElement element, ControlPoco? parent)
{
  List<ControlPoco> result = new List<ControlPoco>();
  foreach (var child in element.Elements())
  {
    var name = child.Attribute("Name")?.Value;
    var isPublic = !string.IsNullOrEmpty(name);

    if (string.IsNullOrWhiteSpace(name))
    {
      name = child.Name.LocalName.ToLower() + "_" + ControlPoco.controlNumberCounter++;
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

        result.AddRange(Recuresive(child, panelPoco));

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

        result.AddRange(Recuresive(child, borderPoco));

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

static void CreateUserFile(string sourceFileName)
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

                namespace consoleToolsTestApp
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
  public static int controlNumberCounter = 0;
  public bool IsPublic { get; set; }
  public required string Name { get; init; }

  public required string Type { get; init; }

  public ControlPoco? Parent { get; init; }
  public List<ControlPoco> Controls { get; set; } = new();
}