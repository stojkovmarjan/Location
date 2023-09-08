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
    }
}