namespace LoginBase.Helper
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using System.IO;
    using System.Web;

    public class FilesHelper : Controller
    {
        //public IWebHostEnvironment _enviroment;

        //public  FilesHelper(IWebHostEnvironment env)
        //{
        //    _enviroment = env;
        //}
        public static bool UploadPhoto(MemoryStream stream, string folder, string name, IWebHostEnvironment _enviroment)
        {
            try
            {
                stream.Position = 0;
                var path = Path.Combine(_enviroment.ContentRootPath,folder, name);
                System.IO.File.WriteAllBytes(path, stream.ToArray());

            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
