using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectApp_Quest
{
    public class ProjectFile
    {
        public string ProjectName { get; set; }
        public string ProjectFileName { get; set; }
        public string ProjectPath { get; set; }
        public string PathInput { get; set; }
        public string PathOutput { get; set; }
        public string Extension { get; set; }
        public string Content { get; set; }

        public DateTime Startd;
        public DateTime Endd;

        public ProjectFile()
        {
            
        }
        public ProjectFile(string projectName, string projectFileName, string pathInput, string pathOutput, DateTime startd, DateTime endd, string extension = ".xml", string path = "")
        {
            ProjectName = projectName;
            ProjectPath = path;
            ProjectFileName = projectFileName;
            PathInput = pathInput;
            PathOutput = pathOutput;
            Extension = extension;
            Startd = startd;
            Endd = endd;
        }

        public override string ToString()
        {
            return "Name: " + ProjectName;
        }
    }
}
