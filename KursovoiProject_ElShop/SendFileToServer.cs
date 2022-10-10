using Renci.SshNet;
using ConnectionInfo = Renci.SshNet.ConnectionInfo;

namespace KursovoiProject_ElShop
{
    public static class SendFileToServer
    {

        private static string host = "89.108.64.223";
        private static string username = "root";
        private static string password = "-$4LK+79k7uL";
        public static int SendFileAdds(IFormFile uploadedFile, string id, string extension, int typeAdds)
        {
            var connectionInfo = new ConnectionInfo(host, "root", new PasswordAuthenticationMethod(username, password));
            using (var sftp = new SftpClient(connectionInfo))
            {
                sftp.Connect();
                if(typeAdds == 1)
                    sftp.ChangeDirectory("/var/www/89-108-64-223.cloudvps.regruhosting.ru/AddsImages/MainPage");
                if (typeAdds == 2)
                    sftp.ChangeDirectory("/var/www/89-108-64-223.cloudvps.regruhosting.ru/AddsImages/Right1");
                if (typeAdds == 3)
                    sftp.ChangeDirectory("/var/www/89-108-64-223.cloudvps.regruhosting.ru/AddsImages/Right2");
                sftp.UploadFile(uploadedFile.OpenReadStream(), id + extension, true);
                sftp.Disconnect();
            }
            return 0;
        }
    }
}
