using Godot;

public partial class Delete : Button
{
    public override void _Ready()
    {
        Pressed += OnExit;
    }

    public void OnExit()
    {
        Owner.GetParent<VBoxContainer>().RemoveChild(Owner);
    }
}