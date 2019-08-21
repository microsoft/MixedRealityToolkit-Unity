	
function createDropdown()
{
	// configurable values:
	var defaultTitle = "mrtk_development"; 	// title in the dropdown for the root version of the docs
	var versionArray = ["prerelease/2.0.0_stabilization"];	// list of all versions in the version folder
	
	//--------------------------------------

	var versionDropDiv = document.getElementById('versionDropdown');
	var btn = document.createElement('button');
	btn.className = "dropbtn";
	var btnText = document.createTextNode("Version");
	btn.appendChild(btnText);
	var innerDiv = document.createElement('div');
	innerDiv.className = "version-dropdown-content";
	versionDropDiv.appendChild(btn);
	versionDropDiv.appendChild(innerDiv);
	
	// get web root path
	var script = document.getElementById('dropdownScript');
	var scriptPath = script.src;
	var versionIndex = scriptPath.lastIndexOf('version/');
	var rootDir = scriptPath;
	if (versionIndex > 0)
	{
		rootDir = scriptPath.substring(0, versionIndex);
	}
	else
	{	
		rootDir = scriptPath.substring(0, scriptPath.lastIndexOf('web/'));
	}
	
	// create default
	createEntry(innerDiv, defaultTitle, rootDir+"README.html");
	
	// create version entries
	for (i = 0; i<versionArray.length; i++) 
	{ 
		createEntry(innerDiv, versionArray[i], rootDir+"version/"+versionArray[i]+"/README.html");
	}
}



function createEntry(attachTo, name, url)
{
	var a = document.createElement('a');
	var linkText = document.createTextNode(name);
	a.appendChild(linkText);
	a.href = url;
	a.title = name;
	attachTo.appendChild(a);
}

createDropdown();