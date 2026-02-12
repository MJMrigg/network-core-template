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
	}
	
	public async void GameCycle(){
		//Remind him how to check if is server and is connected
		
		//Set up the game
		
		//Determine if the game is ready to play
		//Find all Npms and wait until they are all ready
		//To do this, all Npms must have an IsReady variable
		while(!GameStarted){
			//Find all Npms, assume that they all have hit ready, and if one hasn't, don't start
			var x = GetTree().GetNodesInGroup("NPMs");
			if(x.Count <= 2){ //More then 2 clients must connect
				continue;
			}
			//Assume the game has started
			GameStarted = true;
			foreach(var node in x){
				//If that player isn't ready, don't start the game
				UserImplement Npm = (UserImplement)node;
				if(!Npm.IsReady){
					GameStarted = false;
					break;
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
		var y = GetTree().GetNodesInGroup("NPMs");
		int count = 0;
		foreach(UserImplement node in y){
			DumbSprite temp = (DumbSprite)GenericCore.Instance.MainNetworkCore.NetCreateObject(0,new Vector3(128*count,200,0),Quaternion.Identity,node.MyNetID.OwnerId);
			temp.myColor = node.MyColor;
		}
		
		//Start the game
		
		//At the end, add the score screen
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
