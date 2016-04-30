using System.Net;

namespace DLTuber
{
    class InternetConnection
    {
        public bool connected()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.youtube.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
