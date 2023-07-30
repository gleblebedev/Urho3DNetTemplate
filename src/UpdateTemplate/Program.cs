using System.Text;
using System.Xml;
using System.Xml.Linq;

public static class Program
{
    static readonly string ns = "http://schemas.microsoft.com/developer/vstemplate/2005";

    public static void Main()
    {
        var projects = System.IO.Directory.GetDirectories("../../../../../Urho3DNetTemplate", "*");
        foreach (var templatePath in projects)
        {
            var projectName = Path.GetFileName(templatePath);
            Console.WriteLine(projectName);

            var templateFileName = Directory.GetFiles(templatePath, "*.vstemplate").FirstOrDefault();
            var xml = XDocument.Load(templateFileName);
            var project = xml.Element(XName.Get("VSTemplate", ns))
                .Element(XName.Get("TemplateContent", ns))
                .Element(XName.Get("Project", ns));

            project.RemoveNodes();

            Populate(project, templatePath);

            xml.Save(templateFileName);
        }
    }

    private static void Populate(XElement project, string templatePath)
    {
        foreach (var file in Directory.GetFiles(templatePath))
        {
            var fileName = Path.GetFileName(file);
            if (fileName == "__TemplateIcon.ico")
                continue;
            if (fileName.EndsWith(".user.json"))
                continue;
            var replaceParameters = false;
            var ext = Path.GetExtension(file).ToLower();
            switch (ext)
            {
                case ".csproj":
                case ".vstemplate":
                    continue;
                case ".cs":
                case ".xml":
                case ".appxmanifest":
                    replaceParameters = true;
                    break;
            }
            project.Add(new XElement(XName.Get("ProjectItem", ns),
                new XAttribute(XName.Get("ReplaceParameters"), replaceParameters.ToString().ToLower()),
                new XAttribute(XName.Get("TargetFileName"), fileName),
                new XText(fileName)
                ));

            if (ext == ".cs")
            {
                var cs = File.ReadAllText(file);
                for (;;)
                {
                    var res = cs.Replace("Urho3DNetTemplate", "$ext_safeprojectname$");
                    if (res == cs)
                        break;
                    cs = res;
                }
                File.WriteAllText(file, cs, new UTF8Encoding(false));
            }
        }
        foreach (var dir in Directory.GetDirectories(templatePath))
        {
            var dirName = Path.GetFileName(dir);
            if (dirName == "bin" || dirName == "obj")
                continue;
            var folderElement = new XElement(XName.Get("Folder", ns),
                new XAttribute(XName.Get("Name"), dirName), new XAttribute(XName.Get("TargetFolderName"), dirName));
            project.Add(folderElement);
            Populate(folderElement, dir);
        }
    }
}