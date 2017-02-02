Config File                       {#configfile}
============

You can set which server an Sharing-enabled app will connect to by creating a file called SharingConfig.txt and putting it in the working directory of your app. 

In it, add this line:

	ServerAddress <machineName>

Next time you run your app, it will attempt to connect to the session server at that address.  If there is no config file present, it will attempt to connect to the address specified in code when creating the instance of SharingManager.  If none is specified, HoloToolkit.Sharing defaults to looking for the session server at localhost.  

All config options (and their default values):

	ServerAddress localhost
	ServerPort 20602
