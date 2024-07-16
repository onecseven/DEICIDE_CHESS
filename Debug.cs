using Godot;
using System;
using Chess;
public partial class Debug : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var vertical = new Position(7, 7);
		var lols = Ops.getBishop(vertical, 2);
		GD.Print("Checking hor");
		lols.ForEach(lo => GD.Print(lo));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
