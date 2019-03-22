// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DemoHubManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Demos
// </copyright>
// <summary>
//  Used as starting point to let developer choose amongst all demos available.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

using Photon.Pun.Demo.Cockpit;

namespace Photon.Pun.Demo.Hub
{
	public class DemoHubManager : MonoBehaviour {


		public Text TitleText;
		public Text DescriptionText;
		public GameObject OpenSceneButton;
		public GameObject OpenTutorialLinkButton;
		public GameObject OpenDocLinkButton;

        string MainDemoWebLink = "https://doc.photonengine.com/en-us/pun/v2/getting-started/pun-intro";

		struct DemoData
		{
			public string Title;
			public string Description;
			public string Scene;
			public string TutorialLink;
			public string DocLink;
		}

		Dictionary<string,DemoData> _data = new Dictionary<string, DemoData>();

		string currentSelection;

		// Use this for initialization
		void Awake () {

			PunCockpit.Embedded = false;

			OpenSceneButton.SetActive(false);
			
			OpenTutorialLinkButton.SetActive(false);
			OpenDocLinkButton.SetActive(false);

			// Setup data
			_data.Add(
				"DemoBoxes", 
				new DemoData()
				{
					Title = "Demo Boxes",
					Description = "Uses ConnectAndJoinRandom script.\n" +
						"(joins a random room or creates one)\n" +
						"\n" +
						"Instantiates simple prefabs.\n" +
						"Synchronizes positions without smoothing.\n" +
						"Shows that RPCs target a specific object.",
					Scene = "DemoBoxes-Scene" 
				}
			);

			_data.Add(
				"DemoWorker", 
				new DemoData()
				{
				Title = "Demo Worker",
				Description = "Joins the default lobby and shows existing rooms.\n" +
					"Lets you create or join a room.\n" +
					"Instantiates an animated character.\n" +
					"Synchronizes position and animation state of character with smoothing.\n" +
					"Implements simple in-room Chat via RPC calls.",
				Scene = "DemoWorker-Scene" 
				}
			);

			_data.Add(
				"MovementSmoothing", 
				new DemoData()
				{
				Title = "Movement Smoothing",
				Description = "Uses ConnectAndJoinRandom script.\n" +
					"Shows several basic ways to synchronize positions between controlling client and remote ones.\n" +
					"The TransformView is a good default to use.",
				Scene = "DemoSynchronization-Scene" 
				}
			);

			_data.Add(
				"BasicTutorial", 
				new DemoData()
				{
				Title = "Basic Tutorial",
				Description = "All custom code for connection, player and scene management.\n" +
					"Auto synchronization of room levels.\n" +
						"Uses PhotonAnimatoView for Animator synch.\n" +
						"New Unity UI all around, for Menus and player health HUD.\n" +
						"Full step by step tutorial available online.",
				Scene = "PunBasics-Launcher" ,
				TutorialLink = "https://doc.photonengine.com/en-us/pun/v2/demos-and-tutorials/pun-basics-tutorial/intro"
                }
			);
			
			_data.Add(
				"OwnershipTransfer", 
				new DemoData()
				{
				Title = "Ownership Transfer",
				Description = "Shows how to transfer the ownership of a PhotonView.\n" +
					"The owner will send position updates of the GameObject.\n" +
					"Transfer can be edited per PhotonView and set to Fixed (no transfer), Request (owner has to agree) or Takeover (owner can't object).",
				Scene = "DemoChangeOwner-Scene"
				}
			);

			_data.Add(
				"PickupTeamsScores", 
				new DemoData()
				{
				Title = "Pickup, Teams, Scores",
				Description = "Uses ConnectAndJoinRandom script.\n" +
					"Implements item pickup with RPCs.\n" +
					"Uses Custom Properties for Teams.\n" +
					"Counts score per player and team.\n" +
					"Uses Player extension methods for easy Custom Property access.",
				Scene = "DemoPickup-Scene"
				}
			);

			_data.Add(
				"Chat", 
				new DemoData()
				{
				Title = "Chat",
				Description = "Uses the Chat API.\n" +
					"Simple UI.\n" +
					"You can enter any User ID.\n" +
					"Automatically subscribes some channels.\n" +
					"Allows simple commands via text.\n" +
					"\n" +
					"Requires configuration of Chat App ID in ServerSettings.",
						Scene = "DemoChat-Scene",
						DocLink = "http://j.mp/2iwQkPJ" 
				}
			);

			_data.Add(
				"RPGMovement", 
				new DemoData()
				{
				Title = "RPG Movement",
				Description = "Demonstrates how to use the PhotonTransformView component to synchronize position updates smoothly using inter- and extrapolation.\n" +
					"\n" +
					"This demo also shows how to setup a Mecanim Animator to update animations automatically based on received position updates (without sending explicit animation updates).",
				Scene = "DemoRPGMovement-Scene"
				}
			);

			_data.Add(
				"MecanimAnimations", 
				new DemoData()
				{
				Title = "Mecanim Animations",
				Description = "This demo shows how to use the PhotonAnimatorView component to easily synchronize Mecanim animations.\n" +
					"\n" +
					"It also demonstrates another feature of the PhotonTransformView component which gives you more control how position updates are inter-/extrapolated by telling the component how fast the object moves and turns using SetSynchronizedValues().",
				Scene = "DemoMecanim-Scene"
			}
			);

			_data.Add(
				"2DGame", 
				new DemoData()
				{
				Title = "2D Game Demo",
				Description = "Synchronizes animations, positions and physics in a 2D scene.",
				Scene = "Demo2DJumpAndRunWithPhysics-Scene"
				}
			);

			_data.Add(
				"FriendsAndAuth", 
				new DemoData()
				{
				Title = "Friends & Authentication",
				Description = "Shows connect with or without (server-side) authentication.\n" +
					"\n" +
					"Authentication requires minor server-side setup (in Dashboard).\n" +
					"\n" +
					"Once connected, you can find (made up) friends.\nJoin a room just to see how that gets visible in friends list.",
						Scene = "DemoFriends-Scene"
				}
			);

			_data.Add(
				"TurnBasedGame", 
				new DemoData()
				{
				Title = "'Rock Paper Scissor' Turn Based Game",
				Description = "Demonstrate TurnBased Game Mechanics using PUN.\n" +
					"\n" +
					"It makes use of the TurnBasedManager Utility Script",
				Scene = "DemoRPS-Scene"
				}
			);

			_data.Add(
				"Asteroids", 
				new DemoData()
				{
					Title = "Asteroids",
					Description = "Simple asteroid game based on the Unity learning asset.\n",
					Scene = "DemoAsteroids-LobbyScene"
				}
			);

			_data.Add(
				"SlotRacer", 
				new DemoData()
				{
					Title = "Slot Racer",
					Description = "Simple SlotRacing game.\n",
					Scene = "SlotCar-Scene"
				}
			);


			_data.Add(
				"LoadBalancing", 
				new DemoData()
				{
					Title = "Load Balancing",
					Description = "Shows how to use the raw LoadBalancing system.\n" +
						"\n" +
						"This is a simple test scene to connect and join a random room, without using PUN but the actual LoadBalancing api only",
					Scene = "DemoLoadBalancing-Scene"
				}
			);

			_data.Add(
				"PunCockpit", 
					new DemoData()
					{
						Title = "Cockpit",
						Description = "Controls most aspect of PUN.\n" +
							"Connection, Lobby, Room access, Player control",
					Scene = "PunCockpit-Scene"
					}
			);
        }

		public void SelectDemo(string Reference)
		{
			currentSelection = Reference;

			TitleText.text = _data[currentSelection].Title;
			DescriptionText.text = _data[currentSelection].Description;

			OpenSceneButton.SetActive(!string.IsNullOrEmpty(_data[currentSelection].Scene));

			OpenTutorialLinkButton.SetActive(!string.IsNullOrEmpty(_data[currentSelection].TutorialLink));
			OpenDocLinkButton.SetActive(!string.IsNullOrEmpty(_data[currentSelection].DocLink));
		}

		public void OpenScene()
		{
			if (string.IsNullOrEmpty(currentSelection))
		    {
				Debug.LogError("Bad setup, a CurrentSelection is expected at this point");
				return;
			}

			SceneManager.LoadScene(_data[currentSelection].Scene);
		}

		public void OpenTutorialLink()
		{
			if (string.IsNullOrEmpty(currentSelection))
			{
				Debug.LogError("Bad setup, a CurrentSelection is expected at this point");
				return;
			}
			
			Application.OpenURL(_data[currentSelection].TutorialLink);
		}

		public void OpenDocLink()
		{
			if (string.IsNullOrEmpty(currentSelection))
			{
				Debug.LogError("Bad setup, a CurrentSelection is expected at this point");
				return;
			}

			Application.OpenURL(_data[currentSelection].DocLink);
		}

		public void OpenMainWebLink()
		{
			Application.OpenURL(MainDemoWebLink);
		}
	}
}