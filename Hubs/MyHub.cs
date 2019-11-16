using Microsoft.AspNetCore.SignalR;
using MusicRecogniser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace MusicRecogniser.Hubs
{

    public class MyHub : Hub
    {

        #region Variables section
        String output = "";
        public static InfoModel im = new InfoModel();
        string projectPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
        #endregion

        #region To invoke Client Side Function artistNameClient()
        public async Task artistNameServer()
        {
            //return the artist name
            await Clients.Caller.SendAsync("artistNameClient", im.info1);
        }
        #endregion

        public async Task ServerFunc(string msg)
        {


            #region Youtube Video /Youtube-dl
            await Clients.Caller.SendAsync("ClientFunc", "Video file will start downloading in a moment...");
            Thread.Sleep(1500);
            //Youtube-dl
            Process p1 = new Process();
            p1.StartInfo = new ProcessStartInfo("./Tools/youtube-dl.exe", "--format \"bestvideo + bestaudio[ext = m4a] / bestvideo + bestaudio / best\" --merge-output-format mp4 -o " + "\"" + projectPath + @"Output\myvideo.mp4" + "\"" + " " + msg)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            };
            if (File.Exists("./Output/myvideo.mp4"))
                File.Delete("./Output/myvideo.mp4");
            p1.Start();
            string output = "";
            StreamReader sr = p1.StandardOutput;
            while (p1.StandardOutput.EndOfStream == false)
            {
                try
                {
                    output = sr.ReadLine().Split(']')[1];
                    if (output.Contains("recogniser")|| output.Contains("page"))
                        output = "";
                }
                catch (Exception ff) { }
                await Clients.Caller.SendAsync("ClientFunc", output);
            }
            sr.Close();
            p1.WaitForExit(1000);
            p1.Close();
            #endregion
            


            #region Audio File/ffmpeg
            //ffmpeg
            Process p2 = new Process();
            p2.StartInfo = new ProcessStartInfo("cmd.exe", "/c call \"" + projectPath + @"Tools\ffmpeg.exe" + "\" -i " + "\"" + projectPath + @"Output\myvideo.mp4" + "\"" + " " + " -ss 00:00:00 -t 00:01:00 -y " + "\"" + projectPath + @"Output\myaudio.mp3" + "\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true

            };
            p2.Start();
            await Clients.Caller.SendAsync("ClientFunc", "Video file Is being converted to audio..");
            Thread.Sleep(1500);

            if (File.Exists("./Output/myaudio.mp3"))
            {
                await Clients.Caller.SendAsync("ClientFunc", "Successfully converted to audio..");
                Thread.Sleep(1500);
            }


            p2.WaitForExit(1000);
            p2.Close();
            p2.Dispose();
            //Process temp = new Process();
            //temp.StartInfo = new ProcessStartInfo("cmd.exe", "/c taskkill /F /PID "+ p2.Id.ToString() +" /T");
            //temp.Start();

            #endregion



            #region Send audio file to Audd.io for recognition
            //python
            await Clients.Caller.SendAsync("ClientFunc", "File is being sent for recognition..Please wait...");
            Thread.Sleep(1000);

            Process p3 = new Process();
            
            p3.StartInfo = new ProcessStartInfo("py", "\"" + projectPath + @"Scripts\SendFile.py"+ "\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            };
            p3.Start();
            p3.WaitForExit();
           
            string k = "";
            output = " ";

            await Clients.Caller.SendAsync("ClientFunc", "Artist name is being extracted...");
            Thread.Sleep(1000);

            while (p3.StandardOutput.EndOfStream == false)
                output = p3.StandardOutput.ReadLine();

            k = output.Substring(output.IndexOf("artist") + 7, output.IndexOf("title") - 10 - output.IndexOf("artist"));
            k = k.Replace(":", "").Replace(",", "").Trim().Replace("\"","");
            im.info1 = k;

            await Clients.Caller.SendAsync("ClientFunc", "Extraction completed, now videos are being added to the playlist...Note that only videos with videoId will be added..");
            #endregion

        }


    }
}
