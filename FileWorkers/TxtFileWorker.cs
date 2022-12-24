
namespace FileWorkers
{
    public static class TxtFileWorker
    {
        public static async Task<List<string>> GetDataAsync(string path)
        {
            List<string> data = new List<string>();

            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                for (int i = 0; (line = await sr.ReadLineAsync()) != null; i++)
                {
                    data.Add(line);
                }
            }

            return data;
        }
    }
}
