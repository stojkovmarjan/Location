using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationApi.Services
{
    public class FileService
    {
        private readonly string _filePath;
        public FileService(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<bool> AppendToFileAsync(string content){

            var success = true;

            try
                {
                    using (var writer = new StreamWriter(_filePath, append: true))
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