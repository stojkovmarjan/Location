using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationApi.Services
{
    public class FileService
    {
        //private string _filePath;
        public string? FilePath { get; set; }
        public FileService()
        {
            
        }
        public FileService(string filePath)
        {
            FilePath = filePath;
        }


        public async Task<bool> AppendToFileAsync(string content){

            var success = true;

            try
                {
                    using (var writer = new StreamWriter(FilePath!, append: true))
                    {
                        await writer.WriteLineAsync(content);
                    }
                }
            catch (Exception ex)
                {
                    // Handle the exception here
                    success = false;
                }

            return success;
        }

        public async Task<bool> AppendToDevicesList(string deviceId){

            var success = true;

            if (!Directory.Exists("devices"))
            {
                Directory.CreateDirectory("devices");
            }

            try
                {
                    using (var writer = new StreamWriter(FilePath!, append: true))
                    {
                        await writer.WriteLineAsync(deviceId);
                    }
                }
            catch (Exception ex)
                {
                    // Handle the exception here
                    Console.WriteLine("Error adding device to list: "+ex.ToString());
                    success = false;
                }

            return success;
        }

        public List<string> ReadDevicesList(){
            var devicesList = new List<string>();
            try {
                using (StreamReader reader = new StreamReader(FilePath!)){
                    string line = "";
                    while ((line = reader.ReadLine()!) != null) {
                        devicesList.Add(line);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine("Error reading devices list: "+ex.ToString());
                return null!;
            }

            return devicesList;
        }

        public async Task<bool> SaveToFileAsync(string content, string fileDir){

            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            var success = true;

            try
                {
                    
                using var writer = new StreamWriter(FilePath!, append: false);

                await writer.WriteLineAsync(content);
            }
            catch (Exception ex)
                {
                    // Handle the exception here
                    Console.WriteLine(ex.Message);
                    success = false;
                }

            return success;
        }

        public string ReadParamsFromFile(){
            try
            {
                // Check if the file exists
                if (!System.IO.File.Exists(FilePath))
                {
                    return null!; // or throw an exception or handle the missing file scenario
                }
            

                // Read the first line from the file
                string line = System.IO.File.ReadLines(FilePath).FirstOrDefault()!;

                return line;

            }
                catch (Exception ex)
                {
                    // Handle any exceptions that may occur during file reading
                    // You can log the exception or return an error message
                    return null!;
                }
        }

        public bool DeleteFile()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    return true; // File deleted successfully
                }
                    else
                    {
                        return false; // File not found
                    }
            }
            catch (Exception)
            {
                // Handle any exceptions that occur during file deletion
                return false; // File deletion failed
            }
        }
    }

}