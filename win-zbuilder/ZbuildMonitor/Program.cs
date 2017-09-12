using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ContainerNetworkManagement.Logger;
using System.Diagnostics;
using System.Management;
using System.Threading;
using Amazon.S3.Model;
using Amazon.S3;
using System.Reflection;

namespace ZbuildMonitor
{
    class Program
    {
        private static string BATCH_FILE_NAME = "zbuild.bat";
        private static string LOCAL_ZBUILD_LOG_FILE = "\\zbuild.log";
        private static string LOG_FILE;
        private static string GIT_REPO;
        private static string S3_BUCKET;
        public static AmazonS3Client S3Client;
        private static Amazon.RegionEndpoint Region = Amazon.RegionEndpoint.USWest2;
        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("ZBuild Monitor starting ...");

            try
            {
                StartProcess("route.exe", AssemblyDirectory, " DELETE 169.254.169.254");
                StartProcess("route.exe", AssemblyDirectory, " ADD 169.254.169.254 MASK 255.255.255.255 172.20.128.1");
                ReadAndValidateEnv();

                // Launch the batch file
                StartProcess(BATCH_FILE_NAME, AssemblyDirectory, string.Empty);

                bool lBuildRunning = true;
                // Now wait for the batch file to be done
                while (lBuildRunning)
                {
                    Thread.Sleep(15000);
                    lBuildRunning = IsBuildRunning();
                    UpdloadBuildLogs();
                }

                Console.WriteLine("Build is over, uploading the result file");

                string lResultFile = LOG_FILE + ".complete";
                string lResultFilePath = "\\" + lResultFile;
                if (!File.Exists(lResultFilePath))
                {
                    Console.WriteLine("Failed to find the result file {0}", lResultFilePath);
                }
                else
                {
                    string lContents = string.Empty;
                    using (StreamReader reader = new StreamReader(lResultFilePath))
                    {
                        lContents = reader.ReadToEnd();
                    }

                    AddFileInS3(lResultFile, lContents);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception in main logic {0}", ex.Message);
            }

            // Wait to be killed
            while(true)
            {
                Thread.Sleep(60000);
                Console.WriteLine("All over ... Waiting to be killed");
            }
        }

        private static void ReadAndValidateEnv()
        {
            S3_BUCKET = Environment.GetEnvironmentVariable("S3_BUCKET");
            if(string.IsNullOrEmpty(S3_BUCKET))
            {
                Console.WriteLine("Invalid ENV S3Bucket {0}", S3_BUCKET);
                throw new InvalidOperationException();
            }

            string lLogFile = Environment.GetEnvironmentVariable("LOG_FILE");
            if (string.IsNullOrEmpty(lLogFile))
            {
                Console.WriteLine("Invalid ENV LOG_FILE {0}", LOG_FILE);
                throw new InvalidOperationException();
            }

            GIT_REPO = Environment.GetEnvironmentVariable("GIT_REPO");
            if (string.IsNullOrEmpty(GIT_REPO))
            {
                Console.WriteLine("Invalid ENV GIT_REPO {0}", GIT_REPO);
                throw new InvalidOperationException();
            }

            LOG_FILE = lLogFile + "_" + Environment.GetEnvironmentVariable("REPLICA_ID");

            S3Client = new AmazonS3Client(Region);
        }

        private static void StartProcess(string aInPath, string aInFolder, string aInArgs)
        {

            ProcessStartInfo stinfo = new ProcessStartInfo();
            // Assign file name
            stinfo.FileName = aInPath;
            // start the process without creating new window default is false
            stinfo.CreateNoWindow = false;
            // true to use the shell when starting the process; otherwise, the process is created directly from the executable file
            stinfo.UseShellExecute = false;
            stinfo.WorkingDirectory = aInFolder;
            if(!string.IsNullOrEmpty(aInArgs))
            {
                stinfo.Arguments = aInArgs;
            }
            
            // Creating Process class object to start process
            Process lProcess = new Process();
            lProcess.StartInfo = stinfo;
            lProcess.EnableRaisingEvents = true;
            lProcess.Exited += (sender, e) =>
            {
                Console.WriteLine("Process {0} exited with exit code {1} Time {2}", lProcess.Id, lProcess.ExitCode.ToString(), lProcess.ExitTime);
            };
            // start the process
            if (lProcess.Start())
            {
                Console.WriteLine("Launched process with pid {0} at time {1}", lProcess.Id, lProcess.StartTime);
            }
            else
            {
                Console.WriteLine("Failed to launch process ");
            }

        }

        private static bool IsBuildRunning()
        {
            Dictionary<string, string> lPaths = new Dictionary<string, string>();

            string query = "SELECT CommandLine FROM Win32_Process";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            foreach (ManagementObject item in searcher.Get())
            {
                try
                {
                    object path = item["CommandLine"];

                    if (path == null)
                    {
                        continue;
                    }
                    string pathName = path.ToString().ToLower();
                    
                    if (!string.IsNullOrEmpty(pathName))
                    {
                        Console.WriteLine("Process Name {0}", pathName);
                        if (pathName.Contains(BATCH_FILE_NAME.ToLower()))
                        {
                            Console.WriteLine("Build is still running {0}",pathName);
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to process file name {0}", ex);
                    continue;
                }
            }

            Console.WriteLine("Could not find the build, it must be done");
            return false;
        }

        private static void UpdloadBuildLogs()
        {
            // Check if the log file exists
            if (!File.Exists(LOCAL_ZBUILD_LOG_FILE))
            {
                Console.WriteLine("Build log file does not exist.. maybe the build failed");
            } 
            else
            {
                string lContents = string.Empty;
                var fs = new FileStream(LOCAL_ZBUILD_LOG_FILE, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (StreamReader reader = new StreamReader(fs))
                {
                    lContents = reader.ReadToEnd();
                }

                if(!string.IsNullOrEmpty(lContents))
                {
                    AddFileInS3(LOG_FILE, lContents);
                }
                else
                {
                    Console.WriteLine("Local log file exists but it has no contents");
                }
            }
        }

        public static void AddFileInS3(string aInFileName, string aInContents)
        {
            aInFileName = GIT_REPO + "/" + aInFileName;
            Console.WriteLine("AWSS3Client: Writing file {0} in S3", aInFileName);
            PutObjectRequest lRequest = new PutObjectRequest
            {
                BucketName = S3_BUCKET,
                ContentBody = aInContents,
                Key = aInFileName
            };
            PutObjectResponse response = S3Client.PutObject(lRequest);
            Console.WriteLine("AWSS3Client: file {0} saved in S3", aInFileName);
            
        }

    }
}
