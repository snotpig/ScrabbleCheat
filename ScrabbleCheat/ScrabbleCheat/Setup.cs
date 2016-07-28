using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.IO;

namespace ScrabbleCheat
{
    class Setup
    {
        public void CreateFolder(string path)
        {
            IdentityReference user = WindowsIdentity.GetCurrent().User;
            string username = WindowsIdentity.GetCurrent().Name;// .User;
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), path);

            try
            {
                System.IO.Directory.CreateDirectory(folderPath);
                System.IO.DirectoryInfo di = new DirectoryInfo(folderPath);
                DirectorySecurity ds = new DirectorySecurity();

                FileSystemAccessRule fACL = new FileSystemAccessRule(user,
                    FileSystemRights.CreateFiles | FileSystemRights.WriteData | FileSystemRights.ReadData,
                    AccessControlType.Allow); 

                ds.SetAccessRule(fACL);
                di.SetAccessControl(ds);
//                MessageBox.Show("Updated ACL");
            }

            catch (Exception ex)
            {
                MessageBox.Show("Could not update ACL.\nReason: " + ex.Message);
            }
        }
    }
}
