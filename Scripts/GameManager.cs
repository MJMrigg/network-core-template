using Godot;
using System;

public partial class GameManager : Node
{
	[Export] public NetID myId;
	[Export] public bool GameStarted;
	[Export] public bool GameFinished;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		GameCycle();
	}
	
	public async void GameCycle(){
		//Wait until the game is connected
		while(!Node.IsInstanceValid(GenericCore.Instance)){
			await ToSignal(GetTree().CreateTimer(2.5f), SceneTreeTimer.SignalName.Timeout);
		}
		//Don't do anything if it isn't the server
		if(!GenericCore.Instance.IsServer){
			return;
		}
		//Set up the game
		//Determine if the game is ready to play
		//Find all Npms and wait until they are all ready
		//To do this, all Npms must have an IsReady variable
		while(!GameStarted){
			//Find all Npms, assume that they all have hit ready, and if one hasn't, don't start
			var x = GetTree().GetNodesInGroup("NPMs"); //Don't use the connection diciontary. Things will go wrong
				if(x.Count >= 2){ //At least 2 clients must connect
				//Assume the game has started
				GameStarted = true;
				GD.Print("Waiting for players to ready up"); //You will need this print statement
				//Otherwise, a race condition gets raised
				//Dr. Towle doesn't know why. If you can find out, you get 100 points extra credit on final
				foreach(UserImplement node in x){
					//If that player isn't ready, don't start the game
					if(!node.IsReady){
						GameStarted = false;
						//break;
					}
				}
			}
			//Wait for the next game cycle
			await ToSignal(GetTree().CreateTimer(2.5f), SceneTreeTimer.SignalName.Timeout);
			//After the waiting is done, it will go back to the while loop.
			//If the game has started, it will leave the loop
		}
		//Setup and spawn the characters
		//Use a Node2D or Node3D spawner to create the level
		//Or just unhide the level if it's already there. RECOMENDED
		//Spawn the characters
		//This is not a spawnegg. This is spawning in all initial objects
		//Spawneggs are for network objects that you know where it will be on the scene and will be put there in the editor
		var y = GetTree().GetNodesInGroup("NPMs");
		int xCord = 0;
		foreach(UserImplement node in y){
			//Make sure to set the MainNetworkCore for the GenericCore as the one in the level
			//NetCreateObject(PrefabIndex, position, rotation, connection's id)
			DumbSprite temp = (DumbSprite)GenericCore.Instance.MainNetworkCore.NetCreateObject(0,new Vector3((128*xCord)+64,300,0),Quaternion.Identity,node.MyNetID.OwnerId);
			//What about scale? Well, typically, you don't need it, but if you really really really want it, you'll either have to modify Dr. Towle's code
			//Or you can set temp.Scale and in temp's packed scene, sychronize scale (RECOMMENDED)
			temp.myColor = node.MyColor;
			xCord += 1;
			//node.Hide();
			//Warning, if you spawn two objects on top of each other, as in the position vectors are exactly the same, physics will go NUTS
			//So if you want objects to spawn close to each other, add a bit of randomness to their coordinates
		}
		
		//Start the game
		
		//End the game
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
