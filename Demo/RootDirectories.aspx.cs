using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using IZ.WebFileManager;

public partial class RootDirectories : System.Web.UI.Page
{
	protected void Page_Load (object sender, EventArgs e) {

		FileManager1.RootDirectories.Clear ();
		RootDirectory rootDirectory = new RootDirectory ();
		rootDirectory.DirectoryPath = DirectoryManager.GetRootDirectoryPath (Context);
		rootDirectory.Text = "My Documents";
		FileManager1.RootDirectories.Add (rootDirectory);

	}
}
