using Godot;
using System;
using System.Data;

public partial class Ui : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public void Update(Player player)
    {
        var hp = GetNode<Label>("Control/VBoxContainer/lblHP");
        hp.Text = "HP: " + player.HitPoints.ToString();

        var exp = GetNode<Label>("Control/VBoxContainer/lblXP");
        exp.Text = "XP: " + player.Experience.ToString();

        var kh0 = GetNode<Label>("Control/VBoxContainer/lblKillHistory/VBoxContainer/UnknownLabel");
        kh0.Text = "Unknown: " + player.KillHistory[0].ToString();

        var kh1 = GetNode<Label>("Control/VBoxContainer/lblKillHistory/VBoxContainer/CommonLabel");
        kh1.Text = "Common: " + player.KillHistory[1].ToString();

        var kh2 = GetNode<Label>("Control/VBoxContainer/lblKillHistory/VBoxContainer/UncommonLabel");
        kh2.Text = "Uncommon: " + player.KillHistory[2].ToString();

        var kh3 = GetNode<Label>("Control/VBoxContainer/lblKillHistory/VBoxContainer/RareLabel");
        kh3.Text = "Rare: " + player.KillHistory[3].ToString();

        var kh4 = GetNode<Label>("Control/VBoxContainer/lblKillHistory/VBoxContainer/EpicLabel");
        kh4.Text = "Epic: " + player.KillHistory[4].ToString();

        var kh5 = GetNode<Label>("Control/VBoxContainer/lblKillHistory/VBoxContainer/LegendaryLabel");
        kh5.Text = "Legendary: " + player.KillHistory[5].ToString();



    }
}
