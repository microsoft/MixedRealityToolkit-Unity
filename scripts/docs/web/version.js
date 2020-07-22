function createDropdown()
{
	// configurable values:
	var defaultTitle = "releases/2.4.0"; // title in the dropdown for the root version of the docs - alternatively put a version from the version array as a default
	// list of all versions in the version folder
	var versionArray = [
		"mrtk_development",
		"releases/2.0.0",
		"releases/2.1.0",
		"releases/2.2.0",
		"releases/2.3.0",
		"releases/2.4.0"
	];
	
	//--------------------------------------
	
	// get web root path
	var script = document.getElementById('dropdownScript');
	var scriptPath = script.src;
	var currentVersionName = defaultTitle;
	var rootDir = scriptPath.substring(0, scriptPath.lastIndexOf('web/'));
	
	// figure out in which version we're currently working in
	for (var i = 0; i < versionArray.length; i++)
	{
		var currentUrl = window.location.href.toString();
		if (currentUrl.indexOf(versionArray[i]) > 0)
		{
			currentVersionName = versionArray[i];			
			break;
		}
	}
		
	// create dropdown button
	var versionDropDiv = document.getElementById('versionDropdown');
	var btn = document.createElement('button');
	btn.className = "dropbtn";
	var buttonName = "Version - " + currentVersionName;
	var btnText = document.createTextNode(buttonName);
	btn.appendChild(btnText);
	var innerDiv = document.createElement('div');
	innerDiv.className = "version-dropdown-content";
	versionDropDiv.appendChild(btn);
	versionDropDiv.appendChild(innerDiv);
	
	var isDefaultInVersionFolder = false;
	// create version entries
	for (i = 0; i<versionArray.length; i++)
	{
		if (versionArray[i] != currentVersionName)
		{
			createEntry(innerDiv, versionArray[i], rootDir+"version/"+versionArray[i]+"/README.html", false);
			
			// remember if our current version is also the default 
			if (versionArray[i].localeCompare(defaultTitle) == 0)
			{
				isDefaultInVersionFolder = true;
			}
		}
	}
	
	// create default entry
	if (currentVersionName != defaultTitle && isDefaultInVersionFolder == false)
	{
		createEntry(innerDiv, defaultTitle, rootDir+"README.html", true);
	}
	
}

function createEntry(attachTo, name, url, prepend)
{
	var a = document.createElement('a');
	var linkText = document.createTextNode(name);
	a.appendChild(linkText);
	a.href = url;
	a.title = name;
	if (prepend == true)
	{
		attachTo.prepend(a);
	}
	else
	{
		attachTo.appendChild(a);
	}
}

createDropdown();