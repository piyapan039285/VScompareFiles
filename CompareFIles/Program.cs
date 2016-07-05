using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompareFIles
{
    static class Program
    {
        static List<string> visualStudioList = new List<string>()
        {
            @"C:\Program Files (x86)\Microsoft Visual Studio 15.0\Common7\IDE\devenv.exe",
            @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe",
            @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe",
            @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe"
        };

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            string applicationPath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            string applicationFileName = Path.GetFileName(applicationPath);
            string sendToDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\Windows\SendTo");
            string sendToDestination = Path.Combine(sendToDirectoryPath, applicationFileName);

            if(Path.GetFullPath(applicationPath) != Path.GetFullPath(sendToDestination))
                File.Copy(applicationPath, sendToDestination, true);

            string visualStudioPath = null;
            foreach (string path in visualStudioList)
            {
                if (File.Exists(path))
                {
                    visualStudioPath = path;
                    break;
                }
            }

            if(visualStudioPath == null)
            {
                MessageBox.Show("Cannot find Visual Studio App.\nThis program support Visual Studio 2012-2015.", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string fileLeft = null;
            string fileRight = null;

            if (args.Length > 0)
                fileLeft = args[0];

            if (args.Length > 1)
                fileRight = args[1];


            if (string.IsNullOrEmpty(fileLeft))
                fileLeft = selectFile("Select first file");

            if (string.IsNullOrEmpty(fileLeft))
                return;

            if (string.IsNullOrEmpty(fileRight))
                fileRight = selectFile("Select file to compare", Path.GetDirectoryName(fileLeft));

            if (string.IsNullOrEmpty(fileRight))
                return;


            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = Path.GetDirectoryName(visualStudioPath);
            startInfo.FileName = visualStudioPath;
            
            startInfo.Arguments = string.Format(@"/diff ""{0}"" ""{1}"" ""SOURCE:{0}"" ""TARGET:{1}""", fileLeft, fileRight);
            Process p = Process.Start(startInfo);
            //p.WaitForExit();
        }

        static string selectFile(string title, string directory = null)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = false;
            openFileDialog1.Title = title;

            if(Directory.Exists(directory))
                openFileDialog1.InitialDirectory = directory;
            
            DialogResult dialogResult = openFileDialog1.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }

            return null;
        }
    }
}
