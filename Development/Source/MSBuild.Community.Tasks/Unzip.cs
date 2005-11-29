// $Id$

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace MSBuild.Community.Tasks
{
    public class Unzip : Task
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Unzip"/> class.
        /// </summary>
        public Unzip()
        {

        }

        private string _zipFileName;

        /// <summary>
        /// Gets or sets the name of the zip file.
        /// </summary>
        /// <value>The name of the zip file.</value>
        [Required]
        public string ZipFileName
        {
            get { return _zipFileName; }
            set { _zipFileName = value; }
        }

        private string _targetDirectory;

        /// <summary>
        /// Gets or sets the target directory.
        /// </summary>
        /// <value>The target directory.</value>
        [Required]
        public string TargetDirectory
        {
            get { return _targetDirectory; }
            set { _targetDirectory = value; }
        }


        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            if (!File.Exists(_zipFileName))
            {
                Log.LogError("Zip File Not Found: {0}", _zipFileName);
                return false;
            }

            if (!Directory.Exists(_targetDirectory))
            {
                Directory.CreateDirectory(_targetDirectory);
            }
            
            FastZipEvents events = new FastZipEvents();
			events.ProcessDirectory = new ProcessDirectoryDelegate(ProcessDirectory);
			events.ProcessFile = new ProcessFileDelegate(ProcessFile);

            FastZip zip = new FastZip(events);
            zip.CreateEmptyDirectories = false;

            Log.LogMessage("Extracting Zip File \"{0}\"", _zipFileName);
            zip.ExtractZip(_zipFileName, _targetDirectory, null);

            return true;
        }

        private void ProcessFile(object sender, ScanEventArgs e)
        {
            Log.LogMessage("Extracted File \"{0}\"", e.Name);            
        }

        private void ProcessDirectory(object sender, DirectoryEventArgs e)
        {
            if (!e.HasMatchingFiles)
            {
                Log.LogMessage("Extracted Directory \"{0}\"", e.Name);
            }
        }
    }
}